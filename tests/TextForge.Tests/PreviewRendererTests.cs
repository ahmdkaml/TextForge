using TextForge.Core.Documents;
using TextForge.Core.Preview;
using Xunit;

namespace TextForge.Core.Tests.Preview;

public class PreviewRendererTests
{
    [Fact]
    public void Render_WithValidInputs_ReturnsPreviewDocumentWithMatchingValues()
    {
        var document = new DocumentContent("Hello, world!");
        var template = new TextTemplate();

        var result = PreviewRenderer.Render(document, template);

        Assert.NotNull(result);
        Assert.Equal(template.Title, result.Title);
        Assert.Equal("Hello, world!", result.Text);
    }
    [Fact]
    public void Render_WithValidInputs_MapsTitleAndContentToPreviewDocument()
    {
        // Arrange
        var document = new DocumentContent("Hello live preview!");
        var template = new TextTemplate();

        // Act
        var preview = PreviewRenderer.Render(document, template);

        // Assert
        Assert.NotNull(preview);
        Assert.Equal(template.Title, preview.Title);
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
