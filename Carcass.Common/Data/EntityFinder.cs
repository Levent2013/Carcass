using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

using Autofac;
using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class EntityFinder<T, TContext> : IFinder<T>
    {
        private Delegate _loader;

        private TContext _context;

        public EntityFinder(TContext context, Func<TContext, int, T> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _loader = loader;
        }

        public EntityFinder(TContext context, Func<TContext, int, int, T> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _loader = loader;
        }

        public EntityFinder(TContext context, Func<TContext, int, int, int, T> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _loader = loader;
        }

        /// <summary>
        ///  Finds an entity with the given primary key value.
        /// </summary>
        /// <returns>The entity found, or null.</returns>
        public T Find(int id)
        {
            return CastResult(_loader.DynamicInvoke(_context, id));
        }

        /// <summary>
        ///  Finds an entity with the given primary key values
        /// </summary>
        /// <returns>The entity found, or null.</returns>
        public T Find(int id1, int id2)
        {
            return CastResult(_loader.DynamicInvoke(_context, id1, id2));
        }

        /// <summary>
        ///  Finds an entity with the given primary key values
        /// </summary>
        /// <returns>The entity found, or null.</returns>
        public T Find(int id1, int id2, int id3)
        {
            return CastResult(_loader.DynamicInvoke(_context, id1, id2, id3));
        }

        private static T CastResult(object res)
        {
            if (res == null)
            {
                return default(T);
            }

            if (!(res is T))
            {
                throw new InvalidCastException(
                    String.Format("Could not cast {0} to {1} in Finder<T>", res.GetType(), typeof(T)));
            }

            return (T)res;
        }
    }
}
