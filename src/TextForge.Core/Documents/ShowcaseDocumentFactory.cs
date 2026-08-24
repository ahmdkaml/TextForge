using TextForge.Core.Modules;

namespace TextForge.Core.Documents;

public static class ShowcaseDocumentFactory
{
    public static Document Create()
    {
        var document = new Document("TextForge Showcase", "Default");

        // 1. Title Module
        document.AddModule(new Module("Welcome to TextForge", ModuleType.Section, styleKey: "Title"));

        // 2. Lead Paragraph
        document.AddModule(new Module(
            "TextForge is a modular, template-driven document engine designed for consistent live previews and PDF export.",
            ModuleType.Text,
            styleKey: "Body"
        ));

        // 3. Section with Nested SubModules
        var featuresSection = new Module("Core Capabilities", ModuleType.Section, styleKey: "Heading");

        featuresSection.SubModules.Add(new Module("• Hierarchical module composition and submodules", ModuleType.Text, styleKey: "Body"));
        featuresSection.SubModules.Add(new Module("• Cascading template feature resolution", ModuleType.Text, styleKey: "Body"));
        featuresSection.SubModules.Add(new Module("• UI-agnostic rendering pipeline", ModuleType.Text, styleKey: "Body"));

        document.AddModule(featuresSection);

        // 4. Highlighted Callout Box
        document.AddModule(new Module(
            "Tip: Templates provide baseline defaults, while modules can override specific visual features.",
            ModuleType.Text,
            styleKey: "Callout"
        ));

        // 5. Explicit Formatting Overrides
        var styledModule = new Module(
            "Custom styled alert: Bold red text with custom line spacing.",
            ModuleType.Text,
            features: new ModuleFeatures
            {
                Color = "#DC2626",
                FontWeight = ModuleFontWeight.Bold,
                LineSpacing = 1.3
            }
        );
        document.AddModule(styledModule);

        return document;
    }
}
