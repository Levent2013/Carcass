using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class Finder<T> : IFinder<T>
    {
        private Delegate _loader;

        public Finder(Func<int, T> loader)
        {
            Throw.IfNullArgument(loader, "loader");
            _loader = loader;
        }

        public Finder(Func<int, int, T> loader)
        {
            Throw.IfNullArgument(loader, "loader");
            _loader = loader;
        }

        public Finder(Func<int, int, int, T> loader)
        {
            Throw.IfNullArgument(loader, "loader");
            _loader = loader;
        }

        /// <summary>
        ///  Finds an entity with the given primary key values.
        /// </summary>
        /// <returns>The entity found, or null.</returns>
        public T Find(int id)
        {
            var res = _loader.DynamicInvoke(id);
            return CastResult(res);
        }

        public T Find(int id1, int id2)
        {
            var res = _loader.DynamicInvoke(id1, id2);
            return CastResult(res);
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
