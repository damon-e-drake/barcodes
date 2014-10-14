using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Smelds.Barcodes;

namespace ConsoleApp {
  public class Program {
    public static void Main(string[] args) {

      using (var b = new Bookland("9780816155293")) {
        b.SaveAs("9780816155293.png", true);
      }

      using (var c= new Code39("DAC148")) {
        c.SaveAs("122.png", true);
      }

      Console.WriteLine("043942089X".ISBNConvert10To13());
      Console.WriteLine("978034553898".ISBNConvert13to10());

      Console.ReadKey(true);
    }
  }

}
