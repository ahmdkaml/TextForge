using TextForge.Core.Modules;

namespace TextForge.Core.Documents;

/// <summary>
/// Factory generating a default showcase document composed entirely through the central <see cref="ModuleRegistry"/>.
/// </summary>
public static class ShowcaseDocumentFactory
{
    public static Document Create()
    {
        var document = new Document("TextForge Showcase", "Default");

        // 1. Header Archetype
        document.AddModule(ModuleRegistry.CreateModule(
            "title",
            "Welcome to TextForge"
        ));

        // 2. Standard Body
        document.AddModule(ModuleRegistry.CreateModule(
            "paragraph",
            "TextForge is a modular, template-driven document engine designed for consistent live previews and PDF export."
        ));

        // 3. Hierarchical Composition (Heading + Nested SubModules)
        var featuresSection = ModuleRegistry.CreateModule(
            "heading",
            "Core Capabilities"
        );
        featuresSection.SubModules.Add(ModuleRegistry.CreateModule(
            "bullet",
            "Hierarchical module composition and submodules"
        ));
        featuresSection.SubModules.Add(ModuleRegistry.CreateModule(
            "bullet",
            "Cascading template feature resolution"
        ));
        featuresSection.SubModules.Add(ModuleRegistry.CreateModule(
            "bullet",
            "UI-agnostic rendering pipeline"
        ));
        document.AddModule(featuresSection);

        // 4. Highlighted Callout Box
        document.AddModule(ModuleRegistry.CreateModule(
            "callout",
            "Tip: Templates provide baseline defaults, while modules can override specific visual features."
        ));

        // 5. Explicit Formatting Overrides / Alert
        document.AddModule(ModuleRegistry.CreateModule(
            "alert",
            "Custom styled alert: Bold red text with custom line spacing."
        ));

        return document;
    }
}
