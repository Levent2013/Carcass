using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcExtensions;
using WebMatrix.WebData;

using Carcass.Common.Data;
using Carcass.Common.MVC;
using Carcass.Common.MVC.Security;
using Carcass.Data;
using Carcass.Data.Entities;
using Carcass.Models;
using Carcass.Infrastructure;
using Carcass.Resources;
using Carcass.Common.Resources;

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
            return View(new Models.ComplexModel
                {
                    Date = ServerTime.Now,
                    Currency = 50.0m
                });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComplexForm(Models.ComplexModel complexModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ReturnUrl = Url.Action("ComplexForm", "Example");
                return View("Templates/DisplayModel", complexModel);
            }

            return View(complexModel);
        }

        public ActionResult CollectionTemplate(List<CarModel> models)
        {
            models = models ?? new List<CarModel>()
            {
                new CarModel { Id = 1, Brand = "Honda", Model = "Jazz", Price = 12200m, ProductionYear = 2005 },
                new CarModel { Id = 2, Brand = "Honda", Model = "Civic", Price = 10900m, ProductionYear = 2006 },
                new CarModel { Id = 3, Brand = "Acura", Model = "MDX", Price = 41100m, ProductionYear = 2013 },
            };

            return View("Templates/EditModel", models);
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
                if (String.IsNullOrWhiteSpace(model.Annotation))
                {
                    model.Annotation = MvcHelper.GetHtmlPreview(model.Content, 512);
                }

                Query.Lookup<BlogPost>(model).Save();

                TempData["Message"] = "Your post added successfully";
                return RedirectToAction("BlogSpace");
            }

            // If we got this far, something failed, redisplay form with error messages
            return RedirectToAction("AddBlogPost");
        }

        [AuthorizeWithMessage("You must be logged in to get access to this page.")]
        public ActionResult DeleteBlogPost(int id, string returnUrl)
        {
            Query.LookupById<BlogPost>(id).Remove();
            
            if(String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("BlogSpace");

            return Redirect(returnUrl);
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
    }
}
