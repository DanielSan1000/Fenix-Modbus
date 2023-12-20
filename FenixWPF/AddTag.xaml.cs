using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddTag.xaml
    /// </summary>
    public partial class AddTag : MetroWindow, INotifyPropertyChanged
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

        public bool RangeAct { get; set; }
        public int Range { get; set; }

        private string TagName_;

        public string TagName
        {
            get { return TagName_; }
            set
            {
                //Pszeszukanie tagow
                if (Pr.tagsList.Exists(x => x.tagName == value) || Pr.InTagsList.Exists(x => x.tagName == value))
                    throw new ArgumentException("Change name of tag becuse is already exists!");

                TagName_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagName)));
            }
        }

        private BytesOrder BtOrder_;

        public BytesOrder BtOrder
        {
            get { return BtOrder_; }
            set
            {
                BtOrder_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BtOrder)));
            }
        }

        private TypeData TpData_;

        public TypeData TpData
        {
            get { return TpData_; }
            set
            {
                TpData_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TpData)));
                RefreshSecondAdress();
            }
        }

        private MemoryAreaInfo SelArea_;

        public MemoryAreaInfo SelArea
        {
            get { return SelArea_; }
            set
            {
                SelArea_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelArea)));
                RefreshSecondAdress();
            }
        }

        private bool DbBlockAct_;

        public bool DbBlockAct
        {
            get { return DbBlockAct_; }
            set
            {
                DbBlockAct_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbBlockAct)));
            }
        }

        private ObservableCollection<MemoryAreaInfo> MemArList_;

        public ObservableCollection<MemoryAreaInfo> MemArList
        {
            get { return MemArList_; }
            set { MemArList_ = value; }
        }

        private int DbAdress_;

        public int DbAdress
        {
            get { return DbAdress_; }
            set
            {
                DbAdress_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DbAdress)));
            }
        }

        private int Adress_;

        public int Adress
        {
            get { return Adress_; }
            set
            {
                Adress_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Adress)));
            }
        }

        private int SecAdress_;

        public int SecAdress
        {
            get { return SecAdress_; }
            set
            {
                SecAdress_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecAdress)));
            }
        }

        private ObservableCollection<int> ScAdrList_ = new ObservableCollection<int>();

        public ObservableCollection<int> ScAdreList
        {
            get { return ScAdrList_; }
            set { ScAdrList_ = value; }
        }

        private string Desc_;

        public string Desc
        {
            get { return Desc_; }
            set
            {
                Desc_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Desc)));
            }
        }

        /// <summary>
        /// Kontener projektowy
        /// </summary>
        private ProjectContainer PrCon;

        /// <summary>
        /// Project
        /// </summary>
        private Project Pr;

        /// <summary>
        /// Identyfikator projektu
        /// </summary>
        private Guid PrId = Guid.Empty;

        /// <summary>
        /// Identyfikator folderu taga
        /// </summary>
        private Guid DevId = Guid.Empty;

        /// <summary>
        /// Globalna konfoguracja
        /// </summary>
        private GlobalConfiguration gConf = new GlobalConfiguration();

        /// <summary>
        /// Sterownik
        /// </summary>
        private IDriverModel Idrv;

        /// <summary>
        /// Poloczenia
        /// </summary>
        private Connection Con;

        //Ctor
        public AddTag(ref ProjectContainer prCon, Guid prId, Guid devId)
        {
            try
            {
                DataContext = this;
                Range = 2;
                PrCon = prCon;
                PrId = prId;
                Pr = prCon.getProject(prId);
                DevId = devId;
                Device Dev = prCon.getDevice(prId, devId);
                Con = prCon.getConnection(prId, Dev.parentId);
                Idrv = Con.Idrv;

                TpData = TypeData.SHORT;
                MemArList = new ObservableCollection<MemoryAreaInfo>(Idrv.MemoryAreaInf);
                SelArea = Idrv.MemoryAreaInf.First();

                InitializeComponent();

                if (Idrv.driverName == "S7-300-400 Ethernet")
                {
                    DbBlockAct = true;
                    BtOrder = BytesOrder.DCBA;
                }
                else
                {
                    DbBlockAct = false;
                    BtOrder = BytesOrder.BADC;
                }

                string nm = "Tag";
                for (int x = 0; ; x++)
                {
                    if (PrCon.GetAllITags().Exists(k => k.Name == nm))
                        nm = $"{nm}{x}";
                    else
                        break;
                }

                TagName = nm;
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Refresh Second Adress
        private void RefreshSecondAdress()
        {
            try
            {
                int buff = SecAdress;
                ScAdreList.Clear();
                if (TpData == TypeData.BIT)
                {
                    if (SelArea?.AdresSize > 1)
                        //Dodanie elementu do wyboru
                        for (int i = 0; i < SelArea.AdresSize; i++)
                            ScAdreList.Add(i);
                }
                else if (TpData == TypeData.BYTE || TpData == TypeData.SBYTE)
                {
                    if (SelArea?.AdresSize > 8)
                        for (int i = 0; i < SelArea.AdresSize / 8; i++)
                            ScAdreList.Add(i);
                }
                else if (TpData == TypeData.CHAR || TpData == TypeData.SHORT || TpData == TypeData.USHORT)
                {
                    if (SelArea?.AdresSize > 16)
                        for (int i = 0; i < SelArea.AdresSize / 16; i++)
                            ScAdreList.Add(i);
                }
                else if (TpData == TypeData.INT || TpData == TypeData.UINT || TpData == TypeData.FLOAT)
                {
                    if (SelArea?.AdresSize > 32)
                        for (int i = 0; i < SelArea.AdresSize / 32; i++)
                            ScAdreList.Add(i);
                }
                else if (TpData == TypeData.DOUBLE)
                {
                    if (SelArea?.AdresSize > 64)
                        for (int i = 0; i < SelArea.AdresSize / 64; i++)
                            ScAdreList.Add(i);
                }

                if (ScAdreList.Count > 0)
                {
                    if (ScAdreList.ToList().Exists(x => x == buff))
                        SecAdress = buff;
                    else
                        SecAdress = ScAdreList.First();
                }
                else
                    SecAdress = 0;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        //Save
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!RangeAct)
            {
                Tag tg1 = new Tag(TagName, BtOrder, SelArea.Name, Adress, SecAdress, Desc, TpData, Idrv, Con.objId, DbAdress);
                tg1.BlockAdress = DbAdress;
                PrCon.addTag(PrId, DevId, tg1);
            }
            else
            {
                Tag adressTag = null;
                for (int i = 0; i < Range; i++)
                {
                    //Find right name
                    string nm = $"{TagName}{i}";
                    for (int x = 0; ; x++)
                    {
                        if (PrCon.GetAllITags().Exists(k => k.Name == nm))
                            nm = $"{TagName}{x}";
                        else
                            break;
                    }

                    if (adressTag == null) // Pierwszy Tag
                    {
                        Tag tg = new Tag(nm, BtOrder, SelArea.Name, Adress, SecAdress, Desc, TpData, Idrv, Con.objId, DbAdress);
                        tg.BlockAdress = DbAdress;
                        PrCon.addTag(PrId, DevId, tg);
                        adressTag = tg;
                    }
                    else // Nastepny Tag
                    {
                        Tag tg = new Tag(nm, BtOrder, SelArea.Name, adressTag.nextAdress().adress, adressTag.nextAdress().secAdress, Desc, TpData, Idrv, Con.objId, DbAdress);
                        tg.BlockAdress = DbAdress;
                        PrCon.addTag(PrId, DevId, tg);
                        adressTag = tg;
                    }
                }
            }

            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}