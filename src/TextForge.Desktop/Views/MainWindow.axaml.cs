using Avalonia.Controls;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Preview;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ConvertButton_Click(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 1. Gather UI input
        var textBox = this.FindControl<TextBox>("InputText");
        var document = new DocumentContent(textBox?.Text ?? string.Empty);
        var template = new TextTemplate();

        // 2. Delegate generation to Core domain engine
        var preview = PreviewRenderer.Render(document, template);

        // 3. Update Presentation
        var previewText = this.FindControl<TextBlock>("PreviewText");
        if (previewText is not null)
        {
            previewText.Text = preview.Text;
        }

        // 4. Delegate side-effect output to Core
        PdfService.CreatePdf(document, template, "output.pdf");
    }
}
