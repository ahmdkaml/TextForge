using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using TextForge.Desktop;
using Xunit;

namespace TextForge.Desktop.Tests;

public class ResourceTests
{
    [Theory]
    [InlineData("SpacingXs")]
    [InlineData("SpacingSm")]
    [InlineData("SpacingMd")]
    [InlineData("SpacingLg")]
    [InlineData("PaddingContainer")]
    [InlineData("PaddingStatusBar")]
    public void AppResources_ContainRequiredSpacingTokens(string resourceKey)
    {
        var app = new App();
        app.Initialize();

        Assert.True(app.Resources.ContainsKey(resourceKey), $"Resource '{resourceKey}' should be defined in App.axaml.");
        Assert.NotNull(app.Resources[resourceKey]);
    }
    [Fact]
    public void App_LoadsButtonStyles()
    {
        var app = new App();
        app.Initialize();

        // There should be 3 top-level styles: FluentTheme, ColorPicker, and ButtonStyles
        Assert.True(app.Styles.Count >= 3, $"Expected at least 3 styles, got {app.Styles.Count}");
    }
}
