using TextForge.Core.Modules;

namespace TextForge.Core.Presentation;

public interface IModulePaletteProvider
{
    // 1. Get all registered modules for the palette listing
    IReadOnlyList<ModuleDefinition> GetAvailableModules();
}
