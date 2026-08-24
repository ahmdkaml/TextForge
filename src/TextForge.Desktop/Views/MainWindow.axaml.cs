using System.Linq;
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
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InputText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var rawText = textBox?.Text ?? string.Empty;

        // 1. Create Document with a Module from input text
        var document = new Document("Live Preview Document")
            .AddModule(new Module(rawText, ModuleType.Text));

        var template = new DefaultTemplate();

        // 2. Render structured PreviewDocument
        var preview = PreviewRenderer.Render(document, template);

        // 3. Update preview pane
        var previewText = this.FindControl<TextBlock>("PreviewText");
        if (previewText is not null)
        {
            previewText.Text = FlattenPreviewText(preview);
        }
    }

    private void ConvertButton_Click(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("InputText");
        var rawText = textBox?.Text ?? string.Empty;

        var document = new Document("Exported Document")
            .AddModule(new Module(rawText, ModuleType.Text));

        var template = new DefaultTemplate();

        // 1. Render canonical preview model
        var preview = PreviewRenderer.Render(document, template);

        // 2. Generate PDF directly from that model
        PdfService.CreatePdf(preview, "output.pdf");
    }

    private static string FlattenPreviewText(PreviewDocument preview)
    {
        var builder = new StringBuilder();

        void AppendBlock(RenderedBlock block)
        {
            if (!string.IsNullOrEmpty(block.Content))
            {
                builder.AppendLine(block.Content);
            }

            foreach (var child in block.Children)
            {
                AppendBlock(child);
            }
        }

        foreach (var block in preview.Blocks)
        {
            AppendBlock(block);
        }

        return builder.ToString().TrimEnd();
    }
}
