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

    private void ConvertButton_Click(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        PdfService.CreatePdf(_currentDocument, _template, "output.pdf");
    }

    private void RenderCurrentPreview()
    {
        var host = this.FindControl<ContentControl>("PreviewHost");
        if (host is null) return;

        // Render whole visual tree via DocumentEngine and AvaloniaPreviewAdapter
        host.Content = _engine.Render(_currentDocument, _template, _previewAdapter);
    }
}
