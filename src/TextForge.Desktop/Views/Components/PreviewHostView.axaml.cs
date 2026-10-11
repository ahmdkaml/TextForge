using System;
using System.Linq;
using Avalonia.Controls;
using TextForge.Core.Documents;
using TextForge.Core.Templates;

namespace TextForge.Desktop.Views.Components;

public partial class PreviewHostView : UserControl
{
    private Document? _currentDocument;
    private bool _isSyncingSelection;

    public event EventHandler<string>? TemplateChanged;

    public PreviewHostView()
    {
        InitializeComponent();
        InitializeTemplateSelector();
    }

    private void InitializeTemplateSelector()
    {
        // 1. Populate items directly from Core TemplateRegistry
        var templates = TemplateRegistry.Default.GetAll().ToList();
        TemplateSelector.ItemsSource = templates;

        // 2. Default to the primary template
        TemplateSelector.SelectedItem = templates.FirstOrDefault();

        // 3. Hook event
        TemplateSelector.SelectionChanged += TemplateSelector_SelectionChanged;
    }

    public void BindDocument(Document document)
    {
        _currentDocument = document;

        _isSyncingSelection = true;
        try
        {
            var matchingTemplate = TemplateRegistry.Default
                .GetAll()
                .FirstOrDefault(t => string.Equals(t.Name, document.TemplateName, StringComparison.OrdinalIgnoreCase))
                ?? TemplateRegistry.Default.Get("Default");

            TemplateSelector.SelectedItem = matchingTemplate;
        }
        finally
        {
            _isSyncingSelection = false;
        }
    }

    private void TemplateSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSyncingSelection) return;

        if (TemplateSelector.SelectedItem is IDocumentTemplate selectedTemplate)
        {
            if (_currentDocument is not null && _currentDocument.TemplateName != selectedTemplate.Name)
            {
                _currentDocument.TemplateName = selectedTemplate.Name;
                _currentDocument.NotifyChanged();
            }

            TemplateChanged?.Invoke(this, selectedTemplate.Name);
        }
    }
}
