using TextForge.Core.Documents;

namespace TextForge.Core.Preview;

public static class PreviewRenderer
{
    public static PreviewDocument Render(
        DocumentContent document,
        TextTemplate template)
    {
        // Pure domain transform (template formatting applied directly to text)
        return new PreviewDocument(document.Text);
    }
}
