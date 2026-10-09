using CommunityToolkit.Mvvm.Input;
using TextForge.Desktop.ViewModels;

namespace TextForge.Desktop.ViewModels;

public partial class TopCommandBarViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public TopCommandBarViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    [RelayCommand]
    private void NewDocument()
    {
        _mainWindowViewModel.CreateNewDocument("Untitled.txt");
    }

    [RelayCommand]
    private void OpenDocument()
    {
        _mainWindowViewModel.StatusMessage = "Opening document...";
    }

    [RelayCommand]
    private void SaveDocument()
    {
        _mainWindowViewModel.IsModified = false;
        _mainWindowViewModel.StatusMessage = $"Saved {_mainWindowViewModel.CurrentDocumentTitle}";
    }
}
