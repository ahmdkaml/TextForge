using System;
using System.Collections.Generic;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
using Xunit;

namespace TextForge.Core.Tests.Engine;

public class RenderTreeTests
{
    [Fact]
    public void LayoutProperties_MergeWith_PreservesExplicitOverrides()
    {
        var fallback = new LayoutProperties
        {
            MarginTop = 10,
            MarginBottom = 10,
            Alignment = TextAlignment.Center
        };

        var explicitLayout = new LayoutProperties
        {
            MarginTop = 20,
            PaddingLeft = 5
        };

        var merged = explicitLayout.MergeWith(fallback);

        Assert.Equal(20, merged.MarginTop);
        Assert.Equal(10, merged.MarginBottom);
        Assert.Equal(5, merged.PaddingLeft);
        Assert.Equal(TextAlignment.Center, merged.Alignment);
    }

    [Fact]
    public void RenderNode_PreservesHierarchyAndAttributes()
    {
        var child = new RenderNode
        {
            Content = "Child bullet item",
            Type = ModuleType.Text
        };

        var root = new RenderNode
        {
            Content = "Root container",
            Type = ModuleType.Container,
            Children = [child],
            Attributes = new Dictionary<string, object> { ["CustomTag"] = "HeaderGroup" }
        };

        Assert.Single(root.Children);
        Assert.Equal("Child bullet item", root.Children[0].Content);
        Assert.Equal("HeaderGroup", root.Attributes["CustomTag"]);
    }

    [Fact]
    public void RenderTree_InitializesWithMetadataAndNodes()
    {
        var metadata = new DocumentMetadata { Title = "Test Document" };
        var nodes = new List<RenderNode>
        {
            new() { Content = "Heading", Type = ModuleType.Section }
        };

        var tree = new RenderTree(metadata, nodes);

        Assert.Equal("Test Document", tree.Metadata.Title);
        Assert.Single(tree.RootNodes);
        Assert.Equal("Heading", tree.RootNodes[0].Content);
    }
}
