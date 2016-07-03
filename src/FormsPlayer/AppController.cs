using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin.Forms.Player
{
    /// <summary>
    /// Main player app controller, to be applied to your main
    /// Xamarin.Forms application to preserve Styles defined in App.xaml.
    /// </summary>
    public class AppController
    {
        public Application App { get; private set; }
        static readonly string FormsAssemblyName = typeof(ContentPage).AssemblyQualifiedName.Substring(typeof(ContentPage).AssemblyQualifiedName.IndexOf(',') + 1).Trim();

        AppViewModel viewModel;

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public AppController(Application app)
        {
            App = app;
            viewModel = new AppViewModel();
            viewModel.PropertyChanged += OnPropertyChanged;

            SetPage(new MainView { BindingContext = viewModel });
        }

        private Page MainPage
        {
            get { return App.MainPage; }
            set { App.MainPage = value; }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // NOTE: we re-read both XAML and JSON because just re-applying 
            // the JSON to the BindingContext didn't work reliably.
            if (e.PropertyName == "Xaml" || e.PropertyName == "Json")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        // Reload view from XAML and model from JSON. 
                        // Ensures we always refresh both.
                        var content = MainPage;

                        // We first try to read the root element name from the XAML
                        if (!string.IsNullOrEmpty(viewModel.Xaml))
                        {
                            using (var reader = XmlReader.Create(new StringReader(viewModel.Xaml)))
                            {
                                if (reader.MoveToContent() == XmlNodeType.Element)
                                {
                                    var rootElement = reader.Name;

                                    // If it contains a colon, then it is not derived from the Xamarin.Forms namespace
                                    if (rootElement.Contains(":"))
                                    {
                                        // Extract the Namespace, class and assembly info
                                        var classInfo = reader.NamespaceURI;

                                        var colonIndex = classInfo.IndexOf(":");
                                        var fullClassName = string.Format("{0}.{1}",
                                                                            classInfo.Substring(colonIndex + 1, classInfo.IndexOf(";") - colonIndex - 1),
                                                                            reader.LocalName);
                                        var assemblyName = classInfo.Substring(classInfo.IndexOf("=") + 1);

                                        content = CreatePage(fullClassName, assemblyName);
                                    }
                                    else
                                    {
                                        content = CreatePage(string.Format("Xamarin.Forms.{0}", reader.LocalName), FormsAssemblyName);
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException("Failed to retrieve root element name from XAML");
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(viewModel.Json))
                            content.BindingContext = JsonModel.Parse(viewModel.Json);

                        SetPage(content);
                    }
                    catch (Exception ex)
                    {
                        SetPage(new ErrorView { ErrorMessage = ex.ToString() });
                        viewModel.Status = ex.Message;
                    }
                });
            }
        }

        /// <summary>
        /// Used to set MainPage. If page is NavigationPage, existing page is set to root
        /// </summary>
        /// <param name="page">Page</param>
        public void SetPage(Page page)
        {
            if (page is NavigationPage)
            {
                // If setting to navigation page, 
                // replace MainPage with Navigation page and push existing page onto stack.
                var existingPage = MainPage;
                MainPage = page;
                MainPage.Navigation.PushAsync(existingPage);
            }
            else
            {
                // If MainPage is a NavigationPage, reset root
                if (MainPage is NavigationPage)
                {
                    var navigation = MainPage.Navigation;
                    navigation.InsertPageBefore(page, navigation.NavigationStack[0]);
                    navigation.PopToRootAsync(false);
                }
                else
                {
                    MainPage = page;
                }
            }
        }


        /// <summary>
        /// Creates an instance of the page.  If desired type is not an instance of page, a host page will be created
        /// </summary>
        /// <param name="fullClassName">Fully qualified class name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <returns>Page instance</returns>
        private Page CreatePage(string fullClassName, string assemblyName)
        {
            Page content;

            var type = Type.GetType(string.Format("{0}, {1}", fullClassName, assemblyName), true);

            // If root type is not a page, create a page and set content to instance of type
            if (!typeof(Page).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                var view = (View)Activator.CreateInstance(type);
                view.LoadFromXaml(viewModel.Xaml);

                var page = new ContentPage { Content = view };
                content = page;
            }
            else
            {
                content = (Page)Activator.CreateInstance(type);
                content.LoadFromXaml(viewModel.Xaml);
            }
            return content;
        }

        /// <summary>
        /// Starts the app.
        /// </summary>
        public void OnStart()
        {
            viewModel.Start();
        }

        /// <summary>
        /// Sleeps the app.
        /// </summary>
        public void OnSleep()
        {
            viewModel.Sleep();
        }

        /// <summary>
        /// Resumes the app.
        /// </summary>
        public void OnResume()
        {
            viewModel.Resume();
        }
    }
}
