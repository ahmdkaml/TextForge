using System;
using System.Collections.Generic;
using System.Linq;

namespace TextForge.Core.Modules;

/// <summary>
/// Central registry providing all available module archetypes supported by TextForge.
/// Serves as the single authoritative source of module definitions and instantiation.
/// </summary>
public static class ModuleRegistry
{
    private static readonly Dictionary<string, ModuleDefinition> _definitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["title"] = new(
            "title",
            "Document Title",
            "Top-level header for document titles",
            "🏷️",
            (content) => Module.CreateTitle(content ?? "Untitled Document")
        ),
        ["heading"] = new(
            "heading",
            "Section Heading",
            "Category or section separator",
            "📌",
            (content) => Module.CreateHeading(content ?? "Section Heading")
        ),
        ["paragraph"] = new(
            "paragraph",
            "Paragraph Text",
            "Standard body copy block",
            "📝",
            (content) => Module.CreateParagraph(content ?? "Enter paragraph text...")
        ),
        ["bullet"] = new(
            "bullet",
            "Bullet Item",
            "Single bullet point entry",
            "•",
            (content) => Module.CreateBulletItem(content ?? "Bullet point item")
        ),
        ["callout"] = new(
            "callout",
            "Callout Box",
            "Highlighted note, tip, or callout container",
            "💡",
            (content) => Module.CreateCallout(content ?? "Important note or callout message.")
        ),
        ["alert"] = new(
            "alert",
            "Alert Banner",
            "Highlighted alert banner with emphasis",
            "⚠️",
            (content) => Module.CreateAlert(content ?? "Warning or critical notice.")
        )
    };

    /// <summary>
    /// Gets all registered module definitions available for insertion.
    /// </summary>
    public static IReadOnlyList<ModuleDefinition> GetAvailableModules() => _definitions.Values.ToList().AsReadOnly();

    /// <summary>
    /// Creates a module instance for the given registered archetype key with optional initial content.
    /// </summary>
    public static Module CreateModule(string key, string? content = null)
    {
        if (!_definitions.TryGetValue(key, out var definition))
        {
            throw new KeyNotFoundException($"No module definition registered for key: '{key}'.");
        }

        return definition.Create(content);
    }
}
