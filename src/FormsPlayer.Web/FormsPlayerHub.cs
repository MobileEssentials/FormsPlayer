using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Xamarin.Forms.Player
{
	[HubName ("FormsPlayer")]
	public class FormsPlayerHub : Hub
	{
		static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> sessionClients = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
		static ConcurrentDictionary<string, string> clientSessionMap = new ConcurrentDictionary<string, string>();

		public void Xaml (string sessionId, string payload)
		{
			Clients.Group (sessionId).Xaml (payload);
		}

		public void Json (string sessionId, string payload)
		{
			Clients.Group (sessionId).Json (payload);
		}

		public async Task Join (string sessionId)
		{
			// One XF player client can only be connected to one group at a time.
			clientSessionMap.AddOrUpdate (Context.ConnectionId, _ => sessionId, (_, __) => sessionId);

			// Update group clients list.
			var clients = sessionClients.AddOrUpdate (sessionId,
				// First connection to the session
				_ => new ConcurrentDictionary<string, string> (new[] 
				{
					new KeyValuePair<string, string>(Context.ConnectionId, null)
				}),
				// Add to existing sesssion
				(_, session) =>
				{
					session.TryAdd(Context.ConnectionId, null);
					return session;
				});

			await Groups.Add (Context.ConnectionId, sessionId);

			// Notify VS of the new list of clients.
			Clients.Group (sessionId).Connected (clients.Count);
		}

		public override Task OnDisconnected (bool stopCalled)
		{
			// Get group the client was last joined to, if any.
			string sessionId;
			ConcurrentDictionary<string, string> session;
			if (clientSessionMap.TryGetValue (Context.ConnectionId, out sessionId) && 
				sessionClients.TryGetValue(sessionId, out session)) {
				string clientId;
				session.TryRemove (Context.ConnectionId, out clientId);

				// Notify VS of the updated list of clients.
				Clients.Group (sessionId).Connected (session.Count);

				// If there are no more clients in the given session, remove the dictionary, 
				// to avoid memory leaks.
				if (session.Count == 0)
					sessionClients.TryRemove (sessionId, out session);
			}

			return base.OnDisconnected (stopCalled);
		}
	}
}
