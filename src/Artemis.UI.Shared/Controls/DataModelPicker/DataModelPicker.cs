﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.UI.Shared.DataModelVisualization.Shared;
using Artemis.UI.Shared.Events;
using Artemis.UI.Shared.Extensions;
using Artemis.UI.Shared.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Material.Icons.Avalonia;
using ReactiveUI;

namespace Artemis.UI.Shared.DataModelPicker;

/// <summary>
///     Represents a data model picker picker that can be used to select a data model path.
/// </summary>
public class DataModelPicker : TemplatedControl
{
    /// <summary>
    ///     The data model UI service this picker should use.
    /// </summary>
    public static IDataModelUIService? DataModelUIService;

    /// <summary>
    ///     Gets or sets data model path.
    /// </summary>
    public static readonly StyledProperty<DataModelPath?> DataModelPathProperty =
        AvaloniaProperty.Register<DataModelPicker, DataModelPath?>(nameof(DataModelPath), defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    ///     Gets or sets a boolean indicating whether the data model picker should show current values when selecting a path.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDataModelValuesProperty =
        AvaloniaProperty.Register<DataModelPicker, bool>(nameof(ShowDataModelValues));

    /// <summary>
    ///     A list of extra modules to show data models of.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<Module>?> ModulesProperty =
        AvaloniaProperty.Register<DataModelPicker, ObservableCollection<Module>?>(nameof(Modules), new ObservableCollection<Module>());

    /// <summary>
    ///     The data model view model to show, if not provided one will be retrieved by the control.
    /// </summary>
    public static readonly StyledProperty<DataModelPropertiesViewModel?> DataModelViewModelProperty =
        AvaloniaProperty.Register<DataModelPicker, DataModelPropertiesViewModel?>(nameof(DataModelViewModel));

    /// <summary>
    ///     A list of types to filter the selectable paths on.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<Type>?> FilterTypesProperty =
        AvaloniaProperty.Register<DataModelPicker, ObservableCollection<Type>?>(nameof(FilterTypes), new ObservableCollection<Type>());

    /// <summary>
    ///     Gets or sets a boolean indicating whether the picker is in event picker mode.
    ///     When <see langword="true" /> event children aren't selectable and non-events are described as "{PropertyName}
    ///     changed".
    /// </summary>
    public static readonly StyledProperty<bool> IsEventPickerProperty =
        AvaloniaProperty.Register<DataModelPicker, bool>(nameof(IsEventPicker));

    private readonly ObservableCollection<DataModelVisualizationViewModel> _searchResults = [];
    private TextBlock? _currentPathDescription;
    private TextBlock? _currentPathDisplay;

    private MaterialIcon? _currentPathIcon;
    private TreeView? _dataModelTreeView;
    private ListBox? _searchListBox;
    private TextBox? _searchBox;
    private Panel? _searchContainer;
    private StackPanel? _searchEmpty;
    private DispatcherTimer? _updateTimer;
    private string? _lastSearch;
    private bool _firstUpdate;

    static DataModelPicker()
    {
        ModulesProperty.Changed.Subscribe(ModulesChanged);
        DataModelPathProperty.Changed.Subscribe(DataModelPathPropertyChanged);
        DataModelViewModelProperty.Changed.Subscribe(DataModelViewModelPropertyChanged);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="DataModelPicker" /> class.
    /// </summary>
    public DataModelPicker()
    {
        SelectPropertyCommand = ReactiveCommand.Create<DataModelVisualizationViewModel>(selected => ExecuteSelectPropertyCommand(selected));
    }

    /// <summary>
    ///     Gets a command that selects the path by it's view model.
    /// </summary>
    public ReactiveCommand<DataModelVisualizationViewModel, Unit> SelectPropertyCommand { get; }

    /// <summary>
    ///     Gets or sets data model path.
    /// </summary>
    public DataModelPath? DataModelPath
    {
        get => GetValue(DataModelPathProperty);
        set => SetValue(DataModelPathProperty, value);
    }

    /// <summary>
    ///     Gets or sets a boolean indicating whether the data model picker should show current values when selecting a path.
    /// </summary>
    public bool ShowDataModelValues
    {
        get => GetValue(ShowDataModelValuesProperty);
        set => SetValue(ShowDataModelValuesProperty, value);
    }

    /// <summary>
    ///     A list of extra modules to show data models of.
    /// </summary>
    public ObservableCollection<Module>? Modules
    {
        get => GetValue(ModulesProperty);
        set => SetValue(ModulesProperty, value);
    }

    /// <summary>
    ///     The data model view model to show, if not provided one will be retrieved by the control.
    /// </summary>
    public DataModelPropertiesViewModel? DataModelViewModel
    {
        get => GetValue(DataModelViewModelProperty);
        private set => SetValue(DataModelViewModelProperty, value);
    }

    /// <summary>
    ///     A list of types to filter the selectable paths on.
    /// </summary>
    public ObservableCollection<Type>? FilterTypes
    {
        get => GetValue(FilterTypesProperty);
        set => SetValue(FilterTypesProperty, value);
    }

    /// <summary>
    ///     Gets or sets a boolean indicating whether the picker is in event picker mode.
    ///     When <see langword="true" /> event children aren't selectable and non-events are described as "{PropertyName}
    ///     changed".
    /// </summary>
    public bool IsEventPicker
    {
        get => GetValue(IsEventPickerProperty);
        set => SetValue(IsEventPickerProperty, value);
    }

    /// <summary>
    ///     Occurs when a new path has been selected
    /// </summary>
    public event EventHandler<DataModelSelectedEventArgs>? DataModelPathSelected;

    /// <summary>
    ///     Invokes the <see cref="DataModelPathSelected" /> event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnDataModelPathSelected(DataModelSelectedEventArgs e)
    {
        DataModelPathSelected?.Invoke(this, e);
    }

    private static void ModulesChanged(AvaloniaPropertyChangedEventArgs<ObservableCollection<Module>?> e)
    {
        if (e.Sender is DataModelPicker dataModelPicker)
            dataModelPicker.GetDataModel();
    }

    private static void DataModelPathPropertyChanged(AvaloniaPropertyChangedEventArgs<DataModelPath?> e)
    {
        if (e.Sender is DataModelPicker dataModelPicker)
            dataModelPicker.UpdateCurrentPath(false);
    }

    private static void DataModelViewModelPropertyChanged(AvaloniaPropertyChangedEventArgs<DataModelPropertiesViewModel?> e)
    {
        if (e.Sender is DataModelPicker && e.OldValue.Value != null)
            e.OldValue.Value.Dispose();
    }

    private void ExecuteSelectPropertyCommand(DataModelVisualizationViewModel selected)
    {
        if (selected.DataModelPath == null)
            return;
        if (selected.DataModelPath.Equals(DataModelPath))
            return;

        DataModelPath = new DataModelPath(selected.DataModelPath);
        OnDataModelPathSelected(new DataModelSelectedEventArgs(DataModelPath));
    }

    private void Update(object? sender, EventArgs e)
    {
        if (DataModelUIService == null)
            return;

        DataModelViewModel?.Update(DataModelUIService, new DataModelUpdateConfiguration(!IsEventPicker, !string.IsNullOrEmpty(_searchBox?.Text)));
        ApplySearch();

        if (_firstUpdate)
        {
            _firstUpdate = false;
            UpdateCurrentPath(true);
        }
    }

    private void GetDataModel()
    {
        if (DataModelUIService == null)
            return;

        ChangeDataModel(DataModelUIService.GetPluginDataModelVisualization(Modules?.ToList() ?? new List<Module>(), true));
    }

    private void ChangeDataModel(DataModelPropertiesViewModel? dataModel)
    {
        if (DataModelViewModel != null)
        {
            DataModelViewModel.Dispose();
            DataModelViewModel.UpdateRequested -= DataModelOnUpdateRequested;
        }

        DataModelViewModel = dataModel;
        if (DataModelViewModel != null)
            DataModelViewModel.UpdateRequested += DataModelOnUpdateRequested;
    }

    private void DataModelOnUpdateRequested(object? sender, EventArgs e)
    {
        if (DataModelUIService == null || DataModelViewModel == null)
            return;

        DataModelViewModel.Update(DataModelUIService, new DataModelUpdateConfiguration(!IsEventPicker, !string.IsNullOrEmpty(_searchBox?.Text)));
        DataModelViewModel.ApplyTypeFilter(true, FilterTypes?.ToArray() ?? Type.EmptyTypes);
        ApplySearch();
    }

    private void TreeViewOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Multi-select isn't a thing so grab the first one
        object? selected = e.AddedItems[0];
        if (selected == null)
            return;

        if (selected is DataModelPropertyViewModel property && property.DataModelPath != null)
            DataModelPath = new DataModelPath(property.DataModelPath);
        else if (selected is DataModelListViewModel list && list.DataModelPath != null)
            DataModelPath = new DataModelPath(list.DataModelPath);
        else if (selected is DataModelEventViewModel dataModelEvent && dataModelEvent.DataModelPath != null)
            DataModelPath = new DataModelPath(dataModelEvent.DataModelPath);
    }

    private void DataModelTreeViewOnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        TreeViewItem? treeViewItem = (e.Source as ILogical)?.FindLogicalAncestorOfType<TreeViewItem>();
        if (treeViewItem != null)
            treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
    }

