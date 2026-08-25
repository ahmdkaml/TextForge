using System;
using Avalonia.Controls;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Modules;
using TextForge.Core.Templates;
using TextForge.Desktop.Rendering;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly IDocumentEngine _engine = new DocumentEngine();
    private readonly DefaultTemplate _template = new();
    private readonly AvaloniaPreviewAdapter _previewAdapter = new();
    private Document _currentDocument;

    public MainWindow()
    {
        InitializeComponent();

        _currentDocument = ShowcaseDocumentFactory.Create();
        RenderCurrentPreview();
    }

    private void InputText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var rawText = textBox?.Text;

        if (string.IsNullOrWhiteSpace(rawText))
        {
            _currentDocument = ShowcaseDocumentFactory.Create();
        }
        else
        {
            _currentDocument = new Document("User Document")
                .AddModule(new Module(rawText, ModuleType.Text, styleKey: "Body"));
        }

        RenderCurrentPreview();
    }

    private async void ConvertButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var convertButton = sender as Button;
        var statusText = this.FindControl<TextBlock>("StatusText"); // no status report now kept for later

        try
        {
            if (convertButton is not null) convertButton.IsEnabled = false;
            if (statusText is not null) statusText.Text = "Status: Exporting PDF...";

            // Run export asynchronously off the UI thread
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

    private void RenderCurrentPreview()
    {
        var host = this.FindControl<ContentControl>("PreviewHost");
        if (host is null) return;

        // Render whole visual tree via DocumentEngine and AvaloniaPreviewAdapter
        host.Content = _engine.Render(_currentDocument, _template, _previewAdapter);
    }
    private void CloseOutline_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var toggle = this.FindControl<Avalonia.Controls.Primitives.ToggleButton>("OutlineToggle");
        if (toggle is not null)
        {
            toggle.IsChecked = false;
        }
    }
    private string _activeTab = string.Empty;

    private void NavButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;

        var selectedTab = btn.Name switch
        {
            "NavOutlineBtn" => "Outline",
            "NavPaletteBtn" => "Palette",
            _ => string.Empty
        };

        if (_activeTab == selectedTab)
        {
            // Collapse if clicking the currently open tab
            CloseDrawer();
        }
        else
        {
            OpenDrawer(selectedTab);
        }
    }

    private void OpenDrawer(string tab)
    {
        _activeTab = tab;

        var drawer = this.FindControl<Border>("LeftDrawer");
        var splitter = this.FindControl<GridSplitter>("DrawerSplitter");
        var outlineView = this.FindControl<DockPanel>("OutlineView");
        var paletteView = this.FindControl<DockPanel>("PaletteView");

        if (drawer is not null) drawer.IsVisible = true;
        if (splitter is not null) splitter.IsVisible = true;

        if (outlineView is not null) outlineView.IsVisible = tab == "Outline";
        if (paletteView is not null) paletteView.IsVisible = tab == "Palette";
    }

    private void CloseDrawer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CloseDrawer();
    }

    private void CloseDrawer()
    {
        _activeTab = string.Empty;

        var drawer = this.FindControl<Border>("LeftDrawer");
        var splitter = this.FindControl<GridSplitter>("DrawerSplitter");
        var outlineView = this.FindControl<DockPanel>("OutlineView");
        var paletteView = this.FindControl<DockPanel>("PaletteView");

        if (drawer is not null) drawer.IsVisible = false;
        if (splitter is not null) splitter.IsVisible = false;
        if (outlineView is not null) outlineView.IsVisible = false;
        if (paletteView is not null) paletteView.IsVisible = false;
    }
}
