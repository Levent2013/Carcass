using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class QueryBuilder : Carcass.Common.Data.IQueryBuilder
    {
        public IQueryable<T> For<T>()
        {
            var repository = DependencyResolver.Current.GetService<IRepository<T>>();
            Throw.IfTrue(repository == null, (message) => new KeyNotFoundException(message), "Repository for {0} not found", typeof(T).Name);
            
            return repository.Source;
        }
    }
}
