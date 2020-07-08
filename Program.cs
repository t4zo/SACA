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
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Heroku")
                    {
                        var PORT = Environment.GetEnvironmentVariable("PORT");

                        webBuilder.UseStartup<Startup>()
                        .UseUrls($"http://*:{PORT}");
                    }
                    else
                    {
                        webBuilder.UseSentry();
                        webBuilder.UseStartup<Startup>().UseUrls("https://localhost:5501");
                    }
                });
    }
}
