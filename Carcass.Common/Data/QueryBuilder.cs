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

        public IQueryable<T> For<T>(Func<T, bool> precondition)
        {
            var source = For<T>();
            return source.Where(precondition).AsQueryable();
        }
        
        public T Find<T>(int id)        
        {
            return GetFinder<T>().Find(id);
        }

        public T Find<T>(int id1, int id2)
        {
            return GetFinder<T>().Find(id1, id2);
        }

        /// <summary>
        /// Lookup entity of desired type to save (update/insert)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity to lookup</param>
        /// <returns></returns>
        public ISaver<T> Lookup<T>(T entity)
        {
            Throw.IfNullArgument(entity, "entity");
            return GetLookuper<T>().Lookup(entity);
        }

        public ISaver<T> LookupById<T>(int id)
        {
            return GetLookuper<T>().LookupById(id);
        }

        private static ILookuper<T> GetLookuper<T>()
        {
            var lookuper = DependencyResolver.Current.GetService<ILookuper<T>>();
            Throw.IfTrue(lookuper == null, (message) => new KeyNotFoundException(message), "Lookuper for {0} not found", typeof(T).Name);
            return lookuper;
        }

        private static IFinder<T> GetFinder<T>()
        {
            var finder = DependencyResolver.Current.GetService<IFinder<T>>();
            Throw.IfTrue(finder == null, (message) => new KeyNotFoundException(message), "Finder for {0} not found", typeof(T).Name);
            return finder;
        }
    }
}
