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

        Assert.True(app.Resources.ContainsKey(resourceKey), $"Resource '{resourceKey}' should be defined in App.axaml.");
        Assert.NotNull(app.Resources[resourceKey]);
    }
}
