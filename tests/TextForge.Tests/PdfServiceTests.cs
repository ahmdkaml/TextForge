using System.IO;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Export;
using TextForge.Core.Modules;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Tests;

public class PdfServiceTests
{
    [Fact]
    public void QuestPdfAdapter_Render_ReturnsValidPdfBytes()
    {
        var adapter = new QuestPdfAdapter();
        var metadata = new DocumentMetadata { Title = "Adapter Unit Test" };
        var rootNodes = new[]
        {
            new RenderNode
            {
                Content = "Sample Render Node",
                Type = ModuleType.Text,
                Features = ModuleFeatures.Default,
                Layout = LayoutProperties.Default
            }
        };
        var tree = new RenderTree(metadata, rootNodes);

        var bytes = adapter.Render(tree);

        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void CreatePdf_FromDocumentAndTemplate_GeneratesFile()
    {
        var document = ShowcaseDocumentFactory.Create();
        var template = new DefaultTemplate();
        var outputPath = "test_showcase_engine_output.pdf";

        try
        {
            PdfService.CreatePdf(document, template, outputPath);

            Assert.True(File.Exists(outputPath));
            Assert.True(new FileInfo(outputPath).Length > 0);
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
