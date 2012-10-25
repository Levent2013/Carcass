using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface ISaver<T>
    {
        bool IsNew { get; }

        void SetSource(T source);

        T Save();


        void Remove();
    }
}
