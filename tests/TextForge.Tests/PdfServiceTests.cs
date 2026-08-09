using TextForge.Core;

namespace TextForge.Tests;

using TextForge.Core.Documents;
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
        DocumentContent document = new DocumentContent("Hello World");
        TextTemplate template = new TextTemplate();
        PdfService.CreatePdf(document, template, path);

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

        DocumentContent document = new DocumentContent(text);
        TextTemplate template = new TextTemplate();
        PdfService.CreatePdf(document, template, path);

        using var pdfDocument = PdfDocument.Open(path);

        var page = pdfDocument.GetPage(1);

        Assert.Contains(text, page.Text);
        Assert.Contains(template.Title, page.Text);
        // Cleanup
        File.Delete(path);
    }

}
