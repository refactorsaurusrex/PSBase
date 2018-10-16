using System;
using SimpleInjector;

namespace PSBase
{
    internal sealed class PSApplication : IDisposable
    {
        private Container _container;
        private bool _isRegistered;

        internal static PSApplication Create() => new PSApplication();

        private PSApplication() => _container = new Container();

        public TService Register<TService>() where TService : class
        {
            if (_isRegistered)
                return _container.GetInstance<TService>();

            _container = new Container();
            _container.Register<ITokenManager, TokenManager>();
            _container.Verify();

            _isRegistered = true;
            return _container.GetInstance<TService>();
        }

        public void Dispose()
        {
            _container?.Dispose();
            _container = null;
            _isRegistered = false;
        }
    }
}