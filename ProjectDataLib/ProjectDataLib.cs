using Controls;
using CSScriptLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProjectDataLib
{
    /// <summary>
    /// Interfejs modelu sterownika
    /// </summary>
    public interface IDriverModel
    {
        /// <summary>
        /// Unikatowy identyfikator
        /// </summary>
        Guid ObjId { get; }

        /// <summary>
        /// Nazwa sterownika
        /// </summary>
        String driverName { get; }

        /// <summary>
        /// Konfiguracja parametrow transmisji
        /// </summary>
        Object setDriverParam { get; set; }

        /// <summary>
        /// Cykliczna metoda wymiany informacji
        /// </summary>
        /// <param name="tagsList"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Boolean activateCycle(List<ITag> tagsList);

        /// <summary>
        /// Dodanie tagow do komunikacji
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean addTagsComm(List<ITag> tagList);

        /// <summary>
        /// Odjecie tagow od komunikacji
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean removeTagsComm(List<ITag> tagList);

        /// <summary>
        /// Reconfiguruj dane. Zmiane zakre zapytania w w przypadku zmiany tagów
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean reConfig();

        /// <summary>
        /// Zatrzymanie cyklicznej komunikacji
        /// </summary>
        /// <returns></returns>
        Boolean deactivateCycle();

        /// <summary>
        /// Dane zostaly odswierzone
        /// </summary>
        event EventHandler refreshedCycle;

        /// <summary>
        /// Odswierzenie pewnego obszaru pamieci
        /// </summary>
        event EventHandler refreshedPartial;

        /// <summary>
        /// Informacja o bledzie
        /// </summary>
        event EventHandler error;

        /// <summary>
        /// Informacja o zdarzeniach
        /// </summary>
        event EventHandler information;

        /// <summary>
        /// Dane wyslane ze sterownika
        /// </summary>
        event EventHandler dataSent;

        /// <summary>
        /// Dane dostarczone ze sterownika
        /// </summary>
        event EventHandler dataRecived;

        /// <summary>
        /// Informacja o obszarach
        /// </summary>
        MemoryAreaInfo[] MemoryAreaInf{get;}

        /// <summary>
        /// Formatowanie ramki w zaleznosci od protokolu
        /// </summary>
        String FormatFrameRequest ( Byte[] frame, NumberStyles num);

        /// <summary>
        /// Formatowanie ramki w zaleznosci od protokolu
        /// </summary>
        String FormatFrameResponse(Byte[] frame, NumberStyles num);

        /// <summary>
        /// Sterownik dziala
        /// </summary>
        Boolean isAlive { get; set; }

        /// <summary>
        /// Sterownik pracuje
        /// </summary>
        Boolean isBusy { get; }

        /// <summary>
        /// Zapyanie jednostkowe w postacji bajtów z weryfikacją bladów
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] sendBytes(byte[] data);

        /// <summary>
        /// Plaginy dla Modbusa
        /// </summary>
        Object[] plugins { get; }

        /// <summary>
        /// [0] Wlacza lub wylacza dodatkowy adress dla urzadzenia
        /// </summary>
        Boolean[] AuxParam { get; }
    }

    /// <summary>
    /// Klasa dostepna dla scryptow
    /// </summary>
    public class ScriptModel
    {
        /// <summary>
        /// Project
        /// </summary>
        private Project Pr { get; set; }

        private String FileName { get; set; }

        /// <summary>
        /// Kontener projektowy
        /// </summary>
        private ProjectContainer PrCon { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="?"></param>
        public ScriptModel()
        {
            
        }

        /// <summary>
        /// Inicjalizacja
        /// </summary>
        /// <param name="pr"></param>
        public void Init(Project pr, String name)
        {
            this.Pr = pr;
            this.PrCon = pr.PrCon;
            this.FileName = name;
        }

        /// <summary>
        /// Start Skrypt
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// Stop skryptu
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// Wykonaywany cyklicznie
        /// </summary>
        public virtual void Cycle()
        {

        }

        /// <summary>
        /// Ustawia wartość taga
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetTag(string name, object value)
        {
            Pr.SetTag(name, value);
        }

        /// <summary>
        /// Zapis w oknie diagnostycznym
        /// </summary>
        /// <param name="s"></param>
        public void Write(object s)
        {
            Pr.Write(this,Convert.ToString(s));
        }

        /// <summary>
        /// Pobiera wartosc taga
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object GetTag(string name)
        {
            return Pr.GetTag(name);
        }

        public override string ToString()
        {
            return "Script: "+ FileName;
        }
    }

    /// <summary>
    /// Kontener projektów obslugujący zarzadanie grupami projektów
    /// </summary>
    [Synchronization]
    public class ProjectContainer : ITreeViewModel
    {
        #region Fields

        /// <summary>
        /// Project
        /// </summary>
        public List<Project> projectList;

        /// <summary>
        /// Globalna konfiguracja Sterownikowa
        /// </summary>
        public GlobalConfiguration gConf;

        /// <summary>
        /// Zarzadznie oknami
        /// </summary>
        public List<WindowsStatus> winManagment;

        /// <summary>
        /// Symbol Serwera
        /// </summary>
        public Guid ServerGuid = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Internals tag
        /// </summary>
        public Guid IntTagsGuid = new Guid("22222222-2222-2222-2222-222222222222");

        /// <summary>
        /// Guid dla skryptow
        /// </summary>
        public Guid ScriptGuid = new Guid("33333333-3333-3333-3333-333333333333");
        
        //Plik
        public Guid HttpFileGuid = new Guid("44444444-4444-4444-4444-444444444444");

        /// <summary>
        /// Podkatalog dla serwera http
        /// </summary>
        public string HttpCatalog = "\\Http";

        /// <summary>
        /// Podkatalog dla serwera Script
        /// </summary>
        public string ScriptsCatalog = "\\Scripts";

        /// <summary>
        /// Podkatalog dla serwera http
        /// </summary>
        public string TemplateCatalog = "\\Template";

        /// <summary>
        /// Path do bazy danych
        /// </summary>
        public string Database = "\\Database\\ProjDatabase.sqlite";

        /// <summary>
        /// Registry Path
        /// </summary>
        public string RegUserRoot = "HKEY_CURRENT_USER\\Software\\Fenix";
        public string LastPathKey = "LastPath";

        /// <summary>
        /// Task Scheduler Wraper
        /// </summary>
        public string TaskName = "FenixServer";

        public string HelpWebSite = "http://sites.google.com/site/fenmods7/";

        //Zapisanie layoutu
        public string LayoutFile = "Layout_.xml";

        /// <summary>
        /// Bledy wszystkich okienek
        /// </summary>
        public EventHandler ApplicationError;

        //Bufor kopiowanie
        public Guid SrcProject;
        public Guid SrcElement;
        public ElementKind SrcType = ElementKind.Empty;
        public Boolean cutMarks;

        //Dodatkowe zdarzenie
        public EventHandler<ProjectEventArgs> addProjectEv;
        public EventHandler<ProjectEventArgs> pasteProjectEv;
        public EventHandler<ProjectEventArgs> clearProjectsEv;
        public EventHandler<ProjectEventArgs> saveProjectEv;
  
        public EventHandler<ProjectEventArgs> addConnEv;
        public EventHandler<ProjectEventArgs> removeConnEv;
        public EventHandler<ProjectEventArgs> pasteConnEv;

        public EventHandler<ProjectEventArgs> addDeviceEv;
        public EventHandler<ProjectEventArgs> removeDeviceEv;
        public EventHandler<ProjectEventArgs> pasteDeviceEv;

        public EventHandler<ProjectEventArgs> addTagEv;
        public EventHandler<ProjectEventArgs> removeTagEv;
        public EventHandler<ProjectEventArgs> editTagEv;
        public EventHandler<ProjectEventArgs> pasteTagEv;

        public EventHandler<ProjectEventArgs> addIntTagEv;
        public EventHandler<ProjectEventArgs> removeIntTagEv;
        public EventHandler<ProjectEventArgs> editIntTagEv;
        public EventHandler<ProjectEventArgs> pasteIntTagEv;


        public EventHandler<ProjectEventArgs> addInFileEv;
        public EventHandler<ProjectEventArgs> removeInFileEv;

        public EventHandler<ProjectEventArgs> addScriptFileEv;
        public EventHandler<ProjectEventArgs> removeScriptFileEv;

        ObservableCollection<object> _Children;
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
                return ToString();
            }

            set
            {
                
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return true;
            }

            set
            {
                
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        bool ITreeViewModel.IsBlocked
        {
            get
            {
                return false;
            }

            set { }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        #endregion

        #region Konstruktor

        public ProjectContainer()
        {
            try
            {
                winManagment = new List<WindowsStatus>() { new WindowsStatus(0, false, false) };
                projectList = new List<Project>();
                gConf = new GlobalConfiguration();
                _Children = new ObservableCollection<object>();
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }

        }

        #endregion

        #region Project

        /// <summary>
        /// Dodaj Projekt do kontenera
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Guid addProject(Project project)
        {
            try
            {
                //Dodanie projektu
                projectList.Add(project);
                _Children.Add(project);

                //wyslanie zdarzenia
                if (addProjectEv != null)
                    addProjectEv(project, new ProjectEventArgs(project));
                              
                return project.objId;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Pobranie projektu na podstawie Id
        /// </summary>
        /// <param name="Id">Identyfikator Projektu</param>
        /// <returns>Project</returns>
        public Project getProject(Guid Id)
        {
            try
            {
                //Pobranie projektu
                Project pr = projectList.Find(x => x.objId.Equals(Id));

                //Sprawdzenie projektu
                if (pr == null)
                    throw new ApplicationException(String.Format("Guid: {0} isn't sign project",Id));

                return pr;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Otworz dane
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool openProjects(string path)
        {
            try
            {
                //zabezpieczenie przed pustym lancuchem
                if (!File.Exists(path))
                    return false;

                Project buff = null;
                Boolean IsXml = false;

                #region PSF (BINARY)

               
                if(Path.GetExtension(path).Contains("psf"))
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    FileStream fileStr = new FileStream(path, FileMode.OpenOrCreate);
                    buff = (Project)binForm.Deserialize(fileStr);
                    fileStr.Close();
                    path = Path.ChangeExtension(path, "psx");
                }

                #endregion

                #region PSX (XML)

                else if (Path.GetExtension(path).Contains("psx"))
                {
                    XmlSerializer xmlSer = new XmlSerializer(typeof(Project));
                    using (Stream fsream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        buff = (Project)xmlSer.Deserialize(fsream);                   
                    }
                
                    IsXml = true;
                }

                #endregion
      
                //Przypisanie sciezki dostępu
                buff.path = path;
                buff.PrCon = this;

                if (IsXml)
                {
                    buff.OnDeserializedXML();
                    buff.ChartConf.OnDeserializedXML();
                }

                //sprawdzenie czy projekt jest juz w buforze
                foreach (Project pr in projectList)
                 if (pr.objId.Equals(buff.objId))
                    throw new ApplicationException(pr.projectName +": " + "Project is alredy open.");

                //Doloczenie danych
                foreach (InTag tg in buff.InTagsList)
                {
                    tg.Proj = buff;
                    tg.PrCon = this;
                    tg.idrv = buff.InternalTagsDrv;
                }

                //Dodanie danych na temet projektu i buffora
                buff.ScriptEng.Proj = buff;

                List<string> deleteFiles = new List<string>();
                foreach (ScriptFile scf in buff.ScriptFileList)
                {
                    //Sciezka
                    scf.FilePath = Path.GetDirectoryName(buff.path) + this.ScriptsCatalog + "\\" + scf.Name;

                    //Sprawdzanie czy plik istnieje
                    if (!File.Exists(scf.FilePath))
                        deleteFiles.Add(scf.FilePath);

                    scf.Proj = buff;
                    scf.PrCon = this;
                }

                //Usuniecie nieaktywnych plikow
                foreach (string gp in deleteFiles)
                {
                    ScriptFile fb = buff.ScriptFileList.Where(x => x.FilePath == gp).First();

                    if (fb != null)
                        ((ITreeViewModel)buff.ScriptEng).Children.Remove(fb);

                    buff.ScriptFileList.RemoveAll(x => x.FilePath == gp);
                    
                }

                //wyszyszczenie listy
                deleteFiles.Clear();

                //Przepisanie sciezek
                foreach (InFile fil in buff.FileList)
                {
                    //Sciezka
                    fil.FilePath = Path.GetDirectoryName(buff.path) + this.HttpCatalog+ "\\" + fil.Name;

                    //Sprawdzanie czy plik istnieje
                    if (!File.Exists(fil.FilePath))
                        deleteFiles.Add(fil.FilePath);
                }

                //Usuniecie nieaktywnych plikow
                foreach (string gp in deleteFiles)
                    buff.FileList.RemoveAll(x => x.FilePath == gp);
                       
                //Dodanie wszystkich wartości nieserializowanych
                foreach (Connection cn in buff.connectionList)
                {
                   if(IsXml)
                    cn.OnDeserializedXML();

                    //Konfigurator Globalny
                    cn.gConf = gConf;

                    cn.PrCon = this;

                    //Dodanie Sterownika/ Blokada przez wyjątkiem
                    cn.Idrv = gConf?.newDrv(cn.DriverName);

                    //Jezeli biblioteka nie jest zainstalowana w systemie to TRUE
                    cn.assemblyError = cn.Idrv == null ? true : false;

                    //Dorzucenie IDRV do Tagów
                    foreach (Tag tg in buff.tagsList)
                    {
                        //Jezeli poloczenie nalezy do Taga dodaj sterownik
                        if (tg.connId.Equals(cn.objId))
                            tg.idrv = cn.Idrv;

                        //referencji do Kontenera
                        tg.PrCon = this;
                        tg.Proj = buff;

                        if (IsXml)
                            tg.OnDeserializedXml();
                    }
                }

                //Dodanie
                ((IDriversMagazine)buff).Children = new ObservableCollection<IDriverModel>();
                ((IDriversMagazine)buff).Children.Add(buff.InternalTagsDrv);
                ((IDriversMagazine)buff).Children.Add(buff.ScriptEng);

                foreach (Connection cn in buff.connectionList)
                    ((IDriversMagazine)buff).Children.Add(cn.Idrv);


                foreach (Connection cx in buff.connectionList)
                {
                    ((IDriversMagazine)cx).Children = new ObservableCollection<IDriverModel>();
                    ((IDriversMagazine)cx).Children.Add(buff.InternalTagsDrv);
                    ((IDriversMagazine)cx).Children.Add(buff.ScriptEng);
                    ((IDriversMagazine)cx).Children.Add(cx.Idrv);
                }
         
                foreach (Device dx in buff.DevicesList)
                {
                    dx.idrv = (from cn in buff.connectionList where cn.objId == dx.parentId select cn.Idrv).First();                         
                    ((IDriversMagazine)dx).Children = new ObservableCollection<IDriverModel>();
                    ((IDriversMagazine)dx).Children.Add(buff.InternalTagsDrv);
                    ((IDriversMagazine)dx).Children.Add(buff.ScriptEng);
                    ((IDriversMagazine)dx).Children.Add(dx.idrv);
                }

                //Dodanie projektu
                buff.Db.Pr = buff;
                buff.Db.PrCon = this;
                buff.Db.OnDeserializedXML();
                addProject(buff);

                //Dodanie zdarzen do devices
                foreach (Device dev in buff.DevicesList)
                {
                    //Dodanie zdarzen
                    dev.adressChanged += new EventHandler(DevicesAdressChange);

                    //Dodnie kontenera
                    dev.PrCon = this;
                }

                buff.WebServer1.PrCon = this;
                buff.WebServer1.Proj = buff;


                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Zapisz pojedynczy project
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="sfDialog"></param>
        /// <returns></returns>
        public Boolean saveProject(Project proj, string path)
        {

            try
            {
                proj.modMarks = false;
                proj.modifeTime = DateTime.Now;

                XmlSerializer xmlSer = new XmlSerializer(typeof(Project));
                using (Stream fsream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    xmlSer.Serialize(fsream, proj);
                }

                saveProjectEv?.Invoke(proj, new ProjectEventArgs(proj));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        public Boolean closeAllProject(bool blockSaving)
        {
            try
            {

                //Czyszczenie pamieci
                foreach (Project proj in projectList)
                {
                    //Devices
                    foreach (Device dev in proj.DevicesList)
                        ((ITreeViewModel)dev).Children.Clear();

                    //Connection
                    foreach (Connection conn in proj.connectionList)
                        ((ITreeViewModel)conn).Children.Clear();

                    //Scripts              
                    ((ITreeViewModel)proj.ScriptEng).Children.Clear();

                    //HttpServer
                    ((ITreeViewModel)proj.WebServer1).Children.Clear();

                    //Projct
                    ((ITreeViewModel)proj).Children.Clear();

                    proj.Dispose();
                }

                //Projecty zostały zapisane
                ((ITreeViewModel)this).Children.Clear();
                projectList.Clear();

                //wyslanie zdarzenia
                clearProjectsEv?.Invoke(this, new ProjectEventArgs(null));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }

        }

        /// <summary>
        /// Kopiuj element
        /// </summary>
        /// <param name="idEl"></param>
        /// <param name="elKind"></param>
        /// <returns></returns>
        public Boolean copyCutElement(Guid projId, Guid idEl, ElementKind elKind, Boolean IsCut)
        {
            try
            {
                //Zroda wyciecia
                SrcProject = projId;
                SrcElement = idEl;
                cutMarks = IsCut;
                SrcType = elKind;
                return true;

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Wklej z kopiowany element
        /// </summary>
        /// <returns></returns>
        public Boolean pasteElement(Guid TarProj, Guid TarElement)
        {
            try
            {
                switch (SrcType)
                {

                    #region Project
                    case ElementKind.Project:
                        throw new ApplicationException("Operation is not allowed!");
                    #endregion

                    #region Connection
                    case ElementKind.Connection:

                        //Pobranie taga
                        Connection Cnn0 = getConnection(SrcProject, SrcElement);

                        //Dodanie nowego taga
                        Connection Cnn1 = (Connection)Cnn0.Clone();
                        addConnection(TarProj, Cnn1);

                        //Pobranie wszystkich Urzadzen
                        List<Device> Dvs0 = getProject(SrcProject).DevicesList.Where(x => x.parentId == Cnn0.objId).ToList();
                        foreach (Device Dv0 in Dvs0)
                        {
                            //Dodanie urzadzenia
                            Device Dv1 = (Device)Dv0.Clone();
                            addDevice(TarProj, Cnn1.objId, Dv1);

                            //Pobranie wsztstkich tagw
                            List<Tag> Tgs0 = getProject(SrcProject).tagsList.Where(x => x.parentId == Dv0.objId).ToList();
                            foreach (Tag Tg0 in Tgs0)
                            {
                                Tag Tg1 = (Tag)Tg0.Clone();
                                addTag(TarProj, Dv1.objId, Tg1);
                            }
                        }

                        if (pasteConnEv != null)
                            pasteConnEv(getProject(TarProj), new ProjectEventArgs(Cnn1));

                        if(cutMarks)
                            removeConnection(SrcProject, SrcElement);

                        break;
                    #endregion

                    #region Device
                    case ElementKind.Device:

                        //Pobranie zrodla
                        Device Dvv0 = getDevice(SrcProject, SrcElement);

                        //Dodanie urzadzenia
                        Device Dvv1 = (Device)Dvv0.Clone();
                        addDevice(TarProj, TarElement, Dvv1);

                        //Peta
                        List<Tag> Tgs0_ = getProject(SrcProject).tagsList.Where(x => x.parentId == Dvv0.objId).ToList();
                        foreach (Tag Tg0 in Tgs0_)
                        {
                            Tag Tg1 = (Tag)Tg0.Clone();
                            addTag(TarProj, Dvv1.objId, Tg1);
                        }

                        if (pasteDeviceEv != null)
                            pasteDeviceEv(this, new ProjectEventArgs(Dvv1));

                        if (cutMarks)
                            removeDevice(SrcProject, SrcElement);

                        break;
                    #endregion

                    #region Tag
                    case ElementKind.Tag:

                        Tag Tgg0 = (Tag)getTag(SrcProject, SrcElement);
                        Tag Tgg1 = (Tag)Tgg0.Clone();


                        //Sprawdzenie czy tag nie pochodzi z innego sterownika
                        Device devTar = (Device)getElementById(TarProj, TarElement);
                        IDriverModel idvTar = getConnection(TarProj, devTar.parentId).Idrv;

                        if (Tgg0.idrv.MemoryAreaInf[0].Name != idvTar.MemoryAreaInf[0].Name)
                        {
                            MessageBox.Show("You cant copy Tags between different drivers!!!");
                            return false;
                        }

                        addTag(TarProj, TarElement, Tgg1);

                        if (pasteTagEv != null)
                            pasteTagEv(this, new ProjectEventArgs(Tgg1));

                        if (cutMarks)
                            removeTag(SrcProject, SrcElement);

                        break;
                    #endregion
                }

                cutMarks = false;
                SrcProject = Guid.Empty;
                SrcElement = Guid.Empty;
                SrcType = ElementKind.Empty;

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Usuń elementy
        /// </summary>
        /// <param name="idEl"></param>
        /// <param name="elKind"></param>
        /// <returns></returns>
        public Boolean deleteElement(Guid projId, Guid idEl, ElementKind elKind)
        {
            try
            {
                if (elKind == ElementKind.Project)
                {
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.InFile)
                {
                    RemoveInFile(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.ScriptFile)
                {
                    RemoveScriptFile(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.Connection)
                {
                    //Usuniecie elementu
                    removeConnection(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.Device)
                {
                    removeDevice(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.Tag)
                {
                    //Usuniecie Taga
                    removeTag(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                if (elKind == ElementKind.IntTag)
                {
                    RemoveIntTag(projId, idEl);
                    cutMarks = false;
                    return true;
                }

                return false;
            }
            catch (Exception Ex)
            {
                cutMarks = false;
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }


        }

        /// <summary>
        /// Pobranie elementu na podstawie id
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object getElementById(Guid proj, Guid name)
        {
            try
            {
                //Pobranie projektu
                Project pr = getProject(proj);

                //Object 
                Object obj = null;

                //Project
                if (proj == name)
                    return pr;

                //Serwer
                if (name == ServerGuid)
                    return pr.WebServer1;

                if (name == ScriptGuid)
                    return pr.ScriptEng;

                //File
                obj = pr.FileList.Find(x => x.objId == name);
                if (obj != null)
                    return obj;

                //FileScript
                obj = pr.ScriptFileList.Find(x => x.objId == name);
                if (obj != null)
                    return obj;

                //Internals Tag
                if (name == IntTagsGuid)
                    return pr.InternalTagsDrv;

                //InTag
                obj = pr.InTagsList.Find(x => x.objId.Equals(name));
                if (obj != null)
                    return obj;

                //Connection
                obj = pr.connectionList.Find(x => x.objId.Equals(name));
                if (obj != null)
                    return obj;

                //Device
                obj = pr.DevicesList.Find(x => x.objId.Equals(name));
                if (obj != null)
                    return obj;

                //Tags
                obj = pr.tagsList.Find(x => x.objId.Equals(name));
                if (obj != null)
                    return obj;

                return null;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Preszukuje wszystkie projekty w poszukiwanie elementow
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Object getElementById(Guid name)
        {
            try
            {
                //Przeszukiwnie projektów
                foreach (Project pr in projectList)
                {
                    //Object 
                    Object obj = null;

                    //Project
                    if (pr.objId == name)
                        return pr;

                    //Serwer
                    if (name == ServerGuid)
                        return pr.WebServer1;

                    if (name == ScriptGuid)
                        return pr.ScriptEng;

                    //File
                    obj = pr.FileList.Find(x => x.objId == name);
                    if (obj != null)
                        return obj;

                    //FileScript
                    obj = pr.ScriptFileList.Find(x => x.objId == name);
                    if (obj != null)
                        return obj;

                    //Internals Tag
                    if (name == IntTagsGuid)
                        return pr.InternalTagsDrv;

                    //InTag
                    obj = pr.InTagsList.Find(x => x.objId.Equals(name));
                    if (obj != null)
                        return obj;

                    //Connection
                    obj = pr.connectionList.Find(x => x.objId.Equals(name));
                    if (obj != null)
                        return obj;

                    //Device
                    obj = pr.DevicesList.Find(x => x.objId.Equals(name));
                    if (obj != null)
                        return obj;

                    //Tags
                    obj = pr.tagsList.Find(x => x.objId.Equals(name));
                    if (obj != null)
                        return obj;
                }

                //Jesli aplikacja doszla tutaj to znaczy że nic nie zostało znalezione
                return null;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Pobiera wszystkie sterowniki z bierzacej pozycji bez InternalTagsDriver i ScriptDriver
        /// </summary>
        /// <param name="Pr_"></param>
        /// <param name="Obj_"></param>
        /// <returns></returns>
        public List<IDriverModel> GetAllDriversForSel(Guid Pr_, Guid Obj_)
        {
            try
            {
                //Bufor na tagi
                List<IDriverModel> IDriverList = new List<IDriverModel>();

                //Sprawdz co zostalo wybrane
                Type SelectedObject = getElementById(Pr_, Obj_) == null ? null : getElementById(Pr_, Obj_).GetType();

                //Pobranie projektu
                Project Pr = getProject(Pr_);

                //Selekcja tego co zostalo wybrane
                if (SelectedObject == typeof(Project))
                {
                    IDriverList.AddRange((from dr in Pr.connectionList select dr.Idrv).ToList());
                }
                else if (SelectedObject == typeof(Connection))
                {
                    IDriverList.Add(getConnection(Pr_, Obj_).Idrv);
                }
                else if (SelectedObject == typeof(Device))
                {
                    IDriverList.Add(getConnection(Pr_, getDevice(Pr_, Obj_).parentId).Idrv);
                }
                else if (SelectedObject == typeof(Tag))
                {
                    IDriverList.Add(getTag(Pr_, Obj_).idrv);
                }
      
               return IDriverList;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Sprawdznie czy trwa jakokolwiek komunikacja
        /// </summary>
        /// <returns></returns>
        public Boolean anyCommunication()
        {
            try
            {
                //Pobranie aktywnych okien
                foreach (Project pr in this.projectList)
                {
                    //Obieg dla poloczen
                    foreach (Connection cn in pr.connectionList)
                    {
                        if (cn.Idrv != null)
                            if (cn.Idrv.isAlive)
                                return true;
                    }

                    //Komunikacja dla internals tag trwa
                    if (((IDriverModel)pr.InternalTagsDrv).isAlive)
                        return true;

                    //Komunikacja dla internals tag trwa
                    if (((IDriverModel)pr.ScriptEng).isAlive)
                        return true;


                }
                return false;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Funkcja zwaraca informacje czy jakieś okno zostało aktywowane
        /// </summary>
        /// <returns></returns>
        public Boolean anyWindowCommunication(int i)
        {
            try
            {
                WindowsStatus[] stat = winManagment.Where(x => x.Live && x.index != i).ToArray();
                if (stat.Length > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Zmieniono adres w device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DevicesAdressChange(Object sender, EventArgs e)
        {
            //Urządzenie
            try
            {
                //urzedzenie
                Device dev = (Device)sender;

                //Tagi z folderu
                List<Tag> tags = getAllTagsFromDevice(dev.projId, dev.objId);

                //Zmiana parametru
                if (tags.Count > 0)
                {
                    foreach (Tag tg in tags)
                        tg.deviceAdress = dev.adress;
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.DevicesAdressChange :" + Ex.Message);
            }
        }

        #endregion

        #region Connection

        /// <summary>
        /// Dodanie nowego polączenia do Projektu
        /// </summary>
        /// <param name="projId">Id 'long' projektu</param>
        /// <param name="con">Poloczenie do dodania</param>
        /// <returns></returns>
        public Guid addConnection(Guid projId, Connection con)
        {
            try
            {
                //Dodanie parentId
                con.parentId = projId;

                //Projekt
                Project pr = getProject(projId);

                //Pobranie poloczenia
                pr.connectionList.Add(con);
                ((ITreeViewModel)pr).Children.Add(con);

                //Sterownik
                if(con.Idrv == null)
                    con.Idrv = gConf.newDrv(con.DriverName);

                //Dodanie 
                con.gConf = gConf;

                //Dodanie driverów
                ((IDriversMagazine)pr).Children.Add(con.Idrv);
                ((IDriversMagazine)con).Children.Add(con.Idrv);

                //Nie ma takiej samej nazwy poloczenia
                if (addConnEv != null)
                    addConnEv(pr, new ProjectEventArgs(con));

                return con.objId;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Usuniecie poloczenia z proejktu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean removeConnection(Guid proj, Guid id)
        {
            try
            {
                //Pobranie projektu
                Project pr = getProject(proj);

                //Pobranie poloczenia
                Connection cn = getConnection(proj, id);

                //Usuniecie Devices
                List<Device> devces = (from d in pr.DevicesList where d.parentId == cn.objId select d).ToList();
                foreach (Device dev in devces)
                    removeDevice(proj, dev.objId);
                    
                //Usuniecie poloczenia
                pr.connectionList.Remove(cn);
                ((ITreeViewModel)pr).Children.Remove(cn);
                ((IDriversMagazine)pr).Children.Remove(cn.Idrv);

                ((ITableView)cn).Children.Clear();
                ((IDriversMagazine)cn).Children.Clear();

                //Zdarzenia
                if (removeConnEv != null)
                    removeConnEv(pr, new ProjectEventArgs(cn));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;

            }
        }

        /// <summary>
        /// Pobierz poloczenie na podtsawie id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Connection getConnection(Guid proj, Guid cn)
        {
            try
            {
                //Znajdz Projekt
                Project pr = getProject(proj);

                //Pobierz Connection
                Connection cnn = pr.connectionList.Find(x => x.objId.Equals(cn));

                //Zwroc poloczenie
                return cnn;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
            
        }

        /// <summary>
        /// Pobiersz wszystkie tagi z poloczenia
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public List<Tag> getAllTagsFromConnection(Guid proj, Guid con)
        {
            try
            {
                //Project
                Project pr = getProject(proj);
                
                //Pobranie folderów
                List<Device> tagFolder = pr.DevicesList.Where(x=>x.parentId.Equals(con)).ToList();

                //Pobranie 
                List<Tag> buff = new List<Tag>();
                foreach (Device tF in tagFolder)
                {
                    List<Tag> b = pr.tagsList.Where(x => x.parentId.Equals(tF.objId)).ToList();
                    buff.AddRange(b.ToArray());
                }

                return buff;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        #endregion

        #region Device


        /// <summary>
        /// Dodaj folder tagów
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="connId"></param>
        /// <param name="tF"></param>
        /// <returns></returns>
        public Guid addDevice(Guid projId, Guid connId, Device tF)
        {
            try
            {
                //wez projekt
                Project pr = getProject(projId);

                //Pobierz poloczenie
                Connection cn = getConnection(projId, connId);
               
                //ustawienie rodzica
                tF.parentId = cn.objId;

                //Dodanie IDRV
                tF.idrv = cn.Idrv;

                //Project 
                tF.projId = projId;

                //Podpięcie zdarzenia z klasy podrzednej
                tF.adressChanged += new EventHandler(DevicesAdressChange);

                //dodanie Urzadzenia
                pr.DevicesList.Add(tF);
                ((ITreeViewModel)cn).Children.Add(tF);

                //Dodanie drivera
                ((IDriversMagazine)tF).Children.Add(cn.Idrv);

                //Wyslanie zdarzenia
                if (addDeviceEv != null)
                    addDeviceEv(pr, new ProjectEventArgs(tF));

                return tF.objId;
            }
            catch (Exception Ex)
            {

                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Usuń folder tagow
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="connId"></param>
        /// <param name="tagFolId"></param>
        /// <returns></returns>
        public Boolean removeDevice(Guid projId, Guid tagFolId)
        {
            try
            {
                //Pobierz proejkt
                Project pr = getProject(projId);
               
                //Pobierz poloczenie
                Device cF = pr.DevicesList.Find(x => x.objId.Equals(tagFolId));

                //Connection
                Connection cn = getConnection(projId, cF.parentId);

                //Usuniecie sierot z projektu
                var dvs = pr.tagsList.FindAll(x => x.parentId == cF.objId);
                    foreach (Tag tg in dvs)
                        removeTag(pr.objId, tg.objId);

                //Pobierz FolderTagow
                pr.DevicesList.Remove(cF);
                ((ITreeViewModel)cn).Children.Remove(cF);

                ((ITableView)cF).Children.Clear();
                ((IDriversMagazine)cF).Children.Clear();

                //Wyslanie zdarzenia
                if (removeDeviceEv != null)
                    removeDeviceEv(pr, new ProjectEventArgs(cF));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Pobranie folderow tagow
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="tagFolId"></param>
        /// <returns></returns>
        public Device getDevice(Guid proj, Guid tagFolId)
        {
            try
            {
                Project pr = getProject(proj);
                Device tF = pr.DevicesList.Find(x => x.objId.Equals(tagFolId));
                return tF;
                    
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Pobierz wszystkie tagi z folderu
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="tagFolId"></param>
        /// <returns></returns>
        public List<Tag> getAllTagsFromDevice(Guid proj, Guid device)
        {
            try
            {
                Project pr = getProject(proj);
                List<Tag> tagList1 = pr.tagsList.Where(x => x.parentId == device).ToList();
                return tagList1;

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        #endregion

        #region Driver
        public Connection GetConnectionFromDriver(Guid pr, Guid drv)
        {
            try
            {
                Project pp = getProject(pr);

                foreach (Connection cn in pp.connectionList)
                {
                    if (cn.Idrv.ObjId == drv)
                        return cn;
                }

                throw new ApplicationException("connection is not found");

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        #endregion

        #region Tag

        /// <summary>
        /// Dodanie Taga
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="connFolId"></param>
        /// <param name="tg"></param>
        /// <returns></returns>
        public Boolean addTag(Guid projId, Guid tagFolId, Tag tg)
        {
            try
            {
                //To musi byc w tym miejscu gdyz tak przy przypisaniu odwoluje sie do proejktu
                Project pr = getProject(projId);
              
                tg.PrCon = this;
                tg.Proj = pr;

                //nowa logika sprawdzające nazwy
                while (getProject(projId).tagsList.Exists(x => x.tagName == tg.tagName))
                    tg.tagName = tg.tagName + "_Copy";

                //Id foledru połączen
                tg.parentId = tagFolId;

                //Id poloczenia dla Taga
                Device dev = getDevice(projId, tagFolId);
                tg.connId = dev.parentId;
                tg.deviceAdress = dev.adress;

                //Dodanie sterownika
                Connection cn = getConnection(projId, tg.connId);
                tg.idrv = cn.Idrv;

                //Connection
                ((ITableView)cn).Children.Add(tg);

                //Device
                ((ITableView)dev).Children.Add(tg);

                //Project
                pr.tagsList.Add(tg);
                ((ITreeViewModel)dev).Children.Add(tg);

                //Dodanie z projektu
                ((ITableView)pr).Children.Add(tg);
          
                //Wyslanie zdarzenia
                if (addTagEv != null)
                    addTagEv(pr, new ProjectEventArgs(tg));

                return true;
            }

            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Usuń taga
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Boolean removeTag(Guid projId, Guid tagId)
        {
            try
            {
                //Pobranie projektu
                Project pr = getProject(projId);
                
                //Pobranie Taga
                Tag tg = pr.tagsList.Find(x => x.objId.Equals(tagId));
                Device dev = getDevice(projId, tg.parentId);

                //Usuniecie Taga
                pr.tagsList.Remove(tg);
                ((ITreeViewModel)dev).Children.Remove(tg);

                //Connection
                foreach (Connection cn in pr.connectionList)
                    ((ITableView)cn).Children.Remove(tg);

                //Device
                foreach (Device dev1 in pr.DevicesList)
                    ((ITableView)dev1).Children.Remove(tg);

                //Usuniecie
                ((ITableView)pr).Children.Remove(tg);

                //Zdarzenia
                if (removeTagEv != null)
                    removeTagEv(pr, new ProjectEventArgs(tg));

                return true;
            }
            catch (Exception Ex)
            {

                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Pobierz taga
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Tag getTag(Guid projId, Guid tagId)
        {
            try
            {
                Project pr = getProject(projId);
                Tag tg = pr.tagsList.Find(x => x.objId.Equals(tagId));
                return tg;
            }
            catch (Exception Ex)
            {

                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }


        #endregion

        #region InTag

        /// <summary>
        /// Dodaj taga wewnetrzebgo
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Boolean AddIntTag(Guid projId, InTag tag)
        {
            try
            {
                Project pr = getProject(projId);
                tag.Proj = pr;
                tag.idrv = pr.InternalTagsDrv;
                tag.PrCon = this;

                pr.InTagsList.Add(tag);
                ((ITreeViewModel)pr.InternalTagsDrv).Children.Add(tag);

                ((ITableView)pr).Children.Add(tag);

                if (addIntTagEv != null)
                    addIntTagEv(pr, new ProjectEventArgs(tag));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Usun taga
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public Boolean RemoveIntTag(Guid projId, Guid tagId)
        {
            try
            {
                //Projekt
                Project pr = getProject(projId);

                //Chwilowe przetrzymanie zmiennej
                InTag buff = pr.InTagsList.Find(x => x.objId == tagId);

                //Usuniecie
                pr.InTagsList.RemoveAll(x => x.objId == tagId);
                ((ITreeViewModel)pr.InternalTagsDrv).Children.Remove(buff);

                //Connection
                foreach (Connection cn in pr.connectionList)
                    ((ITableView)cn).Children.Remove(buff);

                //Device
                foreach (Device dev1 in pr.DevicesList)
                    ((ITableView)dev1).Children.Remove(buff);

                ((ITableView)pr).Children.Remove(buff);

                //Zdarzenia
                if (removeIntTagEv != null)
                    removeIntTagEv(pr, new ProjectEventArgs(buff));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }

        }

        /// <summary>
        /// Pobierz InTag
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="objid"></param>
        /// <returns></returns>
        public InTag GetIntTag(Guid projId, Guid objid)
        {
            try
            {
                return getProject(projId).InTagsList.Find(x => x.objId == objid);
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        #endregion

        #region ITag

        /// <summary>
        /// Fukcja wysyla zdarzenia informujace o zmianie InTag lub Tag
        /// </summary>
        /// <param name="ir"></param>
        /// <returns></returns>
        public void ITagChanged(ITag ir)
        {
            try
            {
                //Pobranie elementu
                Object obj = getElementById(ir.Id);

                if (obj.GetType() == typeof(Tag))
                {
                    //Wyslanie delegata
                    if (editTagEv != null)
                        editTagEv(((Tag)ir).Proj, new ProjectEventArgs(obj));
                }
                else if (obj.GetType() == typeof(InTag))
                {
                    if (editIntTagEv != null)
                        editIntTagEv(((InTag)ir).Proj, new ProjectEventArgs(obj));
                }
            }
            catch (Exception Ex)
            {

                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));             
            }
        }

        /// <summary>
        /// Pobranie wszystkich ITAG przynalezne do pozycji. Nie pobiera ITag z InternalTags 
        /// </summary>
        /// <param name="Pr_"></param>
        /// <param name="Obj_"></param>
        /// <returns></returns>
        public List<ITag> GetAllITags(Guid Pr_, Guid Obj_)
        {
            try
            {
                //Bufor na tagi
                List<ITag> ITagList = new List<ITag>();

                //Sprawdz co zostalo wybrane
                Type SelectedObject = getElementById(Pr_, Obj_).GetType();

                //Pobranie projektu
                Project Pr = getProject(Pr_);

                if (SelectedObject == typeof(Project))
                {
                    ITagList.AddRange((from tg in Pr.tagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(InternalTagsDriver))
                {
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Connection))
                {
                    ITagList.AddRange((from tg in getAllTagsFromConnection(Pr_, Obj_) select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Device))
                {
                    ITagList.AddRange((from tg in getAllTagsFromDevice(Pr_, Obj_) select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Tag))
                {
                    ITagList.Add((ITag)getTag(Pr_, Obj_));
                }

                else if (SelectedObject == typeof(InTag))
                {
                    ITagList.Add((ITag)GetIntTag(Pr_, Obj_));
                }
                else
                {
                    return null;
                         
                }

                return ITagList;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Pobranie wszystkich ITAG przynalezne do pozycji. Pobiera ITag z InternalTags 
        /// </summary>
        /// <param name="Pr_"></param>
        /// <param name="Obj_"></param>
        /// <returns></returns>
        public List<ITag> GetITagsSel(Guid Pr_, Guid Obj_)
        {
            try
            {
                //Bufor na tagi
                List<ITag> ITagList = new List<ITag>();

                //Sprawdz co zostalo wybrane
                Type SelectedObject = getElementById(Pr_, Obj_).GetType();

                //Pobranie projektu
                Project Pr = getProject(Pr_);

                if (SelectedObject == typeof(Project))
                {
                    ITagList.AddRange((from tg in Pr.tagsList select (ITag)tg).ToList());
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(InternalTagsDriver))
                {
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Connection))
                {
                    ITagList.AddRange((from tg in getAllTagsFromConnection(Pr_, Obj_) select (ITag)tg).ToList());
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Device))
                {
                    ITagList.AddRange((from tg in getAllTagsFromDevice(Pr_, Obj_) select (ITag)tg).ToList());
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else if (SelectedObject == typeof(Tag))
                {
                    ITagList.Add((ITag)getTag(Pr_, Obj_));
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }

                else if (SelectedObject == typeof(InTag))
                {
                    ITagList.Add((ITag)GetIntTag(Pr_, Obj_));
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }
                else
                {
                    throw new ApplicationException("Element not exists");
                }

                return ITagList;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Fukncja pobiera wszystkie ITagi z projektu
        /// </summary>
        /// <param name="Pr1">Projekt</param>
        /// <param name="Obj1">Zakres pobierania</param>
        /// <param name="TabVisble">Uwzglenij widocznosc w Tab</param>
        /// <param name="GraphVisible">Uwzglednij widoczność w Graph</param>
        /// <returns>ITags</returns>
        public List<ITag> GetAllITags(Guid Pr1, Guid Obj1, Boolean TabVisble, Boolean GraphVisible)
        {
            try
            {
                //Bufor na tagi
                List<ITag> ITagList = new List<ITag>();

                //Sprawdz co zostalo wybrane
                Type SelectedObject = getElementById(Pr1, Obj1).GetType();

                //Pobranie projektu
                Project Pr = getProject(Pr1);

                //Projekt
                if (SelectedObject == typeof(Project))
                {
                    ITagList.AddRange((from tg in Pr.tagsList select (ITag)tg).ToList());
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());
                }

                //InernalTags
                else if (SelectedObject == typeof(InternalTagsDriver))
                    ITagList.AddRange((from tg in Pr.InTagsList select (ITag)tg).ToList());

                //Connection
                else if (SelectedObject == typeof(Connection))
                    ITagList.AddRange((from tg in getAllTagsFromConnection(Pr1, Obj1) select (ITag)tg).ToList());

                //Device
                else if (SelectedObject == typeof(Device))
                    ITagList.AddRange((from tg in getAllTagsFromDevice(Pr1, Obj1) select (ITag)tg).ToList());

                //Tag
                else if (SelectedObject == typeof(Tag))
                    ITagList.Add((ITag)getTag(Pr1, Obj1));

                //ITag
                else if (SelectedObject == typeof(InTag))
                    ITagList.Add((ITag)GetIntTag(Pr1, Obj1));

                //Exceptions
                else
                    throw new ApplicationException("Element not exists");

                //Filtorwanie
                //Widoczne w Tabeli i widoczne Graph
                if (TabVisble && GraphVisible)
                    return (from t in ITagList where t.GrVisibleTab && t.GrEnable select t).ToList();

                    //Wszystkie z Tabeli i widoczne w Graph
                else if (!TabVisble && GraphVisible)
                    return (from t in ITagList where t.GrEnable select t).ToList();

                    //Widoczne tylko i wszystko z Tabrli
                else if (TabVisble && !GraphVisible)
                    return (from t in ITagList where t.GrVisibleTab select t).ToList();

                    //Wszystkie jakie są
                else
                    return (from t in ITagList select t).ToList();

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }


        }

        /// <summary>
        /// Funkcja wyszukuje wszystkie ITags przyporzadkowane do IDriverModel
        /// </summary>
        /// <param name="Pr_"></param>
        /// <param name="Obj_"></param>
        /// <returns></returns>
        public List<ITag> GetAllITagsForDriver(Guid Pr_, Guid Driver_)
        {
            try
            {
                //Project
                Project Pr = getProject(Pr_);

                //Pobranie Tagów
                if (Driver_ != IntTagsGuid && Driver_ != ScriptGuid)
                {
                    //Pobranie wszystkich  TAG's
                    List<Tag> tss = Pr.tagsList.FindAll(x => x.idrv.ObjId == Driver_);

                    if (tss.Count > 0) return (from cc in tss select (ITag)cc).ToList();
                    else return new List<ITag>();
                }
                else if (Driver_ == IntTagsGuid)
                {
                    if (Pr.InTagsList.Count > 0) return (from cc in Pr.InTagsList select (ITag)cc).ToList();
                    else return new List<ITag>();
                }

                else
                    return new List<ITag>();

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Pobiera Wsystko
        /// </summary>
        /// <returns></returns>
        public List<ITag> GetAllITags()
        {
            try
            {
                if (projectList.Count > 0)
                {
                    Project Pr = projectList.First();
                    List<ITag> buff1 = Pr.tagsList.Cast<ITag>().ToList();
                    List<ITag> buff2 = Pr.InTagsList.Cast<ITag>().ToList();
                    buff1.AddRange(buff2);

                    return buff1;
                }
                else
                    return null;

            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }



        #endregion

        #region InFile

        /// <summary>
        /// Dodanie Pliku
        /// </summary>
        /// <returns></returns>
        public Boolean AddInFile(Guid projId, InFile file)
        {
            try
            {
                //
                Project pr  = getProject(projId);                          
                ((ITreeViewModel)pr.WebServer1).Children.Add(file);
                pr.FileList.Add(file);

                //Zdarzenia
                if (addInFileEv != null)
                    addInFileEv(pr, new ProjectEventArgs(file));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// usuniecie pliku
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Boolean RemoveInFile(Guid projId, Guid file)
        {
            try
            {
                //
                Project pr = getProject(projId);

                ((ITreeViewModel)pr.WebServer1).Children.Remove(GetInFile(projId,file));

                pr.FileList.RemoveAll(x => x.objId == file);
                     
                //Zdarzenia
                if (removeInFileEv != null)
                    removeInFileEv(pr, new ProjectEventArgs(file));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// Pobierz plik
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public InFile GetInFile(Guid projId, Guid file)
        {
            try
            {
                Project pr = getProject(projId);
                return pr.FileList.Find(x => x.objId == file);
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        #endregion

        #region ScriptFile

        /// <summary>
        /// Pobierz plik
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public ScriptFile GetScriptFile(Guid projId, Guid file)
        {
            try
            {

                Project pr = getProject(projId);
                return pr.ScriptFileList.Find(x => x.objId == file);
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return null;
            }
        }

        /// <summary>
        /// Dodanie Pliku scryptu
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Boolean AddScriptFile(Guid projId, ScriptFile file)
        {
            try
            {
                //
                Project pr = getProject(projId);

                //Dodanie referencji
                file.Proj = pr;
                file.PrCon = pr.PrCon;

                pr.ScriptFileList.Add(file);
                ((ITreeViewModel)pr.ScriptEng).Children.Add(file);

                //Zdarzenia
                if (addScriptFileEv != null)
                    addScriptFileEv(pr, new ProjectEventArgs(file));

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        /// <summary>
        /// usuniecie pliku
        /// </summary>
        /// <param name="projId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public Boolean RemoveScriptFile(Guid projId, Guid file)
        {
            try
            {
                //
                Project pr = getProject(projId);

                ScriptFile sf = GetScriptFile(projId, file);

                if(sf != null)
                {
                    //Usuniecie z treeview
                    if (((ITreeViewModel)pr.ScriptEng).Children.ToList().Exists(x => ((ScriptFile)x).objId == file))
                        ((ITreeViewModel)pr.ScriptEng).Children.Remove(sf);

                    pr.ScriptFileList.Remove(sf);

                    //Zdarzenia
                    if (removeScriptFileEv != null)
                        removeScriptFileEv(pr, new ProjectEventArgs(file));
                }

              

              

                return true;
            }
            catch (Exception Ex)
            {
                ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
                return false;
            }
        }

        #endregion

        public override string ToString()
        {
            return "ProjectContainer";
        }
    }


    [Serializable][System.Runtime.InteropServices.ComVisible(true)]
    public class Project : IDisposable, ITreeViewModel, INotifyPropertyChanged, ITableView, IDriversMagazine
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
        ObservableCollection<object> TreeViewChildren_;
        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return TreeViewChildren_;
            }

            set
            {
                TreeViewChildren_ = value;
            }
        }

        [field: NonSerialized]
        ObservableCollection<ITag> TagChildren;
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
        ObservableCollection<IDriverModel> DriverChildren_;
        ObservableCollection<IDriverModel> IDriversMagazine.Children
        {
            get
            {
                return DriverChildren_;
            }

            set
            {
                DriverChildren_ = value;
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        Guid objId_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "Id")]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        [Category("06 Formats"), DisplayName("DateTime Long"), Description("Format for display")][XmlElement(ElementName = "DBDateTimeFormat")]
        public string longDT { get; set; }

        string projectName_;
        [Category("01 Design"), DisplayName("Project Name"), Description("Current project name")][ComVisible(false)][XmlElement(ElementName = "Name")]
        public string projectName
        {
            get
            {
                return projectName_;
            }
            set
            {
                projectName_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("projectName"));
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                modificationApear();
            }
        }

        Version fileVer_;
        [Category("02 Header"), DisplayName("Version"), Description("Version of files.")][ComVisible(false)][XmlElement(ElementName = "FileVersion",Type = typeof(XMLVersion))]
        public Version fileVer
        {
            get { return fileVer_; }
            set
            {
                fileVer_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fileVer)));
                modificationApear();
            }
        }

        string autor_;
        [Category("03 Information"), DisplayName("Autor")][ComVisible(false)][XmlElement(ElementName = "Autor")]
        public string autor
        {
            get { return autor_; }
            set
            {
                autor_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("autor"));
                modificationApear();
            }
        }

        string company_;
        [Category("03 Information"), DisplayName("Company")][ComVisible(false)][XmlElement(ElementName = "Company")]
        public string company
        {
            get { return company_; }
            set
            {
                company_ = value;
                modificationApear();
                propChanged?.Invoke(this, new PropertyChangedEventArgs("company"));
            }
        }

        string describe_;
        [Category("05 Misc"), DisplayName("Description")][ComVisible(false)][XmlElement(ElementName = "Description")]
        public string describe
        {
            get { return describe_; }
            set
            {
                describe_ = value;
                modificationApear();
                propChanged?.Invoke(this, new PropertyChangedEventArgs("describe"));
            }
        }

        DateTime createTime_;
        [Category("04 Time"), DisplayName("Create Time"),ReadOnly(true)][ComVisible(false)][XmlElement(ElementName = "Created")]
        public DateTime createTime
        {
            get { return createTime_; }
            set { createTime_ = value; }
        }

        DateTime modifeTime_;
        [Category("04 Time"), ReadOnly(true), DisplayName("Modification Time")][ComVisible(false)][XmlElement(ElementName = "LastModification")]
        public DateTime modifeTime
        {
            get { return modifeTime_; }
            set
            {
                modifeTime_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("modifeTime"));
            }
        }

        Boolean modMarks_;
        [Browsable(false)][ComVisible(false)][XmlIgnore]
        public Boolean modMarks
        {
            get { return modMarks_; }
            set
            {
                modMarks_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("modMarks"));
            }
        }

        string path_ = string.Empty;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "ProjectPath")]
        public string path
        {
            get { return path_; }
            set
            {
                path_ = value; modificationApear();
                propChanged?.Invoke(this, new PropertyChangedEventArgs("path"));
            }
        }

        ChartViewConf ChartConf_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "EditorsConfiguration")]
        public ChartViewConf ChartConf
        {
            get { return ChartConf_; }
            set { ChartConf_ = value; modificationApear(); }
        }

        [Browsable(false)][XmlElement(ElementName = "DatabaseConfiguration")]
        public DatabaseModel Db { get; set; }

        [Browsable(false)]
        private WebServer WebServer1_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "WebServerConfiguration")]
        public WebServer WebServer1
        {
            get { return WebServer1_; }
            set { WebServer1_ = value; }
        }

        Boolean IsExpand_;
        [Browsable(false)][ComVisible(false)]
        public Boolean IsExpand
        {
            get { return IsExpand_; }
            set
            {
                IsExpand_ = value;
                modificationApear();

                //ITreeViewModel
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsExpand"));
            }
        }

        [NonSerialized]
        ProjectContainer PrCon_;[Browsable(false)][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return PrCon_; }
            set { PrCon_ = value; }
        }

        [ComVisible(false)][field: NonSerialized][XmlIgnore]
        public MSScriptControl.ScriptControl ScriptCon;

        private List<InFile> FileList_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "FileList", Type = typeof(List<InFile>))]
        public List<InFile> FileList
        {
            get { return FileList_; }
            set { FileList_ = value; }
        }

        private List<ScriptFile> ScriptFileList_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "ScriptFileList", Type = typeof(List<ScriptFile>))]
        public List<ScriptFile> ScriptFileList
        {
            get { return ScriptFileList_; }
            set { ScriptFileList_ = value; }
        }

        List<Connection> connectionList_ = new List<Connection>();
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "ConnectionList", Type = typeof(List<Connection>))]
        public List<Connection> connectionList
        {
            get { return connectionList_; }
            set
            {
                connectionList_ = value;

                modificationApear();
            }
        }

        List<Device> DevicesList_ = new List<Device>();
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "DeviceList", Type = typeof(List<Device>))]
        public List<Device> DevicesList
        {
            get { return DevicesList_; }
            set
            {
                DevicesList_ = value;
                modificationApear();
            }
        }


        List<Tag> tagsList_ = new List<Tag>();
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "TagList", Type = typeof(List<Tag>))]
        public List<Tag> tagsList
        {
            get { return tagsList_; }
            set { tagsList_ = value; modificationApear(); }
        }

        List<InTag> InTagsList_ = new List<InTag>();
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "InternalTagList", Type = typeof(List<InTag>))]
        public List<InTag> InTagsList
        {
            get { return InTagsList_; }
            set { InTagsList_ = value; modificationApear(); }
        }

        private ScriptsDriver ScriptEng_;
        [Browsable(false)][XmlElement(ElementName = "ScriptEngine")]
        public ScriptsDriver ScriptEng
        {
            get { return ScriptEng_; }
            set { ScriptEng_ = value; }
        }

        private InternalTagsDriver InternalTags_;
        [Browsable(false)][ComVisible(false)][XmlElement(ElementName = "IntTagsEngine")]
        public InternalTagsDriver InternalTagsDrv
        {
            get { return InternalTags_; }
            set { InternalTags_ = value; }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return projectName;
            }

            set
            {
                projectName = value;
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return false;
            }
            set { }
        }

        bool ITreeViewModel.IsBlocked
        {
            get
            {
                return false;
            }
            set { }

        }

        public Project()
        {

        }

        public Project(ProjectContainer prcn, string projectName, string autor, string company, string describe)
        {
            //Parametry
            this.projectName = projectName;
            this.autor = autor;
            this.company = company;
            this.describe = describe;
            this.createTime_ = DateTime.Now;
            this.modifeTime = DateTime.Now;
            this.modMarks = true;

            //Formaty
            longDT = "yyyy-MM-dd HH:mm:ss.fff";

            //Wersja pliku
            fileVer_ = Assembly.GetExecutingAssembly().GetName().Version;

            //ProjectCon
            this.PrCon_ = prcn;

            //Skrypte
            ScriptCon = new MSScriptControl.ScriptControl();
            ScriptCon.Language = "VBScript";
            ScriptCon.AddObject("Project", this, true);

            //ServerHttp
            WebServer1_ = new WebServer(null);

            //Scripts Files
            ScriptFileList_ = new List<ScriptFile>();

            //Scrypt Engine
            ScriptEng_ = new ScriptsDriver(this);
            ScriptEng_.Proj = this;

            //Parametr dla TreeView
            IsExpand = true;

            //Inicjalizacja ID
            objId = Guid.NewGuid();

            //Internals Tags
            InternalTags_ = new InternalTagsDriver(this);
            FileList_ = new List<InFile>();

            TreeViewChildren_ = new ObservableCollection<object>();
            TreeViewChildren_.Add(WebServer1_);
            TreeViewChildren_.Add(ScriptEng_);
            TreeViewChildren_.Add(InternalTags_);
            Db = new DatabaseModel();
            TreeViewChildren_.Add(Db);

            ((ITreeViewModel)WebServer1_).Children = new ObservableCollection<object>(FileList_);
            ((ITreeViewModel)ScriptEng_).Children = new ObservableCollection<object>(ScriptFileList_);
            ((ITreeViewModel)InternalTagsDrv).Children = new ObservableCollection<object>(InTagsList_);

            DriverChildren_ = new ObservableCollection<IDriverModel>();
            DriverChildren_.Add((IDriverModel)ScriptEng);
            DriverChildren_.Add((IDriverModel)InternalTagsDrv);

            foreach (var cn in connectionList_)
            {
                //Do projektu
                TreeViewChildren_.Add(cn);

                //Device do Poloczenia
                ((ITreeViewModel)cn).Children = new ObservableCollection<object>(from x in DevicesList_ where x.parentId == cn.objId select x);
            }

            foreach (var dev in DevicesList_)
                ((ITreeViewModel)dev).Children = new ObservableCollection<object>(from x in tagsList_ where x.parentId == dev.objId select x);

            TagChildren = new ObservableCollection<ITag>();

            //Konfiguracja ChartView
            this.ChartConf = new ChartViewConf();
            ((INotifyPropertyChanged)ChartConf).PropertyChanged += Project_PropertyChanged;

        }

        private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            modificationApear();
        }

        [ComVisible(false)]
        void modificationApear()
        {
            modifeTime = DateTime.Now;
            modMarks = true;
        }

        [ComVisible(false)][OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //Skrypte
            ScriptCon = new MSScriptControl.ScriptControl();
            ScriptCon.Language = "VBScript";
            ScriptCon.AddObject("Project", this, true);

            //Dodanie refernecji projektu
            InternalTags_.Proj = this;

            if (string.IsNullOrEmpty(longDT))
                longDT = "yyyy-MM-dd HH:mm:ss.fff";

            if (ScriptEng_ == null)
                ScriptEng_ = new ScriptsDriver(this);

            if (ScriptFileList_ == null)
                ScriptFileList_ = new List<ScriptFile>();

            //Konfiguracja ChartView
            ((INotifyPropertyChanged)ChartConf).PropertyChanged += Project_PropertyChanged;

            //Podlaczenie 
            TreeViewChildren_ = new ObservableCollection<object>();
            TreeViewChildren_.Add(this.WebServer1_);
            TreeViewChildren_.Add(this.ScriptEng_);
            TreeViewChildren_.Add(this.InternalTags_);
            if (Db == null)
                Db = new DatabaseModel();
            TreeViewChildren_.Add(this.Db);

            DirectoryInfo gt = new DirectoryInfo(Path.GetDirectoryName(this.path) + "\\Http");

            if (gt.Exists)
            {
                var SubDir = (from x in gt.GetDirectories() select new CusFile(x)).ToList();
                SubDir.AddRange(from x in gt.GetFiles() select new CusFile(x));
                ((ITreeViewModel)WebServer1_).Children = new ObservableCollection<object>(SubDir);
                FileList.Clear();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.path) + "\\Http");
                var SubDir = (from x in gt.GetDirectories() select new CusFile(x)).ToList();
                SubDir.AddRange(from x in gt.GetFiles() select new CusFile(x));
                ((ITreeViewModel)WebServer1_).Children = new ObservableCollection<object>(SubDir);
                FileList.Clear();
            }


            ((ITreeViewModel)ScriptEng_).Children = new ObservableCollection<object>(ScriptFileList_);
            ((ITreeViewModel)InternalTagsDrv).Children = new ObservableCollection<object>(InTagsList_);

            foreach (var cn in connectionList_)
            {
                //Do projektu
                TreeViewChildren_.Add(cn);

                //Device do Poloczenia
                ((ITreeViewModel)cn).Children = new ObservableCollection<object>(from x in DevicesList_ where x.parentId == cn.objId select x);
                ((ITableView)cn).Children = new ObservableCollection<ITag>((from x in tagsList where x.connId == cn.objId select x).Union<ITag>(InTagsList));
            }

            foreach (var dev in DevicesList_)
            {
                ((ITreeViewModel)dev).Children = new ObservableCollection<object>(from x in tagsList_ where x.parentId == dev.objId select x);
                ((ITableView)dev).Children = new ObservableCollection<ITag>((from x in tagsList where x.parentId == dev.objId select x).Union<ITag>(InTagsList));
            }

            //Tagi
            var query = tagsList.Union<ITag>(InTagsList);
            TagChildren = new ObservableCollection<ITag>(query);
        }

        public void OnDeserializedXML()
        {
            //Skrypte
            ScriptCon = new MSScriptControl.ScriptControl();
            ScriptCon.Language = "VBScript";
            ScriptCon.AddObject("Project", this, true);

            //Dodanie refernecji projektu
            InternalTags_.Proj = this;

            if (string.IsNullOrEmpty(longDT))
                longDT = "yyyy-MM-dd HH:mm:ss.fff";

            if (ScriptEng_ == null)
                ScriptEng_ = new ScriptsDriver(this);

            if (ScriptFileList_ == null)
                ScriptFileList_ = new List<ScriptFile>();

            //Konfiguracja ChartView
            ((INotifyPropertyChanged)ChartConf).PropertyChanged += Project_PropertyChanged;

            //Podlaczenie 
            TreeViewChildren_ = new ObservableCollection<object>();
            TreeViewChildren_.Add(this.WebServer1_);
            TreeViewChildren_.Add(this.ScriptEng_);
            TreeViewChildren_.Add(this.InternalTags_);
            if (Db == null)
                Db = new DatabaseModel();
            TreeViewChildren_.Add(this.Db);

            DirectoryInfo gt = new DirectoryInfo(Path.GetDirectoryName(this.path) + "\\Http");

            if (gt.Exists)
            {
                var SubDir = (from x in gt.GetDirectories() select new CusFile(x)).ToList();
                SubDir.AddRange(from x in gt.GetFiles() select new CusFile(x));
                ((ITreeViewModel)WebServer1_).Children = new ObservableCollection<object>(SubDir);
                FileList.Clear();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.path) + "\\Http");
                var SubDir = (from x in gt.GetDirectories() select new CusFile(x)).ToList();
                SubDir.AddRange(from x in gt.GetFiles() select new CusFile(x));
                ((ITreeViewModel)WebServer1_).Children = new ObservableCollection<object>(SubDir);
                FileList.Clear();
            }


            ((ITreeViewModel)ScriptEng_).Children = new ObservableCollection<object>(ScriptFileList_);
            ((ITreeViewModel)InternalTagsDrv).Children = new ObservableCollection<object>(InTagsList_);

            foreach (var cn in connectionList_)
            {
                //Do projektu
                TreeViewChildren_.Add(cn);

                //Device do Poloczenia
                ((ITreeViewModel)cn).Children = new ObservableCollection<object>(from x in DevicesList_ where x.parentId == cn.objId select x);
                ((ITableView)cn).Children = new ObservableCollection<ITag>((from x in tagsList where x.connId == cn.objId select x).Union<ITag>(InTagsList));
            }

            foreach (var dev in DevicesList_)
            {
                ((ITreeViewModel)dev).Children = new ObservableCollection<object>(from x in tagsList_ where x.parentId == dev.objId select x);
                ((ITableView)dev).Children = new ObservableCollection<ITag>((from x in tagsList where x.parentId == dev.objId select x).Union<ITag>(InTagsList));
            }

            //Tagi
            var query = tagsList.Union<ITag>(InTagsList);
            TagChildren = new ObservableCollection<ITag>(query);
        }

        [ComVisible(false)]
        public object Clone()
        {
            //Klonowanie w pamieci
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;

            //Utworzenie kopi
            Project Pr1 = (Project)bf.Deserialize(ms);
            ms.Close();

            //Nowy Projekt
            Pr1.objId_ = Guid.NewGuid();
            Pr1.connectionList_.Clear();
            Pr1.DevicesList_.Clear();
            Pr1.tagsList_.Clear();

            Pr1.modifeTime_ = DateTime.Now;
            Pr1.createTime_ = DateTime.Now;
            Pr1.path_ = "";
            Pr1.TagChildren = new ObservableCollection<ITag>();
            Pr1.DriverChildren_ = new ObservableCollection<IDriverModel>();
            Pr1.TreeViewChildren_ = new ObservableCollection<object>();

            return Pr1;
        }

        //Skrypty VBA

        [ComVisible(true)]
        public object GetTag(string s)
        {
            try
            {
                Tag tg = tagsList_.Find(x => x.tagName == s);
                if (tg != null)
                    return tg.value;
                else
                {
                    InTag tgs = InTagsList_.Find(x => x.tagName == s);
                    if (tgs != null)
                        return tgs.value;
                    else
                        return 0;
                }

            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return 0;
            }


        }

        [ComVisible(true)]
        public Object SetTag(string s, object val)
        {
            try
            {
                Tag tg = tagsList_.Find(x => x.tagName == s);
                if (tg != null)
                {
                    tg.setValueMethod(val);
                    return tg.value;
                }
                else
                {
                    InTag tgs = InTagsList_.Find(x => x.tagName == s);
                    if (tgs != null)
                    {
                        tgs.value = val;
                        return tgs.value;
                    }

                    else return null;
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return 0;
            }
        }

        public void Write(object sender, string s)
        {
            PrCon_.ApplicationError?.Invoke(sender, new ProjectEventArgs(new Information(s)));
        }

        #region Scripts for Web

        [ComVisible(true)]
        public string SetTagValue(string s, object val)
        {
            try
            {
                Tag tg = tagsList_.Find(x => x.tagName == s);

                if (tg != null)
                {

                    tg.setValueMethod(val);
                    return tg.value.ToString();
                }
                else
                {
                    InTag tgs = InTagsList_.Find(x => x.tagName == s);
                    if (tgs != null)
                    {
                        tgs.value = val;
                        return tgs.value.ToString();
                    }

                    else return tgs.value.ToString();
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return string.Empty;
            }
        }

        [ComVisible(true)]
        public string GetTagValue(string s)
        {
            try
            {
                Tag tg = tagsList_.Find(x => x.tagName == s);
                if (tg != null)
                {
                    return tg.value.ToString();
                }
                else
                {
                    InTag tgs = InTagsList_.Find(x => x.tagName == s);
                    if (tgs != null)
                    {
                        return tgs.value.ToString();
                    }

                    else return tgs.value.ToString();
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        [ComVisible(true)]
        public string GetTimerValue(string s)
        {
            try
            {
                return DateTime.Now.ToString();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        [ComVisible(true)]
        public string GetUserValue(string s)
        {
            try
            {
                return "User";
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        [ComVisible(true)]
        public string GetMachineValue(string s)
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        [ComVisible(true)]
        public string GetTagsAll(string name)
        {
            try
            {
                return JsonConvert.SerializeObject(PrCon_.GetAllITags(objId, objId, false, false));
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        [ComVisible(true)]
        public string GetConnectionsAll(string name)
        {
            try
            {
                return JsonConvert.SerializeObject(connectionList.ToArray());
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return "Empty";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    WebServer1_.Dispose();
                    connectionList_.Clear();
                    DevicesList_.Clear();
                    tagsList_.Clear();
                    InTagsList_.Clear();
                    FileList_.Clear();
                    ScriptFileList_.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Project()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

    }

    [Serializable][Synchronization]
    public class DatabaseModel : ContextBoundObject, ITreeViewModel, INotifyPropertyChanged
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

        [Browsable(false), XmlIgnore]
        public ObservableCollection<object> Children
        {
            get
            {
               return new ObservableCollection<object>();
            }

            set
            {
               
            }
        }

        [JsonIgnore]
        private ProjectContainer PrCon_ { get; set; }
        [Browsable(false), XmlIgnore, JsonIgnore]
        public ProjectContainer PrCon
        {
            get { return PrCon_; }
            set { PrCon_ = value; }
        }

        [field: NonSerialized]
        private Project Pr_;
        [Browsable(false),JsonIgnore,XmlIgnore]
        public Project Pr
        {
            get { return Pr_; }
            set
            {
                Pr_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }
     
        public DatabaseModel()
        {

        }

        public DatabaseModel(ProjectContainer prCon, Project pr)
        {

            PrCon = prCon;
            Pr = pr;

            DbConnection = new SQLiteConnection("Data Source=" + Path.GetDirectoryName(Pr.path) + PrCon.Database + ";Version=3;");

            //utworzenie bazy
            if (!File.Exists(Path.GetDirectoryName(Pr.path) + PrCon.Database))
            {
                //Sprawdzamy czy istnieje
                if (!Directory.Exists(Path.GetDirectoryName(Pr.path) + "\\" + "Database"))
                    Directory.CreateDirectory(Path.GetDirectoryName(Pr.path) + "\\" + "Database");

                SQLiteConnection.CreateFile(Path.GetDirectoryName(Pr.path) + PrCon.Database);
                DbConnection.Open();

                Pr.Db.IsLive = false;

                string sql = "create table tags (name varchar(100), stamp datetime, value real)";
                SQLiteCommand command = new SQLiteCommand(sql, DbConnection);
                command.ExecuteNonQuery();
                DbConnection.Close();
            }

            DbConnection.Open();

            ((ITableView)Pr).Children.CollectionChanged += ITags_CollectionChanged;

            foreach (ITag tg in ((ITableView)Pr).Children)
                ((INotifyPropertyChanged)tg).PropertyChanged += ITag_PropertyChanged;

        }

        private void ITags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += ITag_PropertyChanged;
            }
           else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ((INotifyPropertyChanged)e.OldItems[0]).PropertyChanged -= ITag_PropertyChanged;
                removeDataITagElement(((ITag)e.OldItems[0]).Name);
            }
        }

        private void ITag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           if(e.PropertyName == "Value")
            {
                ITag tg = (ITag)sender;

                if(tg.TypeData_ == TypeData.CHAR)
                {
                    addDataElement(tg.Name, Char.GetNumericValue((char)tg.Value).ToString(), DateTime.Now);
                }
                else if(tg.TypeData_ == TypeData.BIT)
                {
                    addDataElement(tg.Name, ((bool)tg.Value) ? "1.0" : "0.0", DateTime.Now);
                }
                else
                {
                    addDataElement(tg.Name, tg.Value.ToString(), DateTime.Now);
                }             
            }
        }

        //Dodaj element to bazy
        void addDataElement(string name, string value, DateTime tm)
        {
            try
            {
                if (DbConnection != null)
                {
                    string sql1 = String.Format("insert into tags (name, stamp, value) values ('{0}', '{1}' , {2:0.00})", name, tm.ToString("yyyy-MM-dd HH:mm:ss.fff"), value.ToString().Replace(',', '.'));
                    SQLiteCommand command1 = new SQLiteCommand(sql1, DbConnection);
                    command1.ExecuteNonQuery();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Remove element
        void removeDataITagElement(string name)
        {
            try
            {
                if (DbConnection != null)
                {
                    string sql1 = String.Format("delete from tags where name='{0}'", name);
                    SQLiteCommand command1 = new SQLiteCommand(sql1, DbConnection);
                    command1.ExecuteNonQuery();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Reset
        public void Reset()
        {

            if (DbConnection != null)
            {
                if (MessageBox.Show("Do you want to remove all records from database?", 
                                    "Database", 
                                      MessageBoxButtons.OKCancel) ==  DialogResult.OK)
                {
                    string sql1 = "delete from tags";
                    SQLiteCommand command1 = new SQLiteCommand(sql1, DbConnection);
                    command1.ExecuteNonQuery();

                    MessageBox.Show("All records removed!");
                }
            }
        }

        //GetDataTable
        public DataTable GetAll()
        {
            string sql = "select * from tags";
            SQLiteCommand command = new SQLiteCommand(sql, DbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            DataTable tb = new DataTable();
            tb.Load(reader);

            return tb;
        }

        //GetRange
        public DatabaseValues GetRange(ITag tg, DateTime from , DateTime to)
        {
            string sql = String.Format("select * from tags where stamp between '{0}' and '{1}' and name='{2}'", from.ToString("yyyy-MM-dd HH:mm:ss.fff"), to.ToString("yyyy-MM-dd HH:mm:ss.fff"), tg.Name);
            SQLiteCommand command = new SQLiteCommand(sql, DbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            DatabaseValues dbVls = new DatabaseValues();

            while (reader.Read())
            {
                //Dane
                double dtCurr = DateTime.ParseExact(reader.GetString(1), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture).ToOADate();
                double value = Convert.ToDouble(reader["value"].ToString());

                Boolean lastPoint = false;
                if (tg.TypeData_ == TypeData.BIT)
                {
                    if (dbVls.ValPts.Count != 0)
                        lastPoint = Convert.ToBoolean(dbVls.ValPts.Last());

                    if (lastPoint != Convert.ToBoolean(value))
                    {
                        dbVls.TimePts.Add(dtCurr);
                        dbVls.ValPts.Add(Convert.ToDouble(lastPoint));

                        dbVls.TimePts.Add(dtCurr);
                        dbVls.ValPts.Add(value);
                    }
                    else
                    {
                        dbVls.TimePts.Add(dtCurr);
                        dbVls.ValPts.Add(value);
                    }
                }
                else
                {
                    dbVls.TimePts.Add(dtCurr);
                    dbVls.ValPts.Add(value);
                }

            }

            return dbVls;
        }

        private bool IsLive_;
        [XmlIgnore][Browsable(false)]
        public bool IsLive
        {
            get { return IsLive_; }
            set
            {
                IsLive_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLive)));
            }
        }
       
        [ReadOnly(true)]
        public string Name
        {
            get
            {
                return "Database";
            }

            set
            {
                
            }
        }

        [Browsable(false)]
        public bool IsExpand
        {
            get
            {
                return true;
            }

            set
            {

            }
        }

        [Browsable(false)]
        public bool IsBlocked
        {
            get
            {
                return false;
            }

            set
            {

            }
        }

        [Browsable(false),XmlIgnore()]
        SQLiteConnection DbConnection;

        public void OnDeserializedXML()
        {
            //Baza danych
            DbConnection = new SQLiteConnection("Data Source=" + Path.GetDirectoryName(Pr.path) + PrCon.Database + ";Version=3;");

            //utworzenie bazy
            if (!File.Exists(Path.GetDirectoryName(Pr.path) + PrCon.Database))
            {
                //Sprawdzamy czy istnieje
                if (!Directory.Exists(Path.GetDirectoryName(Pr.path) + "\\" + "Database"))
                    Directory.CreateDirectory(Path.GetDirectoryName(Pr.path) + "\\" + "Database");

                SQLiteConnection.CreateFile(Path.GetDirectoryName(Pr.path) + PrCon.Database);
                DbConnection.Open();

                Pr.Db.IsLive = false;

                string sql = "create table tags (name varchar(100), stamp datetime, value real)";
                SQLiteCommand command = new SQLiteCommand(sql, DbConnection);
                command.ExecuteNonQuery();
                DbConnection.Close();
            }

            DbConnection.Open();

            ((ITableView)Pr).Children.CollectionChanged += ITags_CollectionChanged;

            foreach (ITag tg in ((ITableView)Pr).Children)
                ((INotifyPropertyChanged)tg).PropertyChanged += ITag_PropertyChanged;

        }
    }

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
        ObservableCollection<object> TreeViewChildren;
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
        ObservableCollection<ITag> TagChildren;
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
        ObservableCollection<IDriverModel> DriverChildren;
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
        object Parameters_;
        [Category("03 Data"), DisplayName("Parameters"),TypeConverter(typeof(DanConverter))]
        [XmlElement("TCP",typeof(TcpDriverParam))]
        [XmlElement("IO",typeof(IoDriverParam))]
        [XmlElement("S7",typeof(S7DriverParam))]
        public object Parameters
        {
            get { return Parameters_; }
            set 
            { 
                Parameters_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parameters)));
            }
        }

        Guid objId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Id")]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        Guid parentId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Parent")]
        public Guid parentId
        {
            get { return parentId_; }
            set
            {
                parentId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("parentId"));
            }
        }

        string connectionName_;
        [Category("01 Design"),DisplayName("Connection Name")][XmlElement(ElementName ="Name")]
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

        String DriverName_;
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

                if(TreeViewChildren != null)
                foreach (ITreeViewModel itv in TreeViewChildren)
                {
                    itv.IsBlocked = value;

                    foreach (ITreeViewModel itv1 in itv.Children)
                        itv1.IsBlocked = value;
                }
                   
            }
        }

        Boolean IsExpand_;
        [Browsable(false)][JsonIgnore]
        public Boolean IsExpand
        {
            get { return IsExpand_; }
            set
            {
                IsExpand_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpand)));
            }
        }

        Boolean assemblyError_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
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

        [field:NonSerialized]
        IDriverModel Idrv_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
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
        GlobalConfiguration gConf_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
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
        ProjectContainer PrCon_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return PrCon_; }
            set
            {
                PrCon_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("PrCon"));
                
            }
        }

        [Browsable(false)][XmlIgnore]
        public Boolean isLive { get{return Idrv.isAlive;}  }
 
        public Connection(GlobalConfiguration gC,ProjectContainer prCon ,String DriverName_, string connectionName, Object driverParam)
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
            if(Parameters == null)
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
   
    [Serializable]
    public class Device : ITreeViewModel ,INotifyPropertyChanged, ITableView, IDriverModel, IDriversMagazine
    {
        [field:NonSerialized][XmlIgnore]
        public EventHandler adressChanged;

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
 
        Guid projId_;
        [Browsable(false)][XmlElement(ElementName ="ProjectId")]
        public Guid projId
        {
            get { return projId_; }
            set
            {
                projId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("projId"));
            }
        }

        Guid parentId_;
        [Browsable(false)][XmlElement(ElementName ="Parent")]
        public Guid parentId
        {
            get { return parentId_; }
            set 
            {
                //Przypisanie
                parentId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("parentId"));
            }
        }

        Guid objId_;
        [Browsable(false)][XmlElement(ElementName ="Id")]
        public Guid objId
        {
            get { return objId_; }
            set 
            {
                //Zmiana potomokow

                if (PrCon != null)
                {
                    var tgs = from tx in prCon_.getProject(projId_).tagsList where tx.parentId == objId_ select tx;
                    foreach (Tag tx in tgs)
                        tx.parentId = value;
                }

                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        [NonSerialized]
        IDriverModel idrv_;
        [Browsable(false)][XmlIgnore]
        public IDriverModel idrv
        {
            get { return idrv_; }
            set
            {
                idrv_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("idrv"));
            }
        }

        [NonSerialized]
        ProjectContainer prCon_;
        [Browsable(false)][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return prCon_; }
            set
            {
                prCon_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("PrCon"));
            }
        }

        string name_;
        [Category("01 Design"), DisplayName("Device Name")][XmlElement(ElementName ="Name")]
        public string name
        {
            get { return name_; }
            set
            {
                name_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("name"));
            }
        }

        ushort adress_;
        [Category("02 Data"), DisplayName("Device Adress")][XmlElement(ElementName ="Adress")]
        [TypeConverter(typeof(BlockConverter))]
        public ushort adress
        {
            get { return adress_; }
            set 
            { 
                //Adres
                adress_ = value; 

                //Wyslanie zdarzenia do klasy nadrzednej
                if(adressChanged != null)
                    adressChanged(this,new ProjectEventArgs(adress_));

                propChanged?.Invoke(this, new PropertyChangedEventArgs("adress"));
            }
        }

        Boolean IsExpand_ = true;
        [Browsable(false)]
        public Boolean IsExpand
        {
            get { return IsExpand_; }
            set
            {
                IsExpand_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpand)));
            }
        }

        [field: NonSerialized]
        ObservableCollection<object> _Children;
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
                return name;
            }

            set
            {
                name = value;
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
                return idrv_.isAlive;
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
                return PrCon.getConnection(projId_,parentId_).IsBlocked;
            }
            set
            {
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        [field: NonSerialized]
        ObservableCollection<ITag> ChildrenTags;
        ObservableCollection<ITag> ITableView.Children
        {
            get
            {
                return ChildrenTags;
            }

            set
            {
                ChildrenTags = value;
            }
        }
      
        public Device(string name, ushort adresss, ProjectContainer projCon, Guid projId1)
        {
            //nazwa
            this.name = name;
            
            //Project
            this.projId_ = projId1;

            //Adres urzadenia
            this.adress_ = adresss;

            //Kontener projektowy
            this.prCon_ = projCon;

            //identyfikator
            objId = Guid.NewGuid();

            //Dzieci
            _Children = new ObservableCollection<object>();
            ChildrenTags = new ObservableCollection<ITag>();
            Children_ = new ObservableCollection<IDriverModel>();
        }

        public Device()
        {

        }

        /// <summary>
        /// Klonowanie obiektu
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            //Strumieniowanie
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;

            //Kopia Elementu
            Device Dv1 = (Device)bf.Deserialize(ms);
            ms.Close();

            //Dane
            Dv1.objId_ = Guid.NewGuid();
            Dv1.projId_ = Guid.Empty;
            Dv1.parentId_ = Guid.Empty;
            Dv1.prCon_ = prCon_;
            Dv1._Children = new ObservableCollection<object>();
            Dv1.ChildrenTags = new ObservableCollection<ITag>();
            Dv1.Children_ = new ObservableCollection<IDriverModel>();

            return Dv1;
        }

        /// <summary>
        /// Zdarzenia po serilaizacji
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {

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
                return idrv.driverName;
            }
        }

        object IDriverModel.setDriverParam
        {
            get
            {
                return idrv.setDriverParam;
            }

            set
            {
                idrv.setDriverParam = value;
            }
        }

        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get
            {
                return idrv.MemoryAreaInf;
            }
        }

        bool IDriverModel.isAlive
        {
            get
            {
                return idrv.isAlive;
            }

            set
            {
                idrv.isAlive = value;
            }
        }

        bool IDriverModel.isBusy
        {
            get
            {
                return idrv.isBusy;
            }
        }

        object[] IDriverModel.plugins
        {
            get
            {
                return idrv.plugins;
            }
        }

        bool[] IDriverModel.AuxParam
        {
            get
            {
                return idrv.AuxParam;
            }
        }

        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            return idrv.activateCycle(tagsList);
        }

        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            return idrv.addTagsComm(tagList);
        }

        bool IDriverModel.removeTagsComm(List<ITag> tagList)
        {
            return idrv.removeTagsComm(tagList);
        }

        bool IDriverModel.reConfig()
        {
            return idrv.reConfig();
        }

        bool IDriverModel.deactivateCycle()
        {
            return idrv.deactivateCycle();
        }

        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameRequest(frame, num);
        }

        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameResponse(frame, num);
        }

        byte[] IDriverModel.sendBytes(byte[] data)
        {
            return idrv.sendBytes(data);
        }

        event EventHandler IDriverModel.refreshedCycle
        {
            add
            {
                idrv.refreshedCycle += value;
            }

            remove
            {
                idrv.refreshedCycle -= value;
            }
        }

        event EventHandler IDriverModel.refreshedPartial
        {
            add
            {
                idrv.refreshedPartial += value;
            }

            remove
            {
                idrv.refreshedPartial -= value;
            }
        }

        event EventHandler IDriverModel.error
        {
            add
            {
                idrv.error += value;
            }

            remove
            {
                idrv.error -= value;
            }
        }

        event EventHandler IDriverModel.information
        {
            add
            {
                idrv.information += value;
            }

            remove
            {
                idrv.information -= value;
            }
        }

        event EventHandler IDriverModel.dataSent
        {
            add
            {
                idrv.dataSent += value;
            }

            remove
            {
                idrv.dataSent -= value;
            }
        }

        event EventHandler IDriverModel.dataRecived
        {
            add
            {
                idrv.dataRecived += value;
            }

            remove
            {
                idrv.dataRecived -= value;
            }
        }

        [field: NonSerialized]
        ObservableCollection<IDriverModel> Children_;
        ObservableCollection<IDriverModel> IDriversMagazine.Children
        {
            get
            {
                return Children_;
            }

            set
            {
                Children_ = value;
            }
        }

    }

    [Serializable]
    public class Tag : IComparable<Tag>, ITag, INotifyPropertyChanged, IDriverModel, ITreeViewModel
    {
 
        [field: NonSerialized]
        public event PropertyChangedEventHandler propChanged;

        [field: NonSerialized]
        public event EventHandler refreshedCycle;

        [field: NonSerialized]
        public event EventHandler refreshedPartial;

        [field: NonSerialized]
        public event EventHandler error;

        [field: NonSerialized]
        public event EventHandler information;

        [field: NonSerialized]
        public event EventHandler dataSent;

        [field: NonSerialized]
        public event EventHandler dataRecived;

        [field: NonSerialized]
        private ProjectContainer projCon_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return projCon_; }
            set { projCon_ = value; }
        }

        [field: NonSerialized]
        private Project Proj_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set { Proj_ = value; }
        }

        Guid parentId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Parent")]
        public Guid parentId
        {
            get { return parentId_; }
            set { parentId_ = value; }
        }

        Guid connId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="ConnectionId")]
        public Guid connId
        {
            get { return connId_; }
            set { connId_ = value; }
        }

        Guid objId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Id")]
        public Guid objId
        {
            get { return objId_; }
            set { objId_ = value; }
        }

        string tagName_;
        [Category("01 Design"), DisplayName("Tag Name")][XmlElement(ElementName ="Name")]
        public string tagName
        {
            get { return tagName_; }
            set 
            {
              
                
                if (Proj_?.tagsList.Where(x => x.tagName == value).Count<Tag>() > 0 || Proj_?.InTagsList.Where(x => x.tagName == value).Count<InTag>() > 0)
                    tagName_ = tagName_ + "$$";
                else
                    tagName_ = value;

                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        TypeData TypeData_;
        [Category("03 Data"), DisplayName("Value Type")][JsonIgnore]
        public TypeData TypeData
        {
            get { return TypeData_; }
            set 
            { 
                //Zmiana Tablic na skutek zmiany typu danych
                TypeData_ = value;

                if (idrv != null)
                {
                    //Pobieram informacje o obszarze pamieci
                    MemoryAreaInfo mInf = idrv_?.MemoryAreaInf.Where(x => x.Name.Equals(this.areaData_)).ToArray()[0];

                    //Tworzenie pierwotnych danych zmiennych ODCZYT
                    this.coreData_ = new Boolean[getSize() < mInf.AdresSize ? mInf.AdresSize : getSize()];

                    //Tworzenie pierwotnych danych zmiennych ZAPIS
                    this.coreDataSend_ = new Boolean[coreData_.Length];

                    //Bezposredni adres bitowy
                    this.bitAdres_ = (this.startData_ * mInf.AdresSize);

                    //Sprawdzenie SecendAdress
                    int[] war = this.getScAdressPos();
                    if (!war.Contains(this.scAdres_))
                        this.scAdres_ = war[0];
                }

                ResetValue();

                propChanged?.Invoke(this, new PropertyChangedEventArgs("TypeData_"));

            }
        }

        private int BlockAdress_;
        [Category("02 Details"), DisplayName("DBAdress"), Browsable(true)][JsonIgnore][XmlElement(ElementName = "DBAdress")]
        [TypeConverter(typeof(BlockAdressVisibility))]
        public int BlockAdress
        {
            get { return BlockAdress_; }
            set
            {
                BlockAdress_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("BlockAdress"));
            }
        }

        string areaData_;
        [Category("02 Details"), DisplayName("Memory Area")][TypeConverter(typeof(MemoryAreaModbus))][XmlElement(ElementName ="MemoryArea")]
        public string areaData
        {
            get { return areaData_; }
            set 
            { 
                areaData_ = value;

                if (idrv != null)
                {
                    //Pobieram informacje o obszarze pamieci
                    MemoryAreaInfo mInf = idrv_.MemoryAreaInf.Where(x => x.Name.Equals(this.areaData_)).ToArray()[0];

                    //Tworzenie pierwotnych danych zmiennych ODCZYT
                    this.coreData_ = new Boolean[getSize() < mInf.AdresSize ? mInf.AdresSize : getSize()];

                    //Tworzenie pierwotnych danych zmiennych ZAPIS
                    this.coreDataSend_ = new Boolean[coreData_.Length];

                    //Bezposredni adres bitowy
                    this.bitAdres_ = (this.startData_ * mInf.AdresSize);

                    //Sprawdzenie SecendAdress
                    int[] war = this.getScAdressPos();
                    if (!war.Contains(this.scAdres_))
                        this.scAdres_ = war[0];
                }

                propChanged?.Invoke(this, new PropertyChangedEventArgs("AreaData"));
                propChanged?.Invoke(this, new PropertyChangedEventArgs("BitByte"));
            }
        }

        int startData_;
        [Category("02 Details"),DisplayName("Adress")][XmlElement(ElementName ="Adress")]
        public int startData
        {
            get { return startData_; }
            set 
            {
                startData_ = value;

                if (idrv != null)
                {
                    //Pobieram informacje o obszarze pamieci
                    MemoryAreaInfo mInf = idrv_.MemoryAreaInf.Where(x => x.Name.Equals(this.areaData_)).ToArray()[0];

                    //Bezposredni adres bitowy
                    this.bitAdres_ = (this.startData_ * mInf.AdresSize);
                }

                propChanged?.Invoke(this, new PropertyChangedEventArgs("Adress"));
            }
        }

         int bitAdres_;
         [Browsable(false)][JsonIgnore][XmlIgnore]
         public int bitAdres
         {
             get { return bitAdres_; }
             set { bitAdres_ = value; }
         }

         ushort deviceAdress_;
         [Browsable(false)][XmlElement(ElementName ="DeviceAdress")]
         public ushort deviceAdress
         {
             get { return deviceAdress_; }
             set
            {
                deviceAdress_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("DevAdress"));

            }
         }

         int scAdres_;
         [Category("02 Details"), DisplayName("Offset[bit/byte]")][XmlElement("Offset")]
         [TypeConverter(typeof(TagSecendaryAdress))]
         public int scAdres
         {
             get { return scAdres_; }
             set 
             { 
                 //Sprawdzenie
                 scAdres_ = value;
             
                 //Sprawdzenie SecendAdress
                 int[] war = getScAdressPos();

                if (war != null)
                {
                    if (!war.Contains(this.scAdres_))
                        this.scAdres_ = war[0];
                }

                propChanged?.Invoke(this, new PropertyChangedEventArgs("BitByte"));
            }
         }

         BytesOrder bytesOrder_;
         [Category("02 Details"), DisplayName("Bytes Order"), Browsable(true)][JsonIgnore][XmlElement(ElementName ="BytesOrder")]
         public BytesOrder bytesOrder
         {
             get { return bytesOrder_; }
             set
            {
                bytesOrder_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("BytesOrder_"));
            }
         }

        string describe_;
        [Category("01 Design"), DisplayName("Description")][JsonIgnore][XmlElement(ElementName ="Description")]
        public string describe
        {
            get { return describe_; }
            set
            {
                describe_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

        Object value_;
        [Browsable(false)][XmlIgnore]
        public object value
        {
            get
            {
               return value_;
            }

            set
            {
                value_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private string ReadScript_;
        [CusEventProperty][Category("04 Script"), DisplayName("Read"), Browsable(true)][Editor(typeof(ScEditor), typeof(UITypeEditor))][JsonIgnore]
        public string ReadScript
        {
            get { return ReadScript_; }
            set { ReadScript_ = value; }
        }

        private string WriteScript_;
        [CusEventProperty][Category("04 Script"), DisplayName("Write"), Browsable(true)][Editor(typeof(ScEditor), typeof(UITypeEditor))][JsonIgnore]
        public string WriteScript
        {
            get { return WriteScript_; }
            set { WriteScript_ = value; }
        }

        Boolean[] coreData_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public Boolean[] coreData
        {
            get { return coreData_;  }
            set { coreData_ = value; }
        }

        Boolean[] coreDataSend_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public Boolean[] coreDataSend
        {
            get { return coreDataSend_; }
            set { coreDataSend_ = value;}
        }

        [JsonIgnore][XmlIgnore]
        public Boolean setMod;

        [NonSerialized]
        IDriverModel idrv_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public IDriverModel idrv
        {
            get { return idrv_; }
            set { idrv_ = value; }
        }

        public Tag(string tagName,BytesOrder bOrder,string areaData, int startData, int secondAdress, string desribe,TypeData typeData, IDriverModel idr, Guid connId, int blAdres)
        {
            try
            {
                //Podstawowe Dane
                this.tagName_ = tagName;

                //Obszar pamieci
                this.areaData_ = areaData;

                //Adres startowy1
                this.startData_ = startData;

                //Adres startowy 2
                this.scAdres_ = secondAdress;
                
                //Adress Blokowu dla DB
                this.BlockAdress_ = blAdres;

                //porzadek
                this.bytesOrder_ = bOrder;

                //Typ danych
                this.TypeData_ = typeData;

                //popis
                this.describe_ = desribe;

                //Id poloczenia
                this.connId_ = connId;

                //Inicjalizacja ID
                objId = Guid.NewGuid();

                //Marker modyfikacji
                setMod = false;

                //Pobieram informacje o obszarze pamieci
                MemoryAreaInfo aInfo = idr.MemoryAreaInf.Where(x => x.Name.Equals(this.areaData_)).ToArray()[0];

                //Tworzenie pierwotnych danych zmiennych ODCZYT
                this.coreData_ = new Boolean[getSize() < aInfo.AdresSize ? aInfo.AdresSize : getSize()];

                //Tworzenie pierwotnych danych zmiennych ZAPIS
                this.coreDataSend_ = new Boolean[coreData_.Length];

                //Bezposredni adres bitowy
                this.bitAdres_ = (this.startData_ * aInfo.AdresSize);

                //Ustawienie formatowanie
                Format_ = "{0:0.00}";

                //Poczatkowy kolor
                Random random = new Random();
                Clr_ = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

                //Widoscznosc w Graph
                Width_ = 2;

                //Enebla dla graph
                GrEnable_ = true;

                GrVisible_ = true;

                GrVisibleTab_ = true;

                //Reset
                ResetValue();

            }
            catch (Exception Ex)
            {   
                throw new ApplicationException("ProjectContainer.Tag.Tag() :" + Ex.Message);;
            }
        }

        public Tag()
        {

        }

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

        public int getSize()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return 1;

                    case TypeData.SBYTE:
                        return 8;

                    case TypeData.BYTE:
                        return 8;

                    case TypeData.CHAR:
                        return 16;

                    case TypeData.USHORT:
                        return 16;

                    case TypeData.SHORT:
                        return 16;

                    case TypeData.UINT:
                        return 32;

                    case TypeData.INT:
                        return 32;

                    case TypeData.FLOAT:
                        return 32;

                    case TypeData.DOUBLE:
                        return 64;

                    case ProjectDataLib.TypeData.ShortToReal:
                        return 16;

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        public override bool Equals(object obj)
        {
            try
            {
                if (obj is IDriverModel)
                    return Guid.Equals(this.objId, ((ITag)obj).Id);
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        public override int GetHashCode()
        {
            return this.objId.GetHashCode();
        }

        public int CompareTo(Tag b)
        {
            // Alphabetic sort name[A to Z]
            return this.startData.CompareTo(b.startData);
        }

        public void refreshData()
        {
            try
            {
                
                //Ustawienie kolejności bitów wg. BytesOrder
                Boolean[] dane1 = OrderBytesFct(this.bytesOrder_, ref coreData_);

                //Konwersja
                byte[] arrByte = new byte[dane1.Length / 8 == 0 ? 1 : dane1.Length / 8];
                BitArray btAr = new BitArray(dane1);

                //Konwersja na byte[]
                btAr.CopyTo(arrByte, 0);

                //Gdy nie ustawimy skryptu
                Boolean ScriptMark = false;
                if (!String.IsNullOrEmpty(ReadScript_))
                {
                    try
                    {
                        Proj_.ScriptCon.Eval(ReadScript_);
                        ScriptMark = true;
                    }
                    catch (Exception)
                    {
                        ScriptMark = false;
                    }
                }
                else
                    ScriptMark = false;

                  
                //Konwersja
                switch (this.TypeData_)
                {
                    case ProjectDataLib.TypeData.BIT:
                        value = (Boolean)dane1[scAdres_];
                        break;

                    case ProjectDataLib.TypeData.BYTE:

                        if (!ScriptMark)
                        {
                            value = arrByte[scAdres_];
                        }
                        else
                        {
                            value = arrByte[scAdres_];
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (byte)Proj_.ScriptCon.Eval(code);
                        }
                               
                        break;

                    case ProjectDataLib.TypeData.SBYTE:
                        if(!ScriptMark)
                          {       
                            value = (sbyte)arrByte[scAdres_];
                        }
                        else
                        {
                            value = (sbyte)arrByte[scAdres_];
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (sbyte)Proj_.ScriptCon.Eval(code);
                        }                                                                                 
                       break;

                    case ProjectDataLib.TypeData.CHAR:
                        value = BitConverter.ToChar(arrByte, 0);
                        break;

                    case ProjectDataLib.TypeData.SHORT:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToInt16(arrByte, 0);
                        }
                        else
                        {
                            value = BitConverter.ToInt16(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (short)Proj_.ScriptCon.Eval(code);
                        }                                                                                       
                        break;

                    case ProjectDataLib.TypeData.USHORT:

                   
                         if(!ScriptMark)
                         {
                            value = BitConverter.ToUInt16(arrByte, 0);
                        }
                         else
                         {
                            value = BitConverter.ToUInt16(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (ushort)Proj_.ScriptCon.Eval(code);
                        }                             
                                                                                
                        break;

                    case ProjectDataLib.TypeData.INT:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToInt32(arrByte, 0);
                        }
                        else
                        {
                            value = BitConverter.ToInt32(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (int)Proj_.ScriptCon.Eval(code);
                        }                                           
                        break;

                    case ProjectDataLib.TypeData.UINT:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToUInt32(arrByte, 0);
                        }
                        else
                        {
                            value = BitConverter.ToUInt32(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0}", value)).Replace(',', '.');
                            value = (uint)Proj_.ScriptCon.Eval(code);
 
                        }                           
                        break;

                    case ProjectDataLib.TypeData.FLOAT:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToSingle(arrByte, 0);
                        }
                        else
                        {
                            value = BitConverter.ToSingle(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0.000000}", value)).Replace(',', '.');
                            value = (float)Proj_.ScriptCon.Eval(code);
                            break;

                        }
                        break;

                    case ProjectDataLib.TypeData.DOUBLE:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToDouble(arrByte, 0);
                        }
                        else
                        {
                            value = BitConverter.ToDouble(arrByte, 0);
                            string code = ReadScript_.Replace("x", String.Format("{0:0.000000}", value)).Replace(',', '.');
                            value = (double)Proj_.ScriptCon.Eval(code);
                        }
                
                        break;

                    case ProjectDataLib.TypeData.ShortToReal:

                        if (!ScriptMark)
                        {
                            value = BitConverter.ToInt16(arrByte, 0);
                            value = double.Parse(value.ToString());
                            break;
                        }
                        else
                        {
                            value = BitConverter.ToInt16(arrByte, 0);
                            value = float.Parse(value.ToString());
                            string code = ReadScript_.Replace("x", String.Format("{0:0.000000}", value)).Replace(',', '.');
                            value_ = (float)Proj_.ScriptCon.Eval(code);
                        }                          
           
                        break;
                }

            }
            catch (Exception Ex)
            {
                throw new ApplicationException(this.tagName_ + " " + Ex.Message);
            }

        }

        public void setValueMethod(Object obj)
        {
            lock (this)
            {

                if (obj == null)
                    return; 

                //Znacznik modyfikacji
                setMod = true;

                //Przekopiowanie tablic
                Array.Copy(coreData_, coreDataSend_,coreData_.Length);
                BitArray biArr;
                byte[] btArr;

                //Testowanie scryptu
                Boolean ScriptMark = false;
                if (!String.IsNullOrEmpty(WriteScript_))
                {
                    try
                    {
                        Proj_.ScriptCon.Eval(WriteScript_);
                        ScriptMark = true;
                    }
                    catch (Exception)
                    {
                        ScriptMark = false;
                    }
                }
                  
                //Selekcja Danych
                switch (TypeData_)
                {
                    case ProjectDataLib.TypeData.BIT:
                        if (obj.GetType() == typeof(string))
                        {
                            if (obj.ToString() == "1")
                                obj = true;
                            else if (obj.ToString() == "0")
                                obj = false;
                            else
                                obj = Convert.ToBoolean(obj);
                        }

                        coreDataSend_[scAdres_] = (Boolean)obj;
                        break;

                    case ProjectDataLib.TypeData.BYTE:
                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToByte(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (byte)Proj_.ScriptCon.Eval(code);
                        }

                        biArr = new BitArray(coreDataSend_);
                        btArr = new byte[biArr.Length / 8];
                        biArr.CopyTo(btArr, 0);

                        btArr[scAdres_] = (byte)obj;
                        biArr = new BitArray(btArr);
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.SBYTE:
                        
                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToSByte(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (sbyte)Proj_.ScriptCon.Eval(code);
                        }

                        biArr = new BitArray(coreDataSend_);
                        btArr = new byte[biArr.Length / 8];
                        biArr.CopyTo(btArr, 0);

                        //kopiowanie sbyte to byte table (nie można przypisać byte to sbyte)
                        Buffer.BlockCopy(new sbyte[] {(sbyte)obj }, 0, btArr, scAdres_,1);

                        biArr = new BitArray(btArr);
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.CHAR:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToChar(obj);

                        biArr = new BitArray(BitConverter.GetBytes((Char)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.DOUBLE:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToDouble(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (double)Proj_.ScriptCon.Eval(code);
                        }
                        biArr = new BitArray(BitConverter.GetBytes((Double)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.FLOAT:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToSingle(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (float)Proj_.ScriptCon.Eval(code);
                        }

                        biArr = new BitArray(BitConverter.GetBytes((float)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.INT:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToInt32(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (int)Proj_.ScriptCon.Eval(code);
                        }
                        biArr = new BitArray(BitConverter.GetBytes((Int32)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.UINT:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToUInt32(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (uint)Proj_.ScriptCon.Eval(code);
                        }
                        biArr = new BitArray(BitConverter.GetBytes((UInt32)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.SHORT:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToInt16(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (short)Proj_.ScriptCon.Eval(code);
                        }
                        biArr = new BitArray(BitConverter.GetBytes((short)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.USHORT:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToUInt16(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (ushort)Proj_.ScriptCon.Eval(code);
                        }
                        biArr = new BitArray(BitConverter.GetBytes((ushort)obj));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;

                    case ProjectDataLib.TypeData.ShortToReal:

                        if (obj.GetType() == typeof(string))
                            obj = Convert.ToDouble(obj);

                        //Obsluga scryptu
                        if (ScriptMark)
                        {
                            string code = WriteScript_.Replace("x", String.Format("{0:0}", obj)).Replace(',', '.');
                            obj = (double)Proj_.ScriptCon.Eval(code);
                        }
                        short buff = Convert.ToInt16(obj);
                        biArr = new BitArray(BitConverter.GetBytes(buff));
                        biArr.CopyTo(coreDataSend_, 0);
                        break;
                }

                //Zmiana Order
                this.coreDataSend_ = OrderBytesFct(this.bytesOrder_, ref coreDataSend_); 
            }
        }

        public TagAdress nextAdress()
        {
            try
            {
                //Pobieram informacje o obszarze pamieci
                MemoryAreaInfo mInf = idrv_.MemoryAreaInf.Where(x=>x.Name.Equals(this.areaData_)).ToArray()[0];
                
                //Wybór elementu
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        if (mInf.AdresSize == 1)
                        {
                            return new TagAdress(this.startData + 1, 0);
                        }
                        else
                        {
                            if(scAdres_ < mInf.AdresSize - 1)
                             return new TagAdress(this.startData, scAdres_ + 1);
                            else
                             return new TagAdress(this.startData + 1, 0);
                        }
                        
                    case TypeData.SBYTE:

                        if (mInf.AdresSize == 1)
                        {
                            return new TagAdress(this.startData + 8, 0);
                        }
                        else
                        {
                            if (scAdres_ < (mInf.AdresSize / 8)-1)
                                return new TagAdress(this.startData, scAdres_ + 1);
                            else
                                return new TagAdress(this.startData + 1, 0);
                        }
                   
                    case TypeData.BYTE:
                        if (mInf.AdresSize == 1)
                        {
                            return new TagAdress(this.startData + 8, 0);
                        }
                        else
                        {
                            if (scAdres_ < (mInf.AdresSize / 8)-1)
                                return new TagAdress(this.startData, scAdres_ + 1);
                            else
                                return new TagAdress(this.startData + 1, 0);
                        }

                    case TypeData.CHAR:
                        return new TagAdress(this.startData + (getSize()/(mInf.AdresSize)), 0);

                    case TypeData.USHORT:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.SHORT:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.UINT:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.INT:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.DOUBLE:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.FLOAT:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    case TypeData.ShortToReal:
                        return new TagAdress(this.startData + (getSize() / (mInf.AdresSize)), 0);

                    default:
                        throw new ApplicationException("This kind od DataType in unsed!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.nextAdress() :" + Ex.Message);
            }
        }

        public int[] getScAdressPos()
        {
            //Zwracana tablica
            List<int> buffor = new List<int>();

            if (idrv == null)
                return null;

            //Pobranie obszaru danych
            MemoryAreaInfo info = idrv_?.MemoryAreaInf.Where(x => x.Name == this.areaData_).ToList()[0];

            //Obliczenia
            switch (this.TypeData_)
            {
                case TypeData.BIT:
                    for (int i = 0; i < info.AdresSize; i++)
                        buffor.Add(i);
                    break;

                case TypeData.BYTE:
                    for (int i = 0; i < info.AdresSize/8; i++)
                        buffor.Add(i);
                    break;

                case TypeData.SBYTE:
                    for (int i = 0; i < info.AdresSize / 8; i++)
                        buffor.Add(i);
                    break;

                case TypeData.CHAR:
                    for (int i = 0; i < info.AdresSize / 16; i++)
                        buffor.Add(i);
                    break;

                case TypeData.SHORT:
                    for (int i = 0; i < info.AdresSize / 16; i++)
                        buffor.Add(i);
                    break;

                case TypeData.USHORT:
                    for (int i = 0; i < info.AdresSize / 16; i++)
                        buffor.Add(i);
                    break;

                case TypeData.INT:
                    for (int i = 0; i < info.AdresSize / 32; i++)
                        buffor.Add(i);
                    break;

                case TypeData.UINT:
                    for (int i = 0; i < info.AdresSize / 32; i++)
                        buffor.Add(i);
                    break;

                case TypeData.FLOAT:
                    for (int i = 0; i < info.AdresSize / 32; i++)
                        buffor.Add(i);
                    break;

                case TypeData.DOUBLE:
                    for (int i = 0; i < info.AdresSize / 64; i++)
                        buffor.Add(i);
                    break;

            }

            //Zabezpieczenie
            if (buffor.Count == 0)
                buffor.Add(0);


            return buffor.ToArray();
        }

        public Type getOwnType()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return typeof(Boolean);

                    case TypeData.SBYTE:
                        return typeof(SByte);

                    case TypeData.BYTE:
                        return typeof(Byte);

                    case TypeData.CHAR:
                        return typeof(Char);

                    case TypeData.USHORT:
                        return typeof(ushort);

                    case TypeData.SHORT:
                        return typeof(short);

                    case TypeData.UINT:
                        return typeof(uint);

                    case TypeData.INT:
                        return typeof(int);

                    case TypeData.DOUBLE:
                        return typeof(double);

                    case TypeData.FLOAT:
                        return typeof(float);

                    case TypeData.ShortToReal:
                        return typeof(float);

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        private string Format_;
        [Category("05 Appearance"), DisplayName("Format Value")][JsonIgnore][TypeConverter(typeof(FormatValueConv))]
        public string Format
        {
            get { return Format_; }
            set { Format_ = value; }
        }

        private Boolean[] OrderBytesFct(BytesOrder BytesOrder_, ref Boolean[] data)
        {
            try
            {
                switch (bytesOrder_)
                {

                    case BytesOrder.BADC:
                        if (data.Length > 15)
                        {
                            for (int i = 0; i < data.Length; i = i + 16)
                            {
                                //Pobranie 1 bajta
                                Boolean[] b1 = data.ToList().GetRange(i, 8).ToArray();

                                //Pobranie drugiego bajta
                                Boolean[] b2 = data.ToList().GetRange(i + 8, 8).ToArray();

                                //Pierwszy  bajt na miejsce drugiego
                                Array.Copy(b2, 0, data, i, 8);

                                //Drugi  bajt na miejsce pierwszego
                                Array.Copy(b1, 0, data, i + 8, 8);
                            }

                            return data;
                        }

                        else
                            return data;

                    case BytesOrder.ABCD:
                        return data;

                    case BytesOrder.DCBA:
                        //Zamiana na byte[]
                        byte[] dDane = new byte[data.Length / 8 == 1 ? 1 : data.Length / 8];
                        BitArray btArr = new BitArray(data);
                        btArr.CopyTo(dDane, 0);
                        dDane = dDane.Reverse().ToArray();

                        //Powrót do Bit
                        btArr = new BitArray(dDane);
                        btArr.CopyTo(data, 0);
                        return data;

                    default:
                        return data;
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectDataLib.Tag.OrderBytesFct :" + Ex.Message);
            }
        }

        private Color Clr_;
        [Category("06 Graph"), DisplayName("Color")][JsonIgnore][XmlElement(Type = typeof(XmlColor),ElementName ="Color")]
        public Color Clr
        {
            get { return Clr_; }
            set
            {
                Clr_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Clr"));
            }
        }

        private int Width_;
        [Category("06 Graph"), DisplayName("Line Width")][JsonIgnore]
        public int Width
        {
            get { return Width_; }
            set
            {
                Width_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Width"));
            }
        }

        private Boolean GrEnable_;
        [Category("06 Graph"), DisplayName("Enable")][JsonIgnore][XmlElement( ElementName = "ChartEnable")]
        public Boolean GrEnable
        {
            get { return GrEnable_; }
            set
            {
                GrEnable_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrEnable"));
            }
        }

        private Boolean GrVisible_;
        [Category("06 Graph"), DisplayName("Visible")][JsonIgnore][XmlElement(ElementName = "ChartVisible")]
        public Boolean GrVisible
        {
            get { return GrVisible_; }
            set
            {
                GrVisible_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrVisible"));
            }
        }

        private Boolean GrVisibleTab_;
        [Category("07 Table"), DisplayName("Visible")][JsonIgnore][XmlElement( ElementName = "TableVisible")]
        public Boolean GrVisibleTab
        {
            get { return GrVisibleTab_; }
            set
            {
                GrVisibleTab_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrVisibleTab"));
            }
        }

        [Browsable(false)]
        public string formattedValue
        {
            get
            {
                if (Format_ == "{0:ASCII}")
                {
                    dynamic d = value_;
                    return Encoding.ASCII.GetString(BitConverter.GetBytes(d));
                }
                else
                {
                    return string.Format(Format, value);
                }
            }
        }

        [Browsable(false)]
        public string typeData { get { return Enum.GetName(typeof(TypeData), TypeData); } }

        [Browsable(false)]
        public string description { get { return describe; } }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {

            try
            {

                setMod = false;
                ResetValue();

                if (Clr_ == null)
                    Clr_ = Color.Black;

                if (Width_ == 0)
                    Width_ = 2;



            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        public void OnDeserializedXml()
        {

            try
            {

                setMod = false;
                ResetValue();

                if (Clr_ == null)
                    Clr_ = Color.Black;

                if (Width_ == 0)
                    Width_ = 2;

                if (idrv != null)
                {
                    //Pobieram informacje o obszarze pamieci
                    MemoryAreaInfo mInf = idrv_?.MemoryAreaInf.Where(x => x.Name.Equals(this.areaData_)).ToArray()[0];

                    //Tworzenie pierwotnych danych zmiennych ODCZYT
                    this.coreData_ = new Boolean[getSize() < mInf.AdresSize ? mInf.AdresSize : getSize()];

                    //Tworzenie pierwotnych danych zmiennych ZAPIS
                    this.coreDataSend_ = new Boolean[coreData_.Length];

                    //Bezposredni adres bitowy
                    this.bitAdres_ = (this.startData_ * mInf.AdresSize);

                    //Sprawdzenie SecendAdress
                    int[] war = this.getScAdressPos();
                    if (!war.Contains(this.scAdres_))
                        this.scAdres_ = war[0];
                }

            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        Boolean ResetValue()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        value = false;
                        break;

                    case TypeData.SBYTE:
                        value = Convert.ToSByte(0);
                        break;

                    case TypeData.BYTE:
                        value = Convert.ToByte(0);
                        break;

                    case TypeData.CHAR:
                        value = Convert.ToChar(0);
                        break;

                    case TypeData.USHORT:
                        value = Convert.ToUInt16(0);
                        break;

                    case TypeData.SHORT:
                        value = Convert.ToInt16(0);
                        break;

                    case TypeData.UINT:
                        value = Convert.ToUInt32(0);
                        break;

                    case TypeData.INT:
                        value = Convert.ToInt32(0);
                        break;

                    case TypeData.FLOAT:
                        value = Convert.ToSingle(0.0);
                        break;

                    case TypeData.DOUBLE:
                        value = Convert.ToDouble(0.0);
                        break;

                    case ProjectDataLib.TypeData.ShortToReal:
                        value = Convert.ToSingle(0.0);
                        break;

                    default:
                        value = 0;
                        break;
                }

                return true;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return false;

            }
        }

        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            Tag Tg1 = (Tag)bf.Deserialize(ms);
            ms.Close();

            Tg1.objId_ = Guid.NewGuid();
            Tg1.parentId_ = Guid.Empty;
            Tg1.connId_ = Guid.Empty;
            Tg1.projCon_ = projCon_;
            Tg1.Proj_ = Proj_;

            return Tg1;
        }

        //Interfejsc
        Guid ITag.Id
        {
            get { return objId; }
            set
            {
                objId = value;               
            }
        }

        Guid ITag.ParentId
        {
            get { return parentId; }
            set
            {
                parentId = value;
            }
        }

        string ITag.Name
        {
            get { return tagName; }
            set
            {
                tagName = value;
                
            }
        }
        Boolean ITag.ActName
        {
            get { return true; }
        }

        int ITag.DevAdress 
        {
            get { return deviceAdress; }
            set
            {
                deviceAdress = (ushort)value;
               
            }
        }
        Boolean ITag.ActDevAdress { get { return true; } }

        int ITag.Adress 
        {
            get { return startData; }
            set
            {
                startData = value;
            
            }
        }
        Boolean ITag.ActAdress { get { return true; } }

        int ITag.BitByte 
        {
            get {return scAdres;}
            set
            {
                scAdres = value;
             
            }
        }
        Boolean ITag.ActBitByte { get { return true; } }

        TypeData ITag.TypeData_ 
        {
            get { return TypeData; }
            set
            {
                TypeData = value;
               
            } 
        }
        Boolean ITag.ActTypeData_ { get { return true; } }

        string ITag.AreaData 
        {
            get { return areaData; }
            set
            {
                areaData = value;
             
            }
        }
        Boolean ITag.ActAreaData { get { return true; } }

        Object ITag.Value 
        { 
            get {return value;}
            set
            {
                this.value = value;
                
            }
        }
        Boolean ITag.ActValue { get { return true; } }

        string ITag.Description
        { 
            get { return describe; }
            set
            {
                describe = value;
                
            }
        }
        Boolean ITag.ActDscription { get { return true; } }

        Boolean ITag.ActParam
        {
            get { return true; }
        }

        Type ITag.getOwnType()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return typeof(Boolean);

                    case TypeData.SBYTE:
                        return typeof(SByte);

                    case TypeData.BYTE:
                        return typeof(Byte);

                    case TypeData.CHAR:
                        return typeof(Char);

                    case TypeData.USHORT:
                        return typeof(ushort);

                    case TypeData.SHORT:
                        return typeof(short);

                    case TypeData.UINT:
                        return typeof(uint);

                    case TypeData.INT:
                        return typeof(int);

                    case TypeData.DOUBLE:
                        return typeof(double);

                    case TypeData.FLOAT:
                        return typeof(float);

                    case TypeData.ShortToReal:
                        return typeof(float);

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        Boolean ITag.ActSetValue
        {
            get { return true; }
        }
        void ITag.SetValue(Object val)
        {
            try
            {
                this.setValueMethod(val);
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.SetValue() :" + Ex.Message); 
            }
            
        }

        string ITag.GetFormatedValue()
        {
            try
            {
                if (string.IsNullOrEmpty(Format_))
                {
                    if (TypeData_ == TypeData.BIT)
                    {
                        if ((bool)value_)
                            return "1";
                        else
                            return "0";
                    }
                    else
                        return value.ToString();                   
                }
                else if (Format_ == "{0:ASCII}")
                {
                    dynamic d = value_;
                    return Encoding.ASCII.GetString(BitConverter.GetBytes(d));
                }
                else
                {
                    if (TypeData_ == TypeData.BIT)
                    {
                        if ((bool)value_)
                            return "1";
                        else
                            return "0";
                    }
                    else
                        return string.Format(Format, value);
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return value.ToString();
            }
        }

        bool ITag.ActClr
        {
            get { return true; }
        }   

        bool ITag.ActWidth
        {
            get { return true; }
        }

        bool ITag.ActGrEnable
        {
            get { return true; }
        }

        bool ITag.ActGrVisible
        {
            get { return true; }
        }

        bool ITag.ActGrMarkers
        {
            get { return true; }
        }

        bool ITag.ActGrVisibleTab
        {
            get { return true; }
        }

        bool ITag.GrVisibleTab
        {
            get
            {
                return GrVisibleTab;
            }
            set
            {
                GrVisibleTab = value;
            }
        }

        bool ITag.ActBlockAdress
        {
            get
            {
                return idrv_.AuxParam[1];
            }
        }

        BytesOrder ITag.BytesOrder_
        {
            get
            {
                return bytesOrder;
            }

            set
            {
                bytesOrder = value;
    
            }
        }

        bool ITag.ActBytesOrder_
        {
            get
            {
                return true;
            }
        }

        //IDriverModel

        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            return idrv.activateCycle(tagsList);
        }

        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            return idrv.addTagsComm(tagList);
        }

        bool IDriverModel.removeTagsComm(List<ITag> tagList)
        {
            return idrv.removeTagsComm(tagList);
        }

        bool IDriverModel.reConfig()
        {
            return idrv.reConfig();
        }

        bool IDriverModel.deactivateCycle()
        {
            return idrv.deactivateCycle();
        }

        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameRequest(frame, num);
        }

        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameResponse(frame, num);
        }

        byte[] IDriverModel.sendBytes(byte[] data)
        {
            return idrv.sendBytes(data);
        }

        Guid IDriverModel.ObjId
        {
            get
            {
                return idrv.ObjId;
            }
        }

        string IDriverModel.driverName
        {
            get
            {
                return idrv.driverName;
            }
        }

        object IDriverModel.setDriverParam
        {
            get
            {
                return idrv.setDriverParam;
            }

            set
            {
                idrv.setDriverParam = value;
            }
        }

        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get
            {
                return idrv.MemoryAreaInf;
            }
        }

        bool IDriverModel.isAlive
        {
            get
            {
                return idrv.isAlive;
            }

            set
            {
                idrv.isAlive = value;
                //zdarzenia
                propChanged?.Invoke(this, new PropertyChangedEventArgs("isAlive"));
            }
        }

        bool IDriverModel.isBusy
        {
            get
            {
                return idrv.isBusy;
            }
        }

        object[] IDriverModel.plugins
        {
            get
            {
                return idrv.plugins;
            }
        }

        bool[] IDriverModel.AuxParam
        {
            get
            {
                return idrv.AuxParam;
            }
        }

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return new ObservableCollection<object>();
            }

            set
            {
              
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return tagName;
            }

            set
            {
                tagName = value;
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return false;
            }

            set
            {
                
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return idrv_.isAlive;
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
                return PrCon.getConnection(Proj_.objId,connId).IsBlocked;
            }
            set
            {
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));
            }
        }
    }
    
    [Serializable]
    public class InternalTagsDriver : IDriverModel, ITreeViewModel ,INotifyPropertyChanged
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

        Guid objId_;
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
        [Browsable(false)][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set
            {
                Proj_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }

        Boolean isExpand_;
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

        private bool IsBlocked_;
        [Category("01 Design"), DisplayName("IsBlocked")]
        public bool IsBlocked
        {
            get { return IsBlocked_; }
            set
            {
                IsBlocked_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));

                if(_Children != null)
                foreach (ITreeViewModel itv in _Children)
                    itv.IsBlocked = value;
            }
        }

        public InternalTagsDriver(Project pr)
        {
            this.Proj_ = pr;
            this.objId_ = new Guid("22222222-2222-2222-2222-222222222222");

            //Domysly Timer
            Timers_ = new List<CustomTimer>();
            Timers_.Add(new CustomTimer());

        }

        public InternalTagsDriver()
        {                
        }

        [field:NonSerialized]
        private List<InTag> TagList_;
        /// <summary>
        /// Tagi wewnetrzne
        /// </summary>
        [Browsable(false)][XmlIgnore]
        public List<InTag> TagList
        {
            get { return TagList_; }
            set { TagList_ = value; }
        }

        [field: NonSerialized]
        List<System.Threading.Timer> BckTimers = new List<System.Threading.Timer>();

        [field: NonSerialized]
        EventHandler sendInfoEv;

        [field: NonSerialized]
        EventHandler errorSendEv;

        [field: NonSerialized]
        EventHandler refreshedPartial;

        [field: NonSerialized]
        EventHandler refreshCycleEv;

        [field: NonSerialized]
        EventHandler sendLogInfoEv;

        [field: NonSerialized]
        EventHandler reciveLogInfoEv;

        //Informacja o aktywnosci odczytu
        Boolean isLive;

        /// <summary>
        /// Nazwa Sterownika
        /// </summary>
        string IDriverModel.driverName
        {
            get { return "InternalTags"; }
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
                //Pobranie danych
                TagList_ = (from c in tagsList where c is InTag select (InTag)c).ToList();

                //Odnalezinie timerow ktore sa potrzbene dla tagow
                List<CustomTimer> CusTm = (from tk in Proj_.InternalTagsDrv.Timers where Proj_.InTagsList.FindAll(x=>x.TimerName == tk.Name).ToList().Count > 0 select tk).ToList();

                //Utworzenie watkow
                BckTimers.Clear();
                foreach (var ti in CusTm)
                {
                    List<InTag> tgs = (from tt in Proj_.InTagsList where tt.TimerName == ti.Name select tt).ToList();
                    BckTimers.Add(new System.Threading.Timer(TimerTask,tgs,ti.Delay,ti.Time));
                }

                isLive = true;

                foreach (IDriverModel it in this.TagList)
                    it.isAlive = true;

                return true;
            }
            catch (Exception Ex)
            {
                
              if (errorSendEv != null)
                   errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                List<InTag> tgs = (from c in tagList where c is InTag select (InTag)c).ToList();
                this.TagList_.AddRange(tgs);
                return true;
            }
            catch (Exception Ex)
            {
                
                if (errorSendEv != null)
                  errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                //Rzytowanie
                List<InTag> tgs = (from c in tagList  where c is InTag select (InTag)c).ToList();
                
                //usowanie
                foreach(InTag tg in tgs)
                    this.TagList_.RemoveAll(x=>x.objId == tg.objId);
                
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                //Zablokowanie
                foreach (var tm in BckTimers)
                    tm.Dispose();

                BckTimers.Clear();

                isLive = false;

                if (TagList != null)
                {
                    foreach (IDriverModel it in this.TagList)
                        it.isAlive = false;
                }

                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                //Wykonanie scryptow
                foreach (InTag tg in (List<InTag>)StateObj)
                {
                    tg.value = Proj_.ScriptCon.Eval(tg.WriteScript);
                    refreshedPartial?.Invoke(this, new ProjectEventArgs(tg));
                }

                //Odswierzony script
                refreshCycleEv?.Invoke(this, new ProjectEventArgs(StateObj));
    
            }
            catch(Exception Ex)
            {
                if (sendInfoEv != null)
                    sendInfoEv(this, new ProjectEventArgs(new byte[]{0},DateTime.Now,Ex.Message));
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

        ObservableCollection<object> _Children;
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
                return "Internal Tags";
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

    [Serializable]
    public class ScriptsDriver : IDriverModel , ITreeViewModel ,INotifyPropertyChanged
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

        Guid objId_;
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
        [Browsable(false)][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set
            {
                Proj_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }
        Boolean isExpand_;
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

                if(_Children != null)
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

        [field:NonSerialized]
        private List<InTag> TagList_;
        [Browsable(false)][XmlIgnore]
        public List<InTag> TagList
        {
            get { return TagList_; }
            set { TagList_ = value; }
        }

        [field: NonSerialized]
        List<System.Threading.Timer> BckTimers = new List<System.Threading.Timer>();

        [field: NonSerialized]
        EventHandler sendInfoEv;

        [field: NonSerialized]
        EventHandler errorSendEv;

        [field: NonSerialized]
        EventHandler refreshedPartial;

        [field: NonSerialized]
        EventHandler refreshCycleEv;

        [field: NonSerialized]
        EventHandler sendLogInfoEv;

        [field: NonSerialized]
        EventHandler reciveLogInfoEv;

        //Informacja o aktywnosci odczytu
        Boolean isLive;

        //Script
        [field:NonSerialized]
        List<dynamic> scripts = new List<dynamic>();

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
                CSScript.EvaluatorConfig.Engine = EvaluatorEngine.CodeDom;
                
                //Script inicjalizacja
                scripts.Clear();
                BckTimers.Clear();
                foreach (ScriptFile f in Proj_.ScriptFileList)
                {
                    if (f.Enable && !string.IsNullOrEmpty(f.TimerName))
                    {
                        string code = File.ReadAllText(f.FilePath);
                        scripts.Add(CSScript.Evaluator.LoadCode(code));
                        scripts.Last().Init(Proj_,f.Name);
                        scripts.Last().Start();

                        //Timery
                        CustomTimer ti = Proj_.ScriptEng.Timers.Find(x=>x.Name == f.TimerName);
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
                  errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

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
            catch(Exception Ex)
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

        [field:NonSerialized]
        ObservableCollection<object> _Children;
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

    [Serializable]
    public class CustomTimer : IComparable<CustomTimer>
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CustomTimer()
        {
            Name_ = "Timer";
            Time_ = 1000;
            Delay_ = 0;
        }

        private string Name_;
        /// <summary>
        /// Nazwa Timera
        /// </summary>
        [Category("Design"),DisplayName("01 Timer Name")]
        public string Name
        {
            get { return Name_; }
            set { Name_ = value; }
        }

        private int Time_;
        /// <summary>
        /// Czas zegara
        /// </summary>
         [Category("Design"), DisplayName("02 Time [ms]")]
        public int Time
        {
            get { return Time_; }
            set { Time_ = value; }
        }

         private int Delay_;
         /// <summary>
         /// Opoznienie startu
         /// </summary>
         [Category("Design"), DisplayName("03 Delay [ms]")]
         public int Delay
         {
             get { return Delay_; }
             set { Delay_ = value; }
         }

        /// <summary>
        /// Sortowanie
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
         int IComparable<CustomTimer>.CompareTo(CustomTimer other)
         {
             return this.Time_.CompareTo(other.Time_);
         }
    }

    [Serializable][Obsolete("Typ jest przestarzaly")]
    public class InFile : ITreeViewModel, INotifyPropertyChanged
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

        Guid objId_;
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

        string Name_;
        [Category("01 Design"), DisplayName("Name")]
        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;

                //Zmiana nazwy pliku  na dysku podczas edycji w programie
                File.Move(FilePath, Path.GetDirectoryName(FilePath) + "\\" + value);
                FilePath = Path.GetDirectoryName(FilePath) + "\\" + value;

                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));

            }
        }

        private String FilePath_;
        [Category("02 Misc"),ReadOnly(true),DisplayName("Path")]
        public string FilePath
        {
            get { return FilePath_; }
            set
            {
                FilePath_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("FilePath"));
            }
        }

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return new ObservableCollection<object>();
            }

            set
            {
               
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return Name;
            }

            set
            {
                Name = value;
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return false;   
            }

            set
            {
                
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return false;
            }
            set
            {
                //propChanged?.Invoke(this, new PropertyChangedEventArgs("IsLive"));
            }
        }

        bool ITreeViewModel.IsBlocked
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public InFile(string path)
        {
            this.FilePath_ = path;
            this.Name_ = Path.GetFileName(path);
            objId_ = Guid.NewGuid();

        }

        public InFile()
        {

        }
    }

    [Serializable]
    public class ScriptFile : ITreeViewModel ,INotifyPropertyChanged
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
        private ProjectContainer projCon_;
        [Browsable(false)][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return projCon_; }
            set
            {
                projCon_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("PrCon"));
            }
        }

        [field: NonSerialized]
        private Project Proj_;
        [Browsable(false)][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set
            {
                Proj_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }

        Guid objId_;
        [Browsable(false)][XmlElement(ElementName ="Id")]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        string Name_;
        [Category("01 Design"), DisplayName("Name")]
        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;

                //Zmiana nazwy pliku  na dysku podczas edycji w programie

                if (!string.IsNullOrEmpty(FilePath))
                {
                    File.Move(FilePath, Path.GetDirectoryName(FilePath) + "\\" + value);
                    FilePath = Path.GetDirectoryName(FilePath) + "\\" + value;
                }
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));

            }
        }

        private String FilePath_;
        [Category("02 Misc"), ReadOnly(true), DisplayName("Path")]
        public string FilePath
        {
            get { return FilePath_; }
            set
            {
                FilePath_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("FilePath"));
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

        [Category("01 Design"), DisplayName("IsBlocked")]
        public bool IsBlocked
        {
            get { return !Enable; }
            set
            {
                Enable = !value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));
            }
        }

        private string TimerName_;
        [TypeConverter(typeof(ScriptFileTimers))]
        [Category("03 Script"), DisplayName("Trigger"), Browsable(true)]
        public string TimerName
        {
            get { return TimerName_; }
            set
            {
                TimerName_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TimerName"));
            }
        }

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return new ObservableCollection<object>();
            }

            set
            {
             
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return Name;
            }

            set
            {
                Name = value;
            }
        }

        bool ITreeViewModel.IsExpand
        {
            get
            {
                return false;
            }

            set
            {
                
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return ((IDriverModel)Proj_.ScriptEng).isAlive;
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
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        public ScriptFile(string path)
        {
            this.FilePath_ = path;
            this.Name_ = Path.GetFileName(path);
            objId_ = Guid.NewGuid();
            Enable_ = true;
        }

        public ScriptFile()
        {

        }

    }

    [Serializable]
    public class InTag : IComparable<Tag>, ITag, IDriverModel, INotifyPropertyChanged, ITreeViewModel
    {
        [field: NonSerialized][JsonIgnore]
        private ProjectContainer projCon_;

        /// <summary>
        /// Kontener projektowy
        /// </summary>
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return projCon_; }
            set { projCon_ = value; }
        }

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
        public event EventHandler refreshedCycle;

        [field: NonSerialized]
        public event EventHandler refreshedPartial;

        [field: NonSerialized]
        public event EventHandler error;

        [field: NonSerialized]
        public event EventHandler information;

        [field: NonSerialized]
        public event EventHandler dataSent;

        [field: NonSerialized]
        public event EventHandler dataRecived;

        /// <summary>
        /// Projekt w ktorym znajduje sie tag
        /// </summary>
        [field: NonSerialized]
        private Project Proj_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set
            {
                Proj_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Proj"));
            }
        }

        /// <summary>
        /// Interfejs 
        /// </summary>
        [NonSerialized]
        IDriverModel idrv_;
        [Browsable(false)][JsonIgnore][XmlIgnore]
        public IDriverModel idrv
        {
            get { return idrv_; }
            set
            {
                idrv_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("idrv"));
            }
        }

        private string Format_;
        /// <summary>
        ///Formatowanie danych
        /// </summary>
        [Category("04 Appearance"), DisplayName("Format Value")][JsonIgnore]
        [TypeConverter(typeof(FormatValueConv))]
        public string Format
        {
            get { return Format_; }
            set
            {
                Format_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Format"));
            }
        }

        /// <summary>
        /// Id rodzica
        /// </summary>
        Guid parentId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Parent")]
        public Guid parentId
        {
            get { return parentId_; }
            set
            {
                parentId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("parentId"));
            }
        }

        /// <summary>
        /// Identyfikator obiektu
        /// </summary>
        Guid objId_;
        [Browsable(false)][JsonIgnore][XmlElement(ElementName ="Id")]
        public Guid objId
        {
            get { return objId_; }
            set
            {
                objId_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("objId"));
            }
        }

        /// <summary>
        /// Dane identyfikacyjne
        /// </summary>
        string tagName_;
        [Category("01 Design"), DisplayName("Tag Name")][XmlElement(ElementName = "Name")]
        public string tagName
        {
            get { return tagName_; }
            set 
            {
                if (Proj != null)
                {
                    if (Proj_.tagsList.Where(x => x.tagName == value).Count<Tag>() > 0 || Proj_.InTagsList.Where(x => x.tagName == value).Count<InTag>() > 0)
                        tagName_ = tagName_ + "$$";


                    else
                        tagName_ = value;
                }
                else
                    tagName_ = value;

                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Typ danych
        /// </summary>
        TypeData TypeData_;
        [Category("02 Data"), DisplayName("Data Type")][JsonIgnore]
        public TypeData TypeData
        {
            get { return TypeData_; }
            set 
            { 
                TypeData_ = value;
                ResetValue();

                propChanged?.Invoke(this, new PropertyChangedEventArgs("TypeData_"));
            }
        }

        private string WriteScript_;
        [CusEventProperty][Category("03 Script"), DisplayName("Code"), Browsable(true)][Editor(typeof(ScEditor), typeof(UITypeEditor))][JsonIgnore]
        public string WriteScript
        {
            get { return WriteScript_; }
            set { WriteScript_ = value; }
        }

        /// <summary>
        /// Opis Taga
        /// </summary>
        string describe_;
        [Category("01 Design"), DisplayName("Description")][JsonIgnore][XmlElement(ElementName ="Description")]
        public string describe
        {
            get { return describe_; }
            set
            {
                describe_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

        private string TimerName_;
        /// <summary>
        /// Nazwa Timera
        /// </summary>
        [TypeConverter(typeof(InTagsTimers))]
        [Category("03 Script"), DisplayName("Trigger"), Browsable(true)][JsonIgnore]
        public string TimerName
        {
            get { return TimerName_; }
            set { TimerName_ = value; }
        }

        /// <summary>
        /// Wartość danych
        /// </summary>
        Object value_;
        [Category("02 Data"), DisplayName("Value")]
        public object value
        {

            get
            {
              return value_;
            }

            set
            {
                if (value is string)
                {
                    if (TypeData_ == TypeData.BIT)
                    {
                        if ((string)value == "1")
                            value_ = true;
                        else if ((string)value == "0")
                            value = false;
                        else
                            value = false;
                    }
                    else
                    {
                        value_ = value;
                    }
                }
                else
                {
                    value_ = value;
                }  
                                                                               
              propChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));

            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="areaData"></param>
        /// <param name="startData"></param>
        /// <param name="desribe"></param>
        /// <param name="_TypeData"></param>
        public InTag(ProjectContainer prCon, Project pr, string tagName, string desribe,TypeData typeData, string initVal)
        {
            try
            {
                //Podstawowe Dane
                this.tagName_ = tagName;

                //Typ danych
                this.TypeData_ = typeData;

                //popis
                this.describe_ = desribe;

                //Inicjalizacja ID
                objId = Guid.NewGuid();

                //Dodanie projektu
                this.Proj_ = pr;

                //Dodanie 
                this.projCon_ = prCon;

                //Formatowanie
                Format_ = "{0:0.00}";

                //TypyDanych
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        value_ = Convert.ToBoolean(initVal);
                        break;

                    case TypeData.SBYTE:
                        value_ = Convert.ToSByte(initVal);
                        break;

                    case TypeData.BYTE:
                        value_ = Convert.ToByte(initVal);
                        break;

                    case TypeData.CHAR:
                        value_ = Convert.ToChar(initVal);
                        break;

                    case TypeData.USHORT:
                        value_ = Convert.ToUInt16(initVal);
                        break;

                    case TypeData.SHORT:
                        value_ = Convert.ToInt16(initVal);
                        break;

                    case TypeData.UINT:
                        value_ = Convert.ToUInt32(initVal);
                        break;

                    case TypeData.INT:
                        value_ = Convert.ToInt32(initVal);
                        break;

                    case TypeData.FLOAT:
                        value_ = Convert.ToSingle(initVal);
                        break;

                    case TypeData.DOUBLE:
                        value_ = Convert.ToDouble(initVal);
                        break;

                    default:
                        value_ = Convert.ToInt32(initVal);
                        break;
                }

                //Kolor
                //Poczatkowy kolor
                Random random = new Random();
                Clr_ = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

                //Grubosc
                Width_ = 2;

                //Enebla dla graph
                GrEnable_ = true;

                GrVisible_ = true;

                GrVisibleTab_ = true;


                //Reset Value
                ResetValue();

            }
            catch (Exception Ex)
            {   
                throw new ApplicationException("ProjectContainer.Tag.Tag() :" + Ex.Message);;
            }
        }
        public InTag()
        {

        }

        /// <summary>
        /// Pobiera wielkość elementu w Byte na podstawie typu
        /// </summary>
        /// <param name="typeData"></param>
        /// <returns></returns>
        public int getSize()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return 1;

                    case TypeData.SBYTE:
                        return 8;

                    case TypeData.BYTE:
                        return 8;

                    case TypeData.CHAR:
                        return 16;

                    case TypeData.USHORT:
                        return 16;

                    case TypeData.SHORT:
                        return 16;

                    case TypeData.UINT:
                        return 32;

                    case TypeData.INT:
                        return 32;

                    case TypeData.FLOAT:
                        return 32;

                    case TypeData.DOUBLE:
                        return 64;

                    case ProjectDataLib.TypeData.ShortToReal:
                        return 16;

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        /// <summary>
        /// Pobiera type danych Taga
        /// </summary>
        /// <param name="typeData"></param>
        /// <returns></returns>
        public Type getOwnType()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return typeof(Boolean);

                    case TypeData.SBYTE:
                        return typeof(SByte);

                    case TypeData.BYTE:
                        return typeof(Byte);

                    case TypeData.CHAR:
                        return typeof(Char);

                    case TypeData.USHORT:
                        return typeof(ushort);

                    case TypeData.SHORT:
                        return typeof(short);

                    case TypeData.UINT:
                        return typeof(uint);

                    case TypeData.INT:
                        return typeof(int);

                    case TypeData.DOUBLE:
                        return typeof(double);

                    case TypeData.FLOAT:
                        return typeof(float);

                    case TypeData.ShortToReal:
                        return typeof(float);

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        /// <summary>
        /// Porownanie
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {

            if(obj is ITag)
                return Guid.Equals(this.objId,((ITag)obj).Id);
            return false;
        }

        /// <summary>
        /// Porownanie
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.objId.GetHashCode();
        }

        /// <summary>
        /// Metoda do sorotwania
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public int CompareTo(Tag b)
        {
            // Alphabetic sort name[A to Z]
            return this.tagName_.CompareTo(b.tagName);
        }

        private Color Clr_;
        /// <summary>
        /// Color for Editors
        /// </summary>
        [Category("05 Graph"), DisplayName("Color")][JsonIgnore][XmlElement(ElementName ="Color",Type =typeof(XmlColor))]
        public Color Clr
        {
            get { return Clr_; }
            set
            {
                Clr_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Clr"));
            }
        }
        
        private int Width_;
        [Category("05 Graph"), DisplayName("Line Width")][JsonIgnore]
        public int Width
        {
            get { return Width_; }
            set
            {
                Width_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Width"));
            }
        }

        private Boolean GrEnable_;
        [Category("05 Graph"), DisplayName("Enable")][JsonIgnore][XmlElement(ElementName ="ChartEnable")]
        public Boolean GrEnable
        {
            get { return GrEnable_; }
            set
            {
                GrEnable_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrEnable"));
            }
        }

        private Boolean GrVisible_;
        [Category("05 Graph"), DisplayName("Visible")][JsonIgnore][XmlElement(ElementName = "ChartVisible")]
        public Boolean GrVisible
        {
            get { return GrVisible_; }
            set
            {
                GrVisible_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrVisible"));
            }
        }

        private Boolean GrVisibleTab_;
        [Category("06 Table"), DisplayName("Visible")][JsonIgnore][XmlElement(ElementName = "TableVisible")]
        public Boolean GrVisibleTab
        {
            get { return GrVisibleTab_; }
            set
            {
                GrVisibleTab_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("GrVisibleTab"));
            }
        }

        //Dodatkowa Dana Dla JSON
        [Browsable(false)]
        public string formattedValue
        {
            get
            {
                if (Format_ == "{0:ASCII}")
                {
                    dynamic d = value_;
                    return Encoding.ASCII.GetString(BitConverter.GetBytes(d));
                }
                else
                {
                    return string.Format(Format, value);
                }
            }
        }

        [Browsable(false)]
        public string typeData { get { return Enum.GetName(typeof(TypeData), TypeData); } }

        [Browsable(false)]
        public string description { get { return describe; } }

        /// <summary>
        /// Funckja Resetuje Wartosc
        /// </summary>
        /// <returns></returns>
        Boolean ResetValue()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        value = false;
                        break;

                    case TypeData.SBYTE:
                        value = Convert.ToSByte(0);
                        break;

                    case TypeData.BYTE:
                        value = Convert.ToByte(0);
                        break;

                    case TypeData.CHAR:
                        value = Convert.ToChar(0);
                        break;

                    case TypeData.USHORT:
                        value = Convert.ToUInt16(0);
                        break;

                    case TypeData.SHORT:
                        value = Convert.ToInt16(0);
                        break;

                    case TypeData.UINT:
                        value = Convert.ToUInt32(0);
                        break;

                    case TypeData.INT:
                        value = Convert.ToInt32(0);
                        break;

                    case TypeData.FLOAT:
                        value = Convert.ToSingle(0.0);
                        break;

                    case TypeData.DOUBLE:
                        value = Convert.ToDouble(0.0);
                        break;

                    case ProjectDataLib.TypeData.ShortToReal:
                        value = Convert.ToSingle(0.0);
                        break;

                    default:
                        value = 0;
                        break;
                }

                return true;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return false;

            }
        }

        /// <summary>
        /// Desarializacja obiektu
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {

            if (Clr_ == null)
                Clr_ = Color.Black;

            if (Width_ == 0)
                Width_ = 2;

            ResetValue();
        }
       
        /// <summary>
        /// Klonowanie obiektu
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            InTag Tg1 = (InTag)bf.Deserialize(ms);
            ms.Close();

            return Tg1;
        }

        //Interfejsc
        Guid ITag.Id
        {
            get { return objId; }
            set { objId = value; }
        }

        Guid ITag.ParentId
        {
            get { return Proj_.InternalTagsDrv.objId; }
            set { Proj_.InternalTagsDrv.objId = value; }
        }

        string ITag.Name
        {
            get { return tagName; }
            set { tagName = value; }
        }
        Boolean ITag.ActName
        {
            get { return true; }
        }

        int ITag.DevAdress
        {
            get { return 0; }
            set { ; }
        }
        Boolean ITag.ActDevAdress { get { return false; } }

        int ITag.Adress
        {
            get { return 0; }
            set { ; }
        }
        Boolean ITag.ActAdress { get { return false; } }

        int ITag.BitByte
        {
            get { return 0; }
            set { ; }
        }
        Boolean ITag.ActBitByte { get { return false; } }

        TypeData ITag.TypeData_
        {
            get { return TypeData; }
            set { TypeData = value; }
        }
        Boolean ITag.ActTypeData_ { get { return true; } }

        string ITag.AreaData
        {
            get { return string.Empty; }
            set { ;}
        }
        Boolean ITag.ActAreaData { get { return false; } }

        Object ITag.Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
            }
        }
        Boolean ITag.ActValue { get { return true; } }

        string ITag.Description
        {
            get { return describe; }
            set { describe = value; }
        }
        Boolean ITag.ActDscription { get { return true; } }

        Boolean ITag.ActParam
        {
            get { return false; }
        }

        Boolean ITag.ActSetValue
        {
            get { return false; }
        }
        void ITag.SetValue(Object val)
        {
            try
            {
                this.value = val;
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.SetValue() :" + Ex.Message);
            }

        }

        Type ITag.getOwnType()
        {
            try
            {
                switch (this.TypeData_)
                {
                    case TypeData.BIT:
                        return typeof(Boolean);

                    case TypeData.SBYTE:
                        return typeof(SByte);

                    case TypeData.BYTE:
                        return typeof(Byte);

                    case TypeData.CHAR:
                        return typeof(Char);

                    case TypeData.USHORT:
                        return typeof(ushort);

                    case TypeData.SHORT:
                        return typeof(short);

                    case TypeData.UINT:
                        return typeof(uint);

                    case TypeData.INT:
                        return typeof(int);

                    case TypeData.DOUBLE:
                        return typeof(double);

                    case TypeData.FLOAT:
                        return typeof(float);

                    case TypeData.ShortToReal:
                        return typeof(float);

                    default:
                        throw new ApplicationException("typeData isn't appropriate!");
                }
            }
            catch (Exception Ex)
            {
                throw new ApplicationException("ProjectContainer.Tag.getSize() :" + Ex.Message);
            }
        }

        string ITag.GetFormatedValue()
        {
            try
            {
                if (string.IsNullOrEmpty(Format_))
                {

                    if(TypeData_ == TypeData.BIT)
                    {
                        if ((bool)value_)
                            return "1";
                        else
                            return "0";
                    }
                    else
                    return value.ToString();
                }
                else if (Format_ == "{0:ASCII}")
                {
                    dynamic d = value_;
                    return Encoding.ASCII.GetString(BitConverter.GetBytes(d));
                }
                else
                {
                    if (TypeData_ == TypeData.BIT)
                    {
                        if ((bool)value_)
                            return "1";
                        else
                            return "0";
                    }
                    else
                        return string.Format(Format, value);
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));

                return value.ToString();
            }
        }

        bool ITag.ActClr
        {
            get { return true; }
        }

        Color ITag.Clr
        {
            get
            {
                return Clr;
            }
            set
            {
                Clr = value;
            }
        }

        bool ITag.ActWidth
        {
            get { return true; }
        }

        int ITag.Width
        {
            get
            {
                return Width;
            }
            set
            {
                Width = value;
            }
        }

        bool ITag.ActGrEnable
        {
            get { return true; }
        }

        bool ITag.GrEnable
        {
            get
            {
                return GrEnable;
            }
            set
            {
                GrEnable = value;
            }
        }

        bool ITag.ActGrVisible
        {
            get { return  true; }
        }

        bool ITag.GrVisible
        {
            get
            {
                return GrVisible;
            }
            set
            {
                GrVisible = value;
            }
        }

        [Obsolete("Stare")]
        bool ITag.ActGrMarkers
        {
            get { return true; }
        }

        bool ITag.ActGrVisibleTab
        {
            get { return true; }
        }

        bool ITag.GrVisibleTab
        {
            get
            {
                return GrVisibleTab;
            }
            set
            {
                GrVisibleTab = value;
            }
        }

        int ITag.BlockAdress
        {
            get
            {
                return 0;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        bool ITag.ActBlockAdress
        {
            get
            {
                return false;
            }
        }

        BytesOrder ITag.BytesOrder_
        {
            get
            {
                return BytesOrder.ABCD;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        bool ITag.ActBytesOrder_
        {
            get
            {
                return false;
            }
        }

        //IDriverModel

        bool IDriverModel.activateCycle(List<ITag> tagsList)
        {
            return idrv.activateCycle(tagsList);
        }

        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            return idrv.addTagsComm(tagList);
        }

        bool IDriverModel.removeTagsComm(List<ITag> tagList)
        {
            return idrv.removeTagsComm(tagList);
        }

        bool IDriverModel.reConfig()
        {
            return idrv.reConfig();
        }

        bool IDriverModel.deactivateCycle()
        {
            return idrv.deactivateCycle();
        }

        string IDriverModel.FormatFrameRequest(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameRequest(frame, num);
        }

        string IDriverModel.FormatFrameResponse(byte[] frame, NumberStyles num)
        {
            return idrv.FormatFrameResponse(frame, num);
        }

        byte[] IDriverModel.sendBytes(byte[] data)
        {
            return idrv.sendBytes(data);
        }

        Guid IDriverModel.ObjId
        {
            get
            {
                return idrv.ObjId;
            }
        }

        string IDriverModel.driverName
        {
            get
            {
                return idrv.driverName;
            }
        }

        object IDriverModel.setDriverParam
        {
            get
            {
                return idrv.setDriverParam;
            }

            set
            {
                idrv.setDriverParam = value;
            }
        }

        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get
            {
                return idrv.MemoryAreaInf;
            }
        }

        bool IDriverModel.isAlive
        {
            get
            {
                return idrv.isAlive;
            }

            set
            {
                idrv.isAlive = value;
                //zdarzenia
                propChanged?.Invoke(this, new PropertyChangedEventArgs("isAlive"));
            }
        }

        bool IDriverModel.isBusy
        {
            get
            {
                return idrv.isBusy;
            }
        }

        object[] IDriverModel.plugins
        {
            get
            {
                return idrv.plugins;
            }
        }

        bool[] IDriverModel.AuxParam
        {
            get
            {
                return idrv.AuxParam;
            }
        }

        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return new ObservableCollection<object>();
            }

            set
            {
                
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return tagName;
            }

            set
            {
                tagName = value;
            }
        }
        bool ITreeViewModel.IsExpand
        {
            get
            {
                return false;
            }

            set
            {
               
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return ((IDriverModel)Proj.InternalTagsDrv).isAlive;

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
                return Proj_.InternalTagsDrv.IsBlocked;
            }
            set
            {
                propChanged?.Invoke(this, new PropertyChangedEventArgs("IsBlocked"));
            }
        }
    }

    [Serializable]
    public class ChartViewConf : INotifyPropertyChanged
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

        //Osie wykresu
        double maximumX_;
        [XmlElement(ElementName = "ChartX1",Type =typeof(DateTime))]
        public DateTime maximumX
        {
            get
            {
                return   DateTime.FromOADate( double.IsNaN(maximumX_) ? 0 : maximumX_);
            }
            set
            {
                maximumX_ =  value.ToOADate();
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maximumX)));
         
            }
        }

        double minimumX_;
        [XmlElement(ElementName = "ChartX0", Type = typeof(DateTime))]
        public DateTime minimumX
        {
            get
            {
                return DateTime.FromOADate(double.IsNaN(minimumX_) ? 0 : minimumX_);
            }
            set
            {
                minimumX_ = value.ToOADate();
                propChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(minimumX)));
               
            }
        }

        double maximumY_;
        [XmlElement(ElementName = "ChartY1")]
        public double maximumY
        {
            get
            {
                return maximumY_;
            }
            set
            {
                maximumY_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maximumY)));
            }
        }

        double minimumY_;
        [XmlElement(ElementName = "ChartY0")]
        public double minimumY
        {
            get
            {
                return minimumY_;
            }
            set
            {
                minimumY_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minimumY)));
            }
        }

        private bool PanelIsExpand_;
        public bool PanelIsExpand
        {
            get { return PanelIsExpand_; }
            set
            {
                PanelIsExpand_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PanelIsExpand)));
            }
        }


        //Konstruktor
        public ChartViewConf()
        {

            //Osie
            maximumX = DateTime.Now;
            minimumX = DateTime.Now;

            maximumY = double.NaN;
            minimumY = double.NaN;

            TrackSpan = new TimeSpan(0, 0, 15);

            From = DateTime.Now;
            To = DateTime.Now;
        }

        //ChartView
        [OptionalField]
        private TimeSpan TrackSpan_;
        [XmlElement(ElementName ="ChartShowLast",Type = typeof(XmlTimeSpan))]
        public TimeSpan TrackSpan
        {
            get { return TrackSpan_; }
            set
            {
                TrackSpan_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TrackSpan"));
            }
        }

        private DateTime From_;
        [XmlElement(ElementName = "ChartFrom")]
        public DateTime From
        {
            get { return From_; }
            set
            {
                From_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("From"));
            }
        }

        private DateTime To_;
        [XmlElement(ElementName = "ChartTo")]
        public DateTime To
        {
            get { return To_; }
            set
            {
                To_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("To"));
            }
        }

        private bool histData_;
        [XmlElement(ElementName ="ChartDatabaseMode")]
        public bool histData
        {
            get { return histData_; }
            set
            {
                histData_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("histData"));
            }
        }

        //Communication View

        private bool CvCol1_;
        [XmlElement(ElementName = "CommViewColumn1Visibility")]
        public bool CvCol1
        {
            get { return CvCol1_; }
            set
            {
                CvCol1_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol1"));
            }
        }

        private bool CvCol2_;
        [XmlElement(ElementName = "CommViewColumn2Visibility")]
        public bool CvCol2
        {
            get { return CvCol2_; }
            set
            {
                CvCol2_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol2"));
            }
        }

        private bool CvCol3_;
        [XmlElement(ElementName = "CommViewColumn3Visibility")]
        public bool CvCol3
        {
            get { return CvCol3_; }
            set
            {
                CvCol3_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol3"));
            }
        }

        private bool CvCol4_;
        [XmlElement(ElementName = "CommViewColumn4Visibility")]
        public bool CvCol4
        {
            get { return CvCol4_; }
            set
            {
                CvCol4_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol4"));
            }
        }

        private bool CvCol5_;
        [XmlElement(ElementName = "CommViewColumn5Visibility")]
        public bool CvCol5
        {
            get { return CvCol5_; }
            set
            {
                CvCol5_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol5"));
            }
        }

        private bool CvCol6_;
        [XmlElement(ElementName = "CommViewColumn6Visibility")]
        public bool CvCol6
        {
            get { return CvCol6_; }
            set
            {
                CvCol6_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol6"));
            }
        }

        private bool CvCol7_;
        [XmlElement(ElementName = "CommViewColumn7Visibility")]
        public bool CvCol7
        {
            get { return CvCol7_; }
            set
            {
                CvCol7_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("CvCol7"));
            }
        }

        //Table View

        private bool TvCol1_;
        [XmlElement(ElementName = "TableColumn1Visibility")]
        public bool TvCol1
        {
            get { return TvCol1_; }
            set
            {
                TvCol1_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol1"));
            }
        }

        private bool TvCol2_;
        [XmlElement(ElementName = "TableColumn2Visibility")]
        public bool TvCol2
        {
            get { return TvCol2_; }
            set
            {
                TvCol2_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol2"));
            }
        }

        private bool TvCol3_;
        [XmlElement(ElementName = "TableColumn3Visibility")]
        public bool TvCol3
        {
            get { return TvCol3_; }
            set
            {
                TvCol3_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol3"));
            }
        }

        private bool TvCol4_;
        [XmlElement(ElementName = "TableColumn4Visibility")]
        public bool TvCol4
        {
            get { return TvCol4_; }
            set
            {
                TvCol4_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol4"));
            }
        }

        private bool TvCol5_;
        [XmlElement(ElementName = "TableColumn5Visibility")]
        public bool TvCol5
        {
            get { return TvCol5_; }
            set
            {
                TvCol5_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol5"));
            }
        }

        private bool TvCol6_;
        [XmlElement(ElementName = "TableColumn6Visibility")]
        public bool TvCol6
        {
            get { return TvCol6_; }
            set
            {
                TvCol6_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol6"));
            }
        }

        private bool TvCol7_;
        [XmlElement(ElementName = "TableColumn7Visibility")]
        public bool TvCol7
        {
            get { return TvCol7_; }
            set
            {
                TvCol7_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol7"));
            }
        }

        private bool TvCol8_;
        [XmlElement(ElementName = "TableColumn8Visibility")]
        public bool TvCol8
        {
            get { return TvCol8_; }
            set
            {
                TvCol8_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol8"));
            }
        }

        private bool TvCol9_;
        [XmlElement(ElementName = "TableColumn9Visibility")]
        public bool TvCol9
        {
            get { return TvCol9_; }
            set
            {
                TvCol9_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol9"));
            }
        }

        private bool TvCol10_;
        [XmlElement(ElementName = "TableColumn10Visibility")]
        public bool TvCol10
        {
            get { return TvCol10_; }
            set
            {
                TvCol10_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol10"));
            }
        }

        private bool TvCol11_;
        [XmlElement(ElementName = "TableColumn11Visibility")]
        public bool TvCol11
        {
            get { return TvCol11_; }
            set
            {
                TvCol11_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol11"));
            }
        }

        private bool TvCol12_;
        [XmlElement(ElementName = "TableColumn12Visibility")]
        public bool TvCol12
        {
            get { return TvCol12_; }
            set
            {
                TvCol12_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol12"));
            }
        }

        private bool TvCol13_;
        [XmlElement(ElementName = "TableColumn13Visibility")]
        public bool TvCol13
        {
            get { return TvCol13_; }
            set
            {
                TvCol13_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol13"));
            }
        }

        private bool TvCol14_;
        [XmlElement(ElementName = "TableColumn14Visibility")]
        public bool TvCol14
        {
            get { return TvCol14_; }
            set
            {
                TvCol14_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("TvCol14"));
            }
        }

        [OnDeserializing]
        private void OnDeserialized(StreamingContext context)
        {
            if (From.Ticks == 0)
                From = DateTime.Now;

            if (To.Ticks == 0)
                To = DateTime.Now;

            if (TrackSpan.Ticks == 0)
                TrackSpan = new TimeSpan(0, 0, 15);
        }

        public void OnDeserializedXML()
        {
            if (From.Ticks == 0)
                From = DateTime.Now;

            if (To.Ticks == 0)
                To = DateTime.Now;

            if (TrackSpan.Ticks == 0)
                TrackSpan = new TimeSpan(0, 0, 15);
        }
    }

    /// <summary>
    /// Dostepne konversje tagów na platformie fenix
    /// </summary>
    public enum TypeData { BIT, BYTE, SBYTE, CHAR, DOUBLE, FLOAT, INT, UINT, SHORT, USHORT, ShortToReal };

    /// <summary>
    /// Porządek bajtów 
    /// </summary>
    public enum BytesOrder { BADC,ABCD, DCBA};

    /// <summary>
    /// Encja reprezentująca elementy wystepujące w projekcie
    /// </summary>
    public enum ElementKind { Empty,Project ,Connection,Scripts, Device ,Tag, HttpConfig, InternalsTags,ScriptFile ,IntTag, InFile};

    /// <summary>
    /// Klasa informacyjna
    /// </summary>
    public class ProjectEventArgs : EventArgs
    {
        /// <summary>
        /// Element przesyłany
        /// </summary>
        public object element;
        public object element0;
        public object element1;
        public object element2;

        /// <summary>
        /// Konstruktor1
        /// </summary>
        /// <param name="obj"></param>
        public ProjectEventArgs(object obj)
        {
            this.element = obj;
        }

        /// <summary>
        /// Konstrukor2
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        public ProjectEventArgs(object obj,object obj0)
        {
            this.element = obj;
            this.element0 = obj0;
        }

        /// <summary>
        /// Konstruktor 3
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        /// <param name="obj1"></param>
        public ProjectEventArgs(object obj, object obj0,object obj1)
        {
            this.element = obj;
            this.element0= obj0;
            this.element1 = obj1;
        }

        /// <summary>
        /// Konstruktor4
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public ProjectEventArgs(object obj, object obj0, object obj1, object obj2)
        {
            this.element = obj;
            this.element0 = obj0;
            this.element1 = obj1;
            this.element2 = obj2;

        }
    }

    /// <summary>
    /// Klasa obslugująca sterowniki
    /// </summary>
    /// <summary>
    /// Klasa globalnej konfiguracji softu
    /// </summary>
    public class GlobalConfiguration
    {
        /// <summary>
        /// Bufor na sciezki dostępu do bibliotek
        /// </summary>
        public List<String>  assmemblyPath= new List<string>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        public GlobalConfiguration()
        {
            try
            {
                //Odczytanie pliku konfiguracujnego z sterownikami
                openData(Application.StartupPath + "\\GlobalConfiguration.xml");
            }
            catch (Exception)
            {
                //Wyszukaj i dodaj sterowniki
                autoSearchDrv();
            }

        }

        /// <summary>
        /// Zapisz konfiguracje
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Boolean saveData(string path)
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(List<String>));
                Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                xml.Serialize(fs, assmemblyPath);
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Otwarcie listy sterowników
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Boolean openData(string path)
        {
            try
            {
                if (!File.Exists(path))
                  throw new ApplicationException(path + " File does't exist");

                XmlSerializer xml = new XmlSerializer(typeof(List<String>));
                Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                assmemblyPath = (List<String>)xml.Deserialize(fs);
                fs.Close();

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Testowanie na podstawie klasy Asemmbly. True jesli wszystko OK
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public Boolean checkAssembly(Assembly asm)
        {
            try
            {
                //Pobranie typu driver z biblioteki
                Type tp = asm.GetType("nmDriver.Driver");

                //Sprawdzenie czy taki typ znajduję sie biblotece
                if (tp == null)
                    return false;

                //Sprawdzenie czy typ obsluguje IDriverModel interfejs
                Type it = tp.GetInterface("IDriverModel");
                if (it == null)
                    return false;

                //Wychodzi na to że sterownik jest poprawny
                return true;
            }
            catch (Exception)
            {
                throw; ;
            }

        }

        /// <summary>
        /// Testowanie biloteki za pomoca sciezki dostępu
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Boolean checkAssembly(String path)
        {
            try
            {
                Assembly asm = Assembly.LoadFile(path);
                return checkAssembly(asm);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Dodanie manualnie sterownika
        /// </summary>
        public void addDrvMan(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                //Bibloteka
                Assembly asm = Assembly.LoadFile(path);

                //Sprawdzenie bibloteki
                if (!checkAssembly(asm))
                {
                    MessageBox.Show("Library isn't appropriate!");
                    return;
                }
                else
                {
                    //Wszystko ok można dodać 
                    assmemblyPath.Add(path);
                    saveData("GlobalConfiguration.xml");
                }
            }
            else
            {
                MessageBox.Show("Path to Driver is empty!");
            }
        }

        /// <summary>
        /// Usuniecie sterownika manualnie
        /// </summary>
        public void removeDrv(string path)
        {
            try
            {
                assmemblyPath.Remove(path);
                saveData(Application.StartupPath + "\\GlobalConfiguration.xml");
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GlobalConfiguration.removeDrv :" + Ex.Message);
            }
            
        }

        /// <summary>
        /// Autowyszukiwanie steronikow w folderu rozruchowym
        /// </summary>
        public void autoSearchDrv()
        {
            DirectoryInfo dInfo = new DirectoryInfo(Application.StartupPath);
            FileInfo[] paths = dInfo.GetFiles("*.dll");

            //Z resotowanie 
            assmemblyPath.Clear();

            //Szukanie danych
            foreach (FileInfo s in paths)
            {
                try
                {
                    if (checkAssembly(s.FullName))
                        addDrvMan(s.FullName);
                }
                catch (Exception)
                {
                    
                }
            }
        }

        /// <summary>
        /// Pobranie listy nazw dostepnych sterownikow
        /// </summary>
        /// <returns></returns>
        [Obsolete("Przestarzała funkcja. Trzeba użyć GetDrivers ")]
        public string[] getDrvLst()
        {
            //Bufor na nazwy
            List<string> buff = new List<string>();

            //Petla obiegowa
            foreach (string s in assmemblyPath)
            {
                if (!String.IsNullOrEmpty(s))
                {

                    //Sprawdzenie czy plik nie istnieje
                    if (File.Exists(s))
                    {
                        //Zaladowanie bibliote
                        Assembly asm = Assembly.LoadFile(s);

                        //Sprawdzenie czy jest obslugiwany interfejs
                        if (!checkAssembly(asm))
                        {
                            MessageBox.Show("Assembly: " + asm.FullName + " Isn't appropriate!");
                            return null;
                        }
                        else
                        {
                            //Jezeli bibloteka jest prawidlowa ładuj do pamieci sterownika
                            Type tp = asm.GetType("nmDriver.Driver");

                            IDriverModel idrv = (IDriverModel)asm.CreateInstance(tp.FullName);
                            buff.Add(idrv.driverName);
                        }
                    }
 
                }
            }

            return buff.ToArray();
        }

        public List<IDriverModel> GetDrivers()
        {
            List<IDriverModel> buff = new List<IDriverModel>();

            foreach(string s in assmemblyPath)
            {
                if(!string.IsNullOrEmpty(s))
                {
                    //Sprawdzenie czy plik nie istnieje
                    if (File.Exists(s))
                    {
                        //Zaladowanie bibliote
                        Assembly asm = Assembly.LoadFile(s);

                        //Sprawdzenie czy jest obslugiwany interfejs
                        if (!checkAssembly(asm))
                        {
                            MessageBox.Show("Assembly: " + asm.FullName + " Isn't appropriate!");
                            return null;
                        }
                        else
                        {
                            //Jezeli bibloteka jest prawidlowa ładuj do pamieci sterownika
                            Type tp = asm.GetType("nmDriver.Driver");

                            IDriverModel idrv = (IDriverModel)asm.CreateInstance(tp.FullName);
                            buff.Add(idrv);
                        }
                    }
                }
            }

            return buff;
        }

        /// <summary>
        /// Instancja sterownika
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDriverModel newDrv(string name)
        {
            try
            {
                //Kopia
                IDriverModel idrv = null;

                //Sprawdzenie czy w danej sciezce jest nazwa sterownika
                string path = assmemblyPath.Find(x => x.Contains(name));

                //Wyslanie nowej kopi
                if (File.Exists(path))
                {
                    Assembly asDriver = Assembly.LoadFile(path);
                    Type tpDriver = asDriver.GetType("nmDriver.Driver");
                    idrv = (IDriverModel)asDriver.CreateInstance(tpDriver.FullName);

                    return idrv;
                }
                else
                {
                    MessageBox.Show("Please Install Driver: " + name);
                    return null;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem with loading driver: " + name);
                return null;
            }
        }
    }

    /// <summary>
    /// Moja klasa zmiany nazwy parametrów dla parametrow sterownika
    /// </summary>
    internal class DanConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Zmiana wyswietlaniej nazwy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            //Gdy podczas konwersji pojawi się nazwa Object nie wyświtlaj nic
            if (destType == typeof(string) && value is Object)
                return " ";

            return base.ConvertTo(context, culture, value, destType);
        }
    }

    /// <summary>
    /// Moja klasa zmiany nazwy parametrów dla parametrow sterownika
    /// </summary>
    internal class NoName : ExpandableObjectConverter
    {
        /// <summary>
        /// Zmiana wyswietlaniej nazwy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
                return " ";
        }
    }

    /// <summary>
    /// Blokowanie rozszerzonej adresaxji
    /// </summary>
    public class BlockConverter : UInt16Converter
    {
        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {

            //Pobranie danych
            Device dev = (Device)context.Instance;

            //Rozszerzona adresacja
            return !dev.idrv.AuxParam[0];
        }
    }

    /// <summary>
    /// Wybor timerow dla taga
    /// </summary>
    public class InTagsTimers : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            InTag tag = ((InTag)context.Instance);

            //Dodanie wyswitlanych warotosci
            List<String> buff = new List<string>() { String.Empty };
            buff.AddRange(tag.Proj.InternalTagsDrv.Timers.Select(x => x.Name));

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(buff);
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w propGrid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }

    /// <summary>
    /// Wybor timerow dla taga
    /// </summary>
    public class ScriptFileTimers : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            ScriptFile tag = ((ScriptFile)context.Instance);

            //Dodanie wyswitlanych warotosci
            List<String> buff = new List<string>() { String.Empty };
            buff.AddRange(tag.Proj.ScriptEng.Timers.Select(x => x.Name));

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(buff);
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w propGrid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class MemoryAreaModbus : StringConverter
    {

        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(idrv.MemoryAreaInf.Select(x=>x.Name).ToArray());
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w propGrid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class TagSecendaryAdress : Int32Converter
    {

        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;

            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Obszar pamięci
            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            //Lista dostępnych opcji
            List<int> opcje = new List<int>();

            //Wypalnianie Secand Adress
            if (mInf.AdresSize > tg.getSize())
            {
                //Dodanie elementu do wyboru
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                //Brak opcji
                opcje.Add(0);
            }

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(opcje.ToArray());
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w propGrid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
           return true;
        }

        /// <summary>
        /// Proba konwersji
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;

            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Obszar pamięci
            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            //Lista dostępnych opcji
            List<int> opcje = new List<int>();
            if (mInf.AdresSize > tg.getSize())
            {
                //Dodanie elementu do wyboru
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                //Brak opcji
                opcje.Add(0);
            }

            //Konwersja
            if (value is string)
            {
                try
                {
                    //Konwersja pobranej wartości
                    int val = int.Parse((string)value);

                    //Sprawdzenie zakresu
                    if (val < 0 || val > opcje.Count - 1)
                        return 0;
                    else
                        return val;

                }
                catch (Exception)
                {
                    return 0;
                }
             
            }

            return 0;

        }
    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class BlockAdressVisibility : Int32Converter
    {
        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;
            return !tg.idrv.AuxParam[1];
        }
    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid dla rodzaju sterowników
    /// </summary>
    public class DriverKind : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            String[] sterowniki = ((Connection)context.Instance).gConf.getDrvLst();

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(sterowniki);
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w proprtyGrid tylko lista
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid dla rodzaju sterowników
    /// </summary>
    public class FormatValueConv : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {

            ITag tg = (ITag)context.Instance;

            List<String> lista = new List<string>();
            lista.Add("{0:0.0}");
            lista.Add("{0:0.00}");
            lista.Add("{0:0.000}");
            lista.Add("{0:00.000}");
            lista.Add("{0:000.000}");
            lista.Add("{0:0000.0000}");
            lista.Add("{0:ASCII}");

            if (tg.TypeData_ == TypeData.BYTE)
            lista.Add("{0:X4}");

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(lista.ToArray());
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            if (tg.TypeData_ == TypeData.BIT)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            if (tg.TypeData_ == TypeData.BIT)
                return false;
            else
                return true;
        }

    }

    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid dla rodzaju sterowników
    /// </summary>
    public class EmptyConverter : StringConverter
    {
        /// <summary>
        /// Zabepzievznie
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Analiza tego co jest do wyswietlenia
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //Zabezpieczenie przeciwko System.String
            if (destinationType.FullName == "System.String" || destinationType.FullName == "(Collections)")
                return String.Empty;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Blokowanie autoryacji 
    /// </summary>
    public class BlockAuthConverter : StringConverter
    {
        /// <summary>
        /// Kolekcja stadardowa
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] 
            { 
                "Anonymous", 
                "Basic"
            });
        }

        /// <summary>
        /// Czy pokazywac standardowe wartosci
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Powrot do typu
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Konwetuj dane
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (AuthenticationSchemes)Enum.Parse(typeof(AuthenticationSchemes), (string)value);
        }
    }

    /// <summary>
    /// Klasa inforujaca o danym obszarze pamięci w sterowniku
    /// </summary>
    [Serializable]
    public class MemoryAreaInfo : INotifyPropertyChanged
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

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Nazwa Obszaru pamięci</param>
        /// <param name="moduleSizeReq">Rozmiar w bitach ządania</param>
        /// <param name="moduleSizeRes">Odpowiedz w bitach sterownika</param>
        public MemoryAreaInfo(string name, int adresSize, int ctCode)
        {
            //Nazwa
            Name = name;

            //Rozmiar jednostkowego rejestru
            AdresSize = adresSize;
            
            //Kod
            fctCode = ctCode;
        }

        string Name_;
        /// <summary>
        /// Nazwa obszaru
        /// </summary>
        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private int fctCode_;
        /// <summary>
        /// Kod fukcji danego bszaru
        /// </summary>
        public int fctCode
        {
            get { return fctCode_; }
            set
            {
                fctCode_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fctCode)));
            }
        }
        
        int AdresSize_;
        /// <summary>
        /// Rozmiar elementu zapytania
        /// </summary>
        public int AdresSize
        {
            get { return AdresSize_; }
            set
            {
                AdresSize_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdresSize)));
            }
        }
    }

    /// <summary>
    /// Adress Taga
    /// </summary>
    public struct TagAdress
    {
        public int adress;
        public int secAdress;

        //Konstruktor
        public TagAdress(int adress , int secAdress)
        {
            this.adress = adress;
            this.secAdress = secAdress;
        }
    }

    /// <summary>
    /// Klasa zarzadnia zamykaniem poloczen
    /// </summary>
    [Serializable]
    public class WindowsStatus
    {
        public int index;
        public Boolean Visible;
        public Boolean Live;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ind"></param>
        /// <param name="vis"></param>
        /// <param name="liv"></param>
        public WindowsStatus(int ind, Boolean vis, Boolean liv)
        {
            this.index = ind;
            this.Visible = vis;
            this.Live = liv;
        }

        public WindowsStatus()
        {

        }

    }

    /// <summary>
    /// Klasa pomocnicza dla punktow
    /// </summary>
    public class PO
    {
        //Start
        int X_;
        public int X
        {
            set { X_ = value; }
            get { return X_; }
        }

        //Stop
        int Y_;
        public int Y
        {
            set { Y_ = value; }
            get { return Y_; }
        }

        int BlockNum_;
        public int BlockNum
        {
            get { return BlockNum_; }
            set { BlockNum_ = value; }
        }
        //konstruktor
        public PO(int x, int y)
        {
            this.X_ = x;
            this.Y_ = y;
        }
    }

    #region HTTP Server

    /// <summary>
    /// Webserver
    /// </summary>
    [Serializable]
    public class WebServer : IDisposable, ITreeViewModel, INotifyPropertyChanged
    {
        [field:NonSerialized][XmlIgnore]
        public HttpListener _listener = new HttpListener();

        [field: NonSerialized]
        private ProjectContainer projCon_;
        [Browsable(false)][XmlIgnore]
        public ProjectContainer PrCon
        {
            get { return projCon_; }
            set { projCon_ = value; }
        }

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
        private Project Proj_;
        [Browsable(false)][XmlIgnore]
        public Project Proj
        {
            get { return Proj_; }
            set { Proj_ = value; }
        }

        [field:NonSerialized][XmlIgnore]
        public Func<HttpListenerContext, byte[]> _responderMethod;

        Boolean IsExpand_;
        [Browsable(false)]
        public Boolean IsExpand
        {
            get { return IsExpand_; }
            set { IsExpand_ = value; }
        }

        private Guid ObjId_;
        [Browsable(false)][XmlElement(ElementName ="Id")]
        public Guid ObjId
        {
            get { return ObjId_; }
            set { ObjId_ = value; }
        }

        private List<string> Prefixes_ = new List<string>();
        [Browsable(true), Category("03 Adresses"), DisplayName("Prefixes"), Description("Allowed Adress")]
        [TypeConverter(typeof(EmptyConverter))]
        public string[] Prefixes
        {
            get 
            {
                try
                {
                    return _listener.Prefixes.Where(x => x != String.Empty).ToArray();
                }
                catch (Exception) { return null; }
                   
            }
            set 
            {
                try
                {
                    _listener.Prefixes.Clear();
                    Prefixes_.Clear();

                    foreach (string s in value)
                        Prefixes_.Add(s);

                    foreach (string s in value)
                       _listener.Prefixes.Add(s);

                }
                catch (Exception)
                {
                    
                }
                
            }
        }

        private List<UserClass> Users_ = new List<UserClass>();
        [Browsable(true), Category("01 Users"), DisplayName("Users")][XmlElement(ElementName ="Users",Type =typeof(List<UserClass>))]
        [TypeConverter(typeof(EmptyConverter))]
        public List<UserClass> Users
        {
            get { return Users_; }
            set 
            {
                foreach (UserClass s in value)
                    Users_.Add(s);
            }
        }
     
        private AuthenticationSchemes Auth_;
        [Browsable(true), Category("02 Authentication"), DisplayName("Schema"), Description("Authentication Schema")]
        [TypeConverter(typeof(BlockAuthConverter))]
        public AuthenticationSchemes Auth
        {
            get 
            {
                return _listener.AuthenticationSchemes; 
            }
            set 
            { 
                Auth_ = value;
                _listener.AuthenticationSchemes = value;
            }
        }

        public ITreeViewModel DirSearch(string sDir, ITreeViewModel El)
        {
            ITreeViewModel buff = null;

            foreach (ITreeViewModel d in El.Children)
            {
                if (((CusFile)d).FullName == sDir)
                    return d;

                foreach (ITreeViewModel f in d.Children)
                {
                    if (((CusFile)f).FullName == sDir)
                        return f;

                    buff  = DirSearch(sDir, d);

                    if (buff != null)
                        return buff;
                }              
            }

            return null;        
        }

        private Boolean Active_;
        [Browsable(false)]
        public Boolean Acitve
        {
            get { return Active_; }
        }

        //Wszystkie pliki
        [field:NonSerialized]
        ObservableCollection<object> _Children;
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
                return "HttpServer";
            }

            set
            {
               
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
                return false;
            }
            set { }
        }

        bool ITreeViewModel.IsBlocked
        {
            get
            {
                return false;
            }
            set { }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="prefixes"></param>
        /// <param name="method"></param>
        public WebServer(Func<HttpListenerContext, byte[]> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "WebServer.Coinstr  - Needs Windows XP SP2, Server 2003 or later.");

            //Przypisanie
            ObjId_ = new Guid("11111111-1111-1111-1111-111111111111");

            //Utworzenie Serwera i utworzenie standardowych paramatrow
            _listener = new HttpListener();
            Prefixes_.Add("http://+:80/");
            _listener.Prefixes.Add("http://+:80/");

            //Autoryzacja
            Auth = AuthenticationSchemes.Anonymous;

            //
            _Children = new ObservableCollection<object>();
     
            //Przypisanie metody odbiorczej
            if(method != null)
            _responderMethod = method;
        }

        public WebServer()
        {

        }

        /// <summary>
        /// Start
        /// </summary>
        public void Run()
        {   
            
            //Start
            _listener.Start();
            Active_ = true;

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {   
                try
                {
                    while (_listener.IsListening)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;

                            try
                            {
                                byte[] buf = _responderMethod(ctx);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { Active_ = false; } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            try
            {
                _listener.Stop();
                Active_ = false;
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Metoda wywolana po serializacji
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "WebServer.Coinstr  - Needs Windows XP SP2, Server 2003 or later.");

            //Init
            ObjId_ = new Guid("11111111-1111-1111-1111-111111111111");

            //Utworzenie obiektu
            _listener = new HttpListener();

            //Zapisanie auth
            _listener.AuthenticationSchemes = Auth_;

            //Zabezpiecznie gdy jest pusty
            if (Prefixes_ == null)
            {
                Prefixes_ = new List<string>();
                Prefixes_.Add("http://+:80/");
                _listener.Prefixes.Add("http://+:80/");
            }
            else
            {
                if (Prefixes_.Count == 0)
                {
                    Prefixes_.Add("http://+:80/");
                    _listener.Prefixes.Add("http://+:80/");
                }
                else
                {
                    foreach (string s in Prefixes_)
                        _listener.Prefixes.Add(s);
                }
            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_listener == null)
                        return;

                    if (_listener.IsListening)
                    {
                        _listener.Stop();
                        _listener.Close();
                    }
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
      
        }
        #endregion

    }

    /// <summary>
    /// Klasa autoryzacyjna
    /// </summary>
    [Serializable]
    public class UserClass : INotifyPropertyChanged
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

        private string Name_;
        /// <summary>
        /// Nazwa
        /// </summary>
        [Browsable(true), Category("Identy"), DisplayName("01 User Name")][XmlElement(ElementName ="Login")]
        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string Pass_;
        /// <summary>
        /// Haslo
        /// </summary>
        [Browsable(true), Category("Identy"), DisplayName("02 Password")][XmlElement(ElementName = "Password")]
        [PasswordPropertyText(true)]
        public string Pass
        {
            get { return Pass_; }
            set
            {
                Pass_ = value; propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pass)));
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        public UserClass()
        {
            this.Name_ = "User";
            this.Pass_ = "Password";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        public UserClass(string name, string pass)
        {
            this.Name_ = name;
            this.Pass_ = pass;
        }

        public override string ToString()
        {
            return Name ?? "User";
        }
    }

     #endregion

    #region Interfejsy

    /// <summary>
    /// Parametry dla edytorow typu Table. Poloaczenie InTag i Tag
    /// </summary>
    public interface ITag
    {

        Guid Id { get; set; }

        Guid ParentId { get; set; }

        string Name{get;set;}
        Boolean ActName { get; }

        int DevAdress{get;set;}
        Boolean ActDevAdress{get;}

        int BlockAdress { get; set; }
        Boolean ActBlockAdress { get; }

        int Adress { get; set; }
        Boolean ActAdress { get; }

        int BitByte{get;set;}
        Boolean ActBitByte { get; }

        TypeData TypeData_{get;set;}
        Boolean ActTypeData_{get;}

        BytesOrder BytesOrder_ { get; set; }
        Boolean ActBytesOrder_ { get; }

        string AreaData{get;set;}
        Boolean ActAreaData{get;}

        Object Value{get;set;}
        Boolean ActValue{get;}

        string Description{get;set;}
        Boolean ActDscription{get;}

        Boolean ActParam { get; }

        Boolean ActSetValue { get; }
        void SetValue(object val);

        Boolean ActClr { get; }
        Color Clr { get; set; }

        Boolean ActWidth { get; }
        int Width { get;set; }

        Boolean ActGrEnable { get; }
        /// <summary>
        /// Wlaczenie wyswietlania w Edytorze Graph
        /// </summary>
        Boolean GrEnable { get; set; }

        Boolean ActGrVisible { get; }
        Boolean GrVisible { get; set; }

        Boolean ActGrMarkers { get; }

        /// <summary>
        /// Informacja czy parametr jest aktywny
        /// </summary>
        Boolean ActGrVisibleTab { get; }
        
        /// <summary>
        /// Widocznosc w tablei
        /// </summary>
        Boolean GrVisibleTab { get; set; }

        Type getOwnType();

        string GetFormatedValue();
    }

    #endregion

    /// <summary>
    /// Intefejs zostal stworzony by tworzyc interakcje edytor project
    /// </summary>
    public interface IEditorData
    {
        //Pobiera project
        Project Pr { get; }
    }

    /// <summary>
    /// Klasa jednego alarmu
    /// </summary>
    public class AlarmEvent
    {
        public AlarmEvent(string Msg)
        {
            this.Mess = Msg;
            Tm = DateTime.Now;
        }

        public DateTime Tm{get;set;}
        public String Mess { get; set; }
        public string frDateTime { get { return Tm.ToShortDateString() + " " + Tm.ToShortTimeString(); } }
    }

    /// <summary>
    /// Wszystkie dane
    /// </summary>
    public class GraphElement
    {
        /// <summary>
        /// Name
        /// </summary>
        public string label;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="lab"></param>
        public GraphElement(string lab)
        {
            label = lab;
        }

        /// <summary>
        /// Dane prywante
        /// </summary>
        private List<object[]> dane = new List<object[]>();

        /// <summary>
        /// Publiczne dane
        /// </summary>
        public List<object[]> data { get { return dane; } }

        /// <summary>
        /// Add point to
        /// </summary>
        /// <param name="d"></param>
        public void addDataPoint(object[] d)
        {
            dane.Add(d);
        }

        /// <summary>
        /// Usun element o numerze;
        /// </summary>
        /// <param name="nr"></param>
        public void removeElement(int nr)
        {
            if(dane.Count > 0)
                dane.RemoveAt(nr);
        }
    }

    public enum EventType {IN, OUT, INFO, ERROR };

    public class EventElement
    {
        public EventElement(Project pr, object sender, byte[] d, DateTime t, string s, EventType type, EventElement el, string nm)
        {

            Name = nm;
            Sender = sender;
            Dane = d;
            Czas = t;
            Info = s;
            Type = type;
            Pr = pr;

            if(el != null)
            {
                RefTime = Convert.ToInt32((t - el.Czas).TotalMilliseconds);
            }
        }

        public string Name { get; set; }
        public object Sender { get; set; }
        public DateTime Czas { get; set; }
        public byte[] Dane { get; set; }
        public string Info { get; set; }
        public EventType Type { get; set; }
        public Project Pr { get; set; }
        public int RefTime { get; set; }
    }

    public class DatabaseValues
    {

        public DatabaseValues()
        {
            TimePts = new List<double>();
            ValPts = new List<double>();
        }

        public string Name { get; set; }
        public List<double> TimePts { get; set; }
        public List<double> ValPts { get; set; }

    }

    public interface ITreeViewModel
    {
        ObservableCollection<Object> Children { get; set; }    
        string Name { get; set; }
        bool IsExpand { get; set; }
        bool IsLive { get; set; }
        bool IsBlocked { get; set; }
        Color Clr { get; set; }
    }

    public interface ITableView
    {
        ObservableCollection<ITag> Children { get; set; }
    }

    public interface IDriversMagazine
    {
        ObservableCollection<IDriverModel> Children { get; set; }
    }

    //Klasa sluzy do zbierania danych w FenixManagerWPF
    public class CustomException
    {
        public DateTime Czas { get; set; }
        public CustomException(object sender, Exception ex)
        {
            Ex = ex;
            Sender = sender;
            Czas = DateTime.Now;
        }
        public Exception Ex { get; set; }
        public object Sender { get; set; }
    }

    //Klasa sluzaca do wyswietlania informacji ze skryptów
    public class Information : Exception
    {      
       public Information(string s) : base(s)
       { }

        public override string ToString()
        {
            return "-";        
        }
    }

    //Dodatkowy element
    public class CusFile : ITreeViewModel, INotifyPropertyChanged
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

        [Browsable(false)]
        public bool IsFile { get; set; }

        private string FullName_;
        [DisplayName("File Path")][Category("01 Design"),ReadOnly(true)]
        public string FullName
        {
            get { return FullName_; }
            set
            {
                FullName_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        //Ctor1
        public CusFile(DirectoryInfo d)
        {
       
            FullName = d.FullName;
            var SubDir = (from x in d.GetDirectories() select new CusFile(x)).ToList();
            SubDir.AddRange(from x in d.GetFiles() select new CusFile(x));
            Children_ =  new ObservableCollection<object>(SubDir);
        }
       
        //Ctor2
        public CusFile(FileInfo f)
        {
            IsFile = true;
            Children_ = new ObservableCollection<object>();
            FullName = f.FullName;
        }

        //Kolekcja
        ObservableCollection<Object> Children_;
        ObservableCollection<object> ITreeViewModel.Children
        {
            get
            {
                return Children_;
            }

            set
            {
                Children_ = value;
            }
        }


        private bool IsBlocked_;
        [Browsable(false)]
        public bool IsBlocked
        {
            get
            {
                return IsBlocked_;
            }
            set
            {
                IsBlocked_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBlocked)));
            }
        }


        private bool IsExpand_;
        [Browsable(false)]
        public bool IsExpand
        {
            get { return IsExpand_; }
            set
            {
                IsExpand_ = value;
                propChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpand)));
            }
        }

        bool ITreeViewModel.IsLive
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string ITreeViewModel.Name
        {
            get
            {
                return Path.GetFileName(FullName);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        Color ITreeViewModel.Clr
        {
            get { return Color.White; }
            set { }
        }
    }

    [Serializable]
    public class TcpDriverParam
    {
        /// <summary>
        /// Nazwa Portu COM
        /// </summary>
        IPAddress Ip_;
        [Category("Target")]
        [DisplayName("Adress Ip")]
        public String Ip
        {
            get { return Ip_.ToString(); }
            set
            {
                try { Ip_ = IPAddress.Parse(value); }
                catch (Exception Ex) { MessageBox.Show(Ex.Message); }
            }
        }

        ushort Port_;
        [Category("Target")]
        [DisplayName("Port")]
        public ushort Port
        {
            set { Port_ = value; }
            get { return Port_; }
        }

        int Timeout_;
        [Category("Time")]
        [DisplayName("Timeout [ms]")]
        public int Timeout
        {
            get { return Timeout_; }
            set { Timeout_ = value; }
        }

        int ReplyTime_;
        [Category("Time")]
        [DisplayName("Reply Time [ms]")]
        public int ReplyTime
        {
            get { return ReplyTime_; }
            set { ReplyTime_ = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public TcpDriverParam()
        {
            Ip_ = IPAddress.Parse("127.0.0.1");
            Port_ = 502;
            Timeout_ = 1000;
            ReplyTime_ = 1500;
        }

        public override string ToString()
        {
            return " ";
        }
    }

    [Serializable]
    public class IoDriverParam
    {
        /// <summary>
        /// Nazwa Portu COM
        /// </summary>
        string PortName_;
        [Category("COM Settings")]
        [DisplayName("Port Name")]
        public string PortName
        {
            get { return PortName_; }
            set { PortName_ = value; }
        }

        int BaundRate_;
        [Category("COM Settings")]
        [DisplayName("Baund Rate[B/s]")]
        public int BoundRate
        {
            set { BaundRate_ = value; }
            get { return BaundRate_; }
        }

        Parity parity_;
        [Category("COM Settings")]
        [DisplayName("Parity")]
        public Parity parity
        {
            get { return parity_; }
            set { parity_ = value; }
        }

        int DataBits_;
        [Category("COM Settings")]
        [DisplayName("Data Bits")]
        public int DataBits
        {
            get { return DataBits_; }
            set { DataBits_ = value; }
        }

        StopBits stopBits_;
        [Category("COM Settings")]
        [DisplayName("Stop Bits")]
        public StopBits stopBits
        {
            get { return stopBits_; }
            set { stopBits_ = value; }
        }

        int Timeout_;
        [Category("COM Settings")]
        [DisplayName("Timeout [ms]")]
        public int Timeout
        {
            get { return Timeout_; }
            set { Timeout_ = value; }
        }

        int ReplyTime_;
        [Category("COM Settings")]
        [DisplayName("Reply Time [ms]")]
        public int ReplyTime
        {
            get { return ReplyTime_; }
            set { ReplyTime_ = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public IoDriverParam()
        {
            //Port Name
            PortName_ = "COM1";
            BaundRate_ = 9600;
            parity_ = Parity.None;
            DataBits_ = 8;
            stopBits_ = StopBits.One;
            Timeout_ = 1000;
            ReplyTime_ = 1500;
        }
    }

    [Serializable]
    public class S7DriverParam
    {
        /// <summary>
        /// Nazwa Portu COM
        /// </summary>
        IPAddress Ip_;
        [Category("Target")]
        [DisplayName("Adress Ip")]
        public String Ip
        {
            get { return Ip_.ToString(); }
            set
            {
                try { Ip_ = IPAddress.Parse(value); }
                catch (Exception Ex) { MessageBox.Show(Ex.Message); }
            }
        }

        ushort Port_;
        [Category("Target")]
        [DisplayName("Port")]
        public ushort Port
        {
            set { Port_ = value; }
            get { return Port_; }
        }

        private int Rack_;
        [Category("Target")]
        [DisplayName("Rack")]
        public int Rack
        {
            get { return Rack_; }
            set { Rack_ = value; }
        }

        private int Slot_;
        [Category("Target")]
        [DisplayName("Slot")]
        public int Slot
        {
            get { return Slot_; }
            set { Slot_ = value; }
        }

        int Timeout_;
        [Category("Time")]
        [DisplayName("Timeout [ms]")]
        public int Timeout
        {
            get { return Timeout_; }
            set { Timeout_ = value; }
        }

        int ReplyTime_;
        [Category("Time")]
        [DisplayName("Reply Time [ms]")]
        public int ReplyTime
        {
            get { return ReplyTime_; }
            set { ReplyTime_ = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public S7DriverParam()
        {
            Ip_ = IPAddress.Parse("127.0.0.1");
            Port_ = 102;
            Rack_ = 0;
            Slot_ = 2;
            Timeout_ = 1000000;
            ReplyTime_ = 1500;
        }
    }

    public class XmlColor
    {
        private Color color_ = Color.Black;

        public XmlColor() { }
        public XmlColor(Color c) { color_ = c; }

        public Color ToColor()
        {
            return color_;
        }

        public void FromColor(Color c)
        {
            color_ = c;
        }

        public static implicit operator Color(XmlColor x)
        {
            return x.ToColor();
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        [XmlAttribute]
        public string Val
        {
            get { return ColorTranslator.ToHtml(color_); }
            set
            {
                try
                {
                    if (Alpha == 0xFF) // preserve named color value if possible
                        color_ = ColorTranslator.FromHtml(value);
                    else
                        color_ = Color.FromArgb(Alpha, ColorTranslator.FromHtml(value));
                }
                catch (Exception)
                {
                    color_ = Color.Black;
                }
            }
        }

        [XmlAttribute]
        public byte Alpha
        {
            get { return color_.A; }
            set
            {
                if (value != color_.A) // avoid hammering named color if no alpha change
                    color_ = Color.FromArgb(value, color_);
            }
        }

        public bool ShouldSerializeAlpha() { return Alpha < 0xFF; }
    }

    public class XMLVersion
    {
        private Version ver_ = new Version();

        public XMLVersion() { }
        public XMLVersion(Version c) { ver_ = c; }

        public Version ToVersion()
        {
            return ver_;
        }

        public void FromVersion(Version c)
        {
            ver_ = c;
        }

        public static implicit operator Version(XMLVersion x)
        {
            return x.ToVersion();
        }

        public static implicit operator XMLVersion(Version c)
        {
            return new XMLVersion(c);
        }

        [XmlAttribute]
        public string Ver
        {
            get { return ver_.ToString(); }
            set
            {
                try
                {
                    ver_ = new Version(value);
                }
                catch (Exception)
                {
                    ver_ = new Version();
                }
            }
        }
    }

    public class XmlTimeSpan
    {
        private const long TICKS_PER_MS = TimeSpan.TicksPerMillisecond;

        private TimeSpan m_value = TimeSpan.Zero;

        public XmlTimeSpan() { }
        public XmlTimeSpan(TimeSpan source) { m_value = source; }

        public static implicit operator TimeSpan? (XmlTimeSpan o)
        {
            return o == null ? default(TimeSpan?) : o.m_value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan? o)
        {
            return o == null ? null : new XmlTimeSpan(o.Value);
        }

        public static implicit operator TimeSpan(XmlTimeSpan o)
        {
            return o == null ? default(TimeSpan) : o.m_value;
        }

        public static implicit operator XmlTimeSpan(TimeSpan o)
        {
            return o == default(TimeSpan) ? null : new XmlTimeSpan(o);
        }

        [XmlText]
        public long Default
        {
            get { return m_value.Ticks / TICKS_PER_MS; }
            set { m_value = new TimeSpan(value * TICKS_PER_MS); }
        }
    }

}


