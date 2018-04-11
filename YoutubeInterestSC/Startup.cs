using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YoutubeInterestSC.Startup))]
namespace YoutubeInterestSC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
