using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Preview;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Core.Tests.Documents;

public class ShowcaseDocumentFactoryTests
{
    [Fact]
    public void Create_GeneratesDocumentWithExpectedHierarchyAndModules()
    {
        var doc = ShowcaseDocumentFactory.Create();

        Assert.Equal("TextForge Showcase", doc.Metadata.Title);
        Assert.NotEmpty(doc.Modules);

        // Verify section nesting
        var section = Assert.Single(doc.Modules, m => m.Content == "Core Capabilities");
        Assert.Equal(3, section.SubModules.Count);
    }

    [Fact]
    public void ShowcaseDocument_RendersSuccessfullyThroughPreviewRenderer()
    {
        var doc = ShowcaseDocumentFactory.Create();
        var template = new DefaultTemplate();

        var preview = PreviewRenderer.Render(doc, template);

        Assert.NotNull(preview);
        Assert.Equal(doc.Modules.Count, preview.Blocks.Count);

        // Verify title styling resolved
        var titleBlock = preview.Blocks[0];
        Assert.Equal(ModuleFontWeight.Bold, titleBlock.Features.FontWeight);

        // Verify callout highlight resolved
        var calloutBlock = Assert.Single(preview.Blocks, b => b.Content.StartsWith("Tip:"));
        Assert.Equal("#FEF08A", calloutBlock.Features.HighlightMarker);
        Assert.True(calloutBlock.Features.Italic);
    }
}
