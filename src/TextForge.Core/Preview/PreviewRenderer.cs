using TextForge.Core.Documents;

namespace TextForge.Core.Preview;

public static class PreviewRenderer
{
    public static PreviewDocument Render(
        DocumentContent document,
        TextTemplate template)
    {
        return new PreviewDocument(
            template.Title,
            document.Text);
    }
}
