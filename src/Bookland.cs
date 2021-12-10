using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DEDrake.Barcodes {

	public class Bookland : Barcode {
		public override byte[] BinaryImage {
			get {
				if (ms.Length == 0) RenderBarcode(); return ms.ToArray();
			}
		}

		public Bookland(string ISBN) : base(ISBN) {

		}

		protected override void RenderBarcode() {
			var isbn = Code.ToArray();

			BinaryText.Append("101");
			CalculateFirstSet(isbn);
			BinaryText.Append("01010");
			CalculateSecondSet(isbn);
			BinaryText.Append("101");

			using var bmp = new Bitmap(135, 105, PixelFormat.Format32bppRgb);
			using (var pen = new Pen(Color.White)) using (var g = Graphics.FromImage(bmp)) {
				g.Clear(Color.White);

				var r = new Rectangle(10, 72, 10, 15);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.DrawString("9", new Font("Courier", 8), Brushes.Black, r);

				r = new Rectangle(26, 72, 42, 15);
				g.DrawString(Code.Substring(1, 6), new Font("Courier", 8), Brushes.Black, r);

				r = new Rectangle(72, 72, 42, 15);
				g.DrawString(Code.Substring(7, 6), new Font("Courier", 8), Brushes.Black, r);

				g.Flush();

				var binary = BinaryText.ToString().ToCharArray();

				var delim = new int[] { 0, 1, 2, 45, 46, 47, 48, 49, 92, 93, 94 };

				for (var i = 0; i < binary.Length; i++) {
					pen.Color = binary[i] == '1' ? Color.Black : Color.White;

					if (delim.Contains(i)) g.DrawLine(pen, LineBuffer, 20, LineBuffer, 85);
					else g.DrawLine(pen, LineBuffer, 20, LineBuffer, 70);

					LineBuffer++;
				}

			}

			bmp.Save(ms, ImageFormat.Png);
		}

		private void CalculateFirstSet(char[] set) {
			var main = new[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
			var alt = new[] { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };

			for (var i = 1; i <= 6; i++) {
				var index = int.Parse(set[i].ToString());
				BinaryText.Append(i == 1 || i == 4 || i == 6 ? main[index] : alt[index]);
			}
		}

		private void CalculateSecondSet(char[] set) {
			var vals = new[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
			for (var i = 7; i < set.Length; i++) BinaryText.Append(vals[int.Parse(set[i].ToString())]);
		}
	}
}
