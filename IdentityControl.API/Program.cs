using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace IdentityControl.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityControl API";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddCommandLine(args)
                .Build();

            var logConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = logConfiguration.CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                Log.Information("Starting host...");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}