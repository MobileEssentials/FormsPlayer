using System.Reflection;
using System.Resources;
using Xamarin.Forms.Player;

[assembly: AssemblyCompany("Mobile Essentials")]
[assembly: AssemblyProduct("Xamarin.Forms.Player")]
[assembly: AssemblyCopyright("Copyright © Mobile Essentials 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyTitle("Xamarin.Forms.Player")]
[assembly: AssemblyDescription("Preview Xamarin.Forms XAML on devices and simulators, with support for data-binding via JSON dummy view models.")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif

[assembly: AssemblyVersion(ThisAssembly.Version)]
[assembly: AssemblyFileVersion(ThisAssembly.FileVersion)]
[assembly: AssemblyInformationalVersion(ThisAssembly.InformationalVersion)]


namespace Xamarin.Forms.Player
{
	partial class ThisAssembly
	{
		public const string Version = ThisAssembly.Git.BaseVersion.Major + "." + ThisAssembly.Git.BaseVersion.Minor + "." +  ThisAssembly.Git.BaseVersion.Patch + "." + ThisAssembly.Git.Commits;
		public const string FileVersion = Version;
		public const string InformationalVersion = Version + "-" + ThisAssembly.Git.Branch + "+" + ThisAssembly.Git.Commit;

		public const string HubUrl = "http://formsplayer.azurewebsites.net/";
	}
}