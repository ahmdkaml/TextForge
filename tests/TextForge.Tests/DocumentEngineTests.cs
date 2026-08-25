using System.Collections.Generic;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Core.Tests.Engine;

public class DocumentEngineTests
{
    private readonly DocumentEngine _engine = new();
    private readonly DefaultTemplate _template = new();

    private class MockRenderTarget : IRenderTarget<string>
    {
        public string Render(RenderTree tree) => $"Rendered: {tree.Metadata.Title} ({tree.RootNodes.Count} nodes)";
    }

    [Fact]
    public void Evaluate_ResolvesFeaturesAndLayoutHierarchy()
    {
        var document = new Document("Engine Test Doc")
            .AddModule(new Module("Title Text", ModuleType.Section, styleKey: "Title"))
            .AddModule(new Module("Callout Note", ModuleType.Text, styleKey: "Callout"));

        var tree = _engine.Evaluate(document, _template);

        Assert.NotNull(tree);
        Assert.Equal("Engine Test Doc", tree.Metadata.Title);
        Assert.Equal(2, tree.RootNodes.Count);

        // Verify Title Node
        var titleNode = tree.RootNodes[0];
        Assert.Equal("Title Text", titleNode.Content);
        Assert.Equal(ModuleFontWeight.Bold, titleNode.Features.FontWeight);
        Assert.Equal(16, titleNode.Layout.MarginBottom);

        // Verify Callout Node
        var calloutNode = tree.RootNodes[1];
        Assert.Equal("Callout Note", calloutNode.Content);
        Assert.Equal("#FEF08A", calloutNode.Features.HighlightMarker);
        Assert.Equal(12, calloutNode.Layout.PaddingLeft);
        Assert.True(calloutNode.Features.Italic);
    }

    [Fact]
    public void Evaluate_ProcessesNestedSubModulesRecursively()
    {
        var section = new Module("Main Section", ModuleType.Section, styleKey: "Heading");
        section.SubModules.Add(new Module("Child Item 1", ModuleType.Text));
        section.SubModules.Add(new Module("Child Item 2", ModuleType.Text));

        var document = new Document("Hierarchy Doc").AddModule(section);

        var tree = _engine.Evaluate(document, _template);

        Assert.Single(tree.RootNodes);
        var root = tree.RootNodes[0];
        Assert.Equal(2, root.Children.Count);
        Assert.Equal("Child Item 1", root.Children[0].Content);
        Assert.Equal("Child Item 2", root.Children[1].Content);
    }

    [Fact]
    public void Render_DelegatesToTargetAdapterCorrectly()
    {
        var document = new Document("Adapter Doc")
            .AddModule(new Module("Paragraph", ModuleType.Text));
        var target = new MockRenderTarget();

        var result = _engine.Render(document, _template, target);

        Assert.Equal("Rendered: Adapter Doc (1 nodes)", result);
    }
}
