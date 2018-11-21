using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WebService {
    public class Program {
        public static int CurrentUserId = 1234;

        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}