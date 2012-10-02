using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carcass.Data;

namespace Carcass.Controllers
{
    public class ExampleController : Controller
    {
        public ExampleController(DatabaseContext db)
        {
            Db = db;
        }

        private DatabaseContext Db { get; set; }

        public ActionResult ComplexForm()
        {
            return View(new Models.ComplexModel());
        }

        [HttpPost]
        public ActionResult ComplexForm(Models.ComplexModel complexModel)
        {
            return View(complexModel);
        }
    }
}
