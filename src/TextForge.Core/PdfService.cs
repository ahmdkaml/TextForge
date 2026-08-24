using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TextForge.Core.Modules;
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
                    // Render all top-level blocks and their children
                    foreach (var block in preview.Blocks)
                    {
                        RenderBlock(column, block);
                    }
                });
            });
        })
        .GeneratePdf(path);
    }

    private static void RenderBlock(ColumnDescriptor column, RenderedBlock block)
    {
        if (!string.IsNullOrWhiteSpace(block.Content))
        {
            column.Item().Text(text =>
            {
                var span = text.Span(block.Content);

                if (block.Features.FontWeight == ModuleFontWeight.Bold)
                {
                    span.Bold();
                }

                if (block.Features.Italic)
                {
                    span.Italic();
                }

                if (block.Features.Underline)
                {
                    span.Underline();
                }

                if (block.Features.Strikethrough)
                {
                    span.Strikethrough();
                }

                if (!string.IsNullOrWhiteSpace(block.Features.Color))
                {
                    span.FontColor(block.Features.Color);
                }
            });
        }

        // Recursively render child sub-blocks
        foreach (var child in block.Children)
        {
            RenderBlock(column, child);
        }
    }
}
