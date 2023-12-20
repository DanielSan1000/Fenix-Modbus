using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjectDataLib;
using MahApps.Metro.Controls;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddConnection.xaml
    /// </summary>
    public partial class AddConnection : MetroWindow
    {
        ProjectContainer PrCon;
        GlobalConfiguration gConf;
        Connection Cn;
        Guid PrId;

        public AddConnection(ProjectContainer prCon, GlobalConfiguration conf, Guid id)
        {
            try
            {
                InitializeComponent();
                PrCon = prCon;
                Cn = new Connection(conf, prCon, "", "", null);
                gConf = conf;
                PrId = id;
                CbDrivers.ItemsSource = gConf.GetDrivers();
                CbDrivers.SelectedIndex = 0;
                DataContext = Cn;
            }
            catch (Exception Ex)
            {

                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void CbDrivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                IDriverModel idrv = (IDriverModel)((ComboBox)sender).SelectedItem;
               
                Cn.Parameters = idrv.setDriverParam;
                Cn.Idrv = idrv;
                Cn.DriverName = idrv.driverName;

                PgDrvProps.SelectedObject = Cn.Parameters;

            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrCon.addConnection(PrId, Cn);
                Close();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }
    }
}
