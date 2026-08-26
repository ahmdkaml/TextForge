using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
using TextForge.Core.Templates;
using TextForge.Desktop.Rendering;
using TextForge.Desktop.Views.Components;

namespace TextForge.Desktop.Views;

/// <summary>
/// Main application shell and primary presentation coordinator for TextForge.
/// <para>
/// <b>Purpose:</b> Acts as the presentation layer connecting the in-memory domain model (<see cref="Document"/>),
/// the layout rendering engine (<see cref="IDocumentEngine"/>), and user-facing child view components.
/// It observes document changes, updates the visual preview canvas, and routes user edits back to Core.
/// </para>
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// The template-driven layout engine used to resolve module hierarchy and compile visual trees.
    /// </summary>
    private readonly IDocumentEngine _engine = new DocumentEngine();

    /// <summary>
    /// The active visual stylesheet applying layout rules, typography, and color tokens.
    /// </summary>
    private readonly DefaultTemplate _template = new();

    /// <summary>
    /// Platform adapter translating engine instructions into native Avalonia visual controls.
    /// </summary>
    private readonly AvaloniaPreviewAdapter _previewAdapter = new();

    /// <summary>
    /// The root domain document currently being edited and displayed in the workspace.
    /// </summary>
    private Document _currentDocument;

    /// <summary>
    /// Tracks the identifier of the currently opened drawer tab ("Outline" or "Palette") to support toggle behavior.
    /// </summary>
    private string _activeTab = string.Empty;

    #region Lifecycle & Initialization

    /// <summary>
    /// Initializes UI components, loads the default showcase document into memory,
    /// subscribes to domain mutation events, binds child component controls, and triggers the initial preview render.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _currentDocument = ShowcaseDocumentFactory.Create();

        // Subscribe preview re-rendering to document changes so any domain mutation refreshes the UI
        _currentDocument.Changed += RenderCurrentPreview;

        BindModuleList();
        BindAvailableModules();
        RenderCurrentPreview();
    }

    /// <summary>
    /// Populates the module palette drawer with all available module archetypes registered in Core.
    /// </summary>
    private void BindAvailableModules()
    {
        var availableModulesList = this.FindControl<ItemsControl>("AvailableModulesList");
        if (availableModulesList is not null)
        {
            availableModulesList.ItemsSource = ModuleRegistry.GetAvailableModules();
        }
    }

    /// <summary>
    /// Locates the <see cref="ListBox"/> control inside the child <see cref="ModuleEditorView"/>
    /// and binds it to the current document's module collection while hooking selection changes.
    /// </summary>
    private void BindModuleList()
    {
        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        if (moduleEditor is not null)
        {
            moduleEditor.ModuleDeleteRequested += ModuleEditor_ModuleDeleteRequested;

            var moduleListBox = moduleEditor.FindControl<ListBox>("ModuleListBox");
            if (moduleListBox is not null)
            {
                moduleListBox.ItemsSource = _currentDocument.Modules;
                moduleListBox.SelectionChanged += ModuleListBox_SelectionChanged;
            }
        }
    }
    private void ModuleEditor_ModuleDeleteRequested(object? sender, Module module)
    {
        // 1. Remove from domain model (triggers Changed -> updates preview)
        _currentDocument.RemoveModule(module);

        // 2. Refresh the editor ListBox to update UI items
        RefreshModuleEditorList();
    }

    #endregion

    #region Document & Preview Synchronization

    /// <summary>
    /// Compiles the current <see cref="_currentDocument"/> into an Avalonia visual tree via <see cref="_engine"/>
    /// and mounts it into the preview viewport container hosted within <see cref="PreviewHostView"/>.
    /// </summary>
    private void RenderCurrentPreview()
    {
        var previewView = this.FindControl<PreviewHostView>("PreviewView");
        var host = previewView?.FindControl<ContentControl>("PreviewHost");
        if (host is null) return;

        host.Content = _engine.Render(_currentDocument, _template, _previewAdapter);
    }

    /// <summary>
    /// Routed event listener attached to the editor container that intercepts bubbling <see cref="TextBox.TextChanged"/>
    /// events from module text inputs, triggering a domain change notification to re-render the preview.
    /// </summary>
    /// <param name="sender">The control where the text event originated.</param>
    /// <param name="e">Event args associated with the text alteration.</param>
    private void ModuleContent_TextChanged(object? sender, TextChangedEventArgs e)
    {
        _currentDocument.NotifyChanged();
    }

    #endregion

    #region Module Selection & Properties Toolbar

    /// <summary>
    /// Synchronizes selection changes made in the module list with the active <see cref="_currentDocument"/>
    /// and updates the horizontal properties toolbar controls accordingly.
    /// </summary>
    /// <param name="sender">The <see cref="ListBox"/> control raising the selection event.</param>
    /// <param name="e">Selection event data containing newly selected items.</param>
    private void ModuleListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is Module selectedModule)
        {
            _currentDocument.SelectModule(selectedModule);
            SyncPropertiesToolbar(selectedModule);
        }
    }

    /// <summary>
    /// Synchronizes the style selection dropdown in <see cref="PropertiesToolbarView"/> with the
    /// <see cref="Module.StyleKey"/> of the currently selected module.
    /// </summary>
    /// <param name="module">The selected module whose properties are being reflected.</param>
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

    #region Navigation & Drawer Shell

    /// <summary>
    /// Handles left-rail navigation button clicks to open, switch, or collapse sidebar drawer panels.
    /// </summary>
    /// <param name="sender">The navigation button clicked by the user.</param>
    /// <param name="e">Routed event arguments.</param>
    private void NavButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;

        var selectedTab = btn.Name switch
        {
            "NavOutlineBtn" => "Outline",
            "NavPaletteBtn" => "Palette",
            _ => string.Empty
        };

        // If the clicked tab is already open, clicking it again collapses the drawer
        if (_activeTab == selectedTab)
        {
            CloseDrawer();
        }
        else
        {
            OpenDrawer(selectedTab);
        }
    }

    /// <summary>
    /// Opens the left sidebar drawer and displays the requested sub-panel (e.g., Outline or Palette).
    /// </summary>
    /// <param name="tab">The name of the tab view to display.</param>
    private void OpenDrawer(string tab)
    {
        _activeTab = tab;

        SetDrawerVisibility(
            isOpen: true,
            showOutline: tab == "Outline",
            showPalette: tab == "Palette"
        );
    }

    /// <summary>
    /// Event handler for the close button located inside drawer headers.
    /// </summary>
    /// <param name="sender">The button control triggering the closure.</param>
    /// <param name="e">Routed event arguments.</param>
    private void CloseDrawer_Click(object? sender, RoutedEventArgs e)
    {
        CloseDrawer();
    }

    /// <summary>
    /// Collapses the left sidebar drawer and resets the active tab state tracker.
    /// </summary>
    private void CloseDrawer()
    {
        _activeTab = string.Empty;
        SetDrawerVisibility(isOpen: false, showOutline: false, showPalette: false);
    }

    /// <summary>
    /// Centralized visibility helper controlling drawer panel layout elements and splitters.
    /// </summary>
    /// <param name="isOpen">True to expand the drawer container and splitter; false to collapse.</param>
    /// <param name="showOutline">True to render the document outline view inside the drawer.</param>
    /// <param name="showPalette">True to render the module/style palette view inside the drawer.</param>
    private void SetDrawerVisibility(bool isOpen, bool showOutline, bool showPalette)
    {
        var drawer = this.FindControl<Border>("LeftDrawer");
        var splitter = this.FindControl<GridSplitter>("DrawerSplitter");
        var outlineView = this.FindControl<DockPanel>("OutlineView");
        var paletteView = this.FindControl<DockPanel>("PaletteView");

        if (drawer is not null) drawer.IsVisible = isOpen;
        if (splitter is not null) splitter.IsVisible = isOpen;
        if (outlineView is not null) outlineView.IsVisible = showOutline;
        if (paletteView is not null) paletteView.IsVisible = showPalette;
    }

    #endregion

    #region Export & Background Operations

    /// <summary>
    /// Initiates background PDF compilation via <see cref="PdfService"/> using the active document and template,
    /// providing visual feedback and disabling export controls while processing.
    /// </summary>
    /// <param name="sender">The export button triggering the operation.</param>
    /// <param name="e">Routed event arguments.</param>
    private async void ConvertButton_Click(object? sender, RoutedEventArgs e)
    {
        var convertButton = sender as Button;
        var statusText = this.FindControl<TextBlock>("StatusText");

        try
        {
            if (convertButton is not null) convertButton.IsEnabled = false;
            if (statusText is not null) statusText.Text = "Status: Exporting PDF...";

            // Run export asynchronously off the UI thread to keep the application responsive
            await PdfService.CreatePdfAsync(_currentDocument, _template, "output.pdf");

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

    #endregion

    #region Editor & Selection Interactions

    /// <summary>
    /// Appends a new module instance to the document when its archetype card is clicked in the palette.
    /// </summary>
    private void PaletteItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is ModuleDefinition definition)
        {
            // 1. Create a fresh instance using Core's ModuleRegistry definition
            var newModule = definition.Create();

            // 2. Append to domain model (which invokes Changed -> triggers RenderCurrentPreview)
            _currentDocument.AddModule(newModule);

            // 3. Refresh the ModuleEditor ListBox ItemsSource to show the new module
            RefreshModuleEditorList();

            // 4. Select the newly added module
            _currentDocument.SelectModule(newModule);
            SyncPropertiesToolbar(newModule);
        }
    }

    /// <summary>
    /// Re-syncs the module editor ListBox items source when items are appended or modified.
    /// </summary>
    private void RefreshModuleEditorList()
    {
        var moduleEditor = this.FindControl<ModuleEditorView>("ModuleEditor");
        var moduleListBox = moduleEditor?.FindControl<ListBox>("ModuleListBox");
        if (moduleListBox is not null)
        {
            moduleListBox.ItemsSource = null;
            moduleListBox.ItemsSource = _currentDocument.Modules;
            moduleListBox.SelectedItem = _currentDocument.SelectedModule;
        }
    }

    #endregion
}
