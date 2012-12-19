using System;
using System.Linq;
using System.Web.Mvc;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;
using Momo.UI.Models;

namespace Momo.UI.Controllers
{
    [Authorize]
    public class ShoppingListsController : Controller
    {
        public ShoppingListsController(IUnitOfWork uow, IRepository repository)
        {
            _uow = uow;
            _repository = repository;
        }

        private readonly IUnitOfWork _uow;
        private readonly IRepository _repository;

        public ActionResult Index()
        {
            var model = _repository
                .Find<ShoppingList>()
                .Where(x => x.UserProfile.Username == User.Identity.Name)
                .OrderByDescending(x => x.Id)
                .ToArray();

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

            var user = _repository.Get<UserProfile>(x => x.Username == User.Identity.Name);

            user.CreateShoppingList(model.Name);
            _uow.Commit();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Show(int id)
        {
            return Content("Coming soon...");
        }
    }
}
