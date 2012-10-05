using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Carcass.Common.Utility;

namespace Carcass.Common.MVC.DataTypes.Binding
{
    public class PostedFileArrayBinder : IModelBinder
    {
        /// <summary>
        /// Load posted files list into model field
        /// </summary>
        /// <param name="controllerContext">Current controller context</param>
        /// <param name="bindingContext">Current binding context</param>
        /// <returns>Array of <see cref="PostedFile"/></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == null || result.RawValue == null)
            {
                return null;
            }
            
            var inputFile = result.RawValue as System.Web.HttpPostedFileBase;
            if (inputFile != null)
            {
                return new PostedFile(inputFile);
            }

            var inputFiles = result.RawValue as System.Web.HttpPostedFileBase[];
            Throw.IfBadArgument(() => inputFiles == null, "Unsupported input for file upload model");
            
            return inputFiles.Where(p => p != null).Select(p => new PostedFile(p)).ToArray();
        }

    }
}
