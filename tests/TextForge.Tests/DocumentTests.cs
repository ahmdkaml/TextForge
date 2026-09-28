using System;
using System.Linq;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using Xunit;

namespace TextForge.Core.Tests.Documents;

public class DocumentTests
{
    [Fact]
    public void Document_InstantiatedWithDefaults_HasValidIdentityAndEmptyModules()
    {
        // Act
        var document = new Document();

        // Assert
        Assert.NotEqual(Guid.Empty, document.Id);
        Assert.NotNull(document.Metadata);
        Assert.Equal("Untitled Document", document.Metadata.Title);
        Assert.Equal("Default", document.TemplateName);
        Assert.Empty(document.Modules);
        Assert.Equal(1, document.Metadata.SchemaVersion);
    }

    [Fact]
    public void Document_AddModule_PreservesDeterministicOrdering()
    {
        // Arrange
        var document = new Document("My Spec");
        var header = new Module("Header 1", ModuleType.Section, styleKey: "Heading");
        var paragraph = new Module("Body text 1", ModuleType.Text, styleKey: "Body");
        var callout = new Module("Important note", ModuleType.Text, styleKey: "Callout");

        // Act
        document.AddModule(header)
                .AddModule(paragraph)
                .AddModule(callout);

        // Assert
        Assert.Equal(3, document.Modules.Count);
        Assert.Same(header, document.Modules[0]);
        Assert.Same(paragraph, document.Modules[1]);
        Assert.Same(callout, document.Modules[2]);
        Assert.Equal("Header 1", document.Modules.First().Content);
        Assert.Equal("Important note", document.Modules.Last().Content);
    }

    [Fact]
    public void Document_WithCustomMetadata_PreservesTimestampsAndDetails()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var metadata = new DocumentMetadata
        {
            Title = "Architecture RFC",
            Author = "Mohamed Azzam",
            CreatedAt = now,
            ModifiedAt = now,
            SchemaVersion = 2
        };

        // Act
        var document = new Document(metadata, templateName: "TechnicalReport");

        // Assert
        Assert.Equal("Architecture RFC", document.Metadata.Title);
        Assert.Equal("Mohamed Azzam", document.Metadata.Author);
        Assert.Equal("TechnicalReport", document.TemplateName);
        Assert.Equal(2, document.Metadata.SchemaVersion);
        Assert.Equal(now, document.Metadata.CreatedAt);
    }

    [Fact]
    public void Document_SelectRoot_ClearsModuleSelectionAndSetsIsRootSelected()
    {
        var document = new Document("Test Doc");
        var module = new Module("Paragraph", ModuleType.Text);
        document.AddModule(module);

        document.SelectModule(module);
        Assert.Same(module, document.SelectedModule);
        Assert.True(module.IsSelected);
        Assert.False(document.IsRootSelected);

        document.SelectRoot();
        Assert.Null(document.SelectedModule);
        Assert.False(module.IsSelected);
        Assert.True(document.IsRootSelected);
    }

    [Fact]
    public void Document_SelectModule_SetsModuleAndClearsIsRootSelected()
    {
        var document = new Document("Test Doc");
        var module = new Module("Header", ModuleType.Section);
        document.AddModule(module);

        Assert.True(document.IsRootSelected == false || document.SelectedModule == module);

        document.SelectRoot();
        Assert.True(document.IsRootSelected);
        Assert.Null(document.SelectedModule);

        document.SelectModule(module);
        Assert.False(document.IsRootSelected);
        Assert.Same(module, document.SelectedModule);
        Assert.True(module.IsSelected);
    }

    [Fact]
    public void Document_AddModule_WhenParentIsNull_AppendsToRootCollection()
    {
        var document = new Document("Test Doc");
        var parentSection = new Module("Section", ModuleType.Section);
        document.AddModule(parentSection);

        // Explicitly select root
        document.SelectRoot();

        // Adding with parent = null (root active) appends to root collection
        var rootModule = new Module("Root Level Block", ModuleType.Text);
        document.AddModule(rootModule, parent: null);

        Assert.Equal(2, document.Modules.Count);
        Assert.Contains(rootModule, document.Modules);
        Assert.Empty(parentSection.SubModules);
    }

    [Fact]
    public void Document_RemoveModule_WhenSelected_FallsBackToRootSelection()
    {
        var document = new Document("Test Doc");
        var module = new Module("Removable", ModuleType.Text);
        document.AddModule(module);

        document.SelectModule(module);
        Assert.Same(module, document.SelectedModule);
        Assert.False(document.IsRootSelected);

        var removed = document.RemoveModule(module);

        Assert.True(removed);
        Assert.Null(document.SelectedModule);
        Assert.True(document.IsRootSelected);
    }

    [Fact]
    public void Document_ClearModules_EmptiesModulesCollectionAndResetsSelectionToRoot()
    {
        var document = new Document("Test Doc");
        var module1 = new Module("M1", ModuleType.Text);
        var module2 = new Module("M2", ModuleType.Code);
        document.AddModule(module1);
        document.AddModule(module2);

        document.SelectModule(module2);
        Assert.Equal(2, document.Modules.Count);
        Assert.Same(module2, document.SelectedModule);
        Assert.False(document.IsRootSelected);

        document.ClearModules();

        Assert.Empty(document.Modules);
        Assert.Null(document.SelectedModule);
        Assert.True(document.IsRootSelected);
    }

    [Fact]
    public void Document_ClearModules_DispatchesChangedEvent()
    {
        var document = new Document("Test Doc");
        document.AddModule(new Module("M1", ModuleType.Text));

        var changedFired = false;
        document.Changed += () => changedFired = true;

        document.ClearModules();

        Assert.True(changedFired);
    }
}
