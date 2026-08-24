using System.Collections.Generic;
using TextForge.Core.Documents;
using TextForge.Core.Modules;

namespace TextForge.Core.Preview;

public record RenderedBlock(
    string Content,
    ModuleType Type,
    ModuleFeatures Features,
    IReadOnlyList<RenderedBlock> Children);

public record PreviewDocument(
    DocumentMetadata Metadata,
    IReadOnlyList<RenderedBlock> Blocks);
