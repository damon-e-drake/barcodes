using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smelds.Barcodes {
  public sealed class Ean13 : Barcode {
    private class Encoding {
      public string OddParity { get; set; }
      public string EvenParity { get; set; }
      public string RighHand { get; set; }
    }

    private static Dictionary<char, Encoding> NumberEncoding = new Dictionary<char, Encoding> {
      {'0', new Encoding { OddParity = "0001101", EvenParity = "0100111", RighHand = "1110010"} },
      {'1', new Encoding { OddParity = "0011001", EvenParity = "0110011", RighHand = "1100110"} },
      {'2', new Encoding { OddParity = "0010011", EvenParity = "0011011", RighHand = "1101100"} },
      {'3', new Encoding { OddParity = "0111101", EvenParity = "0100001", RighHand = "1000010"} },
      {'4', new Encoding { OddParity = "0100011", EvenParity = "0011101", RighHand = "1011100"} },
      {'5', new Encoding { OddParity = "0110001", EvenParity = "0111001", RighHand = "1001110"} },
      {'6', new Encoding { OddParity = "0101111", EvenParity = "0000101", RighHand = "1010000"} },
      {'7', new Encoding { OddParity = "0111011", EvenParity = "0010001", RighHand = "1000100"} },
      {'8', new Encoding { OddParity = "0110111", EvenParity = "0001001", RighHand = "1001000"} },
      {'9', new Encoding { OddParity = "0001011", EvenParity = "0010111", RighHand = "1110100"} }
    };

    private static Dictionary<char, string> ParityEncoding = new Dictionary<char, string> {
      {'0', "OOOOOO"}, {'1', "OOEOEE"}, {'2', "OOEEOE"}, {'3', "OOEEEO"}, {'4', "OEOOEE"}, {'5', "OEEOOE"}, {'6', "OEEEOO"}, {'7', "OEOEOE"}, {'8', "OEOEEO"}, {'9', "OEEOEO"}
    };

    public override byte[] BinaryImage {
      get {
        if (this.ms.Length == 0) {this.RenderBarcode(); }
        return ms.ToArray();
      }
    }

    public Ean13(string code) : base(code) {

    }

    protected override void RenderBarcode() {
      char[] isbn = this.Code.ToArray();

      this.BinaryText.Append("101");
      this.CalculateLeftSide(isbn);
      this.BinaryText.Append("01010");
      this.CalculateRightSide(isbn);
      this.BinaryText.Append("101");

      using (Bitmap bmp = new Bitmap(135, 105, PixelFormat.Format32bppRgb)) {
        using (Pen pen = new Pen(Color.White)) {
          using (Graphics g = Graphics.FromImage(bmp)) {
            g.Clear(Color.White);

            Rectangle r = new Rectangle(10, 72, 10, 15);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString("9", new Font("Courier", 8), Brushes.Black, r);

            r = new Rectangle(26, 72, 42, 15);
            g.DrawString(Code.Substring(1, 6), new Font("Courier", 8), Brushes.Black, r);

            r = new Rectangle(72, 72, 42, 15);
            g.DrawString(Code.Substring(7, 6), new Font("Courier", 8), Brushes.Black, r);

            g.Flush();

            char[] binary = this.BinaryText.ToString().ToCharArray();

            int[] delim = new int[] { 0, 1, 2, 45, 46, 47, 48, 49, 92, 93, 94 };

            for (int i = 0; i < binary.Length; i++) {
              pen.Color = binary[i] == '1' ? Color.Black : Color.White;

              if (delim.Contains(i)) {
                g.DrawLine(pen, this.LineBuffer, 20, this.LineBuffer, 85);
              }
              else {
                g.DrawLine(pen, this.LineBuffer, 20, this.LineBuffer, 70);
              }

              this.LineBuffer++;
            }

          }
        }

        bmp.Save(ms, ImageFormat.Png);
      }
    }

    private void CalculateLeftSide(char[] set) {
      var parity = Ean13.ParityEncoding[set[0]].ToCharArray();

      for (int i = 1; i <= 6; i++) {
        this.BinaryText.Append((parity[i - 1] == 'O' ? NumberEncoding[set[i]].OddParity : NumberEncoding[set[i]].EvenParity));
      }
    }

    private void CalculateRightSide(char[] set) {
      for (int i = 7; i < set.Length; i++) {
        this.BinaryText.Append(NumberEncoding[set[i]].RighHand);
      }
    }
   
  }
}
