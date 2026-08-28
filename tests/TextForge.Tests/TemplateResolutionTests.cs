using Xunit;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
using TextForge.Core.Templates;

namespace TextForge.Tests;

public class TemplateResolutionTests
{
    private readonly DocumentEngine _engine = new();
    private readonly DefaultTemplate _template = new();

    [Fact]
    public void Evaluate_DefaultDocument_AppliesTemplateBaselineBlueColor()
    {
        // Arrange
        var doc = new Document("Test Showcase");
        doc.AddModule(Module.CreateTitle("Welcome to TextForge"));
        doc.AddModule(Module.CreateParagraph("Standard text body."));

        // Act
        var renderTree = _engine.Evaluate(doc, _template);

        // Assert
        Assert.Equal(2, renderTree.RootNodes.Count);
        Assert.Equal("#2563EB", renderTree.RootNodes[0].Features.Color);
        Assert.Equal("#2563EB", renderTree.RootNodes[1].Features.Color);
    }

    [Fact]
    public void Evaluate_ModuleExplicitColorOverride_TakesPrecedenceOverTemplate()
    {
        // Arrange
        var doc = new Document("Alert Test");
        var alertModule = Module.CreateAlert("Warning Message", color: "#DC2626");
        doc.AddModule(alertModule);

        // Act
        var renderTree = _engine.Evaluate(doc, _template);

        // Assert
        Assert.Single(renderTree.RootNodes);
        Assert.Equal("#DC2626", renderTree.RootNodes[0].Features.Color);
    }

    [Fact]
    public void Evaluate_UnknownOrCustomModule_FallsBackSafelyToBaselineColor()
    {
        // Arrange
        var doc = new Document("Fallback Test");
        var unknownModule = new Module("Custom content", ModuleType.Custom, name: "unsupported-custom-archetype");
        doc.AddModule(unknownModule);

        // Act
        var renderTree = _engine.Evaluate(doc, _template);

        // Assert
        Assert.Single(renderTree.RootNodes);
        Assert.Equal("#2563EB", renderTree.RootNodes[0].Features.Color);
    }
}
