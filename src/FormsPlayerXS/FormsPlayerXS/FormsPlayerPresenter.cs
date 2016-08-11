
using System;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using MonoDevelop.Ide;
using System.Linq;
using System.Net.NetworkInformation;

namespace FormsPlayerXS
{
	public class FormsPlayerPresenter
	{
		FormsPlayerPresenter ()
		{
			SessionId = Helper.SessionId;
			Console.WriteLine ("session id {0}", SessionId);
		}

		public static FormsPlayerPresenter Instance{
			get { 
				return _localInstance ?? (_localInstance = new FormsPlayerPresenter());
			}
		}

		internal bool IsSupportedFile {
			get {
				if (IdeApp.Workbench.ActiveDocument == null) {
					return false;
				}

				string fileExtension = IdeApp.Workbench.ActiveDocument.FileName.Extension.ToLowerInvariant ();
				return FileExtensions.Contains (fileExtension);
			}
		}

		internal void Disconnect ()
		{
			connection.Stop ();
			connection.Dispose ();
			connection = null;
			proxy = null;
			IsConnected = false;

			IdeApp.Workbench.StatusBar.ShowMessage ("Disconnected");
		}

		internal void Connect ()
		{
			IsConnected = false;
			connection = new HubConnection ("http://formsplayer.azurewebsites.net/");
			proxy = connection.CreateHubProxy ("FormsPlayer");

			try {
				connection.Start ().Wait (3000);
				IsConnected = true;
				IdeApp.Workbench.StatusBar.ShowMessage (string.Format("Successfully connected to FormsPlayer. Session ID: {0}", SessionId));
			} catch (Exception e) {
				IdeApp.Workbench.StatusBar.ShowMessage (string.Format("Error connecting to FormsPlayer: {0}", e.Message));
				connection.Dispose ();
				Console.WriteLine (e);
			}
		}

		internal void Publish ()
		{
			if (!IsConnected || !IsSupportedFile) {
				Console.WriteLine ("!FormsPlayer is not connected yet.");
				return;
			}

			var activeDocument = IdeApp.Workbench.ActiveDocument;
			activeDocument.Save ();
			string fileName = activeDocument.FileName.FullPath;

			if (Path.GetExtension (fileName) == ".xaml") {
				PublishXaml (fileName);
			} else if (Path.GetExtension (fileName) == ".json") {
				PublishJson (fileName);
			}
		}


		internal void PublishXaml (string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try {
				using (var reader = XmlReader.Create (fileName)) {
					var xdoc = XDocument.Load (reader);
					// Strip the x:Class attribute since it doesn't make 
					// sense for the deserialization and might break stuff.
					var xclass = xdoc.Root.Attribute ("{http://schemas.microsoft.com/winfx/2009/xaml}Class");
					if (xclass != null)
						xclass.Remove ();
					xclass = xdoc.Root.Attribute ("{http://schemas.microsoft.com/winfx/2006/xaml}Class");
					if (xclass != null)
						xclass.Remove ();

					var xml = xdoc.ToString (SaveOptions.DisableFormatting);
					//tracer.Info ("!Publishing XAML payload");

					proxy.Invoke ("Xaml", SessionId, xml)
						.ContinueWith (Console.WriteLine,
							CancellationToken.None,
							TaskContinuationOptions.OnlyOnFaulted,
							TaskScheduler.Default);
				}
			} catch (XmlException) {
				return;
			}
		}

		internal void PublishJson (string fileName)
		{
			// Make sure we can read it as XML, just to safeguard the client.
			try {

				var json = JObject.Parse (File.ReadAllText (fileName));
				Console.WriteLine ("!Publishing JSON payload");

				proxy.Invoke ("Json", SessionId, json.ToString (Newtonsoft.Json.Formatting.None))
					.ContinueWith (Console.WriteLine,
						CancellationToken.None,
						TaskContinuationOptions.OnlyOnFaulted,
						TaskScheduler.Default);

			} catch (JsonException) {
				return;
			}
		}

		internal bool IsConnected {
			get;
			set;
		}



		internal string SessionId{ get; private set;}

		HubConnection connection;
		IHubProxy proxy;

		readonly string[] FileExtensions = { ".json", ".xaml" };

		static FormsPlayerPresenter _localInstance;
	}
}

