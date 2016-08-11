using System;
using System.Linq;
using MonoDevelop.Core;

namespace FormsPlayerXS
{
	public static class Helper
	{
		static Helper ()
		{
			if (string.IsNullOrEmpty (SessionId)) {
				SessionId = UniqueId ();
			}
		}

		static string UniqueId ()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random ();
			return new string (Enumerable.Repeat (chars, 5)
				.Select (s => s [random.Next (s.Length)]).ToArray ());
		}

		public static string SessionId {
			get { 
				return PropertyService.Get<string> ("FormsPlayer.SessionId", String.Empty); 
			}
			set{ PropertyService.Set ("FormsPlayer.SessionId", value); }
		}
	}
}

