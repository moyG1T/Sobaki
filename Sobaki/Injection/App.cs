using Microsoft.Extensions.DependencyInjection;
using Sobaki.Data.Remote;
using Sobaki.Domain.Contexts;
using Sobaki.Domain.Services;
using Sobaki.ViewModels;
using System;
using System.Windows;

namespace Sobaki
{
    public partial class App
    {
        private readonly IServiceProvider _provider;

        public App()
        {
            var services = new ServiceCollection();

            // контексты
            services.AddSingleton<MainContext>();

            // бд
            services.AddSingleton<StrayDogzEntities>();

            // view models
            services.AddSingleton(MainViewModelFactory);
            services.AddSingleton(MainWindowFactory);

            _provider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = _provider.GetRequiredService<MainWindow>();

            MainWindow.Show();

            base.OnStartup(e);
        }

        // view models
        protected MainViewModel MainViewModelFactory(IServiceProvider p)
        {
            return new MainViewModel(
                AuthMainNavServiceFactory(p), 
                AuthMainNavServiceFactory(p), 
                p.GetRequiredService<MainContext>()
                );
        }
        protected MainWindow MainWindowFactory(IServiceProvider p)
        {
            return new MainWindow
            {
                DataContext = p.GetRequiredService<MainViewModel>(),
            };
        }

        // nav services
        protected MainNavService AuthMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>());
        }
    }
}
