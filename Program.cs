using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                    webBuilder.UseStartup<Startup>().UseUrls("http://localhost:5800");
                });
        }
    }
}