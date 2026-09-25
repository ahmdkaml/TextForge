using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using TextForge.Core;
using TextForge.Core.Documents;
using TextForge.Core.Modules;

namespace TextForge.Desktop.Views.Components;

public partial class ModuleEditorView : UserControl
{
    private Document? _document;
    public event EventHandler? RootSelected;
    public event EventHandler<Module>? ModuleMoveUpRequested;
    public event EventHandler<Module>? ModuleMoveDownRequested;
    public event EventHandler<Module>? ModuleDeleteRequested;
    public event EventHandler<Module>? ModuleDuplicateRequested;
    public event EventHandler<Module>? ModuleDetachRequested;
    // public event EventHandler<Module>? ModuleReconnectRequested;

    public ModuleEditorView()
    {
        InitializeComponent();
    }

    private void DocumentRootCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        RootSelected?.Invoke(this, EventArgs.Empty);
    }

    public void SetRootSelectionHighlight(bool isRootSelected)
    {
        var rootCard = this.FindControl<Border>("DocumentRootCard");
        if (rootCard is null) return;

        if (isRootSelected)
        {
            if (!rootCard.Classes.Contains("selected"))
            {
                rootCard.Classes.Add("selected");
            }
        }
        else
        {
            rootCard.Classes.Remove("selected");
        }
    }

    private void MoveUpButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleMoveUpRequested?.Invoke(this, module);
        }
    }

    private void MoveDownButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleMoveDownRequested?.Invoke(this, module);
        }
    }

    private void DeleteModuleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleDeleteRequested?.Invoke(this, module);
        }
    }

    private void DuplicateModuleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleDuplicateRequested?.Invoke(this, module);
        }
    }
    private void DetachModuleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Module module })
        {
            ModuleDetachRequested?.Invoke(this, module);
        }
    }

    /// <summary>
    /// Binds the editor to a document instance.
    /// Done once per document load.
    /// </summary>
    public void SetDocument(Document document)
    {
        if (_document == document) return;

        _document = document;

        var titleBlock = this.FindControl<TextBlock>("DocumentTitleText");
        if (titleBlock is not null && !string.IsNullOrWhiteSpace(_document.Metadata.Title))
        {
            titleBlock.Text = $"{_document.Metadata.Title} (Root)";
        }

        // Assign the collection directly ONCE.
        // ObservableCollection will handle all subsequent Add/Remove operations automatically.
        ModuleListBox.ItemsSource = _document.Modules;
    }
}
