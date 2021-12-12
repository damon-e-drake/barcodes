using DEDrake.Barcodes.Utils;
using Xunit;

namespace DEDrake.Barcodes.UnitTests {
  public class ISBNTests {
    [Theory(DisplayName = "Convert ISBN 10 to 13")]
    [InlineData("0-358-65303-7", "9780358653035")]
    [InlineData("0358653037", "9780358653035")]
    [InlineData("1098100964", "9781098100964")]
    [InlineData("043942089x", "9780439420891")]
    public void Convert10To13(string isbn10, string isbn13) {
      var converted = isbn10.ISBNConvert10To13();

      Assert.Equal(isbn13, converted);
    }

    [Theory(DisplayName = "Convert ISBN 10 to 13")]
    [InlineData("0-358-65303-7", "978-0-358-65303-5")]
    [InlineData("0358653037", "978-0-358-65303-5")]
    [InlineData("1098100964", "978-1-098-10096-4")]
    [InlineData("043942089x", "978-0-439-42089-1")]
    public void Convert10To13Hyphens(string isbn10, string isbn13) {
      var converted = isbn10.ISBNConvert10To13(ISBNFormat.Hyphens);

      Assert.Equal(isbn13, converted);
    }

    [Theory(DisplayName = "Convert ISBN 10 to 13")]
    [InlineData("0-358-65303-7", "978 0 358 65303 5")]
    [InlineData("0358653037", "978 0 358 65303 5")]
    [InlineData("1098100964", "978 1 098 10096 4")]
    [InlineData("043942089x", "978 0 439 42089 1")]
    public void Convert10To13Spaces(string isbn10, string isbn13) {
      var converted = isbn10.ISBNConvert10To13(ISBNFormat.Spaces);

      Assert.Equal(isbn13, converted);
    }

    [Theory(DisplayName = "Convert ISBN 13 to 10")]
    [InlineData("978 03 5865-3035", "0358653037")]
    [InlineData("9780358653035", "0358653037")]
    [InlineData("9781098100964", "1098100964")]
    [InlineData("9780439420891", "043942089X")]
    public void Convert13To10(string isbn13, string isbn10) {
      var converted = isbn13.ISBNConvert13to10();

      Assert.Equal(isbn10, converted);
    }

    [Theory(DisplayName = "Convert ISBN 13 to 10 with Hyphens")]
    [InlineData("978 03 5865-3035", "0-358-65303-7")]
    [InlineData("9780358653035", "0-358-65303-7")]
    [InlineData("9781098100964", "1-098-10096-4")]
    [InlineData("9780439420891", "0-439-42089-X")]
    public void Convert13To10Hyphens(string isbn13, string isbn10) {
      var converted = isbn13.ISBNConvert13to10(ISBNFormat.Hyphens);

      Assert.Equal(isbn10, converted);
    }

    [Theory(DisplayName = "Convert ISBN 13 to 10 with Spaces")]
    [InlineData("978 03 5865-3035", "0 358 65303 7")]
    [InlineData("9780358653035", "0 358 65303 7")]
    [InlineData("9781098100964", "1 098 10096 4")]
    [InlineData("9780439420891", "0 439 42089 X")]
    public void Convert13To10Spaces(string isbn13, string isbn10) {
      var converted = isbn13.ISBNConvert13to10(ISBNFormat.Spaces);

      Assert.Equal(isbn10, converted);
    }
  }
}
