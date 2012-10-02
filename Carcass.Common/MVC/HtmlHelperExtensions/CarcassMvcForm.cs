using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Properties;

using Carcass.Common.Utility;

namespace Carcass.Common.MVC.HtmlHelperExtensions
{
    public class CarcassMvcFormContext : FormContext
    {
        public CarcassMvcFormContext(string formClass = null)
            : base()
        {
            FormClass = formClass;
        }

        public string FormClass { get; set; }
    }

    public class CarcassMvcForm : IDisposable
    {
        private readonly ViewContext _viewContext;
        private bool _disposed;

        public CarcassMvcForm(ViewContext viewContext, string formClass)
        {
            Throw.IfNullArgument(viewContext, "viewContext");
            _viewContext = viewContext;
            _viewContext.FormContext = new CarcassMvcFormContext(formClass);
            
            FormClass = formClass;
        }

        public string FormClass { get; set; }

        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize((object)this);
        }

        protected virtual void DoDispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                FormExtensions.CarcassEndForm(_viewContext);
            }
        }

        public void EndForm()
        {
            DoDispose();
        }
    }
}
