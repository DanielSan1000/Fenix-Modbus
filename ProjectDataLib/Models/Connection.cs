﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace ProjectDataLib
{
    [Serializable]
    public class Connection : IComparable<Connection>, ITreeViewModel, INotifyPropertyChanged, IDriverModel, ITableView, IDriversMagazine
    {
        [field: NonSerialized]
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

        [field: NonSerialized]
        private ObservableCollection<object> TreeViewChildren;

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return TreeViewChildren;
            }
            set
            {
                TreeViewChildren = value;
            }
        }

        [field: NonSerialized]
        private ObservableCollection<ITag> TagChildren;

        ObservableCollection<ITag> ITableView.Children
        {
            get
            {
                return TagChildren;
            }

            set
            {
                TagChildren = value;
            }
        }

        [field: NonSerialized]
        private ObservableCollection<IDriverModel> DriverChildren;

        ObservableCollection<IDriverModel> IDriversMagazine.Children
        {
            get
            {
                return DriverChildren;
            }

            set
            {
                DriverChildren = value;
            }
        }

        [OptionalField]
        private object Parameters_;

        [Category("03 Data"), DisplayName("Parameters"), TypeConverter(typeof(DanConverter))]
        [XmlElement("TCP", typeof(TcpDriverParam))]
        [XmlElement("IO", typeof(IoDriverParam))]
        [XmlElement("S7", typeof(S7DriverParam))]
        public object Parameters
        {
            get { return Parameters_; }
            set
            {
                Parameters_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parameters)));
            }
        }

        private Guid objId_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlElement(ElementName = "Id")]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        private Guid parentId_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlElement(ElementName = "Parent")]
        public Guid parentId
        {
            get { return parentId_; }
            set
            {
                parentId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("parentId"));
            }
        }

        private string connectionName_;

        [Category("01 Design"), DisplayName("Connection Name")]
        [XmlElement(ElementName = "Name")]
        public string connectionName
        {
            get { return connectionName_; }
            set
            {
                connectionName_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("connectionName"));
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private String DriverName_;

        [Category("02 Driver"), DisplayName("Name"), TypeConverter(typeof(DriverKind))]
        public string DriverName
        {
            get
            {
                return DriverName_;
            }
            set
            {
                //Zmiana nazwy sterownika
                DriverName_ = value;

                if (PrCon != null)
                {
                    ((IDriversMagazine)PrCon?.projectList.First())?.Children?.Remove(Idrv);
                    ((IDriversMagazine)this)?.Children?.Remove(Idrv);

                    foreach (Device d in PrCon?.projectList?.First()?.DevicesList?.Where(x => x.parentId == objId))
                        ((IDriversMagazine)d)?.Children?.Remove(Idrv);

                    //Zmiana sterownika
                    Idrv_ = gConf_?.newDrv(DriverName_);

                    ((IDriversMagazine)PrCon?.projectList?.First())?.Children.Add(Idrv);
                    ((IDriversMagazine)this)?.Children?.Add(Idrv);

                    foreach (Device d in PrCon?.projectList?.First()?.DevicesList?.Where(x => x.parentId == objId))
                        ((IDriversMagazine)d).Children.Add(Idrv);

                    //Ustawienie nowego drivera
                    Parameters_ = Idrv_?.setDriverParam;

                    if (((ITableView)this).Children?.Count > 0)
                    {
                        InTag g = new InTag();

                        foreach (ITag t in ((ITableView)this).Children)
                        {
                            if (t is Tag)
                                ((Tag)t).idrv = Idrv_;
                            else
                                ((InTag)t).idrv = Idrv_;
                        }
                    }
                }
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DriverName)));
            }
        }

        private bool IsBlocked_;

        [Category("01 Design"), DisplayName("IsBlocked")]
        public bool IsBlocked
        {
            get { return IsBlocked_; }
            set
            {
                IsBlocked_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));

                if (TreeViewChildren != null)
                    foreach (ITreeViewModel itv in TreeViewChildren)
                    {
                        itv.IsBlocked = value;

                        foreach (ITreeViewModel itv1 in itv.Children)
                            itv1.IsBlocked = value;
                    }
            }
        }

        private Boolean IsExpand_;

        [Browsable(false)]
        [JsonIgnore]
        public Boolean IsExpand
        {
            get { return IsExpand_; }
            set
            {
                IsExpand_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpand)));
            }
        }

        private Boolean assemblyError_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlIgnore]
        public Boolean assemblyError
        {
            set
            {
                assemblyError_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("assemblyError"));
            }
            get
            {
                return assemblyError_;
            }
        }

        [field: NonSerialized]
        private IDriverModel Idrv_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlIgnore]
        public IDriverModel Idrv
        {
            get { return Idrv_; }
            set
            {
                Idrv_ = value;
                Idrv_.setDriverParam = Parameters;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Idrv)));
            }
        }

        [NonSerialized]
        private GlobalConfiguration gConf_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlIgnore]
        public GlobalConfiguration gConf
        {
            set
            {
                gConf_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("gConf"));
            }
            get { return gConf_; }
        }

        [NonSerialized]
        private ProjectContainer PrCon_;

        [Browsable(false)]
        [JsonIgnore]
        [XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return PrCon_; }
            set
            {
                PrCon_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("PrCon"));
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Boolean isLive
        { get { return Idrv.isAlive; } }

        public Connection(GlobalConfiguration gC, ProjectContainer prCon, String DriverName_, string connectionName, Object driverParam)
        {
            //Nazwa
            this.connectionName_ = connectionName;

            //This
            this.Parameters_ = driverParam;

            //Inicjalizacja ID
            objId_ = Guid.NewGuid();

            //Gloablny konfigurator
            this.gConf_ = gC;

            //Znacznik rozwiniecia
            IsExpand_ = true;

            //Rodzaj sterownika
            this.DriverName_ = DriverName_;

            //Zaladowanie sterownika
            Idrv_ = gC.newDrv(DriverName_);

            //Przypisanie parametrow
            Idrv_.setDriverParam = Parameters_;

            //Dzieci
            TreeViewChildren = new ObservableCollection<object>();
            TagChildren = new ObservableCollection<ITag>();
            DriverChildren = new ObservableCollection<IDriverModel>();

            //Kontener projektowy
            PrCon = prCon;
        }

        public Connection()
        {
        }

        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            Connection Cn1 = (Connection)bf.Deserialize(ms);
            ms.Close();

            //Zmiana Danych
            Cn1.objId_ = Guid.NewGuid();

            Cn1.parentId_ = Guid.Empty;

            //Dodanie Contenera
            Cn1.PrCon = PrCon;

            //Dodanie swierzej kopii sterownika
            Cn1.Idrv_ = null;

            //Rodzice
            Cn1.TreeViewChildren = new ObservableCollection<object>();
            Cn1.TagChildren = new ObservableCollection<ITag>();
            Cn1.DriverChildren = new ObservableCollection<IDriverModel>();

            return Cn1;
        }

        public int CompareTo(Connection b)
        {
            // Alphabetic sort name[A to Z]
            return this.connectionName.CompareTo(b.connectionName);
        }

        string ITreeViewModel.Name
        {
            get
            {
                return connectionName;
            }

            set
            {
                connectionName = value;
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return IsExpand;
            }

            set
            {
                IsExpand = value;
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return isLive;
            }
            set
            {
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsLive"));
            }
        }

        bool ITreeViewModel.IsBlocked
        {
            get
            {
                return IsBlocked;
            }
            set
            {
                IsBlocked = value;
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        Guid IDriverModel.ObjId
        {
            get
            {
                return objId;
            }
        }

        string IDriverModel.driverName
        {
            get
            {
                return Idrv.driverName;
            }
        }

        object IDriverModel.setDriverParam
        {
            get
            {
                return Idrv.setDriverParam;
            }

            set
            {
                Idrv.setDriverParam = value;
            }
        }

        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get
            {
                return Idrv.MemoryAreaInf;
            }
        }

        bool IDriverModel.isAlive
        {
            get
            {
                return Idrv.isAlive;
            }

            set
            {
                Idrv.isAlive = value;
            }
        }

        bool IDriverModel.isBusy
        {
            get
            {
                return Idrv.isBusy;
            }
        }

        object[] IDriverModel.plugins
        {
            get
            {
                return Idrv.plugins;
            }
        }

        bool[] IDriverModel.AuxParam
        {
            get
            {
                return Idrv.AuxParam;
            }
        }

        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            return Idrv.activateCycle(tagsList);
        }

        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            return Idrv.addTagsComm(tagList);
        }

        bool IDriverModel.removeTagsComm(List<ITag> tagList)
        {
            return Idrv.removeTagsComm(tagList);
        }

        bool IDriverModel.reConfig()
        {
            return Idrv.reConfig();
        }

        bool IDriverModel.deactivateCycle()
        {
            return Idrv.deactivateCycle();
        }

        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return Idrv.FormatFrameRequest(frame, num);
        }

        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return Idrv.FormatFrameResponse(frame, num);
        }

        byte[] IDriverModel.sendBytes(byte[] data)
        {
            return Idrv.sendBytes(data);
        }

        event EventHandler IDriverModel.refreshedCycle
        {
            add
            {
                Idrv.refreshedCycle += value;
            }

            remove
            {
                Idrv.refreshedCycle -= value;
            }
        }

        event EventHandler IDriverModel.refreshedPartial
        {
            add
            {
                Idrv.refreshedPartial += value;
            }

            remove
            {
                Idrv.refreshedPartial -= value;
            }
        }

        event EventHandler IDriverModel.error
        {
            add
            {
                Idrv.error += value;
            }

            remove
            {
                Idrv.error -= value;
            }
        }

        event EventHandler IDriverModel.information
        {
            add
            {
                Idrv.information += value;
            }

            remove
            {
                Idrv.information -= value;
            }
        }

        event EventHandler IDriverModel.dataSent
        {
            add
            {
                Idrv.dataSent += value;
            }

            remove
            {
                Idrv.dataSent -= value;
            }
        }

        event EventHandler IDriverModel.dataRecived
        {
            add
            {
                Idrv.dataRecived += value;
            }

            remove
            {
                Idrv.dataRecived -= value;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Parameters == null)
            {
                if (DriverName == "ModbusMasterTCP")
                    Parameters = new TcpDriverParam();
                else if (DriverName == "S7-300-400 Ethernet")
                    Parameters = new S7DriverParam();
                else if (DriverName == "ModbusMasterRTU" || DriverName == "ModbusMasterASCII")
                    Parameters = new IoDriverParam();
            }
        }

        public void OnDeserializedXML()
        {
            if (Parameters == null)
            {
                if (DriverName == "ModbusMasterTCP")
                    Parameters = new TcpDriverParam();
                else if (DriverName == "S7-300-400 Ethernet")
                    Parameters = new S7DriverParam();
                else if (DriverName == "ModbusMasterRTU" || DriverName == "ModbusMasterASCII")
                    Parameters = new IoDriverParam();
            }
        }
    }
}