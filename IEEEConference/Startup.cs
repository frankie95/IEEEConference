using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEEEConference.Startup))]
namespace IEEEConference
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
