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
            services.AddSingleton<UserContext>();

            // бд
            services.AddSingleton<StrayDogzEntities>();

            // view models
            services.AddSingleton(MainViewModelFactory);
            services.AddSingleton(MainWindowFactory);

            services.AddTransient(AuthViewModelFactory);
            services.AddTransient(GuestViewModelFactory);
            services.AddTransient(AdminCallViewModelFactory);
            services.AddTransient(AdminPanelViewModelFactory);

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
                GuestMainNavServiceFactory(p),
                p.GetRequiredService<MainContext>()
                );
        }
        protected AuthViewModel AuthViewModelFactory(IServiceProvider p)
        {
            return new AuthViewModel(
                BackOnlyMainNavServiceFactory(p),
                AdminPanelMainNavServiceFactory(p),
                p.GetRequiredService<UserContext>(),
                p.GetRequiredService<StrayDogzEntities>()
                );
        }
        protected GuestViewModel GuestViewModelFactory(IServiceProvider p)
        {
            return new GuestViewModel(
                AuthMainNavServiceFactory(p),
                AdminCallMainNavServiceFactory(p),
                p.GetRequiredService<StrayDogzEntities>()
                );
        }
        protected AdminCallViewModel AdminCallViewModelFactory(IServiceProvider p)
        {
            return new AdminCallViewModel(BackOnlyMainNavServiceFactory(p));
        }
        protected AdminPanelViewModel AdminPanelViewModelFactory(IServiceProvider p)
        {
            return new AdminPanelViewModel(
                GuestMainNavServiceFactory(p),
                GuestMainNavServiceFactory(p),
                GuestMainNavServiceFactory(p),
                GuestMainNavServiceFactory(p)
                );
        }

        // окна
        protected MainWindow MainWindowFactory(IServiceProvider p)
        {
            return new MainWindow
            {
                DataContext = p.GetRequiredService<MainViewModel>(),
            };
        }

        // nav services
        protected MainNavService BackOnlyMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>());
        }
        protected MainNavService AuthMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<AuthViewModel>);
        }
        protected MainNavService GuestMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<GuestViewModel>);
        }
        protected MainNavService AdminCallMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<AdminCallViewModel>);
        }
        protected MainNavService AdminPanelMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<AdminPanelViewModel>);
        }
    }
}
