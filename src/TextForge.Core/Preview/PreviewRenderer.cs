using System;
using System.Collections.Generic;
using System.Linq;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Core.Templates;

namespace TextForge.Core.Preview;

public static class PreviewRenderer
{
    public static PreviewDocument Render(Document document, DefaultTemplate template)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(template);

        var renderedBlocks = document.Modules
            .Select(m => RenderModule(m, template))
            .ToList();

        return new PreviewDocument(document.Metadata, renderedBlocks);
    }

    private static RenderedBlock RenderModule(Module module, DefaultTemplate template)
    {
        var resolvedFeatures = template.ResolveFeatures(module);
        var children = module.SubModules
            .Select(child => RenderModule(child, template))
            .ToList();

        return new RenderedBlock(
            module.Content,
            module.Type,
            resolvedFeatures,
            children);
    }
}
