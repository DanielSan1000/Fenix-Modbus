using MahApps.Metro.Controls;
using ProjectDataLib;
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

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddDevice.xaml
    /// </summary>
    public partial class AddDevice : MetroWindow
    {
        ProjectContainer PrCon;
        Device Dev;
        Guid connId = Guid.Empty;
        Guid projId = Guid.Empty;

        public AddDevice(ProjectContainer pC, Guid projId, Guid connId)
        {
            InitializeComponent();

            try
            {
                //Poloczenie
                PrCon = pC;

                //Conncetion ID
                this.connId = connId;

                //
                this.projId = projId;

                //Pobranie poloczenia
                Connection cn = pC.getConnection(projId, connId);

                //Ustawienie rozszerzonego adresowania
                TbAdress.IsEnabled = cn.Idrv.AuxParam[0];

                //Device
                Dev = new Device("", 1, PrCon, projId);

                //Context
                DataContext = Dev;

            }
            catch (Exception Ex)
            {

                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Dev.idrv = PrCon.getConnection(projId, connId).Idrv;
                PrCon.addDevice(projId, connId, Dev);
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
