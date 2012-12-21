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

        public ActionResult Index(string username)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

            var model = new ShoppingListsIndexModel
                        {
                            ShowNew = string.Equals(User.Identity.Name, user.Username, StringComparison.OrdinalIgnoreCase),
                            ShoppingLists = user.ShoppingLists.OrderByDescending(x => x.Id).Select(x => x.Name).ToArray()
                        };

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

            var model = new ShoppingListsShowModel
                        {
                            Id = shoppingList.Id,
                            Name = shoppingList.Name
                        };

            return View(model);
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

            ModelState.AddModelErrors(_commandExecutor.Execute<AddShoppingListCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Index");
        }


        [Authorize]
        public ActionResult Rename(string username, string shoppinglist)
        {
            var user = _repository.Get<UserProfile>(x => x.Username == username);
            if (user == null)
                return HttpNotFound();

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

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Rename(ShoppingListsRenameModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ModelState.AddModelErrors(_commandExecutor.Execute<RenameShoppingListCommand>(model));
            if (!ModelState.IsValid)
                return View(model);

            _uow.Commit();
            return RedirectToAction("Index");
        }


        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ModelState.AddModelErrors(_commandExecutor.Execute(new DeleteShoppingListCommand {Username = User.Identity.Name, Id = id}));

            if (ModelState.IsValid)
                _uow.Commit();

            return RedirectToAction("Index");
        }
    }
}
