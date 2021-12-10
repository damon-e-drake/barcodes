using System.Text.RegularExpressions;

namespace DEDrake.Barcodes.Utils {
	public enum ISBNFormat {
		None = 0,
		Hyphens = 1,
		Spaces = 2
	}

	public static class ISBN {
		public static string ISBNConvert10To13(this string isbn10, ISBNFormat format = ISBNFormat.None) {
			var isbn = isbn10.Replace(" ", "").Replace("-", "");

			if (!Regex.IsMatch(isbn, @"^(\d{9}|\d{10}|\d{9}[Xx])$"))
				throw new FormatException("ISBN 10 must be either 9 or 10 digits optinally with an x or X as the 10 character.");

			var s = "978" + isbn[..9];
			var k = 0;

			for (var i = 1; i <= s.Length; i++) k += i % 2 == 0 ? int.Parse(s[i - 1].ToString()) * 3 : int.Parse(s[i - 1].ToString());

			var converted = s + (10 - k % 10).ToString();

			return format switch {
				ISBNFormat.Hyphens => converted,
				ISBNFormat.Spaces => converted,
				ISBNFormat.None => converted,
				_ => converted,
			};
		}

		public static string ISBNConvert13to10(this string isbn13, ISBNFormat format = ISBNFormat.None) {
			var isbn = isbn13.Replace(" ", "").Replace("-", "");

			if (!Regex.IsMatch(isbn, @"^(\d{12}|\d{13})$"))
				throw new FormatException("ISBN 13 must be either 12 or 13 digits.");

			var s = isbn.Substring(3, 9);
			var k = 0;
			var x = 0;

			for (var i = 10; i >= 2; i--) {
				k += i * int.Parse(s[x].ToString());
				x++;
			}

			var converted = s + (11 - k % 11 == 10 ? "X" : (11 - k % 11).ToString());

			return format switch {
				ISBNFormat.Hyphens => converted,
				ISBNFormat.Spaces => converted,
				ISBNFormat.None => converted,
				_ => converted,
			};
		}
	}
}
