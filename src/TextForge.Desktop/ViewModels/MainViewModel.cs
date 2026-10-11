using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
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
    private partial string ActiveTab { get; set; } = string.Empty;

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
    private void ToggleTab(string tabName)
    {
        ActiveTab = (ActiveTab == tabName) ? string.Empty : tabName;
    }

    [RelayCommand]
    private void CloseDrawer()
    {
        ActiveTab = string.Empty;
    }
    
    public IReadOnlyList<ModuleDefinition> AvailableModules { get; } = ModuleRegistry.GetAvailableModules();
    [RelayCommand]
    private void ClearAllModules()
    {
        CurrentDocument.ClearModules();
    }
    
    [ObservableProperty]
    public partial string ExportStatusMessage { get; set; } = "Status: Ready";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportPdfCommand))]
    public partial bool IsExporting { get; set; }

    private bool CanExportPdf() => !IsExporting && CurrentDocument is not null;

    [RelayCommand(CanExecute = nameof(CanExportPdf))]
    private async Task ExportPdfAsync()
    {
        try
        {
            IsExporting = true;
            ExportStatusMessage = "Status: Exporting PDF...";

            await PdfService.CreatePdfAsync(CurrentDocument, Template, "output.pdf");

            ExportStatusMessage = "Status: PDF Exported successfully!";
        }
        catch (Exception ex)
        {
            ExportStatusMessage = $"Status: Export failed ({ex.Message})";
        }
        finally
        {
            IsExporting = false;
        }
    }
    [RelayCommand]
    private void AddModule(ModuleDefinition definition)
    {
        var newModule = definition.Create();
        var targetParent = CurrentDocument.IsRootSelected ? null : CurrentDocument.SelectedModule;
        
        CurrentDocument.AddModule(newModule, parent: targetParent);
    }

}