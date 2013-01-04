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

            var model = new ShoppingListsShowModel
                        {
                            Id = shoppingList.Id,
                            Name = shoppingList.Name,
                            Items = shoppingList.ShoppingListItems.Where(x => x.Quantity > 0).OrderBy(x => x.Aisle).ThenBy(x => x.Name).ToArray()
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
                            Aisle = item.Aisle > 0 ? item.Aisle : (int?)null,
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
        public void ChangePicked(int id, bool picked)
        {
            var item = _repository.Get<ShoppingListItem>(id);
            item.Picked = picked;
            _uow.Commit();
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdatePicked(int id, int? aisle, decimal? price)
        {
            var item = _repository.Get<ShoppingListItem>(id);
            item.Aisle = aisle.HasValue ? aisle.Value : 0;
            item.Price = price.HasValue ? price.Value : 0M;
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
                .Select(x => new {x.Name, x.Quantity, x.Aisle, x.Price})
                .ToArray();

            return Json(model);
        }
    }
}
