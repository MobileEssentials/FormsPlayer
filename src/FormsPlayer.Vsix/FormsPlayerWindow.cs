using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Xamarin.Forms.Player
{
	[Guid ("d619de43-8c34-4d69-96a2-186e6343b238")]
	public class FormsPlayerWindow : ToolWindowPane
	{
		public FormsPlayerWindow () :
			base (null)
		{
			this.Caption = Resources.ToolWindowTitle;

			this.BitmapResourceID = 300;
			this.BitmapIndex = 1;

			base.Content = ComponentModel.GlobalComponents.GetService<FormsPlayer> ();
		}
	}
}
