using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Xamarin.Forms.Player
{
	class Program
	{
		static readonly TraceSource tracer = new TraceSource("*");

		static string sessionId;

		static void Main (string[] args)
		{
			Console.WriteLine ("Enter SessionId [ENTER]");
			sessionId = Console.ReadLine ().Trim ();

			Task.Run (((Func<Task>)Startup));
			Console.ReadLine ();
		}

		static async Task Startup ()
		{
			var connection = new HubConnection("http://formsplayer.azurewebsites.net/");
			var proxy = connection.CreateHubProxy("FormsPlayer");

			proxy.On<string> ("Xaml", xaml => tracer.TraceInformation (@"Received XAML: 
" + xaml));

			proxy.On<string> ("Json", json => tracer.TraceInformation (@"Received JSON: 
" + json));

			await connection.Start ();
			await proxy.Invoke ("Join", sessionId);
		}
	}
}
