using System.IO;
using System.Collections.Generic;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Preview;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Tests;

public class PdfServiceTests
{
    [Fact]
    public void CreatePdf_WithValidPreview_GeneratesFile()
    {
        // Arrange
        var metadata = new DocumentMetadata { Title = "Test PDF Document" };
        var blocks = new List<RenderedBlock>
        {
            new("Sample text for PDF generation", ModuleType.Text, ModuleFeatures.Default, [])
        };
        var preview = new PreviewDocument(metadata, blocks);
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
        var document = new Document("Pipeline Document")
            .AddModule(new Module("Hello from pipeline test", ModuleType.Text));
        var template = new DefaultTemplate();
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
