namespace TextForge.Core.Modules;

public enum ModuleFontWeight
{
    Normal,
    Bold,
    Light
}

public record ModuleFeatures
{
    public string? Color { get; init; } = null;
    public string? Font { get; init; } = null;
    public ModuleFontWeight FontWeight { get; init; } = ModuleFontWeight.Normal;
    public bool Italic { get; init; } = false;
    public bool Underline { get; init; } = false;
    public bool Strikethrough { get; init; } = false;
    public string? HighlightMarker { get; init; } = null;
    public double LineSpacing { get; init; } = 1.0;

    public static ModuleFeatures Default => new();
}
