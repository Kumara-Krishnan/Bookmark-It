using Bookmark_It.ViewModel;
using Bookmark_It.ViewModel.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Util;

namespace Bookmark_It.DI
{
    public sealed class DIServiceProvider : DIServiceProviderBase
    {
        public static DIServiceProvider Instance { get { return DIServiceProviderSingleton.Instance; } }

        private DIServiceProvider() { }

        protected override void AddServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MainPageViewModelBase, MainPageViewModel>();
        }

        private class DIServiceProviderSingleton
        {
            static DIServiceProviderSingleton() { }

            internal static readonly DIServiceProvider Instance = new DIServiceProvider();
        }
    }
}
