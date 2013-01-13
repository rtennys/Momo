using System;
using System.Linq;
using System.Web.Mvc;
using Momo.Common.DataAccess;
using Momo.Domain;
using Momo.Domain.Commands;
using Momo.Domain.Entities;
using Momo.UI.Models;

namespace Momo.UI.Controllers
{
    [Authorize]
    public class ShoppingListsController : AppController
    {
        public ShoppingListsController(IUnitOfWork uow, IRepository repository, ICommandExecutor commandExecutor)
        {
            _uow = uow;
            _repository = repository;
            _commandExecutor = commandExecutor;
        }

        private readonly IUnitOfWork _uow;
        private readonly IRepository _repository;
        private readonly ICommandExecutor _commandExecutor;

        [AllowAnonymous]
        public ActionResult Index(string username)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var model = new ShoppingListsIndexModel
                        {
                            ShoppingLists = user.ShoppingLists.OrderBy(x => x.Name).ToArray(),
                            SharedLists = user.SharedLists.OrderBy(x => x.Name).ToArray()
                        };

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Show(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            return View(new ShoppingListsShowModel {Id = shoppingList.Id, Name = shoppingList.Name});
        }


        public ActionResult Add()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(ShoppingListsAddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_commandExecutor.Execute<AddShoppingListCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Index");
        }


        [ValidateRouteUsername]
        public ActionResult Rename(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            var model = new ShoppingListsRenameModel
                        {
                            Id = shoppingList.Id,
                            Name = shoppingList.Name
                        };

            return View(model);
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Rename(ShoppingListsRenameModel model)
        {
            model.Username = User.Identity.Name;

            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_commandExecutor.Execute<RenameShoppingListCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Index");
        }

        [ValidateRouteUsername]
        public ActionResult Share(string username, string shoppinglist)
        {
            var usernames = _repository.Find<ShoppingList>()
                .Where(x => x.UserProfile.Username == username && x.Name == shoppinglist)
                .SelectMany(x => x.SharedWith)
                .Select(x => x.Username)
                .ToArray();

            return View(usernames);
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartSharing(string username, string shoppinglist, string shareWith)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StartSharing(_repository.Get<UserProfile>(x => x.Username == shareWith));

            _uow.Commit();
            return RedirectToAction("Share");
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult StopSharing(string username, string shoppinglist, string shareWith)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StopSharing(shareWith);

            _uow.Commit();
            return RedirectToAction("Share");
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteShoppingListCommand command)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(command));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Index");
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Clear(ClearShoppingListCommand command)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(command));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Show");
        }


        /* shoppinglists/show ajax calls */

        public ActionResult LoadItems(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null) return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null) return HttpNotFound();

            var model = shoppingList
                .ShoppingListItems
                .OrderBy(x => x.Aisle)
                .ThenBy(x => x.Name)
                .Select(x => new ShoppingListItemModel(x))
                .ToArray();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddItem(AddShoppingListItemCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new {Errors = ModelState.ToErrorList()});

            var result = _commandExecutor.Execute(command);

            if (result.AnyErrors())
            {
                ModelState.AddModelErrors(result);
                return Json(new {Errors = ModelState.ToErrorList()});
            }

            _uow.Commit();

            return Json(new {Success = true, Item = new ShoppingListItemModel(result.Data.Item)});
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditItem(EditShoppingListItemCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new {Errors = ModelState.ToErrorList()});

            var result = _commandExecutor.Execute(command);

            if (result.AnyErrors())
            {
                ModelState.AddModelErrors(result);
                return Json(new {Errors = ModelState.ToErrorList()});
            }

            _uow.Commit();

            return Json(new {Success = true});
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public void ChangePicked(int id, bool picked)
        {
            var item = _repository.Get<ShoppingListItem>(id);
            item.Picked = picked;
            _uow.Commit();
        }
    }
}
