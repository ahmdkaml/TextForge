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

    [Fact]
    public void Module_CreateCodeBlock_SetsExpectedPropertiesAndDefaults()
    {
        var codeBlock = Module.CreateCodeBlock("Console.WriteLine(\"Hello\");");

        Assert.Equal("Console.WriteLine(\"Hello\");", codeBlock.Content);
        Assert.Equal(ModuleType.Code, codeBlock.Type);
        Assert.Equal("Code", codeBlock.StyleKey);
        Assert.Equal("default-code", codeBlock.Name);
        Assert.Equal("Consolas", codeBlock.Features.Font);
        Assert.Equal("#0F172A", codeBlock.Features.Color);
        Assert.Equal("#F8FAFC", codeBlock.Features.HighlightMarker);
        Assert.Equal(1.15, codeBlock.Features.LineSpacing);
    }

    [Fact]
    public void Module_TruncatedPreviewContent_LargeCodeBlock_ClampsToThreeLinesAndAppendsEllipsis()
    {
        var multilineCode = @"line 1: var x = 1;
line 2: var y = 2;
line 3: var z = 3;
line 4: var a = 4;
line 5: var b = 5;";

        var module = Module.CreateCodeBlock(multilineCode);

        var preview = module.TruncatedPreviewContent;
        var lines = preview.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        Assert.True(lines.Length <= 3);
        Assert.EndsWith("...", preview);
        Assert.Contains("line 1", preview);
        Assert.Contains("line 3", preview);
        Assert.DoesNotContain("line 4", preview);
    }

    [Fact]
    public void Module_TruncatedPreviewContent_LargeParagraph_ClampsToWordLimitAndAppendsEllipsis()
    {
        var longParagraph = "Word1 Word2 Word3 Word4 Word5 Word6 Word7 Word8 Word9 Word10 " +
                            "Word11 Word12 Word13 Word14 Word15 Word16 Word17 Word18 Word19 Word20 " +
                            "Word21 Word22 Word23 Word24 Word25 Word26 Word27 Word28 Word29 Word30";

        var module = Module.CreateParagraph(longParagraph);

        var preview = module.TruncatedPreviewContent;
        var words = preview.TrimEnd('.').Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(25, words.Length);
        Assert.EndsWith("...", preview);
        Assert.Contains("Word1", preview);
        Assert.Contains("Word25", preview);
        Assert.DoesNotContain("Word26", preview);
    }

    [Fact]
    public void Module_TruncatedPreviewContent_ShortContent_RemainsUnchanged()
    {
        var shortText = "Short single line text.";
        var module = Module.CreateParagraph(shortText);

        Assert.Equal(shortText, module.TruncatedPreviewContent);
    }

    [Fact]
    public void Module_ContentChanged_RaisesPropertyChangedForTruncatedPreviewContent()
    {
        var module = Module.CreateParagraph("Initial");
        var propertyChangedFired = false;

        module.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(Module.TruncatedPreviewContent))
            {
                propertyChangedFired = true;
            }
        };

        module.Content = "Updated content text with new value";

        Assert.True(propertyChangedFired);
        Assert.Equal("Updated content text with new value", module.TruncatedPreviewContent);
    }
}
