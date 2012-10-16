using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class EntityLookuper<TSource, TDest> : ILookuper<TSource> where TDest: class
    {
        private DbContext _context;
        private DbSet<TDest> _table;
        private Delegate _loader;
        
        public EntityLookuper(DbContext context, DbSet<TDest> table, Func<TSource, int> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(table, "table");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _table = table;
            _loader = loader;
        }

        public EntityLookuper(DbContext context, DbSet<TDest> table, Func<TSource, object> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(table, "table");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _table = table;
            _loader = loader;
        }

        public EntityLookuper(DbContext context, DbSet<TDest> table, Func<TSource, object[]> loader)
        {
            Throw.IfNullArgument(context, "context");
            Throw.IfNullArgument(table, "table");
            Throw.IfNullArgument(loader, "loader");

            _context = context;
            _table = table;
            _loader = loader;
        }

        public ISaver<TSource> Lookup(TSource entity)
        {
            var primaryKey = _loader.DynamicInvoke(entity);
            TDest target;
            
            if (primaryKey is object[])
            {
                var keys = primaryKey as object[];
                switch (keys.Length)
                {
                    case 3:
                        target = _table.Find(keys[0], keys[1], keys[2]);
                        break;
                    case 2:
                        target = _table.Find(keys[0], keys[1]);
                        break;
                    case 1:
                        target = _table.Find(keys[0]);
                        break;
                    default:
                        throw new NotImplementedException("Lookup for entities with complex PK from more than 3 columns is not suppported");
                }
            }
            else
            {
                target = _table.Find(primaryKey);
            }

            return new EntitySaver<TSource, TDest>(entity, target, _table, _context);
        }
    }
}
