using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for CheckVersion.xaml
    /// </summary>
    public partial class CheckVersion : MetroWindow, INotifyPropertyChanged
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

        /// <summary>
        /// Kontener
        /// </summary>
        ProjectContainer PrCon;

        /// <summary>
        /// Delegat1
        /// </summary>
        event EventHandler setProgress;

        /// <summary>
        /// Delagat w
        /// </summary>
        event EventHandler setParam;

        private string SerVer_;
        public string SerVer
        {
            get { return SerVer_; }
            set
            {
                SerVer_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SerVer)));
            }
        }

        private string InsVer_;
        public string InsVer
        {
            get { return InsVer_; }
            set
            {
                InsVer_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InsVer)));
            }
        }

        private string Status_;
        public string Status
        {
            get { return Status_; }

            set
            {
                Status_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        private Boolean Update_;
        public Boolean Update
        {
            get { return Update_; }
            set
            {
                Update_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Update)));
            }
        }

        string Adress { get; set; }

        //Ctor
        public CheckVersion(ProjectContainer prcn)
        {
            InitializeComponent();
            this.PrCon = prcn;
            DataContext = this;

            try
            {
                //Informacja o progresie
                setProgress += new EventHandler(frCheckVersion_setProgress);
                setParam += new EventHandler(frCheckVersion_setParam);

                //Numer Zainstalownej wersji
                InsVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }

            //Update Softwere
            try
            {
                //Sprawdzanie Wersji
                Thread update = new Thread(new ParameterizedThreadStart(updateSoftware));
                update.Start();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Ustawienie parametrów do oczytu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frCheckVersion_setParam(object sender, EventArgs e)
        {
            EventHandler even = delegate (object sen, EventArgs ev)
            {
                //Tablica wysłana z podrzednego wątku
                Object[] data = (Object[])sen;

                //Pokzania wersji na serwerze
                SerVer = ((Version)data[0]).ToString();

                //Ustawienie URL
                Adress = (String)data[2];

                //Uaktywnienie buttona
                Update = (Boolean)data[1];
            };

            try
            {
                //Wywolanie metody
                    Dispatcher?.Invoke(even, sender, e);
            }
            catch (Exception Ex)
            {

                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        /// <summary>
        /// Ustaw progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frCheckVersion_setProgress(object sender, EventArgs e)
        {
            //Anonimowy delegat
            EventHandler evetn = delegate (object sen, EventArgs ev)
            {
                //Przypisanie wartoci
                Object[] data = (Object[])sen;
                pBar.Value = (Int32)data[0];
                Status = (String)data[1];
            };

            try
            {
                //Wywolanie
                pBar?.Dispatcher?.Invoke(evetn, new object[2] { sender, e });
            }
            catch (Exception Ex)
            {

                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }

        }

        /// <summary>
        /// Sprawdzenie Wersji oprogramowania
        /// </summary>
        /// <param name="obj"></param>
        void updateSoftware(object obj)
        {
            //Sprawdzenie czy istnieje poloczenie
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                setProgress(new Object[2] { 0, "No internet connetion." }, new EventArgs());
                return;
            }

            //Dokument XML
            Version newVersion = null;
            string url = "";
            XmlTextReader reader = null;

            //Ustalenie progresu 10%
            setProgress(new Object[2] { 10, "Starting..." }, new EventArgs());

            try
            {
                //Strona pliku z wersją
                string xmlURL = "https://sourceforge.net/projects/fenixmodbus/files/version.xml";

                //Pobranie zawartości
                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";

                //Ustalenie progresu 50%
                setProgress(new Object[2] { 50, "Downloading..." }, new EventArgs());

                //Odczyt XML
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Fenix"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            // for text nodes...  
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {
                                // we check what the name of the node was  
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);

                                        break;
                                    case "url":
                                        url = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }


                //Ustalenie progresu 100%
                setProgress(new Object[2] { 100, "Checking Version..." }, new EventArgs());

                // compare the versions  
                if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.CompareTo(newVersion) < 0)
                {
                    Object[] obj1 = new object[3] { newVersion, true, url };
                    setParam(obj1, new EventArgs());
                }
                else
                {
                    Object[] obj1 = new object[3] { newVersion, false, url };
                    setParam(obj1, new EventArgs());
                }

                //Zakonczenie
                setProgress(new Object[2] { 0, "Finished" }, new EventArgs());

            }
            catch (Exception Ex)
            {
                //Ustalenie progresu 0%
                setProgress(new Object[2] { 0, Ex.Message }, new EventArgs());
            }
            finally
            {
                if (reader != null) reader.Close();

            }
        }

        //Update
        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Adress);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        //Close
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
