﻿using Ninject;
using Ninject.Modules;
using Whip.MessageHandlers;
using Whip.ViewModels.MessageHandlers;

namespace Whip.Ioc
{
    public static class IocKernel
    {
        private static StandardKernel _kernel;

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Initialize(params INinjectModule[] modules)
        {
            if (_kernel == null)
            {
                _kernel = new StandardKernel(modules);
                _kernel.RegisterComponents();
            }
        }

        public static void StartMessageHandlers()
        {
            _kernel.Get<DialogMessageHandler>().Start();
            _kernel.Get<LibraryHandler>().Start();
            _kernel.Get<ShowTabRequestHandler>().Start();
        }

        public static void StopMessageHandlers()
        {
            _kernel.Get<DialogMessageHandler>().Stop();
            _kernel.Get<LibraryHandler>().Stop();
            _kernel.Get<ShowTabRequestHandler>().Stop();
        }
    }
}