    private void ApplySearch()
    {
        if (DataModelViewModel == null || string.IsNullOrWhiteSpace(_searchBox?.Text))
        {
            if (_dataModelTreeView != null)
                _dataModelTreeView.IsVisible = true;
            if (_searchContainer != null)
                _searchContainer.IsVisible = false;
            return;
        }

        if (_dataModelTreeView != null)
            _dataModelTreeView.IsVisible = false;
        if (_searchContainer != null)
            _searchContainer.IsVisible = true;

        if (_searchBox.Text != _lastSearch)
        {
            _searchResults.Clear();
            foreach (DataModelVisualizationViewModel searchResult in DataModelViewModel.GetSearchResults(_searchBox.Text).DistinctBy(r => r.Path).Take(20))
                _searchResults.Add(searchResult);
            _lastSearch = _searchBox.Text;
        }

        if (_searchListBox != null)
        {
            _searchListBox.IsVisible = _searchResults.Count > 0;
            _searchListBox.SelectedItem = _searchResults.FirstOrDefault(r => r.DataModelPath?.Path == DataModelPath?.Path);
        }

        if (_searchEmpty != null)
            _searchEmpty.IsVisible = _searchResults.Count == 0;
        
       
    }

    private void UpdateCurrentPath(bool selectCurrentPath)
    {
        if (DataModelPath == null || _dataModelTreeView == null)
            return;

        if (selectCurrentPath)
        {
            // Expand the path
            DataModel? start = DataModelPath.Target;
            DataModelVisualizationViewModel? root = DataModelViewModel?.Children.FirstOrDefault(c => c.DataModel == start);
            if (root != null)
            {
                root.ExpandToPath(DataModelPath);
                _dataModelTreeView.SelectedItem = root.GetViewModelForPath(DataModelPath);
            }
        }

        if (_currentPathDisplay != null)
            _currentPathDisplay.Text = string.Join(" › ", DataModelPath.Segments.Where(s => s.GetPropertyDescription() != null).Select(s => s.GetPropertyDescription()!.Name));
        if (_currentPathDescription != null)
            _currentPathDescription.Text = DataModelPath.GetPropertyDescription()?.Description;
        if (_currentPathIcon != null)
            _currentPathIcon.Kind = DataModelPath.GetPropertyType().GetTypeIcon();
    }

