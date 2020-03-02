using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Forum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("wwwroot/userfiles"))
            {
                Directory.CreateDirectory("wwwroot/userfiles");
                Directory.CreateDirectory("wwwroot/userfiles/thumbnails");
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}