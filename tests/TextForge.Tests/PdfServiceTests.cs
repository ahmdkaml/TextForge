using TextForge.Core;

namespace TextForge.Tests;

using UglyToad.PdfPig;

public class PdfServiceTests
{
    [Fact]
    public void CreatePdf_CreatesFile()
    {
        // Arrange
        string path = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.pdf");

        // Act
        PdfService.CreatePdf("Hello World", path);

        // Assert
        Assert.True(File.Exists(path));

        // Cleanup
        File.Delete(path);
    }
    [Fact]
    public void CreatePdf_ContentIsCorrect()
    {
        string text = "Hello World";

        string path = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.pdf");

        PdfService.CreatePdf(text, path);

        using var document = PdfDocument.Open(path);

        var page = document.GetPage(1);

        Assert.Equal(text, page.Text);
        // Cleanup
        File.Delete(path);
    }
    
}
