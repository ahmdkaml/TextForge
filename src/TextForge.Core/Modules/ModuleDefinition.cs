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
    Func<Module> Factory
);
