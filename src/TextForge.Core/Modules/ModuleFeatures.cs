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
    public double? LineSpacing { get; init; } = null;

    public static ModuleFeatures Default => new();

    public ModuleFeatures MergeWith(ModuleFeatures? fallback)
    {
        if (fallback is null)
        {
            return this;
        }

        return this with
        {
            Color = Color ?? fallback.Color,
            Font = Font ?? fallback.Font,
            FontWeight = FontWeight != ModuleFontWeight.Normal ? FontWeight : fallback.FontWeight,
            Italic = Italic || fallback.Italic,
            Underline = Underline || fallback.Underline,
            Strikethrough = Strikethrough || fallback.Strikethrough,
            HighlightMarker = HighlightMarker ?? fallback.HighlightMarker,
            LineSpacing = LineSpacing ?? fallback.LineSpacing
        };
    }
}
