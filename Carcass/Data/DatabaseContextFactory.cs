using System;

namespace Carcass.Data
{
    public class DatabaseContextFactory : IDisposable
    {
        private DatabaseContext _context;
        private object _lock = new object();

        ~DatabaseContextFactory()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
            }
        }

        public DatabaseContext Get()
        {
            if (_context == null)
            {
                lock (_lock)
                {
                    if (_context == null)
                    {
                        _context = new DatabaseContext();
                    }
                }
            }

            return _context;
        }
    }
}