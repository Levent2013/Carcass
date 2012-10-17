using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcExtensions;
using WebMatrix.WebData;

using Carcass.Common.Data;
using Carcass.Common.MVC.Security;
using Carcass.Data;
using Carcass.Data.Entities;
using Carcass.Models;
using Carcass.Infrastructure;
using Carcass.Resources;

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
                    Date = ServerTime.Now,
                    Currency = 50.0m
                });
        }

        public ActionResult Bootbox()
        {
            return View();
        }

        [AuthorizeWithMessage("You must be logged in to get access to this page.")]
        [ImportViewDataFromTempData(Key = "AddBlogPost")]
        public ActionResult AddBlogPost()
        {
            return View("EditBlogPost", new BlogPost { AuthorId = WebSecurity.CurrentUserId });
        }

        [HttpPost]
        [AuthorizeWithMessage("You must be logged in to get access to this page.")]
        [ValidateAntiForgeryToken]
        [ExportViewDataToTempData(Key = "AddBlogPost")]
        public ActionResult AddBlogPost(BlogPost model)
        {
            if (ModelState.IsValid)
            {
                Query.Lookup<BlogPost>(model).Save();

                TempData["Message"] = "Your post added successfully";
                return RedirectToAction("BlogSpace");
            }

            // If we got this far, something failed, redisplay form with error messages
            return RedirectToAction("AddBlogPost");
        }

        public ActionResult BlogSpace()
        {
            TempData.Remove("AddBlogPost");
            
            ViewBag.Message = TempData["Message"];
            ViewBag.Title = ExamplesResources.BlogSpace;
            
            var posts = Query.For<BlogPost>().OrderByDescending(p => p.DateModified).ToList();
            return View(posts);
        }

        public ActionResult UserBlog(int id, string username)
        {
            var user = Query.Find<User>(id);

            ViewBag.Title = String.Format(
                ExamplesResources.UserBlog,
                user == null ? AccountResources.UnknownUser : user.FullName);
            var posts = Query.For<BlogPost>(p => p.AuthorId == id)
                .OrderByDescending(p => p.DateModified).ToList();
            return View("BlogSpace", posts);
        }

        public ActionResult ViewBlogPost(int id)
        {
            var post = Query.Find<BlogPost>(id);
            if (post == null)
                return HttpNotFound("Post not found");

            return View(post);
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
