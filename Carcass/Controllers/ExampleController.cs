using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carcass.Data;
using MvcExtensions;

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
            // Use dynamic typing without @model usage on view
            return View(new Models.ComplexModel
                {
                    Date = DateTime.Now,
                    Currency = 50.0m
                });
        }

        public ActionResult Bootbox()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ComplexForm(Models.ComplexModel complexModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ReturnUrl = Url.Action("ComplexForm", "Example");
                return View("DisplayModel", complexModel);
            }

            return View(complexModel);
        }
    }
}
