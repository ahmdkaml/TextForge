using System.Text;
using Avalonia.Controls;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Preview;
using TextForge.Core.Templates;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly DefaultTemplate _template = new();
    private Document _currentDocument;

    public MainWindow()
    {
        InitializeComponent();

        // 1. Load showcase document on startup
        _currentDocument = ShowcaseDocumentFactory.Create();
        RenderCurrentPreview();
    }

    private void InputText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var rawText = textBox?.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(rawText))
        {
            // Reset to showcase if input is completely cleared
            _currentDocument = ShowcaseDocumentFactory.Create();
        }
        else
        {
            // Replace showcase with user text document
            _currentDocument = new Document("User Document")
                .AddModule(new Module(rawText, ModuleType.Text, styleKey: "Body"));
        }

        RenderCurrentPreview();
    }

    private void ConvertButton_Click(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        var preview = PreviewRenderer.Render(_currentDocument, _template);
        PdfService.CreatePdf(preview, "output.pdf");
    }

    private void RenderCurrentPreview()
    {
        var preview = PreviewRenderer.Render(_currentDocument, _template);
        var previewText = this.FindControl<TextBlock>("PreviewText");

        if (previewText is not null)
        {
            previewText.Text = FlattenPreviewText(preview);
        }
    }

    private static string FlattenPreviewText(PreviewDocument preview)
    {
        var builder = new StringBuilder();

        void AppendBlock(RenderedBlock block, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 2);

            if (!string.IsNullOrEmpty(block.Content))
            {
                builder.AppendLine($"{indent}{block.Content}");
            }

            foreach (var child in block.Children)
            {
                AppendBlock(child, indentLevel + 1);
            }
        }

        foreach (var block in preview.Blocks)
        {
            AppendBlock(block, 0);
        }

        return builder.ToString().TrimEnd();
    }
}
