using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextForge.Core.Modules;

/// <summary>
/// Represents an independent content element within a document.
/// Modules can be nested hierarchically to form compound structures.
/// </summary>
public class Module : INotifyPropertyChanged
{
    private string _content = string.Empty;
    private bool _isSelected;
    private bool _isExpanded;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = "default-text";

    public string? StyleKey { get; init; }

    public ModuleType Type { get; init; } = ModuleType.Text;

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
            }
        }
    }

    public ModuleFeatures Features { get; set; } = ModuleFeatures.Default;

    public ObservableCollection<Module> SubModules { get; set; } = [];

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }

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

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Archetype Presets

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

    #endregion
}
