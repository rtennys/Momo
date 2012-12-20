using System;
using System.Linq;
using System.Web.Mvc;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;
using Momo.UI.Models;

namespace Momo.UI.Controllers
{
    public class ShoppingListsController : Controller
    {
        public ShoppingListsController(IUnitOfWork uow, IRepository repository)
        {
            _uow = uow;
            _repository = repository;
        }

        private readonly IUnitOfWork _uow;
        private readonly IRepository _repository;

        public ActionResult Index(string username)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var model = user
                .ShoppingLists
                .OrderByDescending(x => x.Id)
                .ToArray();

            return View(model);
        }

        public ActionResult Show(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var shoppingList = user.ShoppingLists.FirstOrDefault(x => string.Equals(x.Name, shoppinglist, StringComparison.OrdinalIgnoreCase));
            if (shoppingList == null)
                return HttpNotFound();

            return View(shoppingList);
        }


        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(ShoppingListsAddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _repository.Get<UserProfile>(x => x.Username == User.Identity.Name);

            user.CreateShoppingList(model.Name);
            _uow.Commit();

            return RedirectToAction("Index");
        }
    }
}
