using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace SACA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseSentry();
                    webBuilder.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5800);
                    });

                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}