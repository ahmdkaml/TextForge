using TextForge.Core.Documents;
using TextForge.Core.Preview;
using Xunit;

namespace TextForge.Core.Tests.Preview;

public class PreviewRendererTests
{
    [Fact]
    public void Render_WithValidInputs_ReturnsPreviewDocumentWithMatchingText()
    {
        // Arrange
        var document = new DocumentContent("Hello live preview!");
        var template = new TextTemplate();

        // Act
        var preview = PreviewRenderer.Render(document, template);

        // Assert
        Assert.NotNull(preview);
        Assert.Equal("Hello live preview!", preview.Text);
    }

    [Fact]
    public void Render_WithEmptyDocument_ReturnsEmptyPreviewText()
    {
        // Arrange
        var document = new DocumentContent(string.Empty);
        var template = new TextTemplate();

        // Act
        var preview = PreviewRenderer.Render(document, template);

        // Assert
        Assert.NotNull(preview);
        Assert.Equal(string.Empty, preview.Text);
    }
}
