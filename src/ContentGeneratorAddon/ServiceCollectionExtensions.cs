using System;
using System.Linq;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace ContentGeneratorAddon
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContentGeneratorAddon(this IServiceCollection services)
        {
            services.Configure<ProtectedModuleOptions>(
                pm =>
                {
                    if (!pm.Items.Any(i =>
                        i.Name.Equals("content-generator-addon", StringComparison.OrdinalIgnoreCase)))
                    {
                        pm.Items.Add(new ModuleDetails { Name = "content-generator-addon" });
                    }
                });

            return services;
        }
    }
}
