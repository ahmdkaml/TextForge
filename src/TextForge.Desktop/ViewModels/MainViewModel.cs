using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
    public ObservableCollection<ModuleViewModel> ModuleItems { get; } = [];
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

    partial void OnCurrentDocumentChanged(Document value)
    {
        value.Modules.CollectionChanged += OnRootModulesChanged;
        SyncRootModuleItems(value);
    }

    private void SyncRootModuleItems(Document document)
    {
        ModuleItems.Clear();
        foreach (var module in document.Modules)
        {
            ModuleItems.Add(new ModuleViewModel(module, document));
        }
    }

    private void OnRootModulesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
        {
            var index = e.NewStartingIndex;
            foreach (Module module in e.NewItems)
            {
                var item = new ModuleViewModel(module, CurrentDocument);
                if (index >= 0)
                {
                    ModuleItems.Insert(index++, item);
                }
                else
                {
                    ModuleItems.Add(item);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
        {
            foreach (Module module in e.OldItems)
            {
                var item = ModuleItems.FirstOrDefault(existing => existing.Model == module);
                if (item is not null)
                {
                    ModuleItems.Remove(item);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Move)
        {
            ModuleItems.Move(e.OldStartingIndex, e.NewStartingIndex);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            ModuleItems.Clear();
        }
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
    [RelayCommand]
    private void SelectRoot()
    {
        CurrentDocument.SelectRoot();
    }
    [RelayCommand]
    private void Select(Module module)
    {
        CurrentDocument.SelectModule(module);
    }
}
