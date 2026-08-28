using TextForge.Core.Engine;
using TextForge.Core.Modules;

namespace TextForge.Core.Templates;

/// <summary>
/// Defines the contract for document styling and layout resolution templates.
/// </summary>
public interface IDocumentTemplate
{
    string Name { get; }
    string DisplayName { get; }
    string Description { get; }

    ModuleFeatures ResolveFeatures(Module module);
    LayoutProperties ResolveLayout(Module module);
}
