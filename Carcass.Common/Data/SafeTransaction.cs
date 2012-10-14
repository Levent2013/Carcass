using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcExtensions;
using Carcass.Common.Utility;

namespace Carcass.Common.Data
{
    public class SafeTransaction : IDisposable
    {
        private TransactionScope _transaction;
        
        public SafeTransaction(TransactionScopeOption scopeOption = TransactionScopeOption.RequiresNew)
        {
            if (Database.DefaultConnectionFactory is SqlCeConnectionFactory)
                _transaction = null;
            else
                _transaction = new TransactionScope(scopeOption);
        }

        public SafeTransaction(TransactionScopeOption scopeOption, IsolationLevel isolationLevel)
        {
            if (Database.DefaultConnectionFactory is SqlCeConnectionFactory)
                _transaction = null;
            else
                _transaction = new TransactionScope(scopeOption,
                    new TransactionOptions { IsolationLevel = isolationLevel });
        }

        ~SafeTransaction()
        {
            Dispose();
        }

        public void Complete()
        {
            if (_transaction != null)
            {
                _transaction.Complete();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
    