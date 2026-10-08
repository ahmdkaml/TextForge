using TextForge.Core.Presentation;

namespace TextForge.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _statusMessage = "Ready";
    private string _currentDocumentTitle = "Untitled.txt";
    private bool _isModified;
    private readonly IModulePaletteProvider _modulePaletteProvider;

    public MainWindowViewModel(IModulePaletteProvider modulePaletteProvider)
    {
        _modulePaletteProvider = modulePaletteProvider;
    }

    public void CreateNewDocument(string title = "Untitled.txt")
    {
        CurrentDocumentTitle = title;
        IsModified = false;
        StatusMessage = $"Created new document: {title}";
    }
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    public string CurrentDocumentTitle
    {
        get => _currentDocumentTitle;
        set => SetProperty(ref _currentDocumentTitle, value);
    }
    public bool IsModified
    {
        get => _isModified;
        set => SetProperty(ref _isModified, value);
    }
}
