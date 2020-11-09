using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi
{
    public class AppDbContextScope: IDisposable
    {
        private static ServiceProvider CreateServiceProvider(DbConnection connection = null)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(builder =>
                    builder
                        .AddConfiguration(AppConfig.Config.GetSection("Logging"))
                        .AddConsole()
                );
            Startup.ConfigureEntityFramework(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static Lazy<ServiceProvider> DefaultLazyServiceProvider = new Lazy<ServiceProvider>(() => {
            return CreateServiceProvider();
        });

        private IServiceScope _scope;

        public AppDbContextScope()
        {
            var serviceProvider = DefaultLazyServiceProvider.Value;
            _scope = serviceProvider.CreateScope();
        }

        public AppDbContextScope(DbConnection connection = null)
        {
            var serviceProvider = CreateServiceProvider(connection);
            _scope = serviceProvider.CreateScope();
        }

        public AppDbContext AppDb => _scope.ServiceProvider.GetService<AppDbContext>();

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}
