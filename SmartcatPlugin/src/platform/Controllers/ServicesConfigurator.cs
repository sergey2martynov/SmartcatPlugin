using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.DependencyInjection;
using System.Windows.Forms.Design;
using SmartcatPlugin.Interfaces;
using SmartcatPlugin.Services;

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

            serviceCollection.AddScoped(typeof(IBasketService), typeof(BasketService));
        }
    }
}