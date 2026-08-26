using System;

namespace TextForge.Core.Modules;

/// <summary>
/// Metadata descriptor representing an available module type that can be instantiated into documents.
/// </summary>
public record ModuleDefinition(
    string Key,
    string DisplayName,
    string Description,
    string Icon,
    Func<string?, Module> Factory
)
{
    /// <summary>
    /// Instantiates a new module instance with optional initial content.
    /// </summary>
    public Module Create(string? content = null) => Factory(content);
}
