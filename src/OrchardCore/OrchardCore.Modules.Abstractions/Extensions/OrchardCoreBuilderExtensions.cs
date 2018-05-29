using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace OrchardCore.Modules.Extensions
{
    public static class OrchardCoreBuilderExtensions
    {
        /// <summary>
        /// Adds tenant level configuration to serve static files from modules
        /// </summary>
        public static OrchardCoreBuilder UseStaticFiles(this OrchardCoreBuilder builder)
        {
            builder.Startup.Configure((tenant, routes, sp) =>
            {
                tenant.UseStaticFiles();
            });

            return builder;
        }

        public static TenantApplicationBuilder UseStaticFiles(this TenantApplicationBuilder tenant)
        {
            var env = tenant.ApplicationBuilder.ApplicationServices.GetRequiredService<IHostingEnvironment>();

            IFileProvider fileProvider;
            if (env.IsDevelopment())
            {
                var fileProviders = new List<IFileProvider>();
                fileProviders.Add(new ModuleProjectStaticFileProvider(env));
                fileProviders.Add(new ModuleEmbeddedStaticFileProvider(env));
                fileProvider = new CompositeFileProvider(fileProviders);
            }
            else
            {
                fileProvider = new ModuleEmbeddedStaticFileProvider(env);
            }

            tenant.ApplicationBuilder.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "",
                FileProvider = fileProvider
            });

            return tenant;
        }
    }
}
