using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ROHV.Startup))]
namespace ROHV
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
