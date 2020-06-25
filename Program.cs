using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace SACA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // webBuilder.UseStartup<Startup>();
                    // webBuilder.UseSentry();

                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Heroku")
                    {
                        webBuilder.UseStartup<Startup>().UseUrls("https://localhost:5501");
                    }
                    else
                    {
                        var PORT = Environment.GetEnvironmentVariable("PORT");

                        webBuilder.UseStartup<Startup>()
                        .UseUrls($"http://*:{PORT}");
                    }
                });
    }
}