    #region Overrides of TemplatedControl

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_dataModelTreeView != null)
            _dataModelTreeView.SelectionChanged -= TreeViewOnSelectionChanged;
        if (_dataModelTreeView != null)
            _dataModelTreeView.DoubleTapped -= DataModelTreeViewOnDoubleTapped;

        _currentPathIcon = e.NameScope.Find<MaterialIcon>("CurrentPathIcon");
        _currentPathDisplay = e.NameScope.Find<TextBlock>("CurrentPathDisplay");
        _currentPathDescription = e.NameScope.Find<TextBlock>("CurrentPathDescription");
        _dataModelTreeView = e.NameScope.Find<TreeView>("DataModelTreeView");
        _searchBox = e.NameScope.Find<TextBox>("SearchBox");
        _searchContainer = e.NameScope.Find<Panel>("SearchContainer");
        _searchListBox = e.NameScope.Find<ListBox>("SearchListBox");
        _searchEmpty = e.NameScope.Find<StackPanel>("SearchEmpty");

        if (_dataModelTreeView != null)
        {
            _dataModelTreeView.SelectionChanged += TreeViewOnSelectionChanged;
            _dataModelTreeView.DoubleTapped += DataModelTreeViewOnDoubleTapped;
        }

        if (_searchListBox != null)
        {
            _searchListBox.ItemsSource = _searchResults;
            _searchListBox.SelectionChanged += TreeViewOnSelectionChanged;
        }
    }

    #region Overrides of Visual

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        GetDataModel();
        
        _updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(200), DispatcherPriority.Background, Update);
        _updateTimer.Start();
        _firstUpdate = true;
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DataModelViewModel?.Dispose();

        _updateTimer?.Stop();
        _updateTimer = null;

        _lastSearch = null;
        if (_searchBox != null)
            _searchBox.Text = null;
    }

    #endregion

    #endregion
}