using OxyPlot.Series;
using OxyPlot;
using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace FenixWPF
{
    public partial class DbExplorer : UserControl, INotifyPropertyChanged
    {
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _selectedInterval;
        private string _selectedOrder;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Project _project;

        public ObservableCollection<string> TimeIntervals { get; }
        public ObservableCollection<string> OrderOptions { get; }

        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                    FilterData();
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
                    FilterData();
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
                    FilterData();
                }
            }
        }

        public DbExplorer(Project project)
        {
            InitializeComponent();
            DataContext = this;
            _project = project;

            // Initialize default values
            TimeIntervals = new ObservableCollection<string> { "1h", "3h", "6h", "12h", "24h" };
            OrderOptions = new ObservableCollection<string> { "Descending", "Ascending" };
            SelectedInterval = TimeIntervals.First();
            SelectedOrder = OrderOptions.First();

            UpdateDateRange();
            FilterData();
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

        private void FilterData()
        {
            if (SelectedOrder == null || OrderOptions == null) return;

            bool descending = SelectedOrder == OrderOptions[0];
            myDataGrid.ItemsSource = _project.Db.GetDataByStamp(FromDate, ToDate, descending);
        }

        private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            myDataGrid.ItemsSource = _project.Db.GetAllObservableCollection();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}