using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Templates;

namespace TextForge.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public IDocumentEngine Engine { get; } = new DocumentEngine();
    public DefaultTemplate Template { get; } = new();

    [ObservableProperty]
    public partial Document CurrentDocument { get; set; }

    // --- NEW DRAWER STATE ---
    [ObservableProperty]
    public partial string ActiveTab { get; set; } = string.Empty;

    // Derived flags for XAML IsVisible bindings
    public bool IsDrawerOpen => !string.IsNullOrEmpty(ActiveTab);
    public bool ShowOutline => ActiveTab == "Outline";
    public bool ShowPalette => ActiveTab == "Palette";

    public MainViewModel()
    {
        CurrentDocument = ShowcaseDocumentFactory.Create();
    }

    partial void OnActiveTabChanged(string value)
    {
        // Notify XAML of derived visibility changes when ActiveTab mutates
        OnPropertyChanged(nameof(IsDrawerOpen));
        OnPropertyChanged(nameof(ShowOutline));
        OnPropertyChanged(nameof(ShowPalette));
    }

    [RelayCommand]
    public void ToggleTab(string tabName)
    {
        ActiveTab = (ActiveTab == tabName) ? string.Empty : tabName;
    }

    [RelayCommand]
    public void CloseDrawer()
    {
        ActiveTab = string.Empty;
    }
}