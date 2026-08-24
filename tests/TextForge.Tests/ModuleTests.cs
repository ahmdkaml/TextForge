using System;
using TextForge.Core.Modules;
using Xunit;

namespace TextForge.Tests;

public class ModuleTests
{
    [Fact]
    public void ModuleFeatures_Defaults_HaveSensibleValues()
    {
        var features = new ModuleFeatures();

        Assert.Null(features.Color);
        Assert.Null(features.Font);
        Assert.Equal(ModuleFontWeight.Normal, features.FontWeight);
        Assert.False(features.Italic);
        Assert.False(features.Underline);
        Assert.False(features.Strikethrough);
        Assert.Null(features.HighlightMarker);
        Assert.Null(features.LineSpacing);
    }

    [Fact]
    public void Module_InstantiatedWithDefaults_HasUniqueIdAndDefaultProperties()
    {
        var moduleA = new Module();
        var moduleB = new Module();

        Assert.NotEqual(Guid.Empty, moduleA.Id);
        Assert.NotEqual(Guid.Empty, moduleB.Id);
        Assert.NotEqual(moduleA.Id, moduleB.Id);
        Assert.Null(moduleA.StyleKey);
        Assert.Equal(ModuleType.Text, moduleA.Type);
        Assert.Equal(string.Empty, moduleA.Content);
        Assert.NotNull(moduleA.Features);
        Assert.Equal(ModuleFontWeight.Normal, moduleA.Features.FontWeight);
        Assert.Empty(moduleA.SubModules);
    }

    [Fact]
    public void Module_CustomFeatures_RetainsAppliedValues()
    {
        var customFeatures = new ModuleFeatures
        {
            Color = "#FF0000",
            Font = "Roboto",
            FontWeight = ModuleFontWeight.Bold,
            Italic = true,
            Underline = true,
            Strikethrough = false,
            HighlightMarker = "#FFFF00",
            LineSpacing = 1.5
        };

        // Explicitly target features: parameter
        var module = new Module("Header Title", ModuleType.Section, features: customFeatures);

        Assert.Equal("Header Title", module.Content);
        Assert.Equal(ModuleType.Section, module.Type);
        Assert.Null(module.StyleKey);
        Assert.Equal("#FF0000", module.Features.Color);
        Assert.Equal("Roboto", module.Features.Font);
        Assert.Equal(ModuleFontWeight.Bold, module.Features.FontWeight);
        Assert.True(module.Features.Italic);
        Assert.True(module.Features.Underline);
        Assert.False(module.Features.Strikethrough);
        Assert.Equal("#FFFF00", module.Features.HighlightMarker);
        Assert.Equal(1.5, module.Features.LineSpacing);
    }

    [Fact]
    public void Module_HierarchicalNesting_PreservesChildOrderingAndIsolation()
    {
        var root = new Module("Document Root", ModuleType.Container);
        var section1 = new Module("Section 1", ModuleType.Section);
        var paragraph1 = new Module("Paragraph 1.1", ModuleType.Text);
        var paragraph2 = new Module("Paragraph 1.2", ModuleType.Text);
        var section2 = new Module("Section 2", ModuleType.Section);

        section1.SubModules.Add(paragraph1);
        section1.SubModules.Add(paragraph2);
        root.SubModules.Add(section1);
        root.SubModules.Add(section2);

        Assert.Equal(2, root.SubModules.Count);
        Assert.Same(section1, root.SubModules[0]);
        Assert.Same(section2, root.SubModules[1]);

        Assert.Equal(2, root.SubModules[0].SubModules.Count);
        Assert.Equal("Paragraph 1.1", root.SubModules[0].SubModules[0].Content);
        Assert.Equal("Paragraph 1.2", root.SubModules[0].SubModules[1].Content);
        Assert.Empty(root.SubModules[1].SubModules);
    }
}
