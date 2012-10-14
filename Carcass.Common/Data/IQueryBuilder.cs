using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface IQueryBuilder
    {
        IQueryable<T> For<T>();

        IQueryable<T> For<T>(Func<T, bool> precondition);

        T Find<T>(int id);

        T Find<T>(int id1, int id2);
    }
}
