using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Smelds.Barcodes {

  public static class ISBN {
    public static string ISBNConvert10To13(this string isbn10) {
      if (!Regex.IsMatch(isbn10, @"^(\d{9}|\d{10}|\d{9}[Xx])$")) { throw new FormatException("ISBN 10 must be either 9 or 10 digits with no formatting."); }

      var s = "978" + isbn10.Substring(0, 9);
      var k = 0;

      for (int i = 1; i <= s.Length; i++) {
        k += i % 2 == 0 ? int.Parse(s[i - 1].ToString()) * 3 : int.Parse(s[i - 1].ToString());
      }

      return s + (10 - (k % 10)).ToString();
    }

    public static string ISBNConvert13to10(this string isbn13) {
      if (!Regex.IsMatch(isbn13, @"^(\d{12}|\d{13})$")) { throw new FormatException("ISBN 13 must be either 12 or 13 digits with no formatting."); }

      var s = isbn13.Substring(3, 9);
      var k = 0;
      var x = 0;

      for (int i = 10; i >= 2; i--) {
        k += i * int.Parse(s[x].ToString());
        x++;
      }

      return s + ((11 - (k % 11)) == 10 ? "X" : (11 - (k % 11)).ToString());
    }
  }


  public class Bookland : IDisposable {
    public byte[] BinaryImage {
      get {
        return ms.ToArray();
      }
    }

    private Image barcodeImage;
    public Image BarcodeImage {
      get {
        if (this.barcodeImage == null) { this.barcodeImage = Image.FromStream(ms); }
        return this.barcodeImage;
      }
    }

    private StringBuilder BinaryText { get; set; }
    private int LineBuffer { get; set; }

    private MemoryStream ms = new MemoryStream();

    public Bookland(string ISBN) {
      // TODO: Regular Expression Replace all but 0-9
      // TODO: Check Length and Convert 10 to 13 if necessary
      this.BinaryText = new StringBuilder();
      this.LineBuffer = 21;

      char[] isbn = ISBN.ToArray();

      this.BinaryText.Append("101");
      this.CalculateFirstSet(isbn);
      this.BinaryText.Append("01010");
      this.CalculateSecondSet(isbn);
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
            g.DrawString(ISBN.Substring(1, 6), new Font("Courier", 8), Brushes.Black, r);

            r = new Rectangle(72, 72, 42, 15);
            g.DrawString(ISBN.Substring(7, 6), new Font("Courier", 8), Brushes.Black, r);

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

    private void CalculateFirstSet(char[] set) {
      var main = new[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
      var alt = new[] { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };

      for (int i = 1; i <= 6; i++) {
        var index = int.Parse(set[i].ToString());
        this.BinaryText.Append((i == 1 || i == 4 || i == 6 ? main[index] : alt[index]));
      }
    }

    private void CalculateSecondSet(char[] set) {
      var vals = new[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
      for (int i = 7; i < set.Length; i++) {
        this.BinaryText.Append(vals[int.Parse(set[i].ToString())]);
      }
    }

    public void SaveAs(string FilePath, bool OverWrite = false) {
      if (File.Exists(FilePath) && !OverWrite) { throw new IOException("File exits."); }

      using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write)) {
        fs.Write(this.BinaryImage, 0, this.BinaryImage.Length);
      }
    }

    public void Dispose() {
      if (this.BarcodeImage != null) { this.BarcodeImage.Dispose(); }
      this.ms.Dispose();
      this.BinaryText = null;
    }
  }
}