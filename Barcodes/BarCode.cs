using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Smelds.Barcodes {
  public class Barcode : IDisposable {
    protected Image barcodeImage = null;
    protected StringBuilder BinaryText = null;
    protected int LineBuffer = 0;
    protected MemoryStream ms = null;
    protected string Code = null;

    public virtual byte[] BinaryImage {
      get {
        if (this.ms.Length == 0) { RenderBarcode(); }
        return ms.ToArray();
      }
    }

    public Image BarcodeImage {
      get {
          if (this.ms.Length == 0) { RenderBarcode(); }
          if (this.barcodeImage == null) { this.barcodeImage = Image.FromStream(this.ms); }
          return this.barcodeImage;
      }
    }

    public Barcode(string code) {
      this.ms = new MemoryStream();
      this.BinaryText = new StringBuilder();
      this.LineBuffer = 21;
      this.Code = code;
    }

    public void SaveAs(string FilePath, bool OverWrite = false) {
      if (File.Exists(FilePath) && !OverWrite) { throw new IOException("File exits."); }

      using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write)) {
        fs.Write(this.BinaryImage, 0, this.BinaryImage.Length);
      }
    }

    protected virtual void RenderBarcode() {
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

            g.DrawString(Code.ToUpper(), new Font("Courier", 8), Brushes.Black, r, sf);

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

    protected virtual void Dispose(Boolean disposing) {
      if (disposing) {
        if (this.BarcodeImage != null) { this.BarcodeImage.Dispose(); }
        if (this.ms != null) { this.ms.Dispose(); }
      }
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}