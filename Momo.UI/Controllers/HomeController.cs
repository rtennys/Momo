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

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Throw(string msg = null)
        {
            throw new Exception(msg ?? "throwing an exception for your pleasure");
        }
    }
}
