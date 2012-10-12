using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface IQueryBuilder
    {
        IQueryable<T> For<T>();
    }
}
