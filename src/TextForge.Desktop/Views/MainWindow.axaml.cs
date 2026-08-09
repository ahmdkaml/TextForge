using Avalonia.Controls;
using TextForge.Core;
namespace TextForge.Desktop.Views;
using TextForge.Core.Preview;
using TextForge.Core.Documents;
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
        var textBox = this.FindControl<TextBox>("InputText");

        var document = new DocumentContent(textBox!.Text ?? "");
        var template = new TextTemplate();

        var preview = new PreviewDocument(
            template.Title,
            document.Text);

        var previewText = this.FindControl<TextBlock>("PreviewText");

        previewText!.Text = preview.Text;

        PdfService.CreatePdf(document, template, "output.pdf");
    }
}
