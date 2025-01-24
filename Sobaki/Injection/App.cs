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
            services.AddSingleton<DogContext>();
            services.AddSingleton<ReceptionContext>();

            // бд
            services.AddSingleton<StrayDogzEntities>();

            // view models
            services.AddSingleton(MainViewModelFactory);
            services.AddSingleton(MainWindowFactory);

            services.AddTransient(AuthViewModelFactory);
            services.AddTransient(GuestViewModelFactory);
            services.AddTransient(AdminCallViewModelFactory);
            services.AddTransient(AdminPanelViewModelFactory);
            services.AddTransient(DogsViewModelFactory);
            services.AddTransient(DogsEditViewModelFactory);
            services.AddTransient(VetViewModelFactory);
            services.AddTransient(AddReceptionViewModelFactory);

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
                VetMainNavServiceFactory(p),
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
                DogsMainNavServiceFactory(p),
                GuestMainNavServiceFactory(p)
                );
        }
        protected DogsViewModel DogsViewModelFactory(IServiceProvider p)
        {
            return new DogsViewModel(
                BackOnlyMainNavServiceFactory(p),
                DogEditMainNavServiceFactory(p),
                p.GetRequiredService<DogContext>(),
                p.GetRequiredService<StrayDogzEntities>()
                );
        }
        protected DogEditViewModel DogsEditViewModelFactory(IServiceProvider p)
        {
            return new DogEditViewModel(
                BackOnlyMainNavServiceFactory(p),
                p.GetRequiredService<DogContext>(),
                p.GetRequiredService<StrayDogzEntities>()
                );
        }
        protected VetViewModel VetViewModelFactory(IServiceProvider p)
        {
            return new VetViewModel(
                GuestMainNavServiceFactory(p),
                ReceptionMainNavServiceFactory(p),
                DogEditMainNavServiceFactory(p),
                p.GetRequiredService<ReceptionContext>(),
                p.GetRequiredService<StrayDogzEntities>()
                );
        }
        protected AddReceptionViewModel AddReceptionViewModelFactory(IServiceProvider p)
        {
            return new AddReceptionViewModel(
                BackOnlyMainNavServiceFactory(p),
                p.GetRequiredService<ReceptionContext>(),
                p.GetRequiredService<UserContext>(),
                p.GetRequiredService<StrayDogzEntities>()
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
        protected MainNavService DogsMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<DogsViewModel>);
        }
        protected MainNavService DogEditMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<DogEditViewModel>);
        }
        protected MainNavService VetMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<VetViewModel>);
        }
        protected MainNavService ReceptionMainNavServiceFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<AddReceptionViewModel>);
        }
    }
}
