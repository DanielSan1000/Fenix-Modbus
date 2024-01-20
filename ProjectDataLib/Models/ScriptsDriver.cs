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

            Timers_ = new List<CustomTimer>();
            Timers_.Add(new CustomTimer());

            Enable_ = true;

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

        private Boolean isLive;

        [field: NonSerialized]
        private List<dynamic> scripts = new List<dynamic>();

        string IDriverModel.driverName
        {
            get { return "Scripts"; }
        }

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

        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            try
            {
                if (!Enable_)
                    return false;

                CSScript.Evaluator.Reset();
                CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(ProjectDataLib.Project)));
                CSScript.Evaluator.ReferenceAssembly(Assembly.GetAssembly(typeof(System.Windows.Forms.MessageBox)));
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;

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

                        CustomTimer ti = Proj_.ScriptEng.Timers.Find(x => x.Name == f.TimerName);
                        BckTimers.Add(new System.Threading.Timer(TimerTask, (object)scripts.Last(), ti.Delay, ti.Time));
                    }
                }

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

        bool IDriverModel.deactivateCycle()
        {
            try
            {
                if (!Enable_)
                    return false;

                foreach (var tm in BckTimers)
                    tm.Dispose();

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

        event EventHandler IDriverModel.refreshedCycle
        {
            add { refreshCycleEv += value; }
            remove { refreshCycleEv -= value; }
        }

        event EventHandler IDriverModel.refreshedPartial
        {
            add { refreshedPartial += value; }
            remove { refreshedPartial -= value; }
        }

        event EventHandler IDriverModel.error
        {
            add { errorSendEv += value; }
            remove { errorSendEv -= value; }
        }

        event EventHandler IDriverModel.information
        {
            add { sendInfoEv += value; }
            remove { sendInfoEv -= value; }
        }

        event EventHandler IDriverModel.dataSent
        {
            add { sendLogInfoEv += value; }
            remove { sendLogInfoEv -= value; }
        }

        event EventHandler IDriverModel.dataRecived
        {
            add { reciveLogInfoEv += value; }
            remove { reciveLogInfoEv -= value; }
        }

        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get { throw new NotImplementedException(); }
        }

        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return string.Empty;
        }

        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return string.Empty;
        }

        bool IDriverModel.isAlive
        {
            get { return isLive; }
            set
            {
                isLive = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("isAlive"));
            }
        }

        bool IDriverModel.isBusy
        {
            get
            {
                return false;
            }
        }

        byte[] IDriverModel.sendBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        object[] IDriverModel.plugins
        {
            get { throw new NotImplementedException(); }
        }

        bool[] IDriverModel.AuxParam
        {
            get { throw new NotImplementedException(); }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            BckTimers = new List<System.Threading.Timer>();
            scripts = new List<dynamic>();
            isLive = false;
        }

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