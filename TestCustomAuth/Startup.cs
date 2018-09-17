using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TestCustomAuth.Startup))]

namespace TestCustomAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}