using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using TextForge.Core.Documents;
using TextForge.Core.Modules;
using TextForge.Desktop.ViewModels;

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

        var editorPanel = this.FindControl<StackPanel>("EditorStackPanel");
        editorPanel?.AddHandler(
            Control.RequestBringIntoViewEvent,
            (sender, e) =>
            {
                // Suppress unintended automatic scroll jumps caused by module clicks,
                // focus changes, or expander open/close transitions.
                e.Handled = true;
            },
            RoutingStrategies.Bubble,
            handledEventsToo: true);
    }

    private void DocumentRootCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // If the click happened on a nested module card or button, DON'T select root!
        if (e.Source is Visual visual && visual.FindAncestorOfType<Border>()?.Classes.Contains("module-card") == true)
        {
            return; // Let the module card handle its own selection
        }

        var vm = DataContext as MainViewModel ?? this.FindAncestorOfType<Window>()?.DataContext as MainViewModel;
        vm?.CurrentDocument.SelectRoot();
    }
    private void ModuleCard_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is ModuleViewModel clickedModule)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.CurrentDocument.SelectModule(clickedModule.Model);
            }
            else if (this.FindAncestorOfType<Window>()?.DataContext is MainViewModel windowVm)
            {
                windowVm.CurrentDocument.SelectModule(clickedModule.Model);
            }

            e.Handled = true;
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

    }
}
