using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GameManager.Startup))]
namespace GameManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
