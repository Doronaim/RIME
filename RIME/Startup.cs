using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RIME.Startup))]
namespace RIME
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
