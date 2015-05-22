using Owin;

namespace Xamarin.Forms.Player
{
	public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
