using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DEDrake.Barcodes {
	public class Code39 : Barcode {

		private readonly static Dictionary<char, string> Masks = new() {
			{ '*', "100000101110111010" },
			{ ' ', "100000111010111010" },
			{ '-', "100000101011101110" },
			{ '$', "100010001000101000" },
			{ '%', "101000100001000100" },
			{ '.', "111000001010111010" },
			{ '/', "100010000101000010" },
			{ '+', "100001010000100010" },

			{ '0', "101000001110111010" },
			{ '1', "111010000010101110" },
			{ '2', "101110000010101110" },
			{ '3', "111011100000101010" },
			{ '4', "101000001110101110" },
			{ '5', "111010000011101010" },
			{ '6', "101110000011101010" },
			{ '7', "101000001011101110" },
			{ '8', "111010000010111010" },
			{ '9', "101110000010111010" },

			{ 'A', "111010100000101110" },
			{ 'B', "101110100000101110" },
			{ 'C', "111011101000001010" },
			{ 'D', "101011100000101110" },
			{ 'E', "111010111000001010" },
			{ 'F', "101110000011101010" },
			{ 'G', "101010000011101110" },
			{ 'H', "111010100000111010" },
			{ 'I', "101110100000111010" },
			{ 'J', "101011100000111010" },
			{ 'K', "111010101000001110" },
			{ 'L', "101110101000001110" },
			{ 'M', "111011101010000010" },
			{ 'N', "101011101000001110" },
			{ 'O', "111010111010000010" },
			{ 'P', "101110111010000010" },
			{ 'Q', "101010111000001110" },
			{ 'R', "111010101110000010" },
			{ 'S', "101110101110000010" },
			{ 'T', "101011101110000010" },
			{ 'U', "111000001010101110" },
			{ 'W', "111000001110101010" },
			{ 'X', "100000101110101110" },
			{ 'Y', "111000001011101010" },
			{ 'Z', "100000111011101010" }
		};

		public override byte[] BinaryImage {
			get {
				if (ms.Length == 0) RenderBarcode(); return ms.ToArray();
			}
		}

		public Code39(string code) : base(code) {

		}

		protected override void RenderBarcode() {
			var chars = Code.ToUpper().ToCharArray();

			BinaryText.Append(Masks['*']);
			foreach (var c in chars) BinaryText.Append(Masks[c]); BinaryText.Append(Masks['*']);

			var binary = BinaryText.ToString().ToCharArray();
			var width = binary.Length + 41;

			using var bmp = new Bitmap(width, 105, PixelFormat.Format32bppRgb);
			using (var pen = new Pen(Color.White)) using (var g = Graphics.FromImage(bmp)) {
				g.Clear(Color.White);

				var r = new Rectangle(10, 72, 10, 15);
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;

				r = new Rectangle(0, 72, width, 15);
				var sf = new StringFormat {
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Center
				};

				g.DrawString(Code.ToUpper(), new Font("Courier", 8), Brushes.Black, r, sf);

				g.Flush();

				foreach (var c in binary) {
					pen.Color = c == '1' ? Color.Black : Color.White;
					g.DrawLine(pen, LineBuffer, 20, LineBuffer, 70);
					LineBuffer++;
				}

			}

			bmp.Save(ms, ImageFormat.Png);
		}
	}
}
