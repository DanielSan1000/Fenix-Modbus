﻿using OxyPlot.Series;
using OxyPlot;
using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Threading.Tasks;

namespace FenixWPF
{
    public partial class DbExplorer : UserControl, INotifyPropertyChanged
    {
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _selectedInterval;
        private string _selectedOrder;
        private bool _isLoading;
        private readonly Project _project;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> TimeIntervals { get; }
        public ObservableCollection<string> OrderOptions { get; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedInterval
        {
            get => _selectedInterval;
            set
            {
                if (_selectedInterval != value)
                {
                    _selectedInterval = value;
                    OnPropertyChanged();
                    UpdateDateRange();
                }
            }
        }

        public string SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public DbExplorer(Project project)
        {
            InitializeComponent();
            DataContext = this;
            _project = project;

            // Initialize default values
            TimeIntervals = ["1h", "3h", "6h", "12h", "24h"];
            OrderOptions = ["Descending", "Ascending"];
            SelectedInterval = TimeIntervals.First();
            SelectedOrder = OrderOptions.First();

            UpdateDateRange();
            GetDataFormDatabase();
        }

        private void UpdateDateRange()
        {
            DateTime now = DateTime.Now;
            FromDate = SelectedInterval switch
            {
                "1h" => now.AddHours(-1),
                "3h" => now.AddHours(-3),
                "6h" => now.AddHours(-6),
                "12h" => now.AddHours(-12),
                "24h" => now.AddHours(-24),
                _ => FromDate
            };
            ToDate = now;
        }

        private async Task GetDataFormDatabase()
        {
            if (SelectedOrder == null || OrderOptions == null) return;

            bool descending = SelectedOrder == OrderOptions[0];
            if (BlockTimeFiltersCheckBox.IsChecked is true)
            {
                myDataGrid.ItemsSource = await _project.Db.GetAllTagsAsync(descending);
            }
            else
            {
                myDataGrid.ItemsSource = await _project.Db.GetDataByStampAsync(FromDate, ToDate, descending);
            }                
        }

        private async void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IsLoading = true;
            await GetDataFormDatabase();
            IsLoading = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}