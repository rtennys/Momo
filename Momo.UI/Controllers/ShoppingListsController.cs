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
                            IsOwner = string.Equals(User.Identity.Name, user.Username, StringComparison.OrdinalIgnoreCase),
                            ShoppingLists = user.ShoppingLists.OrderBy(x => x.Name).Select(x => x.Name).ToArray()
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

            var model = new ShoppingListsShowModel
                        {
                            IsOwner = string.Equals(User.Identity.Name, user.Username, StringComparison.OrdinalIgnoreCase),
                            Id = shoppingList.Id,
                            Name = shoppingList.Name,
                            Items = shoppingList.ShoppingListItems.Where(x => x.Quantity > 0).OrderBy(x => x.Isle).ThenBy(x => x.Name).ToArray()
                        };

            return View(model);
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


        public ActionResult Rename(string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == User.Identity.Name);
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

        [HttpPost, ValidateAntiForgeryToken]
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


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(new DeleteShoppingListCommand {Username = User.Identity.Name, Id = id}));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Index");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Clear(int id)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(new ClearShoppingListCommand {Username = User.Identity.Name, Id = id}));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Show");
        }


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

        [HttpPost, ValidateAntiForgeryToken]
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


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangedPicked(int id, bool picked)
        {
            _repository.Get<ShoppingListItem>(id).Picked = picked;
            _uow.Commit();

            return Json(new {Success = true});
        }
    }
}
