using System.Collections.Generic;

namespace TextForge.Core.Modules;

/// <summary>
/// Central registry providing all available module archetypes supported by TextForge.
/// </summary>
public static class ModuleRegistry
{
    private static readonly List<ModuleDefinition> _definitions =
    [
        new(
            "title",
            "Document Title",
            "Top-level header for document titles",
            "🏷️",
            () => Module.CreateTitle("Untitled Document")
        ),
        new(
            "heading",
            "Section Heading",
            "Category or section separator",
            "📌",
            () => Module.CreateHeading("Section Heading")
        ),
        new(
            "paragraph",
            "Paragraph Text",
            "Standard body copy block",
            "📝",
            () => Module.CreateParagraph("Enter paragraph text...")
        ),
        new(
            "bullet",
            "Bullet Item",
            "Single bullet point entry",
            "•",
            () => Module.CreateBulletItem("Bullet point item")
        ),
        new(
            "callout",
            "Callout Box",
            "Highlighted note, tip, or callout container",
            "💡",
            () => Module.CreateCallout("Important note or callout message.")
        ),
        new(
            "alert",
            "Alert Banner",
            "Highlighted alert banner with emphasis",
            "⚠️",
            () => Module.CreateAlert("Warning or critical notice.")
        )
    ];

    /// <summary>
    /// Gets all registered module definitions available for insertion.
    /// </summary>
    public static IReadOnlyList<ModuleDefinition> GetAvailableModules() => _definitions.AsReadOnly();
}
