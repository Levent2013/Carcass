using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public static class BootstrapExtensions
    {
        /// <summary>
        /// Render Boostrap's alert close button
        /// </summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <returns>HTML button representation</returns>
        public static IHtmlString BoostrapCloseBtn(this HtmlHelper html)
        {
            var tb = RenderCloseBtn();
            return MvcHtmlString.Create(tb.ToString());
        }

        private static TagBuilder RenderCloseBtn()
        {
            // <button type="button" class="close" data-dismiss="alert">×</button>
            var tb = new TagBuilder("button");
            tb.AddCssClass("close");
            tb.Attributes.Add("type", "button");
            tb.Attributes.Add("data-dismiss", "alert");
            tb.InnerHtml = "×";
            return tb;
        }
    }
}
