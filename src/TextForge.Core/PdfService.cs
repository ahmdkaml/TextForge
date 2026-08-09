using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TextForge.Core.Documents;

namespace TextForge.Core;

public static class PdfService
{
    public static void CreatePdf(DocumentContent document, TextTemplate template, string path)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Content().Column(column =>
                {
                    column.Item().Text(template.Title);
                    column.Item().Text(document.Text);
                });
            });
        })
        .GeneratePdf(path);
    }
}
