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
            var portManager = _serviceProvider.GetService<PortManager>();
            portManager.AddPort();
            var socketManager = _serviceProvider.GetService<SocketManager>();
            //var protocols = portManager.GetPorts();
            //Task.Run(async () =>
            //{
            //    var message = await socketManager.JustListen();
            //    Console.WriteLine(message.Payload);
            //});
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MessageHandler>();
            services.AddSingleton<SocketManager>();
            services.AddSingleton<PortManager>();
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
