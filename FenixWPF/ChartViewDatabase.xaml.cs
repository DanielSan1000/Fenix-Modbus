using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ProjectDataLib;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for ChartViewDatabase.xaml
    /// </summary>
    public partial class ChartViewDatabase : UserControl
    {
        public PlotModel plotModel { get; private set; }

        public LinearAxis AxY1 { get; set; }

        public DateTimeAxis AxX1 { get; set; }

        public ChartViewDatabase(Project pr)
        {
            InitializeComponent();

            //Osie
            AxY1 = new LinearAxis { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Dash };
            AxX1 = new DateTimeAxis { Position = AxisPosition.Bottom, MajorGridlineStyle = LineStyle.Dash };
            AxX1.StringFormat = "HH:mm:ss";

            //Wykres
            plotModel = new PlotModel { };
            plotModel.Axes.Add(AxX1);
            plotModel.Axes.Add(AxY1);
            View.Model = plotModel;

            Load_Data(pr);
        }

        private void Load_Data(Project pr)
        {
            try
            {
                foreach (var gr in pr.Db.GetAllObservableCollection().GroupBy(x => x.Name))
                {
                    ITag t = pr.GetITag(gr.Key);
                    var s1 = new LineSeries
                    {
                        Title = gr.Key,
                        TrackerFormatString = "{0}" + Environment.NewLine + "Y: {4:0.000}" + Environment.NewLine + "X: {2:" + pr.longDT + "}",
                        Color = OxyColor.FromRgb(t.Clr.R, t.Clr.G, t.Clr.B),
                        StrokeThickness = t.Width,
                        IsVisible = true
                    };

                    foreach (var pt in gr)
                    {
                        s1.Points.Add(new DataPoint(pt.Stamp.ToOADate(), pt.Value));
                    }

                    plotModel.Series.Add(s1);
                }

                plotModel.InvalidatePlot(true);
            }
            catch (Exception Ex)
            {
                //PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }
    }
}