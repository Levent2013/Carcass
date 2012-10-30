using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Carcass.Common.Utility;

namespace Carcass.Common.MVC.Html
{
    public static class HtmlRenderer
    {
        /// <summary>
        /// Render simple 'select' control. <c>IsSelected</c> property 
        /// of items will be used to select option.
        /// </summary>
        /// <param name="items">Items list</param>
        /// <returns>Formatted HTML</returns>
        public static IHtmlString Dropdown(string fieldName, IEnumerable<SelectListItem> items)
        {
            Throw.IfNullArgument(items, "items");

            var tb = new TagBuilder("select");

            tb.MergeAttribute("name", fieldName);
            tb.GenerateId(fieldName);

            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var opt = new TagBuilder("option");
                if(!String.IsNullOrEmpty(item.Value))
                    opt.MergeAttribute("value", item.Value);
                
                if (item.Selected)
                    opt.MergeAttribute("selected", "selected");
                
                opt.InnerHtml = item.Text;

                sb.Append(opt.ToString(TagRenderMode.Normal));
            }

            tb.InnerHtml = sb.ToString();
            
            return MvcHtmlString.Create(tb.ToString());
        }
    }
}
