using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface ILookuper<T>
    {
        ISaver<T> Lookup(T entity);

        ISaver<T> LookupById(int id);
    }
}
