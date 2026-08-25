using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TextForge.Core.Engine;
using TextForge.Core.Modules;

namespace TextForge.Core.Export;

public class QuestPdfAdapter : IRenderTarget<byte[]>
{
    public byte[] Render(RenderTree tree)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        using var stream = new MemoryStream();

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Content().Column(column =>
                {
                    column.Spacing(6);

                    foreach (var node in tree.RootNodes)
                    {
                        RenderNode(column, node, 0);
                    }
                });
            });
        })
        .GeneratePdf(stream);

        return stream.ToArray();
    }

    private static void RenderNode(ColumnDescriptor column, RenderNode node, int indentLevel)
    {
        if (!string.IsNullOrWhiteSpace(node.Content))
        {
            var item = column.Item()
                .PaddingLeft((float)(node.Layout.MarginLeft + (indentLevel * 14)))
                .PaddingRight((float)node.Layout.MarginRight)
                .PaddingTop((float)node.Layout.MarginTop)
                .PaddingBottom((float)node.Layout.MarginBottom);

            // Highlighted Callout container
            if (!string.IsNullOrWhiteSpace(node.Features.HighlightMarker))
            {
                item.BorderLeft(4)
                    .BorderColor(Colors.Amber.Medium)
                    .Background(node.Features.HighlightMarker)
                    .PaddingLeft((float)(node.Layout.PaddingLeft > 0 ? node.Layout.PaddingLeft : 10))
                    .PaddingRight((float)(node.Layout.PaddingRight > 0 ? node.Layout.PaddingRight : 10))
                    .PaddingTop((float)(node.Layout.PaddingTop > 0 ? node.Layout.PaddingTop : 6))
                    .PaddingBottom((float)(node.Layout.PaddingBottom > 0 ? node.Layout.PaddingBottom : 6))
                    .Text(text => ApplyTextStyles(text, node, indentLevel));
            }
            else
            {
                item.Text(text => ApplyTextStyles(text, node, indentLevel));
            }
        }

        foreach (var child in node.Children)
        {
            RenderNode(column, child, indentLevel + 1);
        }
    }

    private static void ApplyTextStyles(TextDescriptor text, RenderNode node, int indentLevel)
    {
        var fontSize = node.Type == ModuleType.Section && indentLevel == 0 ? 18 : (node.Features.FontWeight == ModuleFontWeight.Bold ? 14 : 11);
        var span = text.Span(node.Content).FontSize(fontSize);

        if (node.Features.FontWeight == ModuleFontWeight.Bold)
        {
            span.Bold();
        }

        if (node.Features.Italic)
        {
            span.Italic();
        }

        if (node.Features.Underline)
        {
            span.Underline();
        }

        if (node.Features.Strikethrough)
        {
            span.Strikethrough();
        }

        if (!string.IsNullOrWhiteSpace(node.Features.Color))
        {
            span.FontColor(node.Features.Color);
        }
        else
        {
            span.FontColor("#1F2937");
        }
    }
}
