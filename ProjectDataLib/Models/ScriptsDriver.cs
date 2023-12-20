using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ProjectDataLib
{
    [Serializable]
    public class ScriptsDriver : IDriverModel, ITreeViewModel, INotifyPropertyChanged
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

        private Guid objId_;

        [Browsable(false)]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        [field: NonSerialized]
        private Project Proj_;

        [Browsable(false)]
        [XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set
            {
                Proj_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }

        private Boolean isExpand_;

        [Browsable(false)]
        public Boolean isExpand
        {
            get { return isExpand_; }
            set
            {
                isExpand_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("isExpand"));
            }
        }

        private List<CustomTimer> Timers_;

        [Category("01 Design"), DisplayName("Timers")]
        [TypeConverter(typeof(EmptyConverter))]
        public List<CustomTimer> Timers
        {
            get { return Timers_; }
            set
            {
                Timers_ = value;
            }
        }

        private Boolean Enable_;

        [Browsable(false)]
        public Boolean Enable
        {
            get { return Enable_; }
            set
            {
                Enable_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Enable"));
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

                if (_Children != null)
                    foreach (ITreeViewModel itv in _Children)
                        itv.IsBlocked = value;
            }
        }

        public ScriptsDriver(Project pr)
        {
            this.Proj_ = pr;
            this.objId_ = new Guid("33333333-3333-3333-3333-333333333333");

            //Domysly Timer
            Timers_ = new List<CustomTimer>();
            Timers_.Add(new CustomTimer());

            Enable_ = true;

            //Referencje
            CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(ProjectDataLib.Project)));
            CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(System.Windows.Forms.MessageBox)));
        }

        public ScriptsDriver()
        {
        }

        [field: NonSerialized]
        private List<InTag> TagList_;

        [Browsable(false)]
        [XmlIgnore]
        public List<InTag> TagList
        {
            get { return TagList_; }
            set { TagList_ = value; }
        }

        [field: NonSerialized]
        private List<System.Threading.Timer> BckTimers = new List<System.Threading.Timer>();

        [field: NonSerialized]
        private EventHandler sendInfoEv;

        [field: NonSerialized]
        private EventHandler errorSendEv;

        [field: NonSerialized]
        private EventHandler refreshedPartial;

        [field: NonSerialized]
        private EventHandler refreshCycleEv;

        [field: NonSerialized]
        private EventHandler sendLogInfoEv;

        [field: NonSerialized]
        private EventHandler reciveLogInfoEv;

        //Informacja o aktywnosci odczytu
        private Boolean isLive;

        //Script
        [field: NonSerialized]
        private List<dynamic> scripts = new List<dynamic>();

        /// <summary>
        /// Nazwa Sterownika
        /// </summary>
        string IDriverModel.driverName
        {
            get { return "Scripts"; }
        }

        /// <summary>
        /// Parametry Steronika
        /// </summary>
        object IDriverModel.setDriverParam
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Aktywacja sterownika
        /// </summary>
        /// <param name="tagsList"></param>
        /// <returns></returns>
        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            try
            {
                //Wylaczenie danych
                if (!Enable_)
                    return false;

                //Ustawienia silnika
                CSScript.Evaluator.Reset();
                CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(ProjectDataLib.Project)));
                CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(System.Windows.Forms.MessageBox)));
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;

                //Script inicjalizacja
                scripts.Clear();
                BckTimers.Clear();
                foreach (ScriptFile f in Proj_.ScriptFileList)
                {
                    if (f.Enable && !string.IsNullOrEmpty(f.TimerName))
                    {
                        string code = File.ReadAllText(f.FilePath);
                        scripts.Add(CSScript.Evaluator.LoadCode(code));
                        scripts.Last().Init(Proj_, f.Name);
                        scripts.Last().Start();

                        //Timery
                        CustomTimer ti = Proj_.ScriptEng.Timers.Find(x => x.Name == f.TimerName);
                        BckTimers.Add(new System.Threading.Timer(TimerTask, (object)scripts.Last(), ti.Delay, ti.Time));
                    }
                }

                //Znacznik komunikacji
                isLive = true;
                return true;
            }
            catch (Exception Ex)
            {
                if (sendInfoEv != null)
                    sendInfoEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "ScriptDriver.activateCycle: " + Ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Dodanie tagow do komunikacji podczas pracy sterownika
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            try
            {
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message, Ex));

                return false;
            }
        }

        /// <summary>
        /// Usuniecie tagow z komunikacji podczas dzialania sterownika
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        bool IDriverModel.removeTagsComm(List<ITag> tagList)
        {
            try
            {
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message, Ex));

                return false;
            }
        }

        /// <summary>
        /// Rekonfiguruj dane
        /// </summary>
        /// <returns></returns>
        bool IDriverModel.reConfig()
        {
            try
            {
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message, Ex));

                return false;
            }
        }

        /// <summary>
        /// Zatrzymaj komunikacji
        /// </summary>
        /// <returns></returns>
        bool IDriverModel.deactivateCycle()
        {
            try
            {
                //Wylaczenie danych
                if (!Enable_)
                    return false;

                //Zablokowanie
                foreach (var tm in BckTimers)
                    tm.Dispose();

                //Script
                foreach (dynamic sc in scripts)
                {
                    sc.Stop();
                }

                BckTimers.Clear();
                CSScript.Evaluator.Reset();
                isLive = false;
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message, Ex));

                return false;
            }
        }

        /// <summary>
        /// Obieg timerow
        /// </summary>
        /// <param name="StateObj"></param>
        private void TimerTask(object StateObj)
        {
            try
            {
                ((dynamic)StateObj).Cycle();
                refreshedPartial?.Invoke(this, new ProjectEventArgs(StateObj));
                refreshCycleEv?.Invoke(this, new ProjectEventArgs(StateObj));
            }
            catch (Exception Ex)
            {
                if (sendInfoEv != null)
                    sendInfoEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "ScriptDriver.TimerTask: " + Ex.Message));
            }
        }

        /// <summary>
        /// Informacja o zakonczeniu cyklu komunikacyjnego
        /// </summary>
        event EventHandler IDriverModel.refreshedCycle
        {
            add { refreshCycleEv += value; }
            remove { refreshCycleEv -= value; }
        }

        /// <summary>
        /// Informacja o zakonczeniu jakiejs czesci cyklu podstawowego
        /// </summary>
        event EventHandler IDriverModel.refreshedPartial
        {
            add { refreshedPartial += value; }
            remove { refreshedPartial -= value; }
        }

        /// <summary>
        /// Informcja o bledzie
        /// </summary>
        event EventHandler IDriverModel.error
        {
            add { errorSendEv += value; }
            remove { errorSendEv -= value; }
        }

        /// <summary>
        /// Informacja ogolna
        /// </summary>
        event EventHandler IDriverModel.information
        {
            add { sendInfoEv += value; }
            remove { sendInfoEv -= value; }
        }

        /// <summary>
        /// Informacja o wyslaniu danych
        /// </summary>
        event EventHandler IDriverModel.dataSent
        {
            add { sendLogInfoEv += value; }
            remove { sendLogInfoEv -= value; }
        }

        /// <summary>
        /// Informacja o odebraniu danych
        /// </summary>
        event EventHandler IDriverModel.dataRecived
        {
            add { reciveLogInfoEv += value; }
            remove { reciveLogInfoEv -= value; }
        }

        /// <summary>
        /// Odstepne obszary komunikacji dla sterownika
        /// </summary>
        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Formatowanie logu
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return string.Empty;
        }

        /// <summary>
        /// Formatowanie logu
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return string.Empty;
        }

        /// <summary>
        /// Zwraca czy komunikacja trwa
        /// </summary>
        bool IDriverModel.isAlive
        {
            get { return isLive; }
            set
            {
                isLive = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("isAlive"));
            }
        }

        /// <summary>
        /// Zwraca czy komunikacja zakonczyla sie
        /// </summary>
        bool IDriverModel.isBusy
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Stara zaszlosc
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] IDriverModel.sendBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// W Przeszlosci mialy to byc plaginy
        /// </summary>
        object[] IDriverModel.plugins
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Rozne informacje dla edytorow
        /// </summary>
        bool[] IDriverModel.AuxParam
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Desarializacja obiektu
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            BckTimers = new List<System.Threading.Timer>();
            scripts = new List<dynamic>();
            isLive = false;
        }

        /// <summary>
        /// Nowy Interfejsc
        /// </summary>
        Guid IDriverModel.ObjId
        {
            get
            {
                return objId_;
            }
        }

        [field: NonSerialized]
        private ObservableCollection<object> _Children;

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return _Children;
            }
            set
            {
                _Children = value;
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return "Scripts";
            }

            set
            {
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return isExpand;
            }

            set
            {
                isExpand = value;
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
    }
}