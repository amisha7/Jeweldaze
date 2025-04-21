using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jewellery_project.Startup))]
namespace jewellery_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
