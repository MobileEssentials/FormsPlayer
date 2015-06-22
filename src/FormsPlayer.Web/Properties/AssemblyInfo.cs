using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Owin;

[assembly: AssemblyTitle ("FormsPlayer.Web")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("FormsPlayer.Web")]
[assembly: AssemblyCopyright ("Copyright ©  2015")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]

[assembly: ComVisible (false)]

[assembly: Guid ("04737a35-bf01-4a9c-ac7b-75ed57d016b1")]

[assembly: AssemblyVersion ("1.0.0.0")]
[assembly: AssemblyFileVersion ("1.0.0.0")]

[assembly: OwinStartup(typeof(Xamarin.Forms.Player.Startup))]