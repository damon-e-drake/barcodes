using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smelds.Barcodes {
  public class Code39 : IDisposable {

    private static Dictionary<char, string> Masks = new Dictionary<char, string> {
      {'*', "100000101110111010"},
      {' ', "100000111010111010"},
      {'-', "100000101011101110"},

      {'0', "101000001110111010"},
      {'1', "111010000010101110"},
      {'2', "101110000010101110"},
      {'3', "111011100000101010"},
      {'4', "101000001110101110"},
      {'5', "111010000011101010"},
      {'6', "101110000011101010"},
      {'7', "101000001011101110"},
      {'8', "111010000010111010"},
      {'9', "101110000010111010"},

      {'A', "111010100000101110"},
      {'B', "101110100000101110"},
      {'C', "111011101000001010"},
      {'D', "101011100000101110"},
      {'E', "111010111000001010"},
      {'F', "101110000011101010"},
      {'G', "101010000011101110"},
      {'H', "111010100000111010"},
      {'I', "101110100000111010"},
      {'J', "101011100000111010"},
      {'K', "111010101000001110"},
      {'L', "101110101000001110"},
      {'M', "111011101010000010"},
      {'N', "101011101000001110"},
      {'O', "111010111010000010"},
      {'P', "101110111010000010"},
      {'Q', "101010111000001110"},
      {'R', "111010101110000010"},
      {'S', "101110101110000010"},
      {'T', "101011101110000010"},
      {'U', "111000001010101110"},
      {'W', "111000001110101010"},
      {'X', "100000101110101110"},
      {'Y', "111000001011101010"},
      {'Z', "100000111011101010"}




    };

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

    public Code39(string code) {
      this.BinaryText = new StringBuilder();
      this.LineBuffer = 21;

      var chars = code.ToUpper().ToCharArray();

      this.BinaryText.Append(Masks['*']);
      foreach (var c in chars) { this.BinaryText.Append(Masks[c]); }
      this.BinaryText.Append(Masks['*']);

      var binary = this.BinaryText.ToString().ToCharArray();
      var width = binary.Length + 41;

      using (Bitmap bmp = new Bitmap(width, 105, PixelFormat.Format32bppRgb)) {
        using (Pen pen = new Pen(Color.White)) {
          using (Graphics g = Graphics.FromImage(bmp)) {
            g.Clear(Color.White);

            Rectangle r = new Rectangle(10, 72, 10, 15);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            r = new Rectangle(0, 72, width, 15);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            g.DrawString(code.ToUpper(), new Font("Courier", 8), Brushes.Black, r, sf);

            g.Flush();

            foreach (var c in binary) {
              pen.Color = c == '1' ? Color.Black : Color.White;
              g.DrawLine(pen, this.LineBuffer, 20, this.LineBuffer, 70);
              this.LineBuffer++;
            }

          }
        }

        bmp.Save(ms, ImageFormat.Png);
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
