using TextForge.Core.Presentation;
using TextForge.Desktop.ViewModels;
using Xunit;

namespace TextForge.Desktop.Tests;

public class TopCommandBarViewModelTests
{
    [Fact]
    public void NewDocumentCommand_ResetsDocumentStateInMainViewModel()
    {
        var mockPaletteProvider = new TestModulePaletteProvider();
        var mainVM = new MainWindowViewModel(mockPaletteProvider);
        var commandBarVM = new TopCommandBarViewModel(mainVM);

        commandBarVM.NewDocumentCommand.Execute(null);

        Assert.Equal("Untitled.txt", mainVM.CurrentDocumentTitle);
        Assert.False(mainVM.IsModified);
        Assert.Equal("Created new document: Untitled.txt", mainVM.StatusMessage);
    }
}
