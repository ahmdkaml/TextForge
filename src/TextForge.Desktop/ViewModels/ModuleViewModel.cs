using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TextForge.Core.Documents;
using TextForge.Core.Modules;

namespace TextForge.Desktop.ViewModels;

public partial class ModuleViewModel : ObservableObject
{
    public Module Model { get; }
    public ObservableCollection<ModuleViewModel> Children { get; } = [];

    private readonly Document _document;

    public ModuleViewModel(Module model, Document document)
    {
        Model = model;
        _document = document;

        Model.PropertyChanged += OnModelPropertyChanged;
        Model.SubModules.CollectionChanged += OnSubModulesChanged;

        foreach (var child in Model.SubModules)
        {
            Children.Add(new ModuleViewModel(child, document));
        }

    }

    public string Name => Model.Name;
    public string? Content
    {
        get => Model.Content;
        set => Model.Content = value ?? string.Empty;
    }

    public string TruncatedPreviewContent => Model.TruncatedPreviewContent;
    public bool IsSelected => Model.IsSelected;

    public bool IsExpanded
    {
        get => Model.IsExpanded;
        set => Model.IsExpanded = value;
    }

    [RelayCommand]
    private void Delete()
    {
        _document.RemoveModule(Model);
    }

    [RelayCommand]
    private void Detach()
    {
        _document.DetachModule(Model);
    }

    [RelayCommand]
    private void Clone()
    {
        _document.DuplicateModule(Model);
    }
    [RelayCommand]
    private void Select()
    {
        // Operates directly on its own encapsulated Model instance
        _document.SelectModule(Model);
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Module.Content)
            or nameof(Module.TruncatedPreviewContent)
            or nameof(Module.IsSelected)
            or nameof(Module.IsExpanded))
        {
            OnPropertyChanged(e.PropertyName);
        }
    }

    private void OnSubModulesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
        {
            var index = e.NewStartingIndex;
            foreach (Module child in e.NewItems)
            {
                var childViewModel = new ModuleViewModel(child, _document);
                if (index >= 0)
                {
                    Children.Insert(index++, childViewModel);
                }
                else
                {
                    Children.Add(childViewModel);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
        {
            foreach (Module child in e.OldItems)
            {
                var childViewModel = Children.FirstOrDefault(item => item.Model == child);
                if (childViewModel is not null)
                {
                    Children.Remove(childViewModel);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Move)
        {
            Children.Move(e.OldStartingIndex, e.NewStartingIndex);
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            Children.Clear();
        }
    }
}