using System;
using System.Linq;
using System.Web.Mvc;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;

namespace Momo.UI.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;

        [Authorize]
        public ActionResult Index()
        {
            var model = _repository
                .Find<ShoppingList>()
                .Where(x => x.UserProfile.Username == User.Identity.Name)
                .OrderByDescending(x => x.Id)
                .ToArray();

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
