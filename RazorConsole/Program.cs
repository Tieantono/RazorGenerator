using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp;

namespace RazorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            ConfigureDefaultServices(services);

            var provider = services.BuildServiceProvider();

            var renderer = provider.GetRequiredService<RazorViewToStringRenderer>();

            var model = new EmailViewModel
            {
                Username = "User",
                SenderName = "Sender"
            };

            var emailContent = renderer.RenderViewToString("EmailTemplate", model).GetAwaiter().GetResult();

            Console.WriteLine(emailContent);
            Console.ReadLine();
        }

        private static void ConfigureDefaultServices(ServiceCollection services)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = Directory.GetCurrentDirectory();

            var environment = new HostingEnvironment()
            {
                WebRootFileProvider = new PhysicalFileProvider(appDirectory),
                ApplicationName = "Temp"
            };

            services.AddSingleton<IHostingEnvironment>(environment);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
            });

            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);

            services.AddLogging();
            services.AddMvcCore()
                .AddRazorViewEngine();
            services.AddSingleton<RazorViewToStringRenderer>();
        }
    }
}
