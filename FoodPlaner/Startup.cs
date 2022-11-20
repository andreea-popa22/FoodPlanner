using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FoodPlaner.Startup))]
namespace FoodPlaner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
