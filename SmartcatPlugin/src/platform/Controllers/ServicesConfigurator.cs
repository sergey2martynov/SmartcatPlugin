using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.DependencyInjection;
using System.Windows.Forms.Design;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Services;
using SmartcatPlugin.Smartcat;

namespace SmartcatPlugin.Controllers
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(SmartcatController),
                typeof(SmartcatController)));

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(BasketController),
                typeof(BasketController)));

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(AddItemController),
            typeof(AddItemController)));

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(ProjectListController),
                typeof(ProjectListController)));

            serviceCollection.AddScoped(typeof(IBasketService), typeof(BasketService));
            serviceCollection.AddScoped(typeof(IAuthService), typeof(AuthService));
            serviceCollection.AddScoped(typeof(ITranslationService), typeof(TranslationService));

            serviceCollection.AddScoped(typeof(ISmartcatApiClient), typeof(SmartcatApiClient));
        }
    }
}