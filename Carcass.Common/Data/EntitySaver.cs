using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

using Carcass.Common.Utility;
using Carcass.Common.Data.Extensions;

namespace Carcass.Common.Data
{
    internal class EntitySaver<TSource, TDest> : ISaver<TSource> where TDest : class
    {
        private TSource _source;
        private TDest _target;
        private DbContext _context;
        private DbSet<TDest> _targetTable;
        
        public EntitySaver(TSource source, TDest target, DbSet<TDest> targetTable, DbContext context)
        {
            Throw.IfNullArgument(source, "source");
            Throw.IfNullArgument(targetTable, "targetTable");
            Throw.IfNullArgument(context, "context");
            
            _source = source;
            _target = target;
            _context = context;
            _targetTable = targetTable;
        }

        public bool IsNew 
        {
            get { return _target == null; }
        }

        public TSource Save()
        {
            var isNew = false;
            var target = _target;
            if (target == null)
            {
                target = _targetTable.Create();
                isNew = true;
            }

            // copy the values
            _source.MapIntoDynamic(target);

            if(isNew)
            {
                _targetTable.Add(target);
            }
                        
            _context.SaveChanges();

            return target.MapTo<TSource>();
        }

        public TSource Update()
        {
            Throw.IfTrue(_target == null, (msg) => new InvalidOperationException(msg), "Database entity to update not found, source type: {0}", typeof(TSource));
            return Save();
        }

        public TSource Insert()
        {
            Throw.IfTrue(_target != null, (msg) => new InvalidOperationException(msg), "Database entity to insert already exists: {0}", _target);
            return Save();
        }

        public void Remove()
        {
            Throw.IfTrue(_target == null, (msg) => new InvalidOperationException(msg), "Database entity to remove not found, source type: {0}", typeof(TSource));
            _targetTable.Remove(_target);
            _context.SaveChanges();
        }
    }
}
