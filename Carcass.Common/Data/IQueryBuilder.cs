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

        /// <summary>
        /// Lookup entity of desired type to save (update/insert)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity to lookup</param>
        /// <returns></returns>
        ISaver<T> Lookup<T>(T entity);
        
        ISaver<T> LookupById<T>(int id);
    }
}
