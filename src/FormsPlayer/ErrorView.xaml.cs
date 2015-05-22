using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Xamarin.Forms.Player
{
	/// <summary>
	/// Custom view that shows any errors that happen during 
	/// the rendering of the received XAML or JSON.
	/// </summary>
	partial class ErrorView : ContentPage
	{
		/// <summary>
		/// Initializes the view.
		/// </summary>
		public ErrorView ()
		{
			InitializeComponent ();
		}

		/// <summary>
		/// Gets or sets the error message to display.
		/// </summary>
		public string ErrorMessage
		{
			get { return ErrorLabel.Text; }
			set { ErrorLabel.Text = value; }
		}

	}
}
