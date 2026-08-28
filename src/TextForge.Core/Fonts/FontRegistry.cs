using System.Collections.Generic;

namespace TextForge.Core.Fonts;

public static class FontRegistry
{
    /// <summary>
    /// Global collection of fonts available across all document modules.
    /// </summary>
    public static IReadOnlyList<string> AvailableFonts { get; } =
    [
        "Segoe UI",
        "Inter",
        "Arial",
        "Calibri",
        "Times New Roman",
        "Consolas"
    ];
}
