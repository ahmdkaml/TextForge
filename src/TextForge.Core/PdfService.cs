using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TextForge.Core.Preview;

namespace TextForge.Core;

public static class PdfService
{
    public static void CreatePdf(PreviewDocument preview, string path)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content().Column(column =>
                {
                    column.Item().Text(preview.Text);
                });
            });
        })
        .GeneratePdf(path);
    }
}
