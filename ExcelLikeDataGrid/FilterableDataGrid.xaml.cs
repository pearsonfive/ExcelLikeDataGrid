using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ExcelLikeDataGrid
{
    public partial class FilterableDataGrid : UserControl
    {
        // Maintains a list of selected values for each column property
        private Dictionary<string, HashSet<string>> _activeFilters = new Dictionary<string, HashSet<string>>();
        private string _currentFilterProperty;
        private ObservableCollection<FilterItem> _currentFilterItems = new ObservableCollection<FilterItem>();
        private ICollectionView _dataView;
        private int _totalRecordCount;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(System.Collections.IEnumerable), typeof(FilterableDataGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        public System.Collections.IEnumerable ItemsSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FilterableDataGrid)d;
            control.MainDataGrid.ItemsSource = (System.Collections.IEnumerable)e.NewValue;
            
            if (e.NewValue != null)
            {
                control._totalRecordCount = ((System.Collections.IEnumerable)e.NewValue).Cast<object>().Count();
                control._dataView = CollectionViewSource.GetDefaultView((System.Collections.IEnumerable)e.NewValue);
                if (control._dataView != null)
                {
                    control._dataView.Filter = control.FilterData;
                }
            }
            else
            {
                control._dataView = null;
                control._totalRecordCount = 0;
            }
            
            control._activeFilters.Clear();
            control.UpdateStatusBar();
        }

        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(FilterableDataGrid), new PropertyMetadata(false, OnAutoGenerateColumnsChanged));

        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        private static void OnAutoGenerateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FilterableDataGrid)d;
            control.MainDataGrid.AutoGenerateColumns = (bool)e.NewValue;
        }

        public ObservableCollection<DataGridColumn> Columns
        {
            get { return MainDataGrid.Columns; }
        }

        public FilterableDataGrid()
        {
            InitializeComponent();
            FilterItemsListBox.ItemsSource = _currentFilterItems;
        }

        private bool FilterData(object item)
        {
            if (_activeFilters.Count == 0) return true;

            var type = item.GetType();
            foreach (var filter in _activeFilters)
            {
                var prop = type.GetProperty(filter.Key);
                if (prop != null)
                {
                    var val = prop.GetValue(item)?.ToString() ?? string.Empty;
                    if (!filter.Value.Contains(val))
                        return false;
                }
            }
            return true;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button && button.Tag is DataGridColumn column)
            {
                if (column is DataGridBoundColumn boundColumn && boundColumn.Binding is Binding binding)
                {
                    _currentFilterProperty = binding.Path.Path;

                    // Extract all unique values from the underlying data
                    var items = MainDataGrid.ItemsSource?.Cast<object>().ToList();
                    if (items == null || !items.Any()) return;

                    var distinctValues = new HashSet<string>();
                    
                    var type = items.FirstOrDefault()?.GetType();
                    if (type == null) return;
                    
                    var propInfo = type.GetProperty(_currentFilterProperty);
                    if (propInfo == null) return;

                    foreach (var item in items)
                    {
                        var val = propInfo.GetValue(item)?.ToString() ?? string.Empty;
                        distinctValues.Add(val);
                    }

                    _currentFilterItems.Clear();
                    
                    // Check if there is an active filter for this property
                    bool hasActiveFilter = _activeFilters.ContainsKey(_currentFilterProperty);
                    HashSet<string> selectedValues = hasActiveFilter ? _activeFilters[_currentFilterProperty] : new HashSet<string>();

                    foreach (var val in distinctValues.OrderBy(x => x))
                    {
                        var filterItem = new FilterItem
                        {
                            Value = val,
                            IsSelected = !hasActiveFilter || selectedValues.Contains(val)
                        };
                        _currentFilterItems.Add(filterItem);
                    }

                    UpdateSelectAllState();

                    // Open popup
                    FilterPopup.PlacementTarget = button;
                    FilterPopup.IsOpen = true;
                }
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectAllCheckBox.IsChecked.HasValue)
            {
                bool isChecked = SelectAllCheckBox.IsChecked.Value;
                foreach (var item in _currentFilterItems)
                {
                    item.IsSelected = isChecked;
                }
            }
        }

        private void FilterItem_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectAllState();
        }

        private void UpdateSelectAllState()
        {
            if (_currentFilterItems.All(i => i.IsSelected))
                SelectAllCheckBox.IsChecked = true;
            else if (_currentFilterItems.All(i => !i.IsSelected))
                SelectAllCheckBox.IsChecked = false;
            else
                SelectAllCheckBox.IsChecked = null;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = false;

            if (_currentFilterItems.All(i => i.IsSelected))
            {
                if (_activeFilters.ContainsKey(_currentFilterProperty))
                    _activeFilters.Remove(_currentFilterProperty);
            }
            else
            {
                var selectedValues = new HashSet<string>(_currentFilterItems.Where(i => i.IsSelected).Select(i => i.Value));
                _activeFilters[_currentFilterProperty] = selectedValues;
            }

            _dataView?.Refresh();
            UpdateStatusBar();
        }

        private void CancelFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPopup.IsOpen = false;
        }

        private void UpdateStatusBar()
        {
            int visibleRecords = MainDataGrid.Items.Count;
            string statusText = $"Records: {visibleRecords} / {_totalRecordCount}";
            RecordCountTextBlock.Text = statusText;
        }
    }

    public class FilterItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        public string Value { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
