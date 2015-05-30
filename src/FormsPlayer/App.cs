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
		static readonly string FormsAssemblyName = typeof (ContentPage).AssemblyQualifiedName.Substring(typeof (ContentPage).AssemblyQualifiedName.IndexOf(',') + 1).Trim();

		AppViewModel viewModel;

		/// <summary>
		/// Initializes the application.
		/// </summary>
		public App ()
		{
			viewModel = new AppViewModel ();
			viewModel.PropertyChanged += OnPropertyChanged;

			MainPage = new MainView {
				BindingContext = viewModel
			};
		}

		void OnPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			// NOTE: we re-read both XAML and JSON because just re-applying 
			// the JSON to the BindingContext didn't work reliably.
			if (e.PropertyName == "Xaml" || e.PropertyName == "Json") {
				Device.BeginInvokeOnMainThread (() => {
					try {
						// Reload view from XAML and model from JSON. 
						// Ensures we always refresh both.
						var content = MainPage;

						var elementName = "ContentPage";
						// We first try to read the root element name from the 
						// XAML, so that we instantiate the Page-derived type 
						// appropriately.
						if (!string.IsNullOrEmpty (viewModel.Xaml)) {
							using (var reader = XmlReader.Create (new StringReader (viewModel.Xaml))) {
								if (reader.MoveToContent () == XmlNodeType.Element)
									elementName = reader.LocalName;
								else
									throw new ArgumentException ("Failed to retrieve root element name from XAML");
							}

							// NOTE: we assume the type comes from XF itself, no custom root pages for now.
							var typeName = "Xamarin.Forms." + elementName + ", " + FormsAssemblyName;
							var type = Type.GetType(typeName, true);
							if (!typeof (Page).GetTypeInfo ().IsAssignableFrom (type.GetTypeInfo ()))
                                throw new ArgumentException (string.Format ("Root XAML type '{0}' does not derive from required Page base class.", typeName));

							content = (Page)Activator.CreateInstance(type);
							content.LoadFromXaml (viewModel.Xaml);
						}

						if (!string.IsNullOrEmpty (viewModel.Json))
							content.BindingContext = JsonModel.Parse (viewModel.Json);

						MainPage = content;
					} catch (Exception ex) {
						MainPage = new ErrorView { ErrorMessage = ex.ToString () };
						viewModel.Status = ex.Message;
					}
				});
			}
		}
		
		/// <summary>
		/// Starts the app.
		/// </summary>
		protected override void OnStart ()
		{
			viewModel.Start ();
		}

		/// <summary>
		/// Sleeps the app.
		/// </summary>
		protected override void OnSleep ()
		{
			viewModel.Sleep ();
		}

		/// <summary>
		/// Resumes the app.
		/// </summary>
		protected override void OnResume ()
		{
			viewModel.Resume ();
		}
	}
}
