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

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
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


        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public void ChangePicked(int id, bool picked)
        {
            _repository.Get<ShoppingListItem>(id).Picked = picked;
            _uow.Commit();
        }


        [ValidateShoppingListAccess]
        public ActionResult EditItem(string username, string shoppinglist, int id)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            var item = shoppingList.ShoppingListItems.SingleOrDefault(x => x.Id == id);
            if (item == null)
                return HttpNotFound();

            var model = new ShoppingListsEditItemModel
                        {
                            ShoppingListId = shoppingList.Id,
                            Id = item.Id,
                            Name = item.Name,
                            Quantity = item.Quantity > 0 ? item.Quantity : (int?)null,
                            Isle = item.Isle > 0 ? item.Isle : (int?)null,
                            Price = item.Price > 0M ? item.Price : (decimal?)null
                        };

            return View(model);
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditItem(ShoppingListsEditItemModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_commandExecutor.Execute<EditShoppingListItemCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Show");
        }


        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
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
                .Select(x => new {x.Name, x.Quantity, x.Isle, x.Price})
                .ToArray();

            return Json(model);
        }
    }
}
