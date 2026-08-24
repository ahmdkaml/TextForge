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
}
