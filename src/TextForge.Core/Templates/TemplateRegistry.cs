using System;
using System.Collections.Generic;
using System.Linq;

namespace TextForge.Core.Templates;

/// <summary>
/// Central registry providing access to all defined document templates.
/// </summary>
public class TemplateRegistry
{
    private readonly Dictionary<string, IDocumentTemplate> _templates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Global shared registry instance pre-loaded with default templates.
    /// </summary>
    public static TemplateRegistry Default { get; } = CreateDefault();

    /// <summary>
    /// Registers a template into the registry.
    /// </summary>
    public void Register(IDocumentTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);
        _templates[template.Name] = template;
    }

    /// <summary>
    /// Retrieves a template by its unique name, falling back to "Default" if not found.
    /// </summary>
    public IDocumentTemplate Get(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && _templates.TryGetValue(name, out var template))
        {
            return template;
        }

        if (_templates.TryGetValue("Default", out var defaultTemplate))
        {
            return defaultTemplate;
        }

        throw new InvalidOperationException($"Template '{name}' was not found and no Default template is registered.");
    }

    /// <summary>
    /// Retrieves all registered templates.
    /// </summary>
    public IEnumerable<IDocumentTemplate> GetAll() => _templates.Values;

    /// <summary>
    /// Retrieves all registered template identifiers.
    /// </summary>
    public IEnumerable<string> GetAvailableNames() => _templates.Values.Select(t => t.Name);

    private static TemplateRegistry CreateDefault()
    {
        var registry = new TemplateRegistry();
        registry.Register(new DefaultTemplate());
        return registry;
    }
}
