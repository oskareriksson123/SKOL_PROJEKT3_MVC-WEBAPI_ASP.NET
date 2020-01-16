using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DejtApplication10._0.Startup))]
namespace DejtApplication10._0
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
        }
    }
}
