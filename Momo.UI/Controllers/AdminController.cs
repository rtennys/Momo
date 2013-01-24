using System;
using System.Web.Mvc;

namespace Momo.UI.Controllers
{
    [AuthorizeActivity]
    public class AdminController : AppController
    {
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeActivity(Activity = "Foofoo")]
        public ActionResult Foo()
        {
            return Index();
        }
    }
}
