using System.Windows.Input;
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

        Assert.IsAssignableFrom<ICommand>(commandBarVM.NewDocumentCommand);
        commandBarVM.NewDocumentCommand.Execute(null);

        Assert.Equal("Untitled.txt", mainVM.CurrentDocumentTitle);
        Assert.False(mainVM.IsModified);
        Assert.Equal("Created new document: Untitled.txt", mainVM.StatusMessage);
    }

    [Fact]
    public void OpenDocumentCommand_UpdatesStatusMessageInMainViewModel()
    {
        var mockPaletteProvider = new TestModulePaletteProvider();
        var mainVM = new MainWindowViewModel(mockPaletteProvider);
        var commandBarVM = new TopCommandBarViewModel(mainVM);

        Assert.IsAssignableFrom<ICommand>(commandBarVM.OpenDocumentCommand);
        commandBarVM.OpenDocumentCommand.Execute(null);

        Assert.Equal("Opening document...", mainVM.StatusMessage);
    }

    [Fact]
    public void SaveDocumentCommand_ResetsModifiedAndUpdatesStatusMessageInMainViewModel()
    {
        var mockPaletteProvider = new TestModulePaletteProvider();
        var mainVM = new MainWindowViewModel(mockPaletteProvider);
        mainVM.CurrentDocumentTitle = "Report.txt";
        mainVM.IsModified = true;
        var commandBarVM = new TopCommandBarViewModel(mainVM);

        Assert.IsAssignableFrom<ICommand>(commandBarVM.SaveDocumentCommand);
        commandBarVM.SaveDocumentCommand.Execute(null);

        Assert.False(mainVM.IsModified);
        Assert.Equal("Saved Report.txt", mainVM.StatusMessage);
    }
}
