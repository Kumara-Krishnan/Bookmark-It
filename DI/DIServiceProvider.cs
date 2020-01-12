using Bookmark_It.ViewModel;
using Bookmark_It.ViewModel.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark_It.DI
{
    public sealed class DIServiceProvider
    {
        public static DIServiceProvider Instance { get { return DIServiceProviderSingleton.Instance; } }

        private readonly IServiceProvider ServiceProvider;

        private DIServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<MainPageViewModelBase, MainPageViewModel>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        private class DIServiceProviderSingleton
        {
            static DIServiceProviderSingleton() { }

            internal static readonly DIServiceProvider Instance = new DIServiceProvider();
        }
    }
}
