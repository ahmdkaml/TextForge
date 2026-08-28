using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Templates;

namespace TextForge.Core.Engine;

public class DocumentEngine : IDocumentEngine
{
    private readonly TemplateRegistry _registry;

    public DocumentEngine(TemplateRegistry? registry = null)
    {
        _registry = registry ?? TemplateRegistry.Default;
    }

    public RenderTree Evaluate(Document document, IDocumentTemplate? template = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        var activeTemplate = template ?? _registry.Get(document.TemplateName);

        var rootNodes = document.Modules
            .Where(module => module.ConnectionState == ModuleConnectionState.Connected)
            .Select(module => EvaluateModule(module, activeTemplate))
            .ToList();

        return new RenderTree(document.Metadata, rootNodes);
    }

    public TOutput Render<TOutput>(Document document, IDocumentTemplate? template, IRenderTarget<TOutput> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var tree = Evaluate(document, template);
        return target.Render(tree);
    }

    private RenderNode EvaluateModule(Module module, IDocumentTemplate template)
    {
        var resolvedFeatures = template.ResolveFeatures(module);
        var resolvedLayout = template.ResolveLayout(module);

        var children = module.SubModules
            .Where(child => child.ConnectionState == ModuleConnectionState.Connected)
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
