using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TextForge.Core.Modules;

public class Module : INotifyPropertyChanged
{
    private string _content = string.Empty;
    private bool _isSelected;
    private bool _isExpanded;
    private Module? _parent;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = "default-text";

    public string? StyleKey { get; init; }

    public ModuleType Type { get; init; } = ModuleType.Text;

    /// <summary>
    /// Reference to the containing parent module. Null if this is a root-level module.
    /// Ignored during serialization to avoid cyclic object graphs.
    /// </summary>
    [JsonIgnore]
    public Module? Parent
    {
        get => _parent;
        internal set
        {
            if (_parent != value)
            {
                _parent = value;
                OnPropertyChanged();
            }
        }
    }

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

    public ObservableCollection<Module> SubModules { get; } = [];

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

    public Module()
    {
        WireSubModulesCollection();
    }

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
        WireSubModulesCollection();
    }

    private void WireSubModulesCollection()
    {
        SubModules.CollectionChanged += OnSubModulesChanged;
    }

    private void OnSubModulesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (Module child in e.NewItems)
            {
                child.Parent = this;
            }
        }

        if (e.OldItems != null)
        {
            foreach (Module child in e.OldItems)
            {
                if (child.Parent == this)
                {
                    child.Parent = null;
                }
            }
        }
    }

    public Module Clone()
    {
        var clone = new Module
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Type = Type,
            StyleKey = StyleKey,
            Content = Content,
            Features = Features with { },
            IsExpanded = IsExpanded,
            IsSelected = false
        };

        foreach (var subModule in SubModules)
        {
            // Adding automatically assigns clone as the Parent
            clone.SubModules.Add(subModule.Clone());
        }

        return clone;
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
