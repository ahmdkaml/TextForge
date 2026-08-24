using System;
using System.Collections.Generic;

namespace TextForge.Core.Modules;

public class Module
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? StyleKey { get; init; }
    public ModuleType Type { get; init; } = ModuleType.Text;
    public string Content { get; set; } = string.Empty;
    public ModuleFeatures Features { get; set; } = ModuleFeatures.Default;
    public List<Module> SubModules { get; init; } = [];

    public Module() { }

    public Module(
        string content,
        ModuleType type = ModuleType.Text,
        string? styleKey = null,
        ModuleFeatures? features = null)
    {
        Content = content;
        Type = type;
        StyleKey = styleKey;
        Features = features ?? ModuleFeatures.Default;
    }
}
