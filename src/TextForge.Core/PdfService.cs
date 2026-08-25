using System.IO;
using TextForge.Core.Documents;
using TextForge.Core.Engine;
using TextForge.Core.Export;
using TextForge.Core.Templates;

namespace TextForge.Core;

public static class PdfService
{
    private static readonly IDocumentEngine Engine = new DocumentEngine();
    private static readonly QuestPdfAdapter Adapter = new();

    public static void CreatePdf(Document document, DefaultTemplate template, string outputPath)
    {
        var bytes = Engine.Render(document, template, Adapter);
        File.WriteAllBytes(outputPath, bytes);
    }

    public static void CreatePdf(RenderTree tree, string outputPath)
    {
        var bytes = Adapter.Render(tree);
        File.WriteAllBytes(outputPath, bytes);
    }
}
