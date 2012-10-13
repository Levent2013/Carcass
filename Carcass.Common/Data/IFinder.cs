using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface IFinder<T>
    {
        /// <summary>
        ///  Finds an entity with the given primary key value.
        /// </summary>
        /// <returns>The entity found, or null.</returns>
        T Find(int id);

        T Find(int id1, int id2);
    }
}
