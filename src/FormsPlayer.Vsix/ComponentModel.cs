using System;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace Xamarin.Forms.Player
{
	public static class ComponentModel
	{
		static ComponentModel()
		{
			 GlobalComponents = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel> ();
		}

		public static IComponentModel GlobalComponents { get; private set; }
	}
}
