using System.Collections.Generic;
using TextForge.Core.Modules;

namespace TextForge.Core.Templates;

public class DefaultTemplate
{
    public string Name { get; init; } = "Default";

    public ModuleFeatures DocumentDefaults { get; init; } = new()
    {
        Font = "Segoe UI",
        Color = "#1F2937",
        FontWeight = ModuleFontWeight.Normal,
        LineSpacing = 1.2
    };

    public Dictionary<ModuleType, ModuleFeatures> TypeDefaults { get; init; } = new()
    {
        [ModuleType.Text] = new ModuleFeatures
        {
            FontWeight = ModuleFontWeight.Normal,
            LineSpacing = 1.2
        },
        [ModuleType.Section] = new ModuleFeatures
        {
            FontWeight = ModuleFontWeight.Bold,
            LineSpacing = 1.4
        },
        [ModuleType.Container] = new ModuleFeatures
        {
            LineSpacing = 1.0
        },
        [ModuleType.List] = new ModuleFeatures
        {
            LineSpacing = 1.2
        },
        [ModuleType.Custom] = new ModuleFeatures
        {
            LineSpacing = 1.0
        }
    };

    public Dictionary<string, ModuleFeatures> NamedStyles { get; init; } = new()
    {
        ["Title"] = new ModuleFeatures
        {
            FontWeight = ModuleFontWeight.Bold,
            LineSpacing = 1.5
        },
        ["Heading"] = new ModuleFeatures
        {
            FontWeight = ModuleFontWeight.Bold,
            LineSpacing = 1.3
        },
        ["Body"] = new ModuleFeatures
        {
            LineSpacing = 1.0
        },
        ["Callout"] = new ModuleFeatures
        {
            HighlightMarker = "#FEF08A",
            Italic = true,
            LineSpacing = 1.2
        }
    };

    /// <summary>
    /// Correct cascade order (most specific overrides least specific):
    /// Module Explicit -> Named Style -> ModuleType Default -> Document Baseline
    /// </summary>
    public ModuleFeatures ResolveFeatures(Module module)
    {
        var resolved = DocumentDefaults;

        // 1. ModuleType defaults override document baseline
        if (TypeDefaults.TryGetValue(module.Type, out var typeStyle))
        {
            resolved = typeStyle.MergeWith(resolved);
        }

        // 2. Named style overrides module type defaults
        if (module.StyleKey is not null && NamedStyles.TryGetValue(module.StyleKey, out var namedStyle))
        {
            resolved = namedStyle.MergeWith(resolved);
        }

        // 3. Module local features override named style
        return module.Features.MergeWith(resolved);
    }
}
