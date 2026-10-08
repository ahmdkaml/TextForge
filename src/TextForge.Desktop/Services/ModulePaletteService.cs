using System.Collections.Generic;
using TextForge.Core.Modules;
using TextForge.Core.Presentation;

namespace TextForge.Desktop.Services;

public class ModulePaletteService : IModulePaletteProvider
{
    public IReadOnlyList<ModuleDefinition> GetAvailableModules()
    {
        return ModuleRegistry.GetAvailableModules();
    }
}
