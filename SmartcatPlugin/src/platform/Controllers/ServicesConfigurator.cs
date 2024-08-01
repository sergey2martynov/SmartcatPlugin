using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.DependencyInjection;

namespace SmartcatPlugin.Controllers
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(ServiceDescriptor.Transient(typeof(PageController),
                typeof(PageController)));

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(BasketController),
                typeof(BasketController)));

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(AddItemController),
                typeof(AddItemController)));
        }
    }
}