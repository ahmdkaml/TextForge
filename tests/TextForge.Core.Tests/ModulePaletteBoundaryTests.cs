using Moq;
using TextForge.Core.Modules;
using TextForge.Core.Presentation;
using Xunit;

namespace TextForge.Core.Tests;

public class ModulePaletteBoundaryTests
{
    [Fact]
    public void ModulePaletteProvider_CanBeMockedInCore_WithoutAvaloniaDependency()
    {
        var mockProvider = new Mock<IModulePaletteProvider>();
        mockProvider
            .Setup(p => p.GetAvailableModules())
            .Returns(Array.Empty<ModuleDefinition>());

        var result = mockProvider.Object.GetAvailableModules();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}