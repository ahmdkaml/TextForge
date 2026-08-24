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

    private void InputText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        var document = new DocumentContent(textBox?.Text ?? string.Empty);
        var template = new TextTemplate();

        // Direct, immediate render
        var preview = PreviewRenderer.Render(document, template);

        var previewText = this.FindControl<TextBlock>("PreviewText");
        if (previewText is not null)
        {
            previewText.Text = preview.Text;
        }
    }

    private void ConvertButton_Click(
        object? sender,
        Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("InputText");
        var document = new DocumentContent(textBox?.Text ?? string.Empty);
        var template = new TextTemplate();

        PdfService.CreatePdf(document, template, "output.pdf");
    }
}
