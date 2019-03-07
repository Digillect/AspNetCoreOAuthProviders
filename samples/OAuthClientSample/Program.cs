using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace OAuthClientSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
