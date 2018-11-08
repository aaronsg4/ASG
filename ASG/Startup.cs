using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASG.Startup))]
namespace ASG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
