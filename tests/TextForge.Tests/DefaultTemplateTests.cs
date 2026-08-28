using TextForge.Core.Modules;
using TextForge.Core.Templates;
using Xunit;

namespace TextForge.Core.Tests.Templates;

public class DefaultTemplateTests
{
    private readonly DefaultTemplate _template = new();

    [Fact]
    public void ResolveFeatures_StandardTextModule_InheritsDocumentAndTypeDefaults()
    {
        // Arrange: module without explicit features or style key
        var module = new Module("Standard text", ModuleType.Text);

        // Act
        var features = _template.ResolveFeatures(module);

        // Assert: gets baseline font and color from document defaults + type line spacing
        Assert.Equal("Segoe UI", features.Font);
        Assert.Equal("#2563EB", features.Color);
        Assert.Equal(ModuleFontWeight.Normal, features.FontWeight);
        Assert.Equal(1.2, features.LineSpacing);
        Assert.False(features.Italic);
    }

    [Fact]
    public void ResolveFeatures_SectionModule_InheritsBoldWeight()
    {
        // Arrange
        var module = new Module("Section Header", ModuleType.Section);

        // Act
        var features = _template.ResolveFeatures(module);

        // Assert
        Assert.Equal(ModuleFontWeight.Bold, features.FontWeight);
        Assert.Equal(1.4, features.LineSpacing);
        Assert.Equal("Segoe UI", features.Font);
    }

    [Fact]
    public void ResolveFeatures_WithNamedStyle_CascadesCorrectly()
    {
        // Arrange
        var module = new Module("Important callout", ModuleType.Text, styleKey: "Callout");

        // Act
        var features = _template.ResolveFeatures(module);

        // Assert
        Assert.Equal("#FEF08A", features.HighlightMarker);
        Assert.True(features.Italic);
        Assert.Equal("Segoe UI", features.Font);
    }

    [Fact]
    public void ResolveFeatures_WithLocalOverrides_TakesPrecedenceOverAllCascades()
    {
        // Arrange
        var custom = new ModuleFeatures
        {
            Color = "#EF4444",
            Font = "Fira Code",
            Strikethrough = true
        };
        var module = new Module("Custom error", ModuleType.Text, styleKey: "Callout", features: custom);

        // Act
        var features = _template.ResolveFeatures(module);

        // Assert
        Assert.Equal("#EF4444", features.Color);      // Overridden
        Assert.Equal("Fira Code", features.Font);     // Overridden
        Assert.True(features.Strikethrough);          // Overridden
        Assert.True(features.Italic);                 // Inherited from Callout
        Assert.Equal("#FEF08A", features.HighlightMarker); // Inherited from Callout
    }

    [Theory]
    [InlineData(ModuleType.Text)]
    [InlineData(ModuleType.Section)]
    [InlineData(ModuleType.Container)]
    [InlineData(ModuleType.List)]
    [InlineData(ModuleType.Custom)]
    public void ResolveFeatures_AllSupportedModuleTypes_HaveDefinedRepresentations(ModuleType type)
    {
        // Arrange
        var module = new Module("Content", type);

        // Act
        var features = _template.ResolveFeatures(module);

        // Assert
        Assert.NotNull(features);
        Assert.NotNull(features.Font);
        Assert.NotNull(features.Color);
    }
}
