using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TuCalcuTeApuesto.Startup))]
namespace TuCalcuTeApuesto
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
