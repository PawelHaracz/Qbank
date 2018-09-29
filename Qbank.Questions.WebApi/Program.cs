using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Qbank.Questions.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               //.UseKestrel(options => options.ConfigureEndpoints())
               .UseStartup<Startup>();

    }

    //hardcoded secrets in appsettings because docker doesn't work
}
