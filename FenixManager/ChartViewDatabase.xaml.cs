using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for ChartViewDatabase.xaml
    /// </summary>
    public partial class ChartViewDatabase : UserControl, INotifyPropertyChanged
    {
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _selectedInterval;
        private string _interactionDescription;

        public event PropertyChangedEventHandler PropertyChanged;

        public PlotModel PlotModel { get; private set; }
        public PlotController PlotController { get; private set; }

        public LinearAxis AxY1 { get; set; }
        public DateTimeAxis AxX1 { get; set; }

        public ObservableCollection<string> TimeIntervals { get; }

        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                    UpdatePlotAxes();
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
                    UpdatePlotAxes();
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

        public string InteractionDescription
        {
            get => _interactionDescription;
            set
            {
                if (_interactionDescription != value)
                {
                    _interactionDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChartViewDatabase(Project project)
        {
            InitializeComponent();
            DataContext = this;

            // Initialize PlotController
            PlotController = new PlotController();
            PlotController.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            PlotController.BindMouseDown(OxyMouseButton.Right, PlotCommands.ZoomRectangle);
            PlotController.BindMouseWheel(PlotCommands.ZoomWheel);
            PlotController.BindKeyDown(OxyKey.R, PlotCommands.Reset);

            // Set interaction descriptions
            InteractionDescription = "Left Click + Drag: Pan | Right Click + Drag: Zoom Rectangle | Mouse Wheel: Zoom | R: Reset | C: Copy to Clipboard";

            // Axes
            AxY1 = new LinearAxis { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Dash };
            AxX1 = new DateTimeAxis { Position = AxisPosition.Bottom, MajorGridlineStyle = LineStyle.Dash, StringFormat = "HH:mm:ss" };

            // Plot
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(AxX1);
            PlotModel.Axes.Add(AxY1);
            View.Model = PlotModel;

            LoadData(project);

            // Initialize default values
            TimeIntervals = new ObservableCollection<string> { "1h", "3h", "6h", "12h", "24h" };
            SelectedInterval = TimeIntervals.First();
            UpdateDateRange();
        }

        private void LoadData(Project project)
        {
            try
            {
                foreach (var group in project.Db.GetAllObservableCollection().GroupBy(x => x.Name))
                {
                    ITag tag = project.GetITag(group.Key);
                    var series = new LineSeries
                    {
                        Title = group.Key,
                        TrackerFormatString = "{0}" + Environment.NewLine + "Y: {4:0.000}" + Environment.NewLine + "X: {2:" + project.longDT + "}",
                        Color = OxyColor.FromRgb(tag.Clr.R, tag.Clr.G, tag.Clr.B),
                        StrokeThickness = tag.Width,
                        IsVisible = true
                    };

                    foreach (var point in group)
                    {
                        series.Points.Add(new DataPoint(point.Stamp.ToOADate(), point.Value));
                    }

                    PlotModel.Series.Add(series);
                }

                PlotModel.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
            }
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

        private void UpdatePlotAxes()
        {
            if (AxX1 != null)
            {
                AxX1.Minimum = DateTimeAxis.ToDouble(FromDate);
                AxX1.Maximum = DateTimeAxis.ToDouble(ToDate);
                PlotModel.InvalidatePlot(false);
            }
        }

        private void ShowAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PlotModel.Series.Count > 0)
            {
                var allDataPoints = PlotModel.Series
                    .OfType<LineSeries>()
                    .SelectMany(series => series.Points)
                    .ToList();

                if (allDataPoints.Any())
                {
                    FromDate = DateTimeAxis.ToDateTime(allDataPoints.Min(point => point.X));
                    ToDate = DateTimeAxis.ToDateTime(allDataPoints.Max(point => point.X));
                }
            }

            PlotModel.ResetAllAxes();
            PlotModel.InvalidatePlot(true);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}