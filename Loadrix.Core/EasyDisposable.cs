using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core
{
    public abstract class EasyDisposable : IDisposable
    {
        public void Dispose()
        {
            if (IsDisposed) return;
            DoDispose();
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        protected abstract void DoDispose();

        public bool IsDisposed { get; private set; }
    }
}
