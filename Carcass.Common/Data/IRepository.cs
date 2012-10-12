using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface IRepository<T>
    {
        IQueryable<T> Source { get; }
    }
}
