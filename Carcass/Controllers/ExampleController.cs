using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcExtensions;

using Carcass.Common.Data;
using Carcass.Common.MVC.Security;
using Carcass.Data;
using Carcass.Data.Entities;

namespace Carcass.Controllers
{
    public class ExampleController : Controller
    {
        public ExampleController(IQueryBuilder queryBuilder)
        {
            Query = queryBuilder;
        }

        protected IQueryBuilder Query { get; set; }


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

        [AuthorizeWithMessage("You must be logged in to get access to this page.")]
        [ImportViewDataFromTempData]
        public ActionResult AddBlogPost()
        {
            return View(new BlogPostEntity());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ExportViewDataToTempData]
        public ActionResult AddBlogPost(BlogPostEntity entity)
        {
            if (ModelState.IsValid)
            {
                // TODO: Save post

                TempData["Message"] = "Your post added successfully";
                return RedirectToAction("BlogSpace");
            }

            // If we got this far, something failed, redisplay form with error messages
            return RedirectToAction("AddBlogPost");
        }

        public ActionResult BlogSpace()
        {
            ViewBag.Message = TempData["Message"];

            var posts = Query.For<BlogPostEntity>().OrderByDescending(p => p.DateModified).ToList();
            return View(posts);
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
