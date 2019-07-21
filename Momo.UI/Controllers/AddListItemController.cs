using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Momo.Domain;
using Momo.Domain.Entities;
using Momo.Domain.ShoppingLists;

namespace Momo.UI.Controllers
{
    [Authorize]
    public sealed class AddListItemController : AppController
    {
        public AddListItemController(IUnitOfWork uow, IRepository repository, IShoppingListItemService shoppingListItemService)
        {
            _uow = uow;
            _repository = repository;
            _shoppingListItemService = shoppingListItemService;
        }

        private readonly IUnitOfWork _uow;
        private readonly IRepository _repository;
        private readonly IShoppingListItemService _shoppingListItemService;

        [ValidateShoppingListAccess]
        public ActionResult Index(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            return View(new AddShoppingListItemModel {ShoppingListId = shoppingList.Id});
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(AddShoppingListItemModel model)
        {
            if (!ModelState.IsValid)
                return Json(new {Errors = ModelState.ToErrorList()});

            var result = _shoppingListItemService.AddShoppingListItem(model.ShoppingListId, model.Name, model.Quantity ?? 1, model.Aisle ?? 0);

            if (result.AnyErrors())
            {
                ModelState.AddModelErrors(result);
                return Json(new {Errors = ModelState.ToErrorList()});
            }

            _uow.Commit();

            return Json(new {Success = true});
        }

        [ValidateShoppingListAccess, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string username, string shoppinglist, string name)
        {
            var model = _repository
                .Find<ShoppingListItem>()
                .Where(x => x.ShoppingList.UserProfile.Username == username)
                .Where(x => x.ShoppingList.Name == shoppinglist)
                .Where(x => x.Name.Contains(name))
                .OrderBy(x => x.Aisle)
                .ThenBy(x => x.Name)
                .Select(x => new {x.Name, x.Aisle, Quantity = Math.Max(1, x.Quantity), OnList = !x.Picked && x.Quantity > 0, IsNew = false})
                .ToList();

            if (model.All(x => !string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
                model.Add(new {Name = name.ToTitleCase(), Aisle = 0, Quantity = 1, OnList = false, IsNew = true});

            return Json(model);
        }
    }

    public class AddShoppingListItemModel
    {
        [Required]
        public int ShoppingListId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Aisle { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
        public int? Quantity { get; set; }
    }
}
