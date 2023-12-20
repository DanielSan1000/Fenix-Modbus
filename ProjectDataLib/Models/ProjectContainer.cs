using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProjectDataLib
{
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

        #endregion Fields

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

        #endregion Konstruktor

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
                    throw new ApplicationException(String.Format("Guid: {0} isn't sign project", Id));

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

                if (Path.GetExtension(path).Contains("psf"))
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    FileStream fileStr = new FileStream(path, FileMode.OpenOrCreate);
                    buff = (Project)binForm.Deserialize(fileStr);
                    fileStr.Close();
                    path = Path.ChangeExtension(path, "psx");
                }

                #endregion PSF (BINARY)

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

                #endregion PSX (XML)

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
                        throw new ApplicationException(pr.projectName + ": " + "Project is alredy open.");

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
                    fil.FilePath = Path.GetDirectoryName(buff.path) + this.HttpCatalog + "\\" + fil.Name;

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
                    if (IsXml)
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

                    #endregion Project

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

                        if (cutMarks)
                            removeConnection(SrcProject, SrcElement);

                        break;

                    #endregion Connection

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

                    #endregion Device

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

                        #endregion Tag
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

        #endregion Project

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
                if (con.Idrv == null)
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
                List<Device> tagFolder = pr.DevicesList.Where(x => x.parentId.Equals(con)).ToList();

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

        #endregion Connection

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

        #endregion Device

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

        #endregion Driver

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

        #endregion Tag

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

        #endregion InTag

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

        #endregion ITag

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
                Project pr = getProject(projId);
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

                ((ITreeViewModel)pr.WebServer1).Children.Remove(GetInFile(projId, file));

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

        #endregion InFile

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

                if (sf != null)
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

        #endregion ScriptFile

        public override string ToString()
        {
            return "ProjectContainer";
        }
    }
}