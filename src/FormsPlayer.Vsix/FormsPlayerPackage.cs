using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Xamarin.Forms.Player.Diagnostics;

namespace Xamarin.Forms.Player
{
	[PackageRegistration (UseManagedResourcesOnly = true)]
	[InstalledProductRegistration ("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource ("Menus.ctmenu", 1)]
	[ProvideToolWindow (typeof (FormsPlayerWindow))]
	[Guid (GuidList.guidFormsPlayerPkgString)]
	public sealed class FormsPlayerPackage : Package
	{
		void ShowToolWindow (object sender, EventArgs e)
		{
			var window = FindToolWindow (typeof (FormsPlayerWindow), 0, true);
			if ((null == window) || (null == window.Frame)) {
				throw new NotSupportedException (Resources.CanNotCreateWindow);
			}

			var windowFrame = (IVsWindowFrame)window.Frame;
			ErrorHandler.ThrowOnFailure (windowFrame.Show ());
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			var manager = new TracerManager();
			manager.SetTracingLevel (GetType ().Namespace, SourceLevels.Information);
			Tracer.Initialize (manager);

			Tracer.Get<FormsPlayerPackage> ().Info ("!Xamarin Forms Player Initialized");

			OleMenuCommandService mcs = GetService (typeof (IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs) {
				// Create the command for the tool window
				CommandID toolwndCommandID = new CommandID (GuidList.guidFormsPlayerCmdSet, (int)PkgCmdIDList.cmdXamarinFormsPlayer);
				MenuCommand menuToolWin = new MenuCommand (ShowToolWindow, toolwndCommandID);
				mcs.AddCommand (menuToolWin);
			}
		}
	}
}
