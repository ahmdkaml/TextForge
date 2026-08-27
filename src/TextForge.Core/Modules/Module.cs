using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TextForge.Core.Modules;

/// <summary>
/// Represents an independent content element within a document.
/// Modules can be nested hierarchically to form compound structures.
/// </summary>
public class Module
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Archetype identifier used for preset styling and UI representation (e.g., "default-title").
    /// </summary>
    public string Name { get; set; } = "default-text";

    /// <summary>
    /// Optional key corresponding to a template style rule (e.g., "Title", "Heading", "Body").
    /// </summary>
    public string? StyleKey { get; init; }

    public ModuleType Type { get; init; } = ModuleType.Text;

    public string Content { get; set; } = string.Empty;


    /// <summary>
    /// Direct visual overrides that take precedence over the template style.
    /// </summary>
    public ModuleFeatures Features { get; set; } = ModuleFeatures.Default;

    /// <summary>
    /// Nested child modules (e.g., list items under a section heading).
    /// </summary>
    public ObservableCollection<Module> SubModules { get; set; } = [];


    /// <summary>
    /// Ephemeral UI selection state. Retained in-memory during active sessions.
    /// </summary>
    public bool IsSelected { get; set; }

    public Module() { }

    public Module(
        string content,
        ModuleType type = ModuleType.Text,
        string? styleKey = null,
        ModuleFeatures? features = null,
        string name = "default-text")
    {
        Content = content;
        Type = type;
        StyleKey = styleKey;
        Features = features ?? ModuleFeatures.Default;
        Name = name;
    }

    #region Archetype Presets

    /// <summary>
    /// Creates a top-level document title module.
    /// </summary>
    public static Module CreateTitle(string text) =>
        new(text, ModuleType.Section, styleKey: "Title", name: "default-title");

    /// <summary>
    /// Creates a section header module.
    /// </summary>
    public static Module CreateHeading(string text) =>
        new(text, ModuleType.Section, styleKey: "Heading", name: "default-heading");

    /// <summary>
    /// Creates a standard body paragraph module.
    /// </summary>
    public static Module CreateParagraph(string text) =>
        new(text, ModuleType.Text, styleKey: "Body", name: "default-paragraph");

    /// <summary>
    /// Creates a bullet item module with prefixed bullet character.
    /// </summary>
    public static Module CreateBulletItem(string text) =>
        new($"• {text.TrimStart('•', ' ')}", ModuleType.Text, styleKey: "Body", name: "default-bullet");

    /// <summary>
    /// Creates an accent callout banner module.
    /// </summary>
    public static Module CreateCallout(string text) =>
        new(text, ModuleType.Text, styleKey: "Callout", name: "default-callout");

    /// <summary>
    /// Creates an alert module with explicit color and weight overrides.
    /// </summary>
    public static Module CreateAlert(string text, string color = "#DC2626") =>
        new(text, ModuleType.Text, features: new ModuleFeatures
        {
            Color = color,
            FontWeight = ModuleFontWeight.Bold,
            LineSpacing = 1.3
        }, name: "default-alert");

    #endregion
}
