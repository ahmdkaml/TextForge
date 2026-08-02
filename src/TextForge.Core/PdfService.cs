using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TextForge.Core;

public static class PdfService
{
    public static void CreatePdf(string text, string path)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Content().Text(text);
            });
        })
        .GeneratePdf(path);
    }
}