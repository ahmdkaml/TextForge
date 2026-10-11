using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using TextForge.Core;
using TextForge.Core.Modules;
using TextForge.Core.Templates;
using TextForge.Desktop.Rendering;
using TextForge.Desktop.Views.Components;
using Avalonia.Controls.Primitives;
using System.Linq;
using TextForge.Desktop.ViewModels;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly AvaloniaPreviewAdapter _previewAdapter = new();
    
    private string _activeTab = string.Empty;
    
    // Safely casts DataContext to MainViewModel
    private MainViewModel? ViewModel => DataContext as MainViewModel;
    
    #region Lifecycle & Initialization

    public MainWindow()
    {
        InitializeComponent();

        // ONLY listen for DataContext assignment. Do not call Bind/Render here!
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // Guard against null ViewModel or Document
        if (ViewModel?.CurrentDocument is null) return;

        // Subscribe preview re-rendering to document changes
        ViewModel.CurrentDocument.Changed += RenderCurrentPreview;

        BindModuleList();
        BindAvailableModules();

        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        moduleEditor?.SetDocument(ViewModel.CurrentDocument);
        moduleEditor?.SetRootSelectionHighlight(ViewModel.CurrentDocument.IsRootSelected);

        RenderCurrentPreview();
    }

    private void BindAvailableModules()
    {
        var availableModulesList = this.FindControl<ItemsControl>("AvailableModulesList");
        if (availableModulesList is not null)
        {
            availableModulesList.ItemsSource = ModuleRegistry.GetAvailableModules();
        }
    }

    private void BindModuleList()
    {
        if (ViewModel?.CurrentDocument is null) return;

        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        if (moduleEditor is null) return;

        moduleEditor.ModuleMoveUpRequested += ModuleEditor_ModuleMoveUpRequested;
        moduleEditor.ModuleMoveDownRequested += ModuleEditor_ModuleMoveDownRequested;
        moduleEditor.ModuleDeleteRequested += ModuleEditor_ModuleDeleteRequested;
        moduleEditor.ModuleDuplicateRequested += ModuleEditor_ModuleDuplicateRequested;
        moduleEditor.ModuleDetachRequested += ModuleEditor_ModuleDetachRequested;
        moduleEditor.RootSelected += ModuleEditor_RootSelected;

        var moduleListBox = moduleEditor.FindControl<ItemsControl>("ModuleListBox");
        if (moduleListBox is not null)
        {
            moduleListBox.ItemsSource = ViewModel.CurrentDocument.Modules;

            moduleListBox.AddHandler(
                InputElement.PointerPressedEvent,
                OnModuleListBoxPointerPressed,
                RoutingStrategies.Bubble,
                handledEventsToo: true);
        }
    }

    private void ModuleEditor_RootSelected(object? sender, EventArgs e)
    {
        if (ViewModel?.CurrentDocument is null) return;

        ViewModel.CurrentDocument.SelectRoot();
        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        moduleEditor?.SetRootSelectionHighlight(true);
        ApplyModuleSelectionHighlight(null);
    }

    private void OnModuleListBoxPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel?.CurrentDocument is null) return;
        if (e.Source is not Visual sourceVisual) return;

        var button = sourceVisual.FindAncestorOfType<Button>();
        if (button is not null && button is not ToggleButton)
        {
            return;
        }

        var cardBorder = (sourceVisual as Border)?.Classes.Contains("module-card") == true
            ? (Border)sourceVisual
            : sourceVisual.GetVisualAncestors()
                          .OfType<Border>()
                          .FirstOrDefault(b => b.Classes.Contains("module-card"));

        if (cardBorder?.DataContext is Module clickedModule)
        {
            ViewModel.CurrentDocument.SelectModule(clickedModule);
            SyncPropertiesToolbar(clickedModule);
            ApplyModuleSelectionHighlight(clickedModule);
        }
    }

    private void ApplyModuleSelectionHighlight(Module? selectedModule)
    {
        if (ViewModel?.CurrentDocument is null) return;

        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        if (moduleEditor is null) return;

        moduleEditor.SetRootSelectionHighlight(selectedModule is null && ViewModel.CurrentDocument.IsRootSelected);

        var allCardBorders = moduleEditor.GetVisualDescendants()
                                         .OfType<Border>()
                                         .Where(b => b.Classes.Contains("module-card"));

        foreach (var border in allCardBorders)
        {
            if (border.Tag is Module module && module == selectedModule)
            {
                if (!border.Classes.Contains("selected"))
                {
                    border.Classes.Add("selected");
                }
            }
            else
            {
                border.Classes.Remove("selected");
            }
        }
    }

    private void ModuleEditor_ModuleMoveUpRequested(object? sender, Module module)
    {
        if (ViewModel?.CurrentDocument?.MoveModuleUp(module) == true)
        {
            RefreshModuleEditorList();
        }
    }

    private void ModuleEditor_ModuleMoveDownRequested(object? sender, Module module)
    {
        if (ViewModel?.CurrentDocument?.MoveModuleDown(module) == true)
        {
            RefreshModuleEditorList();
        }
    }

    private void ModuleEditor_ModuleDeleteRequested(object? sender, Module module)
    {
        ViewModel?.CurrentDocument?.RemoveModule(module);
        RefreshModuleEditorList();
    }

    private void ModuleEditor_ModuleDuplicateRequested(object? sender, Module module)
    {
        if (ViewModel?.CurrentDocument?.DuplicateModule(module) is not null)
        {
            RefreshModuleEditorList();
        }
    }

    private void ModuleEditor_ModuleDetachRequested(object? sender, Module module)
    {
        if (ViewModel?.CurrentDocument is not null)
        {
            ViewModel.CurrentDocument.DetachModule(module);
            RefreshModuleEditorList();
        }
    }

    #endregion

    #region Document & Preview Synchronization

    private void RenderCurrentPreview()
    {
        if (ViewModel?.CurrentDocument is null) return;

        var previewView = this.FindControl<PreviewHostView>("PreviewView");
        var host = previewView?.FindControl<ContentControl>("PreviewHost");
        if (host is null) return;

        // Use Engine directly from ViewModel
        host.Content = ViewModel.Engine.Render(ViewModel.CurrentDocument, ViewModel.Template, _previewAdapter);
    }

    private void ModuleContent_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ViewModel?.CurrentDocument?.NotifyChanged();
    }

    #endregion

    #region Module Selection & Properties Toolbar
    
    private void SyncPropertiesToolbar(Module module)
    {
        var toolbar = this.FindControl<PropertiesToolbarView>("PropertiesToolbar");
        var styleSelector = toolbar?.FindControl<ComboBox>("StyleSelector");
        if (styleSelector is null || string.IsNullOrEmpty(module.StyleKey)) return;

        foreach (var item in styleSelector.Items)
        {
            if (item is ComboBoxItem cbi &&
                cbi.Content?.ToString()?.Equals(module.StyleKey, StringComparison.OrdinalIgnoreCase) == true)
            {
                styleSelector.SelectedItem = cbi;
                break;
            }
        }
    }

    #endregion
    
    #region Export & Background Operations

    private async void ConvertButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentDocument is null) return;

        var convertButton = sender as Button;
        var statusText = this.FindControl<TextBlock>("StatusText");

        try
        {
            if (convertButton is not null) convertButton.IsEnabled = false;
            if (statusText is not null) statusText.Text = "Status: Exporting PDF...";

            await PdfService.CreatePdfAsync(ViewModel.CurrentDocument, ViewModel.Template, "output.pdf");

            if (statusText is not null) statusText.Text = "Status: PDF Exported successfully!";
        }
        catch (Exception ex)
        {
            if (statusText is not null) statusText.Text = $"Status: Export failed ({ex.Message})";
        }
        finally
        {
            if (convertButton is not null) convertButton.IsEnabled = true;
        }
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        ViewModel?.CurrentDocument?.ClearModules();
        RefreshModuleEditorList();
    }

    #endregion

    #region Editor & Selection Interactions

    private void PaletteItem_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentDocument is null) return;

        if (sender is Button button && button.Tag is ModuleDefinition definition)
        {
            var newModule = definition.Create();
            var targetParent = ViewModel.CurrentDocument.IsRootSelected ? null : ViewModel.CurrentDocument.SelectedModule;
            ViewModel.CurrentDocument.AddModule(newModule, parent: targetParent);

            RefreshModuleEditorList();
            SyncPropertiesToolbar(newModule);
        }
    }

    private void RefreshModuleEditorList()
    {
        if (ViewModel?.CurrentDocument is null) return;

        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        moduleEditor?.SetRootSelectionHighlight(ViewModel.CurrentDocument.IsRootSelected);

        var moduleListBox = moduleEditor?.FindControl<ItemsControl>("ModuleListBox");
        if (moduleListBox is not null)
        {
            moduleListBox.ItemsSource = null;
            moduleListBox.ItemsSource = ViewModel.CurrentDocument.Modules;
        }

        ApplyModuleSelectionHighlight(ViewModel.CurrentDocument.SelectedModule);
    }

    #endregion
}