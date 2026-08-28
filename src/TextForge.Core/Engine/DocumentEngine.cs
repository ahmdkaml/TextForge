using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Templates;

namespace TextForge.Core.Engine;

public class DocumentEngine : IDocumentEngine
{
    private readonly DefaultTemplate _defaultTemplate = new();

    public RenderTree Evaluate(Document document, DefaultTemplate template)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(template);

        var rootNodes = document.Modules
            .Select(module => EvaluateModule(module, template))
            .ToList();

        return new RenderTree(document.Metadata, rootNodes);
    }

    public TOutput Render<TOutput>(Document document, DefaultTemplate template, IRenderTarget<TOutput> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var tree = Evaluate(document, template);
        return target.Render(tree);
    }

    private RenderNode EvaluateModule(Module module, DefaultTemplate template)
    {
        var resolvedFeatures = template.ResolveFeatures(module);
        var resolvedLayout = template.ResolveLayout(module);

        var children = module.SubModules
            .Select(child => EvaluateModule(child, template))
            .ToList();

        return new RenderNode
        {
            Id = module.Id,
            Content = module.Content,
            Type = module.Type,
            Features = resolvedFeatures,
            Layout = resolvedLayout,
            Children = children
        };
    }
}
