using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Preview;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Tests;

public class PreviewRendererTests
{
    [Fact]
    public void Render_WithValidInputs_ReturnsPreviewDocumentWithMatchingBlocks()
    {
        // Arrange
        var document = new Document("Live Preview Doc")
            .AddModule(new Module("Hello live preview!", ModuleType.Text));
        var template = new DefaultTemplate();

        // Act
        var preview = PreviewRenderer.Render(document, template);

        // Assert
        Assert.NotNull(preview);
        Assert.Equal("Live Preview Doc", preview.Metadata.Title);
        Assert.Single(preview.Blocks);
        Assert.Equal("Hello live preview!", preview.Blocks[0].Content);
    }

    [Fact]
    public void Render_WithEmptyDocument_ReturnsEmptyBlocks()
    {
        // Arrange
        var document = new Document("Empty Doc");
        var template = new DefaultTemplate();

        // Act
        var preview = PreviewRenderer.Render(document, template);

        // Assert
        Assert.NotNull(preview);
        Assert.Empty(preview.Blocks);
    }
}
