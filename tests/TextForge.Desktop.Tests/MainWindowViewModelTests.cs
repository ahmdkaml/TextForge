using TextForge.Core.Presentation;
using TextForge.Core.Modules;
using TextForge.Desktop.ViewModels;
using Xunit;

namespace TextForge.Desktop.Tests;

public class MainWindowViewModelTests
{
    [Fact]
    public void CreateNewDocument_UpdatesStateAndStatusMessage()
    {
        // Arrange
        var mockPaletteProvider = new TestModulePaletteProvider();
        var vm = new MainWindowViewModel(mockPaletteProvider);

        // Act
        vm.CreateNewDocument("NewFile.txt");

        // Assert
        Assert.Equal("NewFile.txt", vm.CurrentDocumentTitle);
        Assert.False(vm.IsModified);
        Assert.Equal("Created new document: NewFile.txt", vm.StatusMessage);
    }
}

internal class TestModulePaletteProvider : IModulePaletteProvider
{
    public IReadOnlyList<ModuleDefinition> GetAvailableModules()
    {
        return new List<ModuleDefinition>
        {
            new ModuleDefinition(
                "testModule",
                "Test Module",
                "A module for testing purposes.",
                "test_icon.png",
                _ => null! // Factory delegate returns null since it's unused in this test
            )
        };
    }
}
