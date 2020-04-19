using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CourseLibrary.API
{
    public class AppConfigBuilder
    {
        public IServiceCollection Services { set; get; }
        public IConfiguration Configuration { set; get; }
        public IHostEnvironment Environment { set; get; }
        public IIdentityServerBuilder IdentityServerBuilder { get; set; }
    }
}