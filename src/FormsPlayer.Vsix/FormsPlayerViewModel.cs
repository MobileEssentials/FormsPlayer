using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Player.Diagnostics;

namespace Xamarin.Forms.Player
{
	[PartCreationPolicy (CreationPolicy.Shared)]
	[Export]
	public class FormsPlayerViewModel : INotifyPropertyChanged
	{
		static readonly ITracer tracer = Tracer.Get<FormsPlayerViewModel>();

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		HubConnection connection;
		IHubProxy proxy;
		IDisposable onConnected;
		DocumentEvents events;
		bool isConnected;
		string status;
		string sessionId;
		int clients;

		[ImportingConstructor]
		public FormsPlayerViewModel ([Import (typeof (SVsServiceProvider))] IServiceProvider services)
		{
			ConnectCommand = new DelegateCommand (Connect, () => !isConnected);
			DisconnectCommand = new DelegateCommand (Disconnect, () => isConnected);
			events = services.GetService<DTE> ().Events.DocumentEvents;
			events.DocumentSaved += document => Publish (document.FullName);

			// Initialize SessionId from MAC address.
			var mac = NetworkInterface.GetAllNetworkInterfaces()
				.Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				.Select(nic => nic.GetPhysicalAddress().ToString())
				.First();

			SessionId = NaiveBijective.Encode (NaiveBijective.Decode (mac));

			TaskScheduler.UnobservedTaskException += OnTaskException;
		}

		public ICommand ConnectCommand { get; private set; }

		public ICommand DisconnectCommand { get; private set; }

		public int Clients { get { return clients; } set { SetProperty (ref clients, value, "Clients"); } }

		public bool IsConnected { get { return isConnected; } set { SetProperty (ref isConnected, value, "IsConnected"); } }

		public string SessionId { get { return sessionId; } set { SetProperty (ref sessionId, value, "SessionId"); } }

		public string Status { get { return status; } set { SetProperty (ref status, value, "Status"); } }

		void Publish (string fileName)
		{
			if (!IsConnected) {
				tracer.Warn ("!FormsPlayer is not connected yet.");
				return;
			}

			if (Path.GetExtension (fileName) == ".xaml") {
				PublishXaml (fileName);
			} else if (Path.GetExtension (fileName) == ".json") {
				PublishJson (fileName);
			}
		}

		void PublishXaml (string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try {
				using (var reader = XmlReader.Create (fileName)) {
					var xdoc = XDocument.Load(reader);
					// Strip the x:Class attribute since it doesn't make 
					// sense for the deserialization and might break stuff.
					var xclass = xdoc.Root.Attribute("{http://schemas.microsoft.com/winfx/2009/xaml}Class");
					if (xclass != null)
						xclass.Remove ();
					xclass = xdoc.Root.Attribute ("{http://schemas.microsoft.com/winfx/2006/xaml}Class");
					if (xclass != null)
						xclass.Remove ();

					var xml = xdoc.ToString(SaveOptions.DisableFormatting);
					tracer.Info ("!Publishing XAML payload");

					proxy.Invoke ("Xaml", SessionId, xml)
						.ContinueWith (t =>
							tracer.Error (t.Exception.InnerException, "Failed to publish XAML payload."),
							CancellationToken.None,
							TaskContinuationOptions.OnlyOnFaulted,
							TaskScheduler.Default);
				}
			} catch (XmlException) {
				return;
			}
		}

		void PublishJson (string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try {
				var json = JObject.Parse(File.ReadAllText(fileName));
				tracer.Info ("!Publishing JSON payload");

				proxy.Invoke ("Json", SessionId, json.ToString (Newtonsoft.Json.Formatting.None))
					.ContinueWith (t =>
						tracer.Error (t.Exception.InnerException, "Failed to publish JSON payload."),
						CancellationToken.None,
						TaskContinuationOptions.OnlyOnFaulted,
						TaskScheduler.Default);

			} catch (JsonException) {
				return;
			}
		}

		void Connect ()
		{
			IsConnected = false;
			connection = new HubConnection ("http://formsplayer.azurewebsites.net/");
			proxy = connection.CreateHubProxy ("FormsPlayer");

			try {
				connection.Start ().Wait (3000);
				IsConnected = true;
				Status = "Successfully connected to FormsPlayer";
				onConnected = proxy.On<int> ("Connected", count => Clients = count - 1);
			} catch (Exception e) {
				Status = "Error connecting to FormsPlayer: " + e.Message;
				connection.Dispose ();
			}
		}

		void Disconnect ()
		{
			connection.Stop ();
			connection.Dispose ();
			connection = null;
			proxy = null;
			IsConnected = false;
		}

		void SetProperty<T>(ref T field, T value, string name)
		{
			if (!Object.Equals (field, value)) {
				field = value;
				PropertyChanged (this, new PropertyChangedEventArgs (name));
				switch (name) {
					case "IsConnected":
						// On disconnection, reset clients count.
						if (!isConnected)
							Clients = 0;
						break;
					case "SessionId":
						// If connected and session Id changed, disconnect.
						if (isConnected)
							Disconnect ();
						break;
					default:
						break;
				}
			}
		}

		void OnTaskException (object sender, UnobservedTaskExceptionEventArgs e)
		{
			tracer.Error (e.Exception.GetBaseException ().InnerException, "Background task exception.");
			e.SetObserved ();
		}
	}
}
