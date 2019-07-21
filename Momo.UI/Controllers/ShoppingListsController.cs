using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Momo.Domain;
using Momo.Domain.Entities;
using Momo.Domain.ShoppingLists;
using Momo.UI.Models;

namespace Momo.UI.Controllers
{
    [Authorize]
    public sealed class ShoppingListsController : AppController
    {
        public ShoppingListsController(IUnitOfWork uow, IRepository repository, IShoppingListService shoppingListService, IShoppingListItemService shoppingListItemService)
        {
            _uow = uow;
            _repository = repository;
            _shoppingListService = shoppingListService;
            _shoppingListItemService = shoppingListItemService;
        }

        private readonly IUnitOfWork _uow;
        private readonly IRepository _repository;
        private readonly IShoppingListService _shoppingListService;
        private readonly IShoppingListItemService _shoppingListItemService;

        [AllowAnonymous]
        public ActionResult Index(string username)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var model = new ShoppingListsIndexModel
            {
                ShoppingLists = user.ShoppingLists.OrderBy(x => x.Name).Select(x => new ShoppingListModel(x, Url)).ToArray(),
                SharedLists = user.SharedLists.OrderBy(x => x.Name).Select(x => new ShoppingListModel(x, Url)).ToArray()
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

            var listItems = shoppingList
                .ShoppingListItems
                .OrderBy(x => x.Aisle)
                .ThenBy(x => x.Name)
                .Select(x => new ShoppingListItemModel(x))
                .ToArray();

            return View(new ShoppingListsShowModel { Id = shoppingList.Id, ListItems = listItems });
        }


        [ValidateRouteUsername]
        public ActionResult Rename(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            return View(new ShoppingListsRenameModel { Id = shoppingList.Id, Name = shoppingList.Name });
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Rename(ShoppingListsRenameModel model)
        {
            model.Username = User.Identity.Name;

            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_shoppingListService.RenameShoppingList(model.Username, model.Id, model.Name));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();

            return RedirectToAction("Show", new { username = model.Username, shoppingList = model.Name });
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
            var shoppingList = user.ShoppingLists.First(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StartSharing(_repository.Get<UserProfile>(x => x.Username == shareWith));

            _uow.Commit();
            return RedirectToAction("Share");
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult StopSharing(string username, string shoppinglist, string shareWith)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            var shoppingList = user.ShoppingLists.First(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));

            shoppingList.StopSharing(shareWith);

            _uow.Commit();
            return RedirectToAction("Share");
        }

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(ShoppingListsDeleteModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelErrors(_shoppingListService.DeleteShoppingList(model.Username, model.Id));
                if (ModelState.IsValid)
                    _uow.Commit();
            }

            return RedirectToAction("Index");
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Clear(ShoppingListsClearModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelErrors(_shoppingListService.ClearShoppingList(model.Username, model.Id, model.CheckedOnly));
                if (ModelState.IsValid)
                    _uow.Commit();
            }

            return RedirectToAction("Show");
        }


        /* shoppinglists/index ajax calls */

        [ValidateRouteUsername, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(ShoppingListsAddModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { Errors = ModelState.ToErrorList() });

            var result = _shoppingListService.AddShoppingList(model.Username, model.Name);

            if (result.AnyErrors())
            {
                ModelState.AddModelErrors(result);
                return Json(new { Errors = ModelState.ToErrorList() });
            }

            _uow.Commit();

            var list = result.Data.ShoppingList;

            return Json(new { Success = true, ShoppingList = new ShoppingListModel(list, Url) });
        }


        /* shoppinglists/show ajax calls */

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditItem(ShoppingListsEditItemModel model)
        {
            ModelState.AddModelErrors(_shoppingListItemService.EditShoppingListItem(model.ShoppingListId, model.Id, model.Name, model.Quantity ?? 0, model.Aisle ?? 0, model.Price ?? 0));

            if (!ModelState.IsValid)
                return Json(new { Errors = ModelState.ToErrorList() });

            _uow.Commit();
            return Json(new { Success = true });
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteItem(ShoppingListsDeleteItemModel model)
        {
            _shoppingListItemService.DeleteShoppingListItem(model.Id);
            _uow.Commit();
            return Json(new { Success = true });
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public void ChangePicked(int id, bool picked)
        {
            var item = _repository.Get<ShoppingListItem>(id);
            item.Picked = picked;
            _uow.Commit();
        }
    }

    public sealed class ShoppingListsAddModel
    {
        [Required]
        public string Username { get; set; }

        [Required, RegularExpression(@"^[A-Za-z]+[A-Za-z0-9-]*$", ErrorMessage = "Start with a letter and then letters, numbers, and dashes only")]
        public string Name { get; set; }
    }

    public sealed class ShoppingListsRenameModel
    {
        [Required]
        public string Username { get; set; }

        [Required, RegularExpression(@"^[A-Za-z]+[A-Za-z0-9-]*$", ErrorMessage = "Start with a letter and then letters, numbers, and dashes only")]
        public string Name { get; set; }

        [Required]
        public int Id { get; set; }
    }

    public sealed class ShoppingListsDeleteModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Id { get; set; }
    }

    public sealed class ShoppingListsClearModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Id { get; set; }

        public bool CheckedOnly { get; set; }
    }

    public class ShoppingListsEditItemModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ShoppingListId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Aisle { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }

    public class ShoppingListsDeleteItemModel
    {
        public int Id { get; set; }
    }
}
