using TextForge.Core.Modules;

namespace TextForge.Core.Documents;

public static class ShowcaseDocumentFactory
{
    public static Document Create()
    {
        var document = new Document("TextForge Showcase", "Default");

        // 1. Title Module ("default-title")
        document.AddModule(Module.CreateTitle("Welcome to TextForge"));

        // 2. Lead Paragraph ("default-paragraph")
        document.AddModule(Module.CreateParagraph(
            "TextForge is a modular, template-driven document engine designed for consistent live previews and PDF export."
        ));

        // 3. Section with Nested SubModules ("default-heading" & "default-bullet")
        var featuresSection = Module.CreateHeading("Core Capabilities");
        featuresSection.SubModules.Add(Module.CreateBulletItem("Hierarchical module composition and submodules"));
        featuresSection.SubModules.Add(Module.CreateBulletItem("Cascading template feature resolution"));
        featuresSection.SubModules.Add(Module.CreateBulletItem("UI-agnostic rendering pipeline"));
        document.AddModule(featuresSection);

        // 4. Highlighted Callout Box ("default-callout")
        document.AddModule(Module.CreateCallout(
            "Tip: Templates provide baseline defaults, while modules can override specific visual features."
        ));

        // 5. Explicit Formatting Override Alert ("default-alert")
        document.AddModule(Module.CreateAlert(
            "Custom styled alert: Bold red text with custom line spacing."
        ));

        return document;
    }
}
