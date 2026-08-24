using System.Linq;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Preview;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Core.Tests.Preview;

public class PreviewDocumentTests
{
    private readonly DefaultTemplate _template = new();

    [Fact]
    public void Render_EmptyDocument_ProducesPreviewWithMetadataAndZeroBlocks()
    {
        var document = new Document("Blank Doc");

        var preview = PreviewRenderer.Render(document, _template);

        Assert.NotNull(preview);
        Assert.Equal("Blank Doc", preview.Metadata.Title);
        Assert.Empty(preview.Blocks);
    }

    [Fact]
    public void Render_PreservesModuleOrderingAndResolvesStyles()
    {
        var document = new Document("Ordered Doc")
            .AddModule(new Module("Header 1", ModuleType.Section, styleKey: "Heading"))
            .AddModule(new Module("Paragraph 1", ModuleType.Text, styleKey: "Body"))
            .AddModule(new Module("Callout 1", ModuleType.Text, styleKey: "Callout"));

        var preview = PreviewRenderer.Render(document, _template);

        Assert.Equal(3, preview.Blocks.Count);

        // Assert Ordering
        Assert.Equal("Header 1", preview.Blocks[0].Content);
        Assert.Equal("Paragraph 1", preview.Blocks[1].Content);
        Assert.Equal("Callout 1", preview.Blocks[2].Content);

        // Assert Resolved Features
        Assert.Equal(ModuleFontWeight.Bold, preview.Blocks[0].Features.FontWeight);
        Assert.Equal(1.0, preview.Blocks[1].Features.LineSpacing);
        Assert.True(preview.Blocks[2].Features.Italic);
        Assert.Equal("#FEF08A", preview.Blocks[2].Features.HighlightMarker);
    }

    [Fact]
    public void Render_HierarchicalSubModules_PreservesRecursiveStructure()
    {
        var parentModule = new Module("Parent Container", ModuleType.Container);
        var child1 = new Module("Child 1", ModuleType.Text);
        var child2 = new Module("Child 2", ModuleType.Text);
        parentModule.SubModules.Add(child1);
        parentModule.SubModules.Add(child2);

        var document = new Document("Tree Doc").AddModule(parentModule);

        var preview = PreviewRenderer.Render(document, _template);

        Assert.Single(preview.Blocks);
        var parentBlock = preview.Blocks[0];
        Assert.Equal("Parent Container", parentBlock.Content);
        Assert.Equal(2, parentBlock.Children.Count);
        Assert.Equal("Child 1", parentBlock.Children[0].Content);
        Assert.Equal("Child 2", parentBlock.Children[1].Content);
    }
}
