using System.Linq;

namespace Xamarin.Forms.Player
{
	/// <summary>
	/// From https://gist.github.com/dgritsko/9554733
	/// See http://stackoverflow.com/questions/742013
	/// </summary>
	public static class NaiveBijective
	{
		static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
		static readonly int Base = Alphabet.Length;


		public static string Encode (int i)
		{
			if (i == 0) return Alphabet[0].ToString ();

			var s = string.Empty;

			while (i > 0) {
				s += Alphabet[i % Base];
				i = i / Base;
			}

			return string.Join (string.Empty, s.Reverse ());
		}

		public static int Decode (string s)
		{
			var i = 0;

			foreach (var c in s) {
				i = (i * Base) + Alphabet.IndexOf (c);
			}

			return i;
		}

	}
}
