using System;
using System.Collections.Generic;
using TextForge.Core.Engine;
using TextForge.Core.Modules;

namespace TextForge.Core.Templates;

public class DefaultTemplate
{
    public string Name { get; init; } = "Default";

    /// <summary>
    /// Global template baseline defaults. Default text color is standard blue.
    /// </summary>
    public ModuleFeatures DocumentDefaults { get; init; } = new()
    {
        Font = "Segoe UI",
        Color = "#2563EB", // Standard Template Blue
        FontWeight = ModuleFontWeight.Normal,
        LineSpacing = 1.2
    };

    public LayoutProperties DocumentLayoutDefaults { get; init; } = new()
    {
        MarginBottom = 8,
        Spacing = 6
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

    public Dictionary<ModuleType, LayoutProperties> TypeLayoutDefaults { get; init; } = new()
    {
        [ModuleType.Section] = new LayoutProperties
        {
            MarginTop = 16,
            MarginBottom = 8
        },
        [ModuleType.Text] = new LayoutProperties
        {
            MarginBottom = 6
        },
        [ModuleType.Container] = new LayoutProperties
        {
            MarginBottom = 12,
            Spacing = 8
        },
        [ModuleType.List] = new LayoutProperties
        {
            MarginLeft = 16,
            MarginBottom = 4
        }
    };

    public Dictionary<string, ModuleFeatures> NamedStyles { get; init; } = new(StringComparer.OrdinalIgnoreCase)
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

    public Dictionary<string, LayoutProperties> NamedLayoutStyles { get; init; } = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Title"] = new LayoutProperties
        {
            MarginTop = 0,
            MarginBottom = 16
        },
        ["Heading"] = new LayoutProperties
        {
            MarginTop = 18,
            MarginBottom = 8
        },
        ["Callout"] = new LayoutProperties
        {
            PaddingLeft = 12,
            PaddingRight = 12,
            PaddingTop = 8,
            PaddingBottom = 8,
            MarginTop = 8,
            MarginBottom = 8
        }
    };

    public ModuleFeatures ResolveFeatures(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);

        var resolved = DocumentDefaults;

        if (TypeDefaults.TryGetValue(module.Type, out var typeStyle))
        {
            resolved = typeStyle.MergeWith(resolved);
        }

        var key = module.StyleKey ?? module.Name;
        if (!string.IsNullOrWhiteSpace(key) && NamedStyles.TryGetValue(key, out var namedStyle))
        {
            resolved = namedStyle.MergeWith(resolved);
        }

        return module.Features.MergeWith(resolved);
    }

    public LayoutProperties ResolveLayout(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);

        var resolved = DocumentLayoutDefaults;

        if (TypeLayoutDefaults.TryGetValue(module.Type, out var typeLayout))
        {
            resolved = typeLayout.MergeWith(resolved);
        }

        var key = module.StyleKey ?? module.Name;
        if (!string.IsNullOrWhiteSpace(key) && NamedLayoutStyles.TryGetValue(key, out var namedLayout))
        {
            resolved = namedLayout.MergeWith(resolved);
        }

        return resolved;
    }
}
