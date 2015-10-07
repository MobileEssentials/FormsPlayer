using EnvDTE;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Forms.Player.Diagnostics;

namespace Xamarin.Forms.Player
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export]
	public class FormsPlayerViewModel : INotifyPropertyChanged
	{
		const string SettingsPath = "Xamarin\\FormsPlayer";
		const string SettingsKey = "LastSessionId";

		static readonly ITracer tracer = Tracer.Get<FormsPlayerViewModel>();

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		HubConnection connection;
		IHubProxy proxy;
		IDisposable onConnected;
		DocumentEvents events;
		bool isConnected;
		bool isConnecting;
		string status;
		string sessionId;
		int clients;
		WritableSettingsStore settings;

		[ImportingConstructor]
		public FormsPlayerViewModel([Import(typeof(SVsServiceProvider))] IServiceProvider services)
		{
			ConnectCommand = new DelegateCommand(Connect, () => !isConnected);
			DisconnectCommand = new DelegateCommand(Disconnect, () => isConnected);
			events = services.GetService<DTE>().Events.DocumentEvents;
			events.DocumentSaved += document => Publish(document.FullName);

			var manager = new ShellSettingsManager(services);
			settings = manager.GetWritableSettingsStore(SettingsScope.UserSettings);
			if (!settings.CollectionExists(SettingsPath))
				settings.CreateCollection(SettingsPath);
			if (settings.PropertyExists(SettingsPath, SettingsKey))
				SessionId = settings.GetString(SettingsPath, SettingsKey, "");

			if (string.IsNullOrEmpty(SessionId))
			{
				// Initialize SessionId from MAC address.
				var mac = NetworkInterface.GetAllNetworkInterfaces()
					.Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
					.Select(nic => nic.GetPhysicalAddress().ToString())
					.First();

				SessionId = NaiveBijective.Encode(NaiveBijective.Decode(mac));
			}

			TaskScheduler.UnobservedTaskException += OnTaskException;
		}

		public ICommand ConnectCommand { get; private set; }

		public ICommand DisconnectCommand { get; private set; }

		public int Clients { get { return clients; } set { SetProperty(ref clients, value, "Clients"); } }

		public bool IsConnected { get { return isConnected; } set { SetProperty(ref isConnected, value, "IsConnected"); } }

		public bool IsConnecting { get { return isConnecting; } }

		public string SessionId { get { return sessionId; } set { SetProperty(ref sessionId, value, "SessionId"); } }

		public string Status { get { return status; } set { SetProperty(ref status, value, "Status"); } }

		void Publish(string fileName)
		{
			if (!IsConnected)
			{
				tracer.Warn("!FormsPlayer is not connected yet.");
				return;
			}

			if (Path.GetExtension(fileName) == ".xaml")
			{
				PublishXaml(fileName);
			}
			else if (Path.GetExtension(fileName) == ".json")
			{
				PublishJson(fileName);
			}
		}

		void PublishXaml(string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try
			{
				using (var reader = XmlReader.Create(fileName))
				{
					var xdoc = XDocument.Load(reader);
					// Strip the x:Class attribute since it doesn't make
					// sense for the deserialization and might break stuff.
					var xclass = xdoc.Root.Attribute("{http://schemas.microsoft.com/winfx/2009/xaml}Class");
					if (xclass != null)
						xclass.Remove();
					xclass = xdoc.Root.Attribute("{http://schemas.microsoft.com/winfx/2006/xaml}Class");
					if (xclass != null)
						xclass.Remove();

					var xml = xdoc.ToString(SaveOptions.DisableFormatting);
					tracer.Info("!Publishing XAML payload");

					proxy.Invoke("Xaml", SessionId, xml)
						.ContinueWith(t =>
						   tracer.Error(t.Exception.InnerException, "Failed to publish XAML payload."),
							CancellationToken.None,
							TaskContinuationOptions.OnlyOnFaulted,
							TaskScheduler.Default);
				}
			}
			catch (XmlException)
			{
				return;
			}
		}

		void PublishJson(string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try
			{
				var json = JObject.Parse(File.ReadAllText(fileName));
				tracer.Info("!Publishing JSON payload");

				proxy.Invoke("Json", SessionId, json.ToString(Newtonsoft.Json.Formatting.None))
					.ContinueWith(t =>
					   tracer.Error(t.Exception.InnerException, "Failed to publish JSON payload."),
						CancellationToken.None,
						TaskContinuationOptions.OnlyOnFaulted,
						TaskScheduler.Default);

			}
			catch (JsonException)
			{
				return;
			}
		}

		void Connect()
		{
			IsConnected = false;
			SetProperty(ref isConnecting, true, "IsConnecting");

			System.Threading.Tasks.Task.Run(() =>
			{
				connection = new HubConnection(ThisAssembly.HubUrl);
				proxy = connection.CreateHubProxy("FormsPlayer");

				try
				{
					onConnected = proxy.On<int>("Connected", count => Clients = count - 1);
					connection.Start().Wait(3000);
					proxy.Invoke("Join", SessionId);
					IsConnected = true;
					Status = "Successfully connected to FormsPlayer";
				}
				catch (Exception e)
				{
					Status = "Error connecting to FormsPlayer: " + e.Message;
					connection.Dispose();
				}
				finally
				{
					SetProperty(ref isConnecting, false, "IsConnecting");
				}
			});
		}

		void Disconnect()
		{
			onConnected.Dispose();
			connection.Stop();
			connection.Dispose();
			connection = null;
			proxy = null;
			IsConnected = false;
		}

		void SetProperty<T>(ref T field, T value, string name)
		{
			if (!Object.Equals(field, value))
			{
				field = value;
				Application.Current.Dispatcher.BeginInvoke((Action)(() =>
				{
					PropertyChanged(this, new PropertyChangedEventArgs(name));
					switch (name)
					{
						case "IsConnected":
							// On disconnection, reset clients count.
							if (!isConnected)
								Clients = 0;
							break;
						case "SessionId":
							// If connected and session Id changed, disconnect.
							if (isConnected)
								Disconnect();

							// Always save the last-used session id.
							settings.SetString(SettingsPath, SettingsKey, SessionId);
							break;
						default:
							break;
					}
				}));
			}
		}

		void OnTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			tracer.Error(e.Exception.GetBaseException().InnerException, "Background task exception.");
			e.SetObserved();
		}
	}
}
