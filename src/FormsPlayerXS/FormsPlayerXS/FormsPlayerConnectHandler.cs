
using System;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using System.Net.NetworkInformation;
using Microsoft.AspNet.SignalR.Client;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FormsPlayerXS;

namespace FormsPlayerXS
{
	public class FormsPlayerConnectHandler : CommandHandler
	{
		readonly FormsPlayerPresenter _presenter;

		public FormsPlayerConnectHandler ()
		{
			_presenter = FormsPlayerPresenter.Instance;
		}

		protected override void Run ()
		{
			base.Run ();
			Console.WriteLine ("Start Player");

			if (_presenter.IsConnected)
				_presenter.Disconnect ();
			else
				_presenter.Connect ();
		}

		protected override void Update (CommandInfo info)
		{
			//info.Enabled = IsSupportedFile;
			info.Text = _presenter.IsConnected ? "Disconnect" : "Connect";

			Console.WriteLine (info.Command.Id);
		}
	}

	public class PublishHandler : CommandHandler
	{
		readonly FormsPlayerPresenter _presenter;

		public PublishHandler ()
		{
			_presenter = FormsPlayerPresenter.Instance;	
		}

		protected override void Update (CommandInfo info)
		{
			info.Enabled = _presenter.IsSupportedFile;
		}

		protected override void Run ()
		{
			base.Run ();
			_presenter.Publish ();
		}
	}

	public class SessionIdHandler : CommandHandler {
		protected override void Update (CommandInfo info)
		{
			base.Update (info);
			//info.Enabled = false;
			info.Visible = FormsPlayerPresenter.Instance.IsConnected;
			info.Text = string.Format("Session ID: {0}",  FormsPlayerPresenter.Instance.SessionId);
		}
	}
}

