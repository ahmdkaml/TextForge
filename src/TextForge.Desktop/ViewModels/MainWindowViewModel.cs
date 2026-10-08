using TextForge.Core.Presentation;

namespace TextForge.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IModulePaletteProvider _modulePaletteProvider;

    public MainWindowViewModel(IModulePaletteProvider modulePaletteProvider)
    {
        _modulePaletteProvider = modulePaletteProvider;
    }
}