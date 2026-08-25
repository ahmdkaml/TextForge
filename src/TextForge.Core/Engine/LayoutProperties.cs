namespace TextForge.Core.Engine;

public enum TextAlignment
{
    Left,
    Center,
    Right,
    Justify
}

public record LayoutProperties
{
    public double MarginTop { get; init; } = 0;
    public double MarginBottom { get; init; } = 0;
    public double MarginLeft { get; init; } = 0;
    public double MarginRight { get; init; } = 0;

    public double PaddingTop { get; init; } = 0;
    public double PaddingBottom { get; init; } = 0;
    public double PaddingLeft { get; init; } = 0;
    public double PaddingRight { get; init; } = 0;

    public TextAlignment Alignment { get; init; } = TextAlignment.Left;
    public double Spacing { get; init; } = 0;

    public static LayoutProperties Default => new();

    public LayoutProperties MergeWith(LayoutProperties? fallback)
    {
        if (fallback is null)
        {
            return this;
        }

        return this with
        {
            MarginTop = MarginTop != 0 ? MarginTop : fallback.MarginTop,
            MarginBottom = MarginBottom != 0 ? MarginBottom : fallback.MarginBottom,
            MarginLeft = MarginLeft != 0 ? MarginLeft : fallback.MarginLeft,
            MarginRight = MarginRight != 0 ? MarginRight : fallback.MarginRight,

            PaddingTop = PaddingTop != 0 ? PaddingTop : fallback.PaddingTop,
            PaddingBottom = PaddingBottom != 0 ? PaddingBottom : fallback.PaddingBottom,
            PaddingLeft = PaddingLeft != 0 ? PaddingLeft : fallback.PaddingLeft,
            PaddingRight = PaddingRight != 0 ? PaddingRight : fallback.PaddingRight,

            Alignment = Alignment != TextAlignment.Left ? Alignment : fallback.Alignment,
            Spacing = Spacing != 0 ? Spacing : fallback.Spacing
        };
    }
}
