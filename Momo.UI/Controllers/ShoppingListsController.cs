using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Momo.Common.DataAccess;
using Momo.Domain;
using Momo.Domain.Commands;
using Momo.Domain.Entities;
using Momo.UI.Models;

namespace Momo.UI.Controllers
{
    [System.Web.Mvc.Authorize]
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

        [System.Web.Mvc.AllowAnonymous]
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

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Show(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            var model = new ShoppingListsShowModel
                        {
                            Id = shoppingList.Id,
                            Name = shoppingList.Name
                        };

            return View(model);
        }


        public ActionResult Add()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
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

        [ValidateRouteUsername, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
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

        [ValidateRouteUsername, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartSharing(string username, string shoppinglist, string shareWith)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StartSharing(_repository.Get<UserProfile>(x => x.Username == shareWith));

            _uow.Commit();
            return RedirectToAction("Share");
        }

        [ValidateRouteUsername, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult StopSharing(string username, string shoppinglist, string shareWith)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StopSharing(shareWith);

            _uow.Commit();
            return RedirectToAction("Share");
        }


        [ValidateRouteUsername, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteShoppingListCommand command)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(command));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Index");
        }


        [ValidateShoppingListAccess, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult Clear(ClearShoppingListCommand command)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(command));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Show");
        }


        [ValidateShoppingListAccess]
        public ActionResult AddItem(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            var model = new ShoppingListsAddItemModel {ShoppingListId = shoppingList.Id};

            return View(model);
        }

        [ValidateShoppingListAccess, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddItem(ShoppingListsAddItemModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_commandExecutor.Execute<AddShoppingListItemCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Show");
        }



        [ValidateShoppingListAccess, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public void ChangePicked(int id, bool picked)
        {
            var item = _repository.Get<ShoppingListItem>(id);
            item.Picked = picked;
            _uow.Commit();
        }


        [ValidateShoppingListAccess, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult GetSuggestions(string username, string shoppinglist, string search)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            var model = shoppingList
                .ShoppingListItems
                .Where(x => x.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Name)
                .Take(5)
                .Select(x => new {x.Name, x.Quantity, x.Aisle, x.Price})
                .ToArray();

            return Json(model);
        }

        public ActionResult Load(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var model = shoppingList
                .ShoppingListItems
                .Where(x => x.Quantity > 0)
                .OrderBy(x => x.Aisle)
                .ThenBy(x => x.Name)
                .Select(x => new
                             {
                                 x.Id,
                                 x.Name,
                                 x.Aisle,
                                 x.Quantity,
                                 x.Price,
                                 x.Picked
                             })
                .ToArray();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [ValidateShoppingListAccess, System.Web.Mvc.HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditItem(ShoppingListsEditItemModel model)
        {
            if (!ModelState.IsValid)
                return Json(new {Errors = ModelState.ToErrorList()});

            var result = _commandExecutor.Execute<EditShoppingListItemCommand>(model);

            if (result.AnyErrors())
            {
                ModelState.AddModelErrors(result);
                return Json(new {Errors = ModelState.ToErrorList()});
            }

            _uow.Commit();

            return Json(new {Success = true});
        }
    }
}
