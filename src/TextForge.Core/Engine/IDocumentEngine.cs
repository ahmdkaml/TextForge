using TextForge.Core.Documents;
using TextForge.Core.Templates;

namespace TextForge.Core.Engine;

/// <summary>
/// Target-specific adapter that translates a canonical RenderTree into a concrete output format.
/// </summary>
public interface IRenderTarget<out TOutput>
{
    TOutput Render(RenderTree tree);
}

/// <summary>
/// Core evaluation engine responsible for resolving document hierarchy, template cascading, and layout calculation.
/// </summary>
public interface IDocumentEngine
{
    RenderTree Evaluate(Document document, IDocumentTemplate? template = null);

    TOutput Render<TOutput>(Document document, IDocumentTemplate? template, IRenderTarget<TOutput> target);
}
