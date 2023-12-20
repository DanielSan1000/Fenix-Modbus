using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for DriverConfigurator.xaml
    /// </summary>
    public partial class DriverConfigurator : MetroWindow
    {
        /// <summary>
        /// Gloabalna konfiguracja
        /// </summary>
        GlobalConfiguration gConf;

        /// <summary>
        /// Kontener
        /// </summary>
        ProjectContainer PrCon;

        private ObservableCollection<Drv> Drvs_ = new ObservableCollection<Drv>();
        public ObservableCollection<Drv> Drvs
        {
            get { return Drvs_; }
            set { Drvs_ = value; }
        }


        public DriverConfigurator(GlobalConfiguration conf, ProjectContainer prcn)
        {
            InitializeComponent();

            DataContext = this;

            this.gConf = conf;
            this.PrCon = prcn;

            //Jezeli na liscie sterowników brakuje sciezek wróc
            if (gConf.assmemblyPath.Count == 0)
                return;

            for (int i = 0; i < gConf.assmemblyPath.Count; i++)
            {
                //Sprawdzenie czy plik istnieje
                if (System.IO.File.Exists(gConf.assmemblyPath[i]))
                {
                    //Zaladownie biblioteki
                    Assembly asm = Assembly.LoadFile(gConf.assmemblyPath[i]);

                    //Sprawdzenie interfejsu czy obsluguje interfejs
                    if (gConf.checkAssembly(asm))
                    {
                        //Zaladowanie Sterownika
                        Type tp = asm.GetType("nmDriver.Driver");
                        IDriverModel idrv = (IDriverModel)asm.CreateInstance(tp.FullName);
                        Drv d = new Drv() { Index = i, Name = idrv.driverName, Ver = tp.Assembly.GetName().Version.ToString(), Path = gConf.assmemblyPath[i] };
                        Drvs.Add(d);
                    }
                }

            }
        }

        //Add Drivers
        private void Button_AddDriver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fDialog = new OpenFileDialog();
                fDialog.Filter = "Dll Files (*.dll) | *.dll";
                fDialog.Multiselect = true;
                fDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

                if (fDialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                {

                    //Dodawanie plików
                    foreach (string s in fDialog.FileNames)
                    {

                        Assembly asm = Assembly.LoadFile(s);

                        if (gConf.checkAssembly(asm))
                        {                           
                            Type tp = asm.GetType("nmDriver.Driver");
                            IDriverModel idrv = (IDriverModel)asm.CreateInstance(tp.FullName);                        
                            Drv d = new Drv() { Index = Drvs.Count, Name = idrv.driverName, Ver = tp.Assembly.GetName().Version.ToString(), Path = s };
                            Drvs.Add(d);
                            gConf.addDrvMan(s);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        //RemoveDriver
        private void Button_RemoveDriver_Click(object sender, RoutedEventArgs e)
        {
            Drv dr = (Drv)View.SelectedItem;
            if(dr != null)
            {
                gConf.removeDrv(dr.Path);
                Drvs.Remove(dr);
            }
        }

        //Reset
        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.Windows.Forms.MessageBox.Show("Do you want reset all driver configuration?", "Driver Configuration", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (System.IO.File.Exists("GlobalConfiguration.xml"))
                    {
                        System.IO.File.Delete("GlobalConfiguration.xml");
                        System.Windows.Forms.MessageBox.Show("Please Reset Software!");
                        Close();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please Reset Software!");
                        Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }
    }

    public class Drv : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler propChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                propChanged += value;
            }

            remove
            {
                propChanged -= value;
            }
        }

        private int Index_;
        public int Index
        {
            get { return Index_; }
            set
            {
                Index_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
            }
        }

        private string Name_;
        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string Ver_;
        public string Ver
        {
            get { return Ver_; }
            set
            {
                Ver_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ver)));
            }
        }

        private string Path_;
        public string Path
        {
            get { return Path_; }
            set
            {
                Path_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
            }
        }

    }
}
