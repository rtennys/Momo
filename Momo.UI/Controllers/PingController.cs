using System;
using System.Linq;
using System.Web.Mvc;
using Momo.Domain;
using Momo.Domain.Entities;

namespace Momo.UI.Controllers
{
    public class PingController : Controller
    {
        public PingController(IRepository repository)
        {
            _repository = repository;
        }

        private readonly IRepository _repository;

        public ActionResult Index()
        {
            var errorLevels = new[] {"Error", "Fatal"};

            var model = new PingIndexModel
            {
                LastError = _repository.Find<Log>()
                    .Where(x => errorLevels.Contains(x.Level))
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault()
            };

            var acceptTypes = Request.AcceptTypes.Join(", ");
            if (string.IsNullOrEmpty(acceptTypes) || acceptTypes.Contains("json"))
                return Json(model, JsonRequestBehavior.AllowGet);

            return View(model);
        }
    }

    public class PingIndexModel
    {
        public Log LastError { get; set; }
    }
}
