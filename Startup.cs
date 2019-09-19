using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HunterW_FinancialPortal.Startup))]
namespace HunterW_FinancialPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
