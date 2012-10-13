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

        public T Find<T>(int id)        
        {
            var finder = DependencyResolver.Current.GetService<IFinder<T>>();
            Throw.IfTrue(finder == null, (message) => new KeyNotFoundException(message), "Finder for {0} not found", typeof(T).Name);

            return finder.Find(id);
        }

        public T Find<T>(int id1, int id2)
        {
            var finder = DependencyResolver.Current.GetService<IFinder<T>>();
            Throw.IfTrue(finder == null, (message) => new KeyNotFoundException(message), "Finder for {0} not found", typeof(T).Name);

            return finder.Find(id1, id2);
        }
    }
}
