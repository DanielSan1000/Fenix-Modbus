using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;
using io = System.IO;
using wf = System.Windows.Forms;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for CommunicationView.xaml
    /// </summary>
    public partial class CommunicationView : UserControl, INotifyPropertyChanged
    {
        //Field
        private ProjectContainer PrCon;

        public Project Pr { get; set; }
        private Guid Sel;
        private ElementKind ElKind;
        private LayoutAnchorable Win;
        private ObservableCollection<IDriverModel> DriverList = new ObservableCollection<IDriverModel>();
        private ObservableCollection<EventElement> Stack = new ObservableCollection<EventElement>();

        private int index;

        private PropertyChangedEventHandler propChanged_;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                propChanged_ += value;
            }

            remove
            {
                propChanged_ -= value;
            }
        }

        private bool mScroll_ = true;
        public Boolean mScroll
        { get { return mScroll_; } set { mScroll_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mScroll")); } }

        //Konstuktor
        public CommunicationView(ProjectContainer prCon, Guid pr, Guid sel, ElementKind elKind, LayoutAnchorable win)
        {
            InitializeComponent();

            //zmienne
            PrCon = prCon;
            Pr = prCon.projectList.First();
            Sel = sel;
            ElKind = elKind;
            Win = win;

            //dataselection
            if (elKind == ElementKind.Project)
            {
                //Title
                Win.Title = Pr.projectName;

                DriverList = ((IDriversMagazine)Pr).Children;

                //Zdarzenia
                ((INotifyPropertyChanged)Pr).PropertyChanged += CommunicationView_PropertyChanged;
            }
            else if (elKind == ElementKind.Connection)
            {
                if (Pr.connectionList.Exists(x => x.objId == Sel))
                {
                    //Typy
                    Connection cn = PrCon.getConnection(Pr.objId, Sel);

                    DriverList = ((IDriversMagazine)cn).Children;

                    //Title
                    Win.Title = Pr.projectName + "." + cn.connectionName;

                    //Zmiana tytulu
                    ((INotifyPropertyChanged)Pr).PropertyChanged += CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)cn).PropertyChanged += CommunicationView_PropertyChanged;
                }
                else
                {
                    Win.Close();
                }
            }
            else if (elKind == ElementKind.Device)
            {
                if (Pr.DevicesList.Exists(x => x.objId == Sel))
                {
                    //Typy bazowe
                    Device dev = PrCon.getDevice(Pr.objId, Sel);
                    Connection cn = PrCon.getConnection(Pr.objId, dev.parentId);

                    DriverList = ((IDriversMagazine)dev).Children;

                    //Title
                    Win.Title = Pr.projectName + "." + cn.connectionName + "." + dev.name;

                    //Zdarzenie
                    ((INotifyPropertyChanged)Pr).PropertyChanged += CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)cn).PropertyChanged += CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)dev).PropertyChanged += CommunicationView_PropertyChanged;
                }
                else
                {
                    Win.Close();
                }
            }

            //Drivery
            foreach (IDriverModel idrv in DriverList)
            {
                idrv.dataRecived += new EventHandler(idrv_reciveLogInfo);
                idrv.dataSent += new EventHandler(idrv_sendLogInfo);
                idrv.error += new EventHandler(idrv_errorRecived);
                idrv.information += new EventHandler(idrv_informationRecived);
            }

            //Obserwacja stacku
            DataContext = this;
            View.DataContext = this;
            ((FrameworkElement)Resources["ProxyElement"]).DataContext = this;
            View.ItemsSource = Stack;

            //Zdarzenia
            Win.Closing += Win_Closing;

            //Dodanie indeksu okna do globalnego buffora informacji o stanie okien
            index = PrCon.winManagment.Count;
            PrCon.winManagment.Add(new WindowsStatus(index, true, false));

            //zmiana kolkcji
            Stack.CollectionChanged += Stack_CollectionChanged;
            DriverList.CollectionChanged += DriverList_CollectionChanged;
        }

        //Zmiana listy driverow
        private void DriverList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    IDriverModel idrv = ((IDriverModel)e.OldItems[0]);
                    idrv.dataRecived -= new EventHandler(idrv_reciveLogInfo);
                    idrv.dataSent -= new EventHandler(idrv_sendLogInfo);
                    idrv.error -= new EventHandler(idrv_errorRecived);
                    idrv.information -= new EventHandler(idrv_informationRecived);
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    IDriverModel idrv = ((IDriverModel)e.NewItems[0]);
                    idrv.dataRecived += new EventHandler(idrv_reciveLogInfo);
                    idrv.dataSent += new EventHandler(idrv_sendLogInfo);
                    idrv.error += new EventHandler(idrv_errorRecived);
                    idrv.information += new EventHandler(idrv_informationRecived);
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Title data
        private void CommunicationView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                //dataselection
                if (ElKind == ElementKind.Project)
                {
                    Win.Title = Pr.projectName;
                }
                else if (ElKind == ElementKind.Connection)
                {
                    //Typy
                    Connection cn = PrCon.getConnection(Pr.objId, Sel);

                    //Title
                    Win.Title = Pr.projectName + "." + cn.connectionName;
                }
                else if (ElKind == ElementKind.Device)
                {
                    //Typy bazowe
                    Device dev = PrCon.getDevice(Pr.objId, Sel);
                    Connection cn = PrCon.getConnection(Pr.objId, dev.parentId);

                    //Title
                    Win.Title = Pr.projectName + "." + cn.connectionName + "." + dev.name;
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Scroll Button
        private void Stack_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (View.Items.Count > 0 && mScroll)
            {
                var border = VisualTreeHelper.GetChild(View, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }

        //Zamykanie okna
        private void Win_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //Zdarzenia
                Win.Closing -= Win_Closing;

                if (ElKind == ElementKind.Project)
                {
                    ((INotifyPropertyChanged)Pr).PropertyChanged -= CommunicationView_PropertyChanged;
                }
                else if (ElKind == ElementKind.Connection)
                {
                    //Typy
                    Connection cn = PrCon.getConnection(Pr.objId, Sel);

                    //Zmiana tytulu
                    ((INotifyPropertyChanged)Pr).PropertyChanged -= CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)cn).PropertyChanged -= CommunicationView_PropertyChanged;
                }
                else if (ElKind == ElementKind.Device)
                {
                    //Typy bazowe
                    Device dev = PrCon.getDevice(Pr.objId, Sel);
                    Connection cn = PrCon.getConnection(Pr.objId, dev.parentId);

                    //Zdarzenie
                    ((INotifyPropertyChanged)Pr).PropertyChanged -= CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)cn).PropertyChanged -= CommunicationView_PropertyChanged;
                    ((INotifyPropertyChanged)dev).PropertyChanged -= CommunicationView_PropertyChanged;
                }

                DriverList.CollectionChanged -= DriverList_CollectionChanged;

                //Informacja o stanie okna
                PrCon.winManagment.RemoveAll(x => x.index == index);
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        /// Wyslanie danych przez sterownik
        private void idrv_sendLogInfo(object sender1, EventArgs e1)
        {
            //Delegat
            EventHandler crossDef = delegate (object sender, EventArgs e)
            {
                //Projekt Event
                ProjectEventArgs ev = (ProjectEventArgs)e;

                IDriverModel idrv = (IDriverModel)sender;
                Connection cn = null;
                if (Pr.connectionList.Exists(x => x.Idrv.ObjId == idrv.ObjId))
                    cn = Pr.connectionList.Find(x => x.Idrv.ObjId == idrv.ObjId);

                //Dane
                byte[] data = (byte[])ev.element;

                //Czas
                DateTime time = (DateTime)ev.element0;

                //Informacja
                String description = (String)ev.element1;

                EventElement lsEl = null;
                if (Stack.Count > 0)
                    lsEl = Stack.Last();

                Stack.Add(new EventElement(Pr, sender, data, time, description, EventType.OUT, lsEl, cn?.connectionName ?? "No Name"));
            };

            try
            {
                //Wywolanie
                this.Dispatcher?.Invoke(crossDef, sender1, e1);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        // Odebranie danych przez sterownik
        private void idrv_reciveLogInfo(object sender1, EventArgs e1)
        {
            EventHandler crossDef = delegate (object sender, EventArgs e)
            {
                ProjectEventArgs ev = (ProjectEventArgs)e;

                IDriverModel idrv = (IDriverModel)sender;
                Connection cn = null;
                if (Pr.connectionList.Exists(x => x.Idrv.ObjId == idrv.ObjId))
                    cn = Pr.connectionList.Find(x => x.Idrv.ObjId == idrv.ObjId);

                //Dane
                byte[] data = (byte[])ev.element;

                //Czas
                DateTime time = (DateTime)ev.element0;

                //Informacja
                String description = (String)ev.element1;

                EventElement lsEl = null;
                if (Stack.Count > 0)
                    lsEl = Stack.Last();

                Stack.Add(new EventElement(Pr, sender, data, time, description, EventType.IN, lsEl, cn?.connectionName ?? "No Name"));
            };

            try
            {
                this.Dispatcher?.Invoke(crossDef, sender1, e1);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        //Informacja
        private void idrv_informationRecived(object sender, EventArgs e)
        {
            EventHandler lacznik = delegate (object se, EventArgs e1)
            {
                ProjectEventArgs ev = (ProjectEventArgs)e1;

                IDriverModel idrv = (IDriverModel)sender;
                Connection cn = null;
                if (Pr.connectionList.Exists(x => x.Idrv.ObjId == idrv.ObjId))
                    cn = Pr.connectionList.Find(x => x.Idrv.ObjId == idrv.ObjId);

                //Dane
                byte[] data = (byte[])ev.element;

                //Czas
                DateTime time = (DateTime)ev.element0;

                //Informacja
                String description = (String)ev.element1;

                EventElement lsEl = null;
                if (Stack.Count > 0)
                    lsEl = Stack.Last();

                //Stack add
                Stack.Add(new EventElement(Pr, se, data, time, description, EventType.INFO, lsEl, cn?.connectionName ?? "No Name"));
            };

            View.Dispatcher?.Invoke(lacznik, sender, e);
        }

        // Error
        private void idrv_errorRecived(object sender, EventArgs e)
        {
            EventHandler lacznik = delegate (object se, EventArgs e1)
            {
                ProjectEventArgs ev = (ProjectEventArgs)e1;

                IDriverModel idrv = (IDriverModel)sender;
                Connection cn = null;
                if (Pr.connectionList.Exists(x => x.Idrv.ObjId == idrv.ObjId))
                    cn = Pr.connectionList.Find(x => x.Idrv.ObjId == idrv.ObjId);

                //Dane
                byte[] data = (byte[])ev.element;

                //Czas
                DateTime time = (DateTime)ev.element0;

                //Informacja
                String description = (String)ev.element1;

                EventElement lsEl = null;
                if (Stack.Count > 0)
                    lsEl = Stack.Last();

                Stack.Add(new EventElement(Pr, se, data, time, description, EventType.ERROR, lsEl, cn?.connectionName ?? "No Name"));
            };

            View.Dispatcher?.Invoke(lacznik, sender, e);
        }

        //Wyczysc Log
        private void Button_Clr_Click(object sender, RoutedEventArgs e)
        {
            Stack.Clear();
        }

        //Save CSV
        private void SaveCSV(object sender, RoutedEventArgs e)
        {
            wf.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "*.CSV | *.csv";

            if (sfd.ShowDialog() == wf.DialogResult.OK)
            {
                //View.SelectAllCells();
                View.SelectAll();
                View.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, View);
                View.UnselectAll();
                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);

                //Zapisanie danych
                io.File.WriteAllText(sfd.FileName, result);
                Clipboard.Clear();
            }
        }

        private void Clipbord(object sender, RoutedEventArgs e)
        {
            //View.SelectAllCells();
            View.SelectAll();
            View.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, View);
            View.UnselectAll();
        }
    }

    public class DateTimeFormConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime tm = (DateTime)values[0];
            Project pr = (Project)values[1];
            return tm.ToString(pr.longDT);
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue };
        }
    }

    public class DataBytes : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IDriverModel idrv = (IDriverModel)values[0];
            byte[] data = (byte[])values[1];
            string info = (string)values[2];
            EventType Type = (EventType)values[3];

            if (Type == EventType.OUT)
                return idrv.FormatFrameRequest(data, NumberStyles.HexNumber);
            else if (Type == EventType.IN)
                return idrv.FormatFrameResponse(data, NumberStyles.HexNumber);
            else
            {
                return "INTERNAL PROBLEM";
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue };
        }
    }

    public class ColorConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EventElement ev = (EventElement)value;

            if (ev.Type == EventType.ERROR)
                return new SolidColorBrush(Colors.Red);
            else if (ev.Type == EventType.INFO)
                return new SolidColorBrush(Colors.Yellow);
            else if (ev.Type == EventType.IN)
                return new SolidColorBrush(Colors.LightGreen);
            else if (ev.Type == EventType.OUT)
                return new SolidColorBrush(Colors.LightBlue);

            return new SolidColorBrush(Colors.Gray);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class TagsConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EventElement ev = (EventElement)value;

            string s = "";

            var tgs = from t in ev.Pr.tagsList where t.idrv.ObjId == ((IDriverModel)ev.Sender).ObjId select t;
            foreach (ITag tg in tgs)
                s = s + string.Format("{0}: {1}  ;", tg.Name, tg.GetFormatedValue());

            return s;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class BoolToVis : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}