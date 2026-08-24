using System.IO;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Preview;
using Xunit;

namespace TextForge.Tests;

public class PdfServiceTests
{
    [Fact]
    public void CreatePdf_WithValidPreview_GeneratesFile()
    {
        // Arrange
        var preview = new PreviewDocument("Sample text for PDF generation");
        var outputPath = "test_output.pdf";

        try
        {
            // Act
            PdfService.CreatePdf(preview, outputPath);

            // Assert
            Assert.True(File.Exists(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void CreatePdf_FromRenderedDocument_GeneratesFile()
    {
        // Arrange
        var document = new DocumentContent("Hello from pipeline test");
        var template = new TextTemplate();
        var preview = PreviewRenderer.Render(document, template);
        var outputPath = "test_pipeline_output.pdf";

        try
        {
            // Act
            PdfService.CreatePdf(preview, outputPath);

            // Assert
            Assert.True(File.Exists(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }
}
