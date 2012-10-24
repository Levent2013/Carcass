using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Carcass.Common.Utility;
using Carcass.Common.Data.Extensions;

namespace Carcass.Common.Data
{
    public class EntityRepository<TDbContext, T> : IRepository<T> 
        where T : class
        where TDbContext : DbContext
    {
        private IQueryable<T> _source;

        public EntityRepository(IComponentContext container, Func<TDbContext, DbSet<T>> loader)
        {
            Throw.IfNullArgument(container, "container");
            Throw.IfNullArgument(loader, "loader");

            var context = container.Resolve<TDbContext>();
            _source = loader(context).AsNoTracking();
        }

        public IQueryable<T> Source
        {
            get { return _source; }
        }
    }

    public class EntityRepository<TDbContext, TSource, TDest> : IRepository<TDest>
        where TSource : class
        where TDbContext : DbContext
    {
        private IQueryable<TDest> _source;

        public EntityRepository(IComponentContext container, Func<TDbContext, DbSet<TSource>> loader, Func<TSource, TDest> selector)
        {
            Throw.IfNullArgument(container, "container");
            Throw.IfNullArgument(loader, "loader");
            Throw.IfNullArgument(selector, "selector");

            var context = container.Resolve<TDbContext>();
            _source = loader(context).AsNoTracking().Select(p => selector(p));
        }

        public IQueryable<TDest> Source
        {
            get { return _source; }
        }
    }
}
