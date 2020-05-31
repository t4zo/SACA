using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sentry;

namespace SACA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //using (SentrySdk.Init("https://e194eda5ab6149a6a3eab91fa61c7a5d@sentry.io/1878203"))
            //{
                CreateHostBuilder(args).Build().Run();
            //}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseStartup<Startup>();
                    webBuilder.UseStartup<Startup>().UseUrls("https://localhost:5501");
                });
    }
}
