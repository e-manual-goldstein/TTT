using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TTT.Common;

namespace TTT.Tools
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            Console.WriteLine("Hello World!");
            var socketManager = _serviceProvider.GetService<SocketManager>();
            Task.Run(async () => 
                await socketManager.Listen()
                );
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MessageHandler>();
            services.AddSingleton<SocketManager>();
            services.AddSingleton<Logger>();
            services.AddSingleton<ControllerManager>(provider =>
            {
                return new ControllerManager(provider, provider.GetService<Logger>(), new Type[]
                {
             
                });
            });
        }
    }
}
