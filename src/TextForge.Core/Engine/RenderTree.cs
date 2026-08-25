using System;
using System.Collections.Generic;
using TextForge.Core.Documents;
using TextForge.Core.Modules;

namespace TextForge.Core.Engine;

public record RenderNode
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Content { get; init; } = string.Empty;
    public ModuleType Type { get; init; } = ModuleType.Text;
    public ModuleFeatures Features { get; init; } = ModuleFeatures.Default;
    public LayoutProperties Layout { get; init; } = LayoutProperties.Default;
    public IReadOnlyList<RenderNode> Children { get; init; } = [];

    // Open property bag for target-specific extensions or future features without breaking schema
    public IReadOnlyDictionary<string, object> Attributes { get; init; } = new Dictionary<string, object>();
}

public record RenderTree(
    DocumentMetadata Metadata,
    IReadOnlyList<RenderNode> RootNodes);
