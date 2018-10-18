using System;
using SimpleInjector;

namespace PSBase
{
    public abstract class PSApplicationBase : IDisposable
    {
        protected Container Container;
        private bool _isRegistered;

        protected PSApplicationBase() => Container = new Container();

        internal TService Register<TService>() where TService : class
        {
            if (_isRegistered)
                return Container.GetInstance<TService>();

            Register();
            Container.Register<ITokenManager, TokenManager>();
            Container.Verify();

            _isRegistered = true;
            return Container.GetInstance<TService>();
        }

        protected abstract void Register();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container?.Dispose();
                Container = null;
                _isRegistered = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}