using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Xamarin.Forms.Player
{
	[HubName ("FormsPlayer")]
	public class FormsPlayerHub : Hub
	{
		public Task Join (string sessionId)
		{
			return Groups.Add (Context.ConnectionId, sessionId);
		}

		public void Xaml (string sessionId, string payload)
		{
			Clients.Group (sessionId).Xaml (payload);
		}

		public void Json (string sessionId, string payload)
		{
			Clients.Group (sessionId).Json (payload);
		}
	}
}
