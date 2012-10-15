using System;
using System.Collections.Generic;
using System.Linq;

namespace Carcass.Common.Data
{
    public interface ISaver<T>
    {
        bool IsNew { get; }

        T Save();

        T Update();

        T Insert();

        void Remove();
    }
}
