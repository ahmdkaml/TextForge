using System.Collections.Generic;
using TextForge.Core.Modules;

namespace TextForge.Core.Templates;

public class DefaultTemplate
{
    public string Name { get; init; } = "Default";

    // Global document-level baseline styling
    public ModuleFeatures DocumentDefaults { get; init; } = new()
    {
        Font = "Segoe UI",
        Color = "#1F2937", // Neutral dark slate
        FontWeight = ModuleFontWeight.Normal,
        LineSpacing = 1.2
    };

    // Default representations keyed by standard ModuleType
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

    // Specialized archetypes (Heading, Callout, etc.)
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
        ["Callout"] = new ModuleFeatures
        {
            HighlightMarker = "#FEF08A",
            Italic = true,
            LineSpacing = 1.2
        }
    };

    /// <summary>
    /// Resolves the effective feature set by cascading:
    /// Module Explicit Features -> Named Style (if any) -> ModuleType Default -> Document Baseline
    /// </summary>
    public ModuleFeatures ResolveFeatures(Module module)
    {
        // 1. Start with the baseline document defaults
        var resolved = DocumentDefaults;

        // 2. Layer the default styling for this specific module type
        if (TypeDefaults.TryGetValue(module.Type, out var typeStyle))
        {
            resolved = typeStyle.MergeWith(resolved);
        }

        // 3. Layer the named style if a StyleKey is specified
        if (module.StyleKey is not null && NamedStyles.TryGetValue(module.StyleKey, out var namedStyle))
        {
            resolved = namedStyle.MergeWith(resolved);
        }

        // 4. Layer the module's own explicit local overrides
        return module.Features.MergeWith(resolved);
    }
}
