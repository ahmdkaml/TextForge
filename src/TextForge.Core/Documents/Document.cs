using System;
using System.Collections.Generic;
using TextForge.Core.Modules;

namespace TextForge.Core.Documents;

public class Document
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DocumentMetadata Metadata { get; set; } = new();
    public string TemplateName { get; set; } = "Default";
    public List<Module> Modules { get; init; } = [];

    public Document() { }

    public Document(string title, string templateName = "Default")
    {
        Metadata = new DocumentMetadata { Title = title };
        TemplateName = templateName;
    }

    public Document(DocumentMetadata metadata, IEnumerable<Module>? modules = null, string templateName = "Default")
    {
        Metadata = metadata;
        TemplateName = templateName;
        if (modules is not null)
        {
            Modules.AddRange(modules);
        }
    }

    /// <summary>
    /// Helper to append a module while maintaining deterministic sequential ordering.
    /// </summary>
    public Document AddModule(Module module)
    {
        ArgumentNullException.ThrowIfNull(module);
        Modules.Add(module);
        return this;
    }
}
