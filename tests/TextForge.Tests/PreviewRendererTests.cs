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
}
