using System;

namespace Nimble
{
    internal abstract class Disposable: IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                FreeUnmanagedResources();
            }
        }

        protected abstract void FreeUnmanagedResources();
    }
}
