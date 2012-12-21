using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    public class HomeController : AppController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
