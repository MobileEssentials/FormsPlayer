using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Forms.Player
{
	/// <summary>
	/// Main player app, to be used from the main entry point for your 
	/// Xamarin.Forms application.
	/// </summary>
	public class App : Application
	{
	    AppController controller;

	    /// <summary>
		/// Initializes the application.
		/// </summary>
		public App ()
		{
            controller = new AppController(this);
		}
        
		/// <summary>
		/// Starts the app.
		/// </summary>
		protected override void OnStart ()
		{
			controller.OnStart();
		}

		/// <summary>
		/// Sleeps the app.
		/// </summary>
		protected override void OnSleep ()
		{
			controller.OnSleep();
		}

		/// <summary>
		/// Resumes the app.
		/// </summary>
		protected override void OnResume ()
		{
			controller.OnResume();
		}
	}
}
