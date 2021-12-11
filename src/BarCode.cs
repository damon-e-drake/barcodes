using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace DEDrake.Barcodes {
  public class Barcode : IDisposable {
    protected Image? barcodeImage = null;
    protected StringBuilder BinaryText;
    protected int LineBuffer = 0;
    protected MemoryStream ms;
    protected string Code;

    public virtual byte[] BinaryImage {
      get {
        if (ms.Length == 0) RenderBarcode(); return ms.ToArray();
      }
    }

    public Image BarcodeImage {
      get {
        if (ms.Length == 0) RenderBarcode();
        if (barcodeImage == null) barcodeImage = Image.FromStream(ms);
        return barcodeImage;
      }
    }

    public Barcode(string code) {
      ms = new MemoryStream();
      BinaryText = new StringBuilder();
      LineBuffer = 21;
      Code = code;
    }

    public void SaveAs(string FilePath, bool OverWrite = false) {
      if (File.Exists(FilePath) && !OverWrite)
        throw new IOException("File exits.");
      using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write); fs.Write(BinaryImage, 0, BinaryImage.Length);
    }

    protected virtual void RenderBarcode() {
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

    protected virtual void Dispose(bool disposing) {
      if (disposing) {
        if (BarcodeImage != null) BarcodeImage.Dispose();
        if (ms != null) ms.Dispose();
      }
    }

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
