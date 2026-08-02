using Avalonia.Controls;
using TextForge.Core;

namespace TextForge.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ConvertButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("InputText");

        PdfService.CreatePdf(textBox!.Text ?? "", "output.pdf");
    }
}