using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class Repository<T> : IRepository<T>
    {
        private IQueryable<T> _source;

        public Repository(IQueryable<T> source)
        {
            Throw.IfNullArgument(source, "source");            
            _source = source;
        }

        public Repository(IEnumerable<T> source)
        {
            Throw.IfNullArgument(source, "source");
            _source = source.AsQueryable();
        }

        public IQueryable<T> Source 
        { 
            get { return _source; }  
        }
    }
}
