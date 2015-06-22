using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNet.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace FormsPlayer.Tests
{
	public class HubTests
	{
#if DEBUG
		const string HubUrl = "http://formsplayer-dev.azurewebsites.net/";
#else
		const string HubUrl = "http://formsplayer.azurewebsites.net/";
#endif

		private ITestOutputHelper output;

		public HubTests (ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void Xaml ()
		{
			output.WriteLine (typeof (HubTests).AssemblyQualifiedName.Substring(typeof (HubTests).AssemblyQualifiedName.IndexOf(',') + 1));
		}

		[Fact]
		public async Task EndToEndHub ()
		{
			var sessionId = Guid.NewGuid().ToString();
			var clients = 0;

			var pubConn = new HubConnection(HubUrl);
			var pubProxy = pubConn.CreateHubProxy("FormsPlayer");

			var subConn = new HubConnection(HubUrl);
			var subProxy = subConn.CreateHubProxy("FormsPlayer");

			var xamls = new List<string>();
			var jsons = new List<string>();

			pubProxy.On<int> ("Connected", count => clients = count);

			subProxy.On<string> ("Xaml", xaml => xamls.Add (xaml));
			subProxy.On<string> ("Json", json => jsons.Add (json));

			await pubConn.Start ();
			await subConn.Start ();

			await pubProxy.Invoke ("Join", sessionId);
			await subProxy.Invoke ("Join", sessionId);

			SpinWait.SpinUntil (() => clients == 2, 1000);

			// Count will be 1+ than actual "clients", since the publisher/VS is also a client.
			Assert.Equal (2, clients);

			await pubProxy.Invoke ("Xaml", sessionId, "xaml");
			await pubProxy.Invoke ("Xaml", Guid.NewGuid ().ToString (), "xaml");
			await pubProxy.Invoke ("Json", sessionId, "json");
			await pubProxy.Invoke ("Json", Guid.NewGuid ().ToString (), "json");

			Assert.Equal (1, xamls.Count);
			Assert.Equal (1, jsons.Count);

			Assert.Equal ("xaml", xamls[0]);
			Assert.Equal ("json", jsons[0]);

			subConn.Dispose ();

			SpinWait.SpinUntil (() => clients == 1, 1000);
			Assert.Equal (1, clients);
		}
	}
}
