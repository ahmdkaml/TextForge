using System;
using System.Collections.Generic;

namespace TextForge.Core.Modules;

public class Module
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "default-text";
    public string? StyleKey { get; init; }
    public ModuleType Type { get; init; } = ModuleType.Text;
    public string Content { get; set; } = string.Empty;
    public ModuleFeatures Features { get; set; } = ModuleFeatures.Default;
    public List<Module> SubModules { get; init; } = [];
    public bool IsSelected { get; set; }

    public Module() { }

    public Module(
        string content,
        ModuleType type = ModuleType.Text,
        string? styleKey = null,
        ModuleFeatures? features = null,
        string name = "default-text")
    {
        Name = name;
        Content = content;
        Type = type;
        StyleKey = styleKey;
        Features = features ?? ModuleFeatures.Default;
    }

    // --- Default Archetype Presets ---

    public static Module CreateTitle(string text) =>
        new(text, ModuleType.Section, styleKey: "Title", name: "default-title");

    public static Module CreateHeading(string text) =>
        new(text, ModuleType.Section, styleKey: "Heading", name: "default-heading");

    public static Module CreateParagraph(string text) =>
        new(text, ModuleType.Text, styleKey: "Body", name: "default-paragraph");

    public static Module CreateBulletItem(string text) =>
        new($"• {text.TrimStart('•', ' ')}", ModuleType.Text, styleKey: "Body", name: "default-bullet");

    public static Module CreateCallout(string text) =>
        new(text, ModuleType.Text, styleKey: "Callout", name: "default-callout");

    public static Module CreateAlert(string text, string color = "#DC2626") =>
        new(text, ModuleType.Text, features: new ModuleFeatures
        {
            Color = color,
            FontWeight = ModuleFontWeight.Bold,
            LineSpacing = 1.3
        }, name: "default-alert");
}
