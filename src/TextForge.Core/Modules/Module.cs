using System;
using System.Collections.Generic;

namespace TextForge.Core.Modules;

public class Module
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public ModuleType Type { get; init; } = ModuleType.Text;
    public string Content { get; set; } = string.Empty;
    public ModuleFeatures Features { get; set; } = ModuleFeatures.Default;
    public List<Module> SubModules { get; init; } = [];

    public Module() { }

    public Module(string content, ModuleType type = ModuleType.Text, ModuleFeatures? features = null)
    {
        Content = content;
        Type = type;
        Features = features ?? ModuleFeatures.Default;
    }
}
