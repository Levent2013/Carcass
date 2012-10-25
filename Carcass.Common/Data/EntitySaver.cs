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
        private Action<TDest> _initializer;

        public EntitySaver(TSource source, TDest target, DbSet<TDest> targetTable, DbContext context, Action<TDest> initializer = null)
        {
            Throw.IfNullArgument(targetTable, "targetTable");
            Throw.IfNullArgument(context, "context");
            
            _source = source;
            _target = target;
            _context = context;
            _targetTable = targetTable;
            _initializer = initializer;
        }

        public bool IsNew 
        {
            get { return _target == null; }
        }

        public void SetSource(TSource source)
        {
            _source = source;
        }

        public TSource Save()
        {
            Throw.IfNullArgument(_source, "Could not save entity, source is not set");
            Throw.IfTrue(_target == null, (msg) => new InvalidOperationException(msg), "Database entity to update not found, source type: {0}", typeof(TSource));

            var isNew = false;
            var target = _target;
            if (target == null)
            {
                target = _targetTable.Create();
                isNew = true;
            }

            // copy the values
            _source.MapIntoDynamic(target);

            if (_initializer != null)
                _initializer(target);

            if(isNew)
            {
                _targetTable.Add(target);
            }

            try
            {
                _context.SaveChanges();
            } 
            catch (System.Data.Entity.Validation.DbEntityValidationException vex)
            {
                var sb = new StringBuilder();
                foreach (var error in vex.EntityValidationErrors)
                {
                    var messages = error.ValidationErrors.Select(p => p.ErrorMessage).ToArray();
                    sb.Append(String.Join(System.Environment.NewLine, messages))
                        .Append(System.Environment.NewLine);
                }

                throw new ArgumentException(sb.ToString(), vex);
                
            }

            return target.MapTo<TSource>();
        }

        public void Remove()
        {
            Throw.IfTrue(_target == null, (msg) => new InvalidOperationException(msg), "Database entity to remove not found, source type: {0}", typeof(TSource));
            _targetTable.Remove(_target);
            _context.SaveChanges();
        }
    }
}
