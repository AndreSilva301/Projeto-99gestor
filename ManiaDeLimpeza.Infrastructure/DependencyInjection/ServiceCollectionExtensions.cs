using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMarkedDependencies(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .ToArray();

            foreach (var type in assemblies.SelectMany(a => a.GetTypes()))
            {
                if (!type.IsClass || type.IsAbstract) continue;

                var interfaces = type.GetInterfaces();

                if (interfaces.Contains(typeof(IScopedDependency)))
                    RegisterMatchingInterface(services, type, ServiceLifetime.Scoped);
                else if (interfaces.Contains(typeof(ITransientDependency)))
                    RegisterMatchingInterface(services, type, ServiceLifetime.Transient);
                else if (interfaces.Contains(typeof(ISingletonDependency)))
                    RegisterMatchingInterface(services, type, ServiceLifetime.Singleton);
            }

            return services;
        }

        private static void RegisterMatchingInterface(IServiceCollection services, Type implementation, ServiceLifetime lifetime)
        {
            var serviceInterface = implementation.GetInterfaces()
                .FirstOrDefault(i =>
                    i != typeof(IScopedDependency) &&
                    i != typeof(ITransientDependency) &&
                    i != typeof(ISingletonDependency));

            if (serviceInterface != null)
            {
                services.Add(new ServiceDescriptor(serviceInterface, implementation, lifetime));
            }
        }
    }
}
