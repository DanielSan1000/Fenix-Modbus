using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Xml;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for CheckVersion.xaml
    /// </summary>
    public partial class CheckVersion : MetroWindow, INotifyPropertyChanged
    {
        private PropertyChangedEventHandler propChanged;

        /// <inheritdoc/>
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

        private ProjectContainer PrCon;

        private event EventHandler setProgress;

        private event EventHandler setParam;

        private string SerVer_;

        /// <summary>
        /// Gets or sets the server version.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the installed version.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether an update is available.
        /// </summary>
        public Boolean Update
        {
            get { return Update_; }
            set
            {
                Update_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Update)));
            }
        }

        private string Adress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckVersion"/> class.
        /// </summary>
        /// <param name="prcn">The project container.</param>
        public CheckVersion(ProjectContainer prcn)
        {
            InitializeComponent();
            this.PrCon = prcn;
            DataContext = this;

            try
            {
                setProgress += new EventHandler(frCheckVersion_setProgress);
                setParam += new EventHandler(frCheckVersion_setParam);

                InsVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }

            try
            {
                Thread update = new Thread(new ParameterizedThreadStart(updateSoftware));
                update.Start();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Event handler for setting parameters.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void frCheckVersion_setParam(object sender, EventArgs e)
        {
            EventHandler even = delegate (object sen, EventArgs ev)
            {
                Object[] data = (Object[])sen;

                SerVer = ((Version)data[0]).ToString();

                Adress = (String)data[2];

                Update = (Boolean)data[1];
            };

            try
            {
                Dispatcher?.Invoke(even, sender, e);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        /// <summary>
        /// Event handler for setting progress.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void frCheckVersion_setProgress(object sender, EventArgs e)
        {
            EventHandler evetn = delegate (object sen, EventArgs ev)
            {
                Object[] data = (Object[])sen;
                pBar.Value = (Int32)data[0];
                Status = (String)data[1];
            };

            try
            {
                pBar?.Dispatcher?.Invoke(evetn, new object[2] { sender, e });
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        /// <summary>
        /// Method for updating the software.
        /// </summary>
        /// <param name="obj">The parameter object.</param>
        private void updateSoftware(object obj)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                setProgress(new Object[2] { 0, "No internet connection." }, new EventArgs());
                return;
            }

            Version newVersion = null;
            string url = "";
            XmlTextReader reader = null;

            setProgress(new Object[2] { 10, "Starting..." }, new EventArgs());

            try
            {
                string xmlURL = "https://github.com/DanielSan1000/Fenix-Modbus/blob/master/version.xml";

                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";

                setProgress(new Object[2] { 50, "Downloading..." }, new EventArgs());

                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Fenix"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {
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

                setProgress(new Object[2] { 100, "Checking Version..." }, new EventArgs());

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

                setProgress(new Object[2] { 0, "Finished" }, new EventArgs());
            }
            catch (Exception Ex)
            {
                setProgress(new Object[2] { 0, Ex.Message }, new EventArgs());
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Event handler for the update button click.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Event handler for the close button click.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}