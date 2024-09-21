using MahApps.Metro.Controls;
using Microsoft.Win32;
using ProjectDataLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using io = System.IO;
using wf = System.Windows.Forms;

namespace FenixWPF
{
    public partial class FenixMenager : MetroWindow, INotifyPropertyChanged
    {
        #region Visibilty

        private Boolean mFile_;
        private Boolean mNew_;
        private Boolean mOpen_;
        private Boolean mAdd_;
        private Boolean mConnection_;
        private Boolean mDevice_;
        private Boolean mTag_;
        private Boolean mIntTag_;
        private Boolean mScriptFile_;
        private Boolean mFolder_;
        private Boolean mInFile_;
        private Boolean mClosePr_;
        private Boolean mSave_;
        private Boolean mSaveAs_;
        private Boolean mExit_;

        private Boolean mEdit_;
        private Boolean mCut_;
        private Boolean mCopy_;
        private Boolean mPaste_;
        private Boolean mDelete_;

        private Boolean mView_;
        private Boolean mSolution_;
        private Boolean mProperties_;
        private Boolean mOutput_;
        private Boolean mTable_;
        private Boolean mChart_;
        private Boolean mCommView_;
        private Boolean mEditor_;

        private Boolean mDriversSt_;
        private Boolean mStart_;
        private Boolean mStop_;
        private Boolean mStartAll_;
        private Boolean mStopAll_;

        private Boolean mTools_;
        private Boolean mBlock_;
        private Boolean mUnBlock_;
        private Boolean mSimulate_;
        private Boolean mShowLoc_;
        private Boolean mDrivers_;

        private Boolean mDatabase_;
        private Boolean mDbShowFile_;
        private Boolean mDbReset_;
        private Boolean mShowDb_;
        private Boolean mShowTrendDb_;
        private Boolean mSaveCSV_;

        private Boolean mHelp_;
        private Boolean mUpdates_;
        private Boolean mAbout_;
        private Boolean mViewHelp_;

        public Boolean mFile
        { get { return mFile_; } set { mFile_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mFile")); } }

        public Boolean mNew
        { get { return mNew_; } set { mNew_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mNew")); } }

        public Boolean mOpen
        { get { return mOpen_; } set { mOpen_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mOpen")); } }

        public Boolean mAdd
        { get { return mAdd_; } set { mAdd_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mAdd")); } }

        public Boolean mConnection
        { get { return mConnection_; } set { mConnection_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mConnection")); } }

        public Boolean mDevice
        { get { return mDevice_; } set { mDevice_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDevice")); } }

        public Boolean mTag
        { get { return mTag_; } set { mTag_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mTag")); } }

        public Boolean mIntTag
        { get { return mIntTag_; } set { mIntTag_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mIntTag")); } }

        public Boolean mScriptFile
        { get { return mScriptFile_; } set { mScriptFile_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mScriptFile")); } }

        public Boolean mFolder
        { get { return mFolder_; } set { mFolder_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mFolder")); } }

        public Boolean mInFile
        { get { return mInFile_; } set { mInFile_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mInFile")); } }

        public Boolean mClosePr
        { get { return mClosePr_; } set { mClosePr_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mClosePr")); } }

        public Boolean mSave
        { get { return mSave_; } set { mSave_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mSave")); } }

        public Boolean mSaveAs
        { get { return mSaveAs_; } set { mSaveAs_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mSaveAs")); } }

        public Boolean mExit
        { get { return mExit_; } set { mExit_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mExit")); } }

        public Boolean mEdit
        { get { return mEdit_; } set { mEdit_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mEdit")); } }

        public Boolean mCut
        { get { return mCut_; } set { mCut_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mCut")); } }

        public Boolean mCopy
        { get { return mCopy_; } set { mCopy_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mCopy")); } }

        public Boolean mPaste
        { get { return mPaste_; } set { mPaste_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mPaste")); } }

        public Boolean mDelete
        { get { return mDelete_; } set { mDelete_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDelete")); } }

        public Boolean mView
        { get { return mView_; } set { mView_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mView")); } }

        public Boolean mSolution
        { get { return mSolution_; } set { mSolution_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mSolution")); } }

        public Boolean mProperties
        { get { return mProperties_; } set { mProperties_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mProperties")); } }

        public Boolean mOutput
        { get { return mOutput_; } set { mOutput_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mOutput")); } }

        public Boolean mTable
        { get { return mTable_; } set { mTable_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mTable")); } }

        public Boolean mChart
        { get { return mChart_; } set { mChart_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mChart")); } }

        public Boolean mCommView
        { get { return mCommView_; } set { mCommView_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mCommView")); } }

        public Boolean mEditor
        { get { return mEditor_; } set { mEditor_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mEditor")); } }

        public Boolean mDriversSt
        { get { return mDriversSt_; } set { mDriversSt_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDriversSt")); } }

        public Boolean mStart
        { get { return mStart_; } set { mStart_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mStart")); } }

        public Boolean mStop
        { get { return mStop_; } set { mStop_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mStop")); } }

        public Boolean mStartAll
        { get { return mStartAll_; } set { mStartAll_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mStartAll")); } }

        public Boolean mStopAll
        { get { return mStopAll_; } set { mStopAll_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mStopAll")); } }

        public Boolean mTools
        { get { return mTools_; } set { mTools_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mTools")); } }

        public Boolean mBlock
        { get { return mBlock_; } set { mBlock_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mBlock")); } }

        public Boolean mUnBlock
        { get { return mUnBlock_; } set { mUnBlock_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mUnBlock")); } }

        public Boolean mSimulate
        { get { return mSimulate_; } set { mSimulate_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mSimulate")); } }

        public Boolean mShowLoc
        { get { return mShowLoc_; } set { mShowLoc_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mShowLoc")); } }

        public Boolean mDrivers
        { get { return mDrivers_; } set { mDrivers_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDrivers")); } }

        public Boolean mDatabase
        { get { return mDatabase_; } set { mDatabase_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDatabase")); } }

        public Boolean mDbShowFile
        { get { return mDbShowFile_; } set { mDbShowFile_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDbShowFile")); } }

        public Boolean mDbReset
        { get { return mDbReset_; } set { mDbReset_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mDbReset")); } }

        public Boolean mShowDb
        { get { return mShowDb_; } set { mShowDb_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mShowDb")); } }

        public Boolean mShowTrendDb
        { get { return mShowTrendDb_; } set { mShowTrendDb_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mShowTrendDb")); } }

        public Boolean mSaveCSV
        { get { return mSaveCSV_; } set { mSaveCSV_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mSaveCSV")); } }

        public Boolean mHelp
        { get { return mHelp_; } set { mHelp_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mHelp")); } }

        public Boolean mUpdates
        { get { return mUpdates_; } set { mUpdates_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mUpdates")); } }

        public Boolean mAbout
        { get { return mAbout_; } set { mAbout_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mAbout")); } }

        public Boolean mViewHelp
        { get { return mViewHelp_; } set { mViewHelp_ = value; propChanged_?.Invoke(this, new PropertyChangedEventArgs("mViewHelp")); } }

        #endregion Visibilty

        #region Fileds

        private ProjectContainer PrCon = new ProjectContainer();
        private ElementKind actualKindElement;

        private object SelObj;
        private Guid SelGuid;

        private Project Pr;
        private string pathRun = "";

        private string SelSrcPath = string.Empty;

        private LayoutAnchorable laPropGrid = new LayoutAnchorable();
        private PropertiesGridManager propManag = new PropertiesGridManager();

        private LayoutAnchorable laTvMain = new LayoutAnchorable();
        private TreeViewManager tvMain = new TreeViewManager();

        private LayoutAnchorGroup laGrOutput = new LayoutAnchorGroup();
        private LayoutAnchorable laOutput = new LayoutAnchorable();
        private Output frOutput;

        private ObservableCollection<CustomException> exList = new ObservableCollection<CustomException>();

        private io.FileSystemWatcher FsWatcher;

        #region External Events

        private void AddProjectEvent(object sender, ProjectEventArgs ev)
        {
            try
            {
                Project pr = (Project)ev.element;
                this.Pr = pr;

                #region Sprawdzenie czy istnieje zapisany layout

                if (io.File.Exists(io.Path.GetDirectoryName(PrCon.projectList.First().path) + "\\" + PrCon.LayoutFile))
                {
                    Project pp = (Project)sender;
                    XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);

                    serializer.LayoutSerializationCallback += (s, args) =>
                    {
                        string[] param = args.Model.ContentId.Split(';');
                        switch (param[0])
                        {
                            case "Properties":
                                args.Content = propManag;
                                break;

                            case "Solution":
                                args.Content = tvMain;
                                break;

                            case "Output":
                                args.Content = frOutput;
                                break;

                            case "Database":
                                //args.Content = new DbExplorer(null);
                                break;

                            case "TableView":
                                LayoutAnchorable laTableView = (LayoutAnchorable)args.Model;
                                laTableView.CanClose = true;
                                TableView tbView = new TableView(PrCon, pp.objId, Guid.Parse(param[1]), (ElementKind)Enum.Parse(typeof(ElementKind), param[2]), laTableView);
                                laTableView.Closed += LaCtrl_Closed;
                                args.Content = tbView;
                                break;

                            case "ChartView":
                                LayoutAnchorable laChartView = (LayoutAnchorable)args.Model;
                                laChartView.CanClose = true;
                                ChartView chView = new ChartView(PrCon, pp.objId, Guid.Parse(param[1]), (ElementKind)Enum.Parse(typeof(ElementKind), param[2]), laChartView);
                                laChartView.Closed += LaCtrl_Closed;
                                args.Content = chView;
                                break;

                            case "CommView":
                                LayoutAnchorable laCommView = (LayoutAnchorable)args.Model;
                                laCommView.CanClose = true;
                                CommunicationView comView = new CommunicationView(PrCon, pp.objId, Guid.Parse(param[1]), (ElementKind)Enum.Parse(typeof(ElementKind), param[2]), laCommView);
                                laCommView.Closed += LaCtrl_Closed;
                                args.Content = comView;
                                break;

                            case "Editor":
                                LayoutAnchorable laEditorView = (LayoutAnchorable)args.Model;
                                laEditorView.CanClose = true;

                                //Zabezpieczenie
                                if (io.File.Exists(param[1]))
                                {
                                    Editor edView = new Editor(PrCon, pp.objId, param[1], (ElementKind)Enum.Parse(typeof(ElementKind), param[2]), laEditorView);
                                    laEditorView.Closed += LaCtrl_Closed;
                                    args.Content = edView;
                                }
                                else
                                {
                                    laEditorView.Close();
                                }

                                break;

                            default:
                                args.Content = new System.Windows.Controls.TextBox() { Text = args.Model.ContentId };
                                break;
                        }
                    };

                    string ss = io.Path.GetDirectoryName(PrCon.projectList.First().path) + "\\" + PrCon.LayoutFile;
                    serializer.Deserialize(ss);
                }

                #endregion Sprawdzenie czy istnieje zapisany layout

                tvMain.View.DataContext = ((ITreeViewModel)PrCon).Children;
                tvMain.View.ItemsSource = ((ITreeViewModel)PrCon).Children;

                TreeViewItem PrNode = FindTviFromObjectRecursive(tvMain.View, pr);
                if (PrNode != null) PrNode.IsSelected = true;

                if (!io.Directory.Exists(io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog))
                    io.Directory.CreateDirectory(io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog);

                FsWatcher = new io.FileSystemWatcher(io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog);
                FsWatcher.IncludeSubdirectories = true;
                FsWatcher.Created += FsWatch_Changed;
                FsWatcher.Deleted += FsWatch_Changed;
                FsWatcher.Renamed += FsWatch_Changed;
                FsWatcher.EnableRaisingEvents = true;

                lbPathProject.Content = Pr.path;
                Registry.SetValue(PrCon.RegUserRoot, PrCon.LastPathKey, Pr.path);

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void FsWatch_Changed(object sender, io.FileSystemEventArgs e)
        {
            try
            {
                #region Renamed

                if (e.ChangeType == io.WatcherChangeTypes.Renamed)
                {
                    io.RenamedEventArgs k = (io.RenamedEventArgs)e;
                    ITreeViewModel m = Pr.WebServer1.DirSearch(k.OldFullPath, Pr.WebServer1);

                    if (m != null)
                        ((CusFile)m).FullName = k.FullPath;
                }

                #endregion Renamed

                #region Creted

                else if (e.ChangeType == io.WatcherChangeTypes.Created)
                {
                    if (io.File.Exists(e.FullPath))
                    {
                        string dir = io.Path.GetDirectoryName(e.FullPath);
                        if (dir == io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ((ITreeViewModel)Pr.WebServer1).Children.Add(new CusFile(new io.FileInfo(e.FullPath)));
                            }));
                        }
                        else
                        {
                            ITreeViewModel tm = Pr.WebServer1.DirSearch(dir, Pr.WebServer1);
                            if (tm != null)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    tm.Children.Add(new CusFile(new io.FileInfo(e.FullPath)));
                                }));
                            }
                        }
                    }
                    else
                    {
                        string rDir = io.Directory.GetParent(e.FullPath).FullName;
                        if (rDir == io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ((ITreeViewModel)Pr.WebServer1).Children.Add(new CusFile(new io.DirectoryInfo(e.FullPath)));
                            }));
                        }
                        else
                        {
                            ITreeViewModel tm = Pr.WebServer1.DirSearch(rDir, Pr.WebServer1);
                            if (tm != null)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    tm.Children.Add(new CusFile(new io.DirectoryInfo(e.FullPath)));
                                }));
                            }
                        }
                    }
                }

                #endregion Creted

                #region Delete

                if (e.ChangeType == System.IO.WatcherChangeTypes.Deleted)
                {
                    string sDir = io.Directory.GetParent(e.FullPath).FullName;
                    if (sDir == io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            var rem = ((ITreeViewModel)Pr.WebServer1).Children.ToList().Where(x => ((CusFile)x).FullName == e.FullPath).First();
                            ((ITreeViewModel)Pr.WebServer1).Children.Remove(rem);
                        }));
                    }
                    else
                    {
                        ITreeViewModel tm = Pr.WebServer1.DirSearch(e.FullPath, Pr.WebServer1);
                        ITreeViewModel par = Pr.WebServer1.DirSearch(sDir, Pr.WebServer1);

                        if (tm != null)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                (par).Children.Remove((CusFile)tm);
                            }));
                        }
                    }
                }

                #endregion Delete
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Error(object sender, EventArgs ev)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProjectEventArgs e = (ProjectEventArgs)ev;

                if (e.element is Exception)
                    exList.Add(new CustomException(sender, (Exception)e.element));
                else if (e.element2 is Exception)
                    exList.Add(new CustomException(sender, (Exception)e.element2));
                else
                    exList.Add(new CustomException(sender, new Exception(e.element1.ToString())));

                LayoutAnchorable lpAnchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(x => x.Title == "Output").First();
                lpAnchor.IsActive = true;
            });
        }

        public static TreeViewItem FindTviFromObjectRecursive(ItemsControl ic, object o)
        {
            //Search for the object model in first level children (recursively)
            TreeViewItem tvi = ic.ItemContainerGenerator.ContainerFromItem(o) as TreeViewItem;
            if (tvi != null) return tvi;
            //Loop through user object models
            foreach (object i in ic.Items)
            {
                //Get the TreeViewItem associated with the iterated object model
                TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                tvi = FindTviFromObjectRecursive(tvi2, o);
                if (tvi != null) return tvi;
            }
            return null;
        }

        #endregion External Events

        private PropertyChangedEventHandler propChanged_;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                propChanged_ += value;
            }

            remove
            {
                propChanged_ -= value;
            }
        }

        #endregion Fileds

        #region Konstruktor

        public FenixMenager()
        {
            InitializeComponent();

            this.DataContext = this;

            //PropertyGrid

            laPropGrid.Content = propManag;
            laPropGrid.Title = "Properties";
            laPropGrid.ContentId = "Properties";
            RightPan.Children.Add(laPropGrid);

            //TreeView

            tvMain.View.SelectedItemChanged += View_SelectedItemChanged;

            laTvMain.Content = tvMain;
            laTvMain.Title = "Solution";
            laTvMain.ContentId = "Solution";
            LeftPan.Children.Add(laTvMain);

            //Exceptions
            frOutput = new Output(PrCon, exList);
            laOutput.Title = "Output";
            laOutput.Content = frOutput;
            laOutput.ContentId = "Output";
            laGrOutput.Children.Add(laOutput);
            BottomPan.Children.Add(laGrOutput);

            //frOutput.View
            frOutput.View.DataContext = exList;
            frOutput.View.ItemsSource = exList;

            Title = "Fenix Manager " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            PrCon.addProjectEv += new EventHandler<ProjectEventArgs>(AddProjectEvent);

            PrCon.ApplicationError += new EventHandler(Error);

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    Title = Title + " (Administrator)";
            }

            checkAccess();

            string[] s = Environment.GetCommandLineArgs();
            if (s.Length > 1)
            {
                if (io.File.Exists(s[1]))
                {
                    PrCon.openProjects(s[1]);
                    Pr = PrCon.projectList.First();
                    Registry.SetValue(PrCon.RegUserRoot, PrCon.LastPathKey, Pr.path);
                }
            }
        }

        #endregion Konstruktor

        #region Internal Commands

        private void checkAccess()
        {
            try
            {
                #region Nic nie jest zaznaczone

                if (tvMain.View.SelectedItem == null)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = false;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = false;
                    mSave = false;
                    mSaveAs = false;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = false;
                    mStart = false;
                    mStop = false;
                    mStartAll = false;
                    mStopAll = false;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = false;
                    mDbShowFile = false;
                    mDbReset = false;
                    mShowDb = false;
                    mShowTrendDb = false;
                    mSaveCSV = false;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;
                }

                #endregion Nic nie jest zaznaczone

                #region Zaznaczony Projekt

                if (tvMain.View.SelectedItem is Project)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = true;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = PrCon.SrcType == ElementKind.Connection ? true : false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = true;
                    mChart = true;
                    mCommView = true;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = true;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;
                }

                #endregion Zaznaczony Projekt

                #region InternalTags

                if (tvMain.View.SelectedItem is InternalTagsDriver)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = true;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = (!((ITreeViewModel)SelObj).IsBlocked && !((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStop = (!((ITreeViewModel)SelObj).IsBlocked && ((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = !((ITreeViewModel)SelObj).IsBlocked;
                    mUnBlock = ((ITreeViewModel)SelObj).IsBlocked;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion InternalTags

                #region Scripts

                if (tvMain.View.SelectedItem is ScriptsDriver)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = true;
                    mFolder = false;
                    mInFile = true;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = (!((ITreeViewModel)SelObj).IsBlocked && !((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStop = (!((ITreeViewModel)SelObj).IsBlocked && ((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = !((ITreeViewModel)SelObj).IsBlocked;
                    mUnBlock = ((ITreeViewModel)SelObj).IsBlocked;
                    mShowLoc = true;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion Scripts

                #region File

                if (tvMain.View.SelectedItem is CusFile)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = !((CusFile)tvMain.View.SelectedItem).IsFile;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = !((CusFile)tvMain.View.SelectedItem).IsFile;
                    mInFile = !((CusFile)tvMain.View.SelectedItem).IsFile;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = ((CusFile)tvMain.View.SelectedItem).IsFile;
                    mCopy = ((CusFile)tvMain.View.SelectedItem).IsFile;
                    mPaste = !((CusFile)tvMain.View.SelectedItem).IsFile && PrCon.SrcType == ElementKind.InFile;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = ((CusFile)tvMain.View.SelectedItem).IsFile;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = !((CusFile)tvMain.View.SelectedItem).IsFile;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion File

                #region ScriptFile

                if (tvMain.View.SelectedItem is ScriptFile)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = false;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = true;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = !((ITreeViewModel)SelObj).IsBlocked;
                    mUnBlock = ((ITreeViewModel)SelObj).IsBlocked;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion ScriptFile

                #region IntTag

                if (tvMain.View.SelectedItem is InTag)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = false;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion IntTag

                #region Database

                if (tvMain.View.SelectedItem is DatabaseModel)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = false;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = false;
                    mSave = false;
                    mSaveAs = false;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = false;
                    mStart = false;
                    mStop = false;
                    mStartAll = false;
                    mStopAll = false;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;
                }

                #endregion Database

                #region Connection

                if (tvMain.View.SelectedItem is Connection)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = false;
                    mDevice = true;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mInFile = false;
                    mFolder = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = true;
                    mCopy = true;
                    mPaste = PrCon.SrcType == ElementKind.Device ? true : false;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = true;
                    mChart = true;
                    mCommView = true;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = (!((ITreeViewModel)SelObj).IsBlocked && !((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStop = (!((ITreeViewModel)SelObj).IsBlocked && ((ITreeViewModel)SelObj).IsLive) ? true : false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = !((ITreeViewModel)SelObj).IsBlocked;
                    mUnBlock = ((ITreeViewModel)SelObj).IsBlocked;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion Connection

                #region HttpServer

                if (tvMain.View.SelectedItem is WebServer)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = true;
                    mInFile = true;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = PrCon.SrcType == ElementKind.InFile;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = true;
                    mSimulate = true;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;
                }

                #endregion HttpServer

                #region Device

                if (tvMain.View.SelectedItem is Device)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = true;
                    mConnection = false;
                    mDevice = false;
                    mTag = true;
                    mScriptFile = false;
                    mFolder = false;
                    mIntTag = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = true;
                    mCopy = true;
                    mPaste = PrCon.SrcType == ElementKind.Tag ? true : false;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = true;
                    mChart = true;
                    mCommView = true;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion Device

                #region Zaznaczone Tag

                if (tvMain.View.SelectedItem is Tag)
                {
                    mFile = true;
                    mNew = true;
                    mOpen = true;
                    mAdd = false;
                    mConnection = false;
                    mDevice = false;
                    mTag = false;
                    mIntTag = false;
                    mScriptFile = false;
                    mFolder = false;
                    mInFile = false;
                    mClosePr = true;
                    mSave = true;
                    mSaveAs = true;
                    mExit = true;

                    mEdit = false;
                    mCut = true;
                    mCopy = true;
                    mPaste = false;
                    mDelete = true;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    mTable = false;
                    mChart = false;
                    mCommView = false;
                    mEditor = false;

                    mDriversSt = true;
                    mStart = false;
                    mStop = false;
                    mStartAll = true;
                    mStopAll = true;

                    mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    mShowLoc = false;
                    mSimulate = false;
                    mDrivers = true;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = true;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = true;
                    mViewHelp = true;

                    propManag.View.Enabled = !((ITreeViewModel)SelObj).IsLive;
                }

                #endregion Zaznaczone Tag

                #region Trwa Komunikacja

                if (PrCon.anyCommunication())
                {
                    if (SelObj == null)
                        return;

                    mFile = true;
                    mNew = false;
                    mOpen = false;
                    mAdd = true;
                    mConnection = !((ITreeViewModel)SelObj).IsLive;
                    mDevice = !((ITreeViewModel)SelObj).IsLive;
                    mTag = !((ITreeViewModel)SelObj).IsLive;
                    mIntTag = !((ITreeViewModel)SelObj).IsLive;
                    mScriptFile = false;
                    mFolder = true;
                    mInFile = !((ITreeViewModel)SelObj).IsLive;
                    mClosePr = false;
                    mSave = true;
                    mSaveAs = false;
                    mExit = true;

                    mEdit = false;
                    mCut = false;
                    mCopy = false;
                    mPaste = false;
                    mDelete = false;

                    mView = true;
                    mSolution = true;
                    mProperties = true;
                    mOutput = true;
                    //mTable = false;
                    //mChart = false;
                    //mCommView = false;
                    //mEditor = false;

                    // mDriversSt = true;
                    //mStart = (!((ITreeViewModel)SelObj).IsBlocked && !((ITreeViewModel)SelObj).IsLive) ? true : false;
                    //mStop = (!((ITreeViewModel)SelObj).IsBlocked && ((ITreeViewModel)SelObj).IsLive) ? true : false;
                    //mStartAll = true;
                    //mStopAll = true;

                    //mTools = true;
                    mBlock = false;
                    mUnBlock = false;
                    //mShowLoc = true;
                    //mSimulate = false;
                    mDrivers = false;

                    mDatabase = true;
                    mDbShowFile = true;
                    mDbReset = false;
                    mShowDb = true;
                    mShowTrendDb = true;
                    mSaveCSV = true;

                    mHelp = true;
                    mAbout = true;
                    mUpdates = false;
                    mViewHelp = true;
                }

                #endregion Trwa Komunikacja
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void DeleteElementMethod(Guid projId, Guid id, ElementKind elKind)
        {
            try
            {
                //Brak projektów w Buforze
                if (PrCon.projectList.Count == 0)
                    return;

                //Chcemy usunąc projekt

                wf.DialogResult result = wf.MessageBox.Show("Do you really want delate this element", "Warning", wf.MessageBoxButtons.OKCancel, wf.MessageBoxIcon.Warning);
                if (result == wf.DialogResult.OK)
                {
                    PrCon.deleteElement(Pr.objId, id, elKind);
                    return;
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        [Obsolete("Trzeba to zmienić")]
        private void updateSoftware(object sender)
        {
            Version newVersion = null;
            string url = "";
            XmlTextReader reader = null;

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    lbInfo.Content = "No Internet connection.";
                }));

                return;
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                lbInfo.Content = "Checking update for software...";
            }));

            try
            {
                string xmlURL = "https://sourceforge.net/projects/fenixmodbus/files/version.xml";
                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";

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

                this.Dispatcher.Invoke(new Action(() =>
                {
                    lbInfo.Content = "Completed";
                }));
                checkVersion(newVersion, url, (Boolean)sender);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        private void checkVersion(Version newVersion, string url, bool automatic)
        {
            // get the running version
            Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            // compare the versions
            if (curVersion.CompareTo(newVersion) < 0)
            {
                // ask the user if he would like
                // to download the new version
                string title = "New version detected.";
                string question = "Download the new version Fenix " + newVersion.ToString() + " ?";

                if (wf.DialogResult.Yes == wf.MessageBox.Show(question, title, wf.MessageBoxButtons.YesNo, wf.MessageBoxIcon.Question))
                {
                    // navigate the default web
                    // browser to our app
                    // homepage (the url
                    // comes from the xml content)
                    Process.Start(url);
                }
            }
            else
            {
                if (automatic)
                {
                    MessageBox.Show("Your version is actual");
                }
            }
        }

        #endregion Internal Commands

        #region Internal Events

        private void New0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //dodanie projektu
                AddProject fr = new AddProject(PrCon);
                fr.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Open0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pr != null)
                {
                    MessageBox.Show("Project is already load. Please close project and try again!");
                    return;
                }

                string startupPath = io.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                string strp = (string)Registry.GetValue(PrCon.RegUserRoot, PrCon.LastPathKey, startupPath + "\\Project.psf");
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = io.Path.GetDirectoryName(strp);
                ofd.Filter = "XML Files (*.psx)|*.psx";

                if (ofd.ShowDialog() == true)
                {
                    PrCon.openProjects(ofd.FileName);
                    Pr = PrCon.projectList.First();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Connection0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddConnection addConnection_ = new AddConnection(PrCon, PrCon.gConf, Pr.objId);
                addConnection_.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Device0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddDevice addTagFolder_ = new FenixWPF.AddDevice(PrCon, Pr.objId, SelGuid);
                addTagFolder_.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Tag0_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                AddTag addTag_ = new FenixWPF.AddTag(ref PrCon, Pr.objId, SelGuid);

                //reakcja USERA
                addTag_.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void IntTag0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FenixWPF.AddInTag aTag = new FenixWPF.AddInTag(Pr.objId, PrCon);
                aTag.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tvMain.View.SelectedItem is CusFile)
                {
                    AddFolder fr = new AddFolder(PrCon, Pr, ((CusFile)tvMain.View.SelectedItem).FullName, actualKindElement);
                    fr.Show();
                }
                else if (tvMain.View.SelectedItem is WebServer)
                {
                    AddFolder fr = new AddFolder(PrCon, Pr, io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog, actualKindElement);
                    fr.Show();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void File0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tvMain.View.SelectedItem is CusFile)
                {
                    AddCusFile fr = new AddCusFile(PrCon, Pr, ((CusFile)tvMain.View.SelectedItem).FullName, actualKindElement);
                    fr.Show();
                }
                else if (tvMain.View.SelectedItem is WebServer)
                {
                    AddCusFile fr = new AddCusFile(PrCon, Pr, io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog, actualKindElement);
                    fr.Show();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void ScriptFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddScript fr = new AddScript(PrCon, Pr, SelGuid, actualKindElement);
                fr.Show();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void ShLocation0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (actualKindElement == ElementKind.Project)
                {
                    Process.Start(io.Path.GetDirectoryName(Pr.path));
                }
                else if (actualKindElement == ElementKind.HttpConfig)
                {
                    Process.Start(io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog);
                }
                else if (actualKindElement == ElementKind.Scripts)
                {
                    Process.Start(io.Path.GetDirectoryName(Pr.path) + PrCon.ScriptsCatalog);
                }
                else if (actualKindElement == ElementKind.InFile)
                {
                    Process.Start(((CusFile)tvMain.View.SelectedItem).FullName);
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void ClProject0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Zapisanie layoutu
                if (Pr != null)
                {
                    XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                    serializer.Serialize(System.IO.Path.GetDirectoryName(PrCon.projectList.First().path) + "\\" + PrCon.LayoutFile);
                }

                //Zamkniecie edytorow
                var docs = dockManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .Where(x => x.Title != "Output" && x.Title != "Properties" && x.Title != "Solution")
                    .Select(x => x).ToList();

                for (int i = 0; i < docs.Count(); i++)
                    ((LayoutAnchorable)docs[i]).Close();

                propManag.View.SelectedObject = null;
                exList.Clear();

                tvMain.View.ItemsSource = null;

                actualKindElement = ElementKind.Empty;

                FsWatcher.EnableRaisingEvents = false;
                FsWatcher.Created -= FsWatch_Changed;
                FsWatcher.Deleted -= FsWatch_Changed;
                FsWatcher.Renamed -= FsWatch_Changed;
                FsWatcher = null;

                PrCon.closeAllProject(true);

                Pr = null;

                //Sprawdzenie i ustawienie menu
                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Save0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Zapisanie projectu

                if (String.IsNullOrEmpty(Pr.path))
                {
                    wf.SaveFileDialog sfd = new wf.SaveFileDialog();
                    sfd.Filter = "Fenix files (*.psx)|*.psx|All files (*.*)|*.*";
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        PrCon.saveProject(Pr, sfd.FileName);
                    }
                }
                else
                    PrCon.saveProject(Pr, Pr.path);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Start0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelObj == null)
                    return;

                if (((ITreeViewModel)SelObj).IsBlocked)
                    throw new Exception(((ITreeViewModel)SelObj).Name + ": Element is blocked!");

                if (SelObj is IDriverModel)
                {
                    ((IDriverModel)SelObj).error -= Error;
                    ((IDriverModel)SelObj).information -= Error;

                    List<ITag> tgs = PrCon.GetAllITags(Pr.objId, ((IDriverModel)SelObj).ObjId);
                    ((IDriverModel)SelObj).error += Error;
                    ((IDriverModel)SelObj).information += Error;
                    ((IDriverModel)SelObj).activateCycle(tgs);

                    ((ITreeViewModel)SelObj).IsLive = ((IDriverModel)SelObj).isAlive;
                    foreach (ITreeViewModel obj1 in ((ITreeViewModel)SelObj).Children)
                    {
                        obj1.IsLive = ((IDriverModel)SelObj).isAlive;
                        foreach (ITreeViewModel obj2 in ((ITreeViewModel)obj1).Children)
                            obj2.IsLive = ((IDriverModel)SelObj).isAlive;
                    }

                    checkAccess();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Stop0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelObj == null)
                    return;

                if (SelObj is IDriverModel)
                {
                    ((IDriverModel)SelObj).error -= Error;
                    ((IDriverModel)SelObj).information -= Error;
                    ((IDriverModel)SelObj).deactivateCycle();

                    ((ITreeViewModel)SelObj).IsLive = ((IDriverModel)SelObj).isAlive;
                    foreach (ITreeViewModel obj1 in ((ITreeViewModel)SelObj).Children)
                    {
                        obj1.IsLive = ((IDriverModel)SelObj).isAlive;
                        foreach (ITreeViewModel obj2 in ((ITreeViewModel)obj1).Children)
                            obj2.IsLive = ((IDriverModel)SelObj).isAlive;
                    }

                    if (((IDriverModel)SelObj).isAlive)
                    {
                        CommStop fr = new CommStop((IDriverModel)SelObj);
                        fr.ShowDialog();
                    }

                    checkAccess();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void StartAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (IDriverModel id in ((IDriversMagazine)Pr).Children)
                {
                    id.information -= Error;
                    id.error -= Error;

                    List<ITag> tagsList = PrCon.GetAllITagsForDriver(Pr.objId, id.ObjId);

                    id.information += Error;
                    id.error += Error;

                    id.activateCycle(tagsList);
                }

                List<object> lista1 = new List<object>();
                lista1.AddRange(Pr.connectionList.ToArray());
                lista1.Add(Pr.InternalTagsDrv);
                lista1.Add(Pr.ScriptEng);

                foreach (object obj in lista1)
                {
                    ((ITreeViewModel)obj).IsLive = ((IDriverModel)obj).isAlive;
                    foreach (ITreeViewModel obj1 in ((ITreeViewModel)obj).Children)
                    {
                        obj1.IsLive = ((IDriverModel)obj).isAlive;
                        foreach (ITreeViewModel obj2 in ((ITreeViewModel)obj1).Children)
                            obj2.IsLive = ((IDriverModel)obj).isAlive;
                    }
                }

                checkAccess();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void StopAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (IDriverModel id in ((IDriversMagazine)Pr).Children)
                {
                    id.information -= Error;
                    id.error -= Error;
                    id.deactivateCycle();

                    if (id.isAlive)
                    {
                        CommStop fr = new CommStop(id);
                        fr.ShowDialog();
                    }
                }

                List<object> lista1 = new List<object>();
                lista1.AddRange(Pr.connectionList.ToArray());
                lista1.Add(Pr.InternalTagsDrv);
                lista1.Add(Pr.ScriptEng);

                foreach (object obj in lista1)
                {
                    ((ITreeViewModel)obj).IsLive = ((IDriverModel)obj).isAlive;
                    foreach (ITreeViewModel obj1 in ((ITreeViewModel)obj).Children)
                    {
                        obj1.IsLive = ((IDriverModel)obj).isAlive;
                        foreach (ITreeViewModel obj2 in ((ITreeViewModel)obj1).Children)
                            obj2.IsLive = ((IDriverModel)obj).isAlive;
                    }
                }

                checkAccess();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Simulation0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.FileName = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\\FenixServer.exe";
                pInfo.UseShellExecute = true;
                pInfo.Verb = "runas";
                pInfo.Arguments = "-s";

                Process.Start(pInfo);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Cut0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tvMain.View.SelectedItem is CusFile)
                {
                    CusFile f = (CusFile)tvMain.View.SelectedItem;
                    if (f.IsFile)
                    {
                        PrCon.SrcType = actualKindElement;
                        SelSrcPath = f.FullName;
                        PrCon.cutMarks = true;
                    }
                }
                else
                {
                    PrCon.copyCutElement(Pr.objId, SelGuid, actualKindElement, true);
                    SelSrcPath = string.Empty;
                }

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Copy0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tvMain.View.SelectedItem is CusFile)
                {
                    CusFile f = (CusFile)tvMain.View.SelectedItem;
                    if (f.IsFile)
                    {
                        PrCon.SrcType = actualKindElement;
                        SelSrcPath = f.FullName;
                        PrCon.cutMarks = false;
                    }
                }
                else
                {
                    PrCon.copyCutElement(Pr.objId, SelGuid, actualKindElement, false);
                    SelSrcPath = string.Empty;
                }

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Paste0_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (PrCon.SrcType != ElementKind.InFile)
                {
                    PrCon.pasteElement(Pr.objId, SelGuid);
                    checkAccess();
                }
                else
                {
                    if (PrCon.cutMarks)
                    {
                        if (!string.IsNullOrEmpty(SelSrcPath))
                        {
                            if (tvMain.View.SelectedItem is WebServer)
                            {
                                string dest = io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog + "\\" + io.Path.GetFileName(SelSrcPath);
                                if (dest == SelSrcPath)
                                    throw new ApplicationException("This operation is forbbiden");

                                io.File.Copy(SelSrcPath, dest, true);
                                io.File.Delete(SelSrcPath);

                                SelSrcPath = string.Empty;
                                PrCon.SrcType = ElementKind.Empty;
                                PrCon.cutMarks = false;
                            }
                            else
                            {
                                string dest = ((CusFile)tvMain.View.SelectedItem).FullName + "\\" + io.Path.GetFileName(SelSrcPath);

                                if (dest == SelSrcPath)
                                    throw new ApplicationException("This operation is forbbiden");

                                io.File.Copy(SelSrcPath, dest, true);
                                io.File.Delete(SelSrcPath);

                                SelSrcPath = string.Empty;
                                PrCon.SrcType = ElementKind.Empty;
                                PrCon.cutMarks = false;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SelSrcPath))
                        {
                            if (tvMain.View.SelectedItem is WebServer)
                            {
                                string dest = io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog + "\\" + io.Path.GetFileName(SelSrcPath);
                                if (dest == SelSrcPath)
                                    throw new ApplicationException("This operation is forbbiden");

                                io.File.Copy(SelSrcPath, dest, true);

                                SelSrcPath = string.Empty;
                                PrCon.SrcType = ElementKind.Empty;
                                PrCon.cutMarks = false;
                            }
                            else
                            {
                                string dest = ((CusFile)tvMain.View.SelectedItem).FullName + "\\" + io.Path.GetFileName(SelSrcPath);

                                if (dest == SelSrcPath)
                                    throw new ApplicationException("This operation is forbbiden");

                                io.File.Copy(SelSrcPath, dest, true);

                                SelSrcPath = string.Empty;
                                PrCon.SrcType = ElementKind.Empty;
                                PrCon.cutMarks = false;
                            }
                        }
                    }

                    SelSrcPath = string.Empty;
                    checkAccess();
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Delete0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tvMain.View.SelectedItem is CusFile)
                {
                    CusFile f = (CusFile)tvMain.View.SelectedItem;
                    if (f == null)
                        return;

                    if (wf.MessageBox.Show("Do you want to remove this file or directory?", "Attention", wf.MessageBoxButtons.OKCancel) == wf.DialogResult.OK)
                    {
                        if (f.IsFile)
                            io.File.Delete(f.FullName);
                        else
                            io.Directory.Delete(f.FullName, true);

                        checkAccess();
                    }
                }
                else
                {
                    DeleteElementMethod(Pr.objId, SelGuid, actualKindElement);
                    checkAccess();
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Solution0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable lpAnchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(x => x.Title == "Solution").First();
                lpAnchor.IsVisible = true;
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Properties0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable lpAnchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(x => x.Title == "Properties").First();
                lpAnchor.IsVisible = true;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Output0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable lpAnchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Where(x => x.Title == "Output").First();
                lpAnchor.IsVisible = true;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void TableView0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pr == null)
                    return;

                LayoutAnchorable laTableView = new LayoutAnchorable();
                laTableView.CanClose = true;
                laTableView.ContentId = "TableView;" + SelGuid.ToString() + ";" + actualKindElement.ToString();
                TableView tbView = new TableView(PrCon, Pr.objId, SelGuid, actualKindElement, laTableView);
                laTableView.Closed += LaCtrl_Closed;
                laTableView.Content = tbView;

                var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();

                MiddlePan1.Children.Add(laTableView);
                laTableView.IsActive = true;

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void ChartView0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable laChartView = new LayoutAnchorable();
                laChartView.CanClose = true;
                laChartView.ContentId = "ChartView;" + SelGuid.ToString() + ";" + actualKindElement.ToString();
                ChartView chartView = new ChartView(PrCon, Pr.objId, SelGuid, actualKindElement, laChartView);
                laChartView.Closed += LaCtrl_Closed;

                laChartView.Content = chartView;
                var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
                MiddlePan1.Children.Add(laChartView);
                laChartView.IsActive = true;

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void CommView0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable laCommView = new LayoutAnchorable();
                laCommView.CanClose = true;
                laCommView.ContentId = "CommView;" + SelGuid.ToString() + ";" + actualKindElement.ToString();
                CommunicationView commView = new CommunicationView(PrCon, Pr.objId, SelGuid, actualKindElement, laCommView);

                laCommView.Closed += LaCtrl_Closed;

                laCommView.Content = commView;
                var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
                MiddlePan1.Children.Add(laCommView);
                laCommView.IsActive = true;
                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Editor0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (actualKindElement == ElementKind.InFile)
                {
                    LayoutAnchorable laEdit = new LayoutAnchorable();
                    laEdit.CanClose = true;
                    laEdit.ContentId = "Editor;" + ((CusFile)tvMain.View.SelectedItem).FullName + ";" + actualKindElement.ToString();
                    Editor edit = new Editor(PrCon, Pr.objId, ((CusFile)tvMain.View.SelectedItem).FullName, actualKindElement, laEdit);
                    laEdit.Closed += LaCtrl_Closed; ;
                    laEdit.Title = System.IO.Path.GetFileName(((CusFile)tvMain.View.SelectedItem).FullName);

                    laEdit.Content = edit;
                    var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
                    MiddlePan1.Children.Add(laEdit);
                    laEdit.IsActive = true;
                    checkAccess();
                }

                if (actualKindElement == ElementKind.ScriptFile)
                {
                    LayoutAnchorable laEdit = new LayoutAnchorable();
                    laEdit.CanClose = true;
                    laEdit.ContentId = "Editor;" + SelGuid.ToString() + ";" + actualKindElement.ToString();
                    ScriptFile file = PrCon.GetScriptFile(Pr.objId, SelGuid);
                    Editor edit = new Editor(PrCon, Pr.objId, file.FilePath, actualKindElement, laEdit);

                    laEdit.Closed += LaCtrl_Closed;
                    laEdit.Title = file.Name;

                    laEdit.Content = edit;
                    var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
                    MiddlePan1.Children.Add(laEdit);
                    laEdit.IsActive = true;
                }
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void LaCtrl_Closed(object sender, EventArgs e)
        {
            try
            {
                //Okno
                LayoutAnchorable win = (LayoutAnchorable)sender;
                win.Closed -= LaCtrl_Closed;

                //TableView
                if (win.Content is TableView)
                {
                    TableView tbVw = (TableView)win.Content;
                    tbVw.View.ItemsSource = null;
                    win.Content = null;
                    GC.Collect();
                }
                else if (win.Content is ChartView)
                {
                    ChartView tbVw = (ChartView)win.Content;
                    win.Content = null;
                    tbVw = null;
                    GC.Collect();
                }
                else if (win.Content is Editor)
                {
                    Editor editor = (Editor)win.Content;
                    win.Content = null;
                }
                else if (win.Content is CommunicationView)
                {
                    CommunicationView commView = (CommunicationView)win.Content;
                    win.Content = null;
                    GC.Collect();
                }

                win = null;
                checkAccess();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void DriveConf0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DriverConfigurator dConf = new DriverConfigurator(PrCon.gConf, PrCon);
                dConf.ShowDialog();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void About0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                About about = new About();
                about.Show();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Updates0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckVersion frVersion = new CheckVersion(PrCon);
                frVersion.Show();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void ShHelp0_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(PrCon.HelpWebSite);
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (Pr != null)
                {
                    XmlLayoutSerializer serializer = new XmlLayoutSerializer(dockManager);
                    serializer.Serialize(io.Path.GetDirectoryName(PrCon.projectList.First().path) + "\\" + PrCon.LayoutFile);
                }

                lbPathProject.Content = string.Empty;
                Pr = null;
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread update = new Thread(new ParameterizedThreadStart(updateSoftware));
                update.Start(false);

                if (!String.IsNullOrEmpty(pathRun))
                {
                    //Ta metoda zostala przeniesiona tu w konstruktorze wyrzucala blad
                    PrCon.openProjects(pathRun);
                    Pr = PrCon.projectList[0];
                    Registry.SetValue(PrCon.RegUserRoot, PrCon.LastPathKey, Pr.path);

                    checkAccess();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void Exit0_Click(object sender, RoutedEventArgs e)
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

        private void Block_Click(object sender, RoutedEventArgs e)
        {
            if (SelObj != null)
            {
                if (!((ITreeViewModel)SelObj).IsBlocked)
                {
                    ((ITreeViewModel)SelObj).IsBlocked = true;
                    checkAccess();
                }
            }
        }

        private void Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (SelObj != null)
            {
                if (((ITreeViewModel)SelObj).IsBlocked)
                {
                    ((ITreeViewModel)SelObj).IsBlocked = false;
                    checkAccess();
                }
            }
        }

        private void MenuItem_DbReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pr.Db.Reset();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void View_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                propManag.View.SelectedObject = e.NewValue;
                SelObj = e.NewValue;

                if (e.NewValue is Project)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxProject"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxProject"];
                    SelGuid = ((Project)e.NewValue).objId;
                    actualKindElement = ElementKind.Project;
                }
                else if (e.NewValue is WebServer)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxHttpServer"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxHttpServer"];
                    SelGuid = ((WebServer)e.NewValue).ObjId;
                    actualKindElement = ElementKind.HttpConfig;
                }
                else if (e.NewValue is DatabaseModel)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxDatabse"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxDatabse"];
                }
                else if (e.NewValue is CusFile)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    if (((CusFile)e.NewValue).IsFile)
                    {
                        ((ContextMenu)Resources["CtxInFile"]).DataContext = this;
                        tvMain.View.ContextMenu = (ContextMenu)Resources["CtxInFile"];
                        SelGuid = PrCon.HttpFileGuid;
                        actualKindElement = ElementKind.InFile;
                    }
                    else
                    {
                        ((ContextMenu)Resources["CtxHttpServer"]).DataContext = this;
                        tvMain.View.ContextMenu = (ContextMenu)Resources["CtxHttpServer"];
                        SelGuid = PrCon.HttpFileGuid;
                        actualKindElement = ElementKind.InFile;
                    }
                }
                else if (e.NewValue is ScriptsDriver)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxScripts"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxScripts"];
                    SelGuid = ((ScriptsDriver)e.NewValue).objId;
                    actualKindElement = ElementKind.Scripts;
                }
                else if (e.NewValue is ScriptFile)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxScriptFile"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxScriptFile"];
                    SelGuid = ((ScriptFile)e.NewValue).objId;
                    actualKindElement = ElementKind.ScriptFile;
                }
                else if (e.NewValue is InternalTagsDriver)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxInternalTags"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxInternalTags"];
                    SelGuid = ((InternalTagsDriver)e.NewValue).objId;
                    actualKindElement = ElementKind.InternalsTags;
                }
                else if (e.NewValue is InTag)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxIntTag"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxIntTag"];
                    SelGuid = ((InTag)e.NewValue).objId;
                    actualKindElement = ElementKind.IntTag;

                    if (e.NewValue != null)
                        ((INotifyPropertyChanged)e.NewValue).PropertyChanged += FenixMenager_PropertyChanged;
                }
                else if (e.NewValue is Connection)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxConnection"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxConnection"];
                    SelGuid = ((Connection)e.NewValue).objId;
                    actualKindElement = ElementKind.Connection;
                }
                else if (e.NewValue is Device)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxDevice"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxDevice"];
                    SelGuid = ((Device)e.NewValue).objId;
                    actualKindElement = ElementKind.Device;
                }
                else if (e.NewValue is Tag)
                {
                    if (e.OldValue != null)
                        ((INotifyPropertyChanged)e.OldValue).PropertyChanged -= FenixMenager_PropertyChanged;

                    ((ContextMenu)Resources["CtxTag"]).DataContext = this;
                    tvMain.View.ContextMenu = (ContextMenu)Resources["CtxTag"];
                    SelGuid = ((Tag)e.NewValue).objId;
                    actualKindElement = ElementKind.Tag;

                    if (e.NewValue != null)
                        ((INotifyPropertyChanged)e.NewValue).PropertyChanged += FenixMenager_PropertyChanged;
                }

                checkAccess();
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        private void FenixMenager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (SelObj is Tag || SelObj is ITag)
                {
                    if (!((IDriverModel)sender).isAlive)
                        propManag.View.SelectedObject = sender;
                }
            }));
        }

        private void MenuItem_ShowDbFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string p = io.Path.GetDirectoryName(Pr.path) + io.Path.GetDirectoryName(PrCon.Database);
                if (io.Directory.Exists(p))
                    Process.Start(p);
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void MenuItem_ShowDatabse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable laTableView = new LayoutAnchorable();
                laTableView.CanClose = true;
                laTableView.ContentId = "Database";
                DbExplorer db = new DbExplorer(Pr);
                laTableView.Closed += LaCtrl_Closed;
                laTableView.Content = db;

                var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();

                MiddlePan1.Children.Add(laTableView);
                laTableView.IsActive = true;
                laTableView.Title = "Database";

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void MenuItem_ShowTrendDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LayoutAnchorable laTableView = new LayoutAnchorable();
                laTableView.CanClose = true;
                laTableView.ContentId = "TrendDatabase";
                ChartViewDatabase chart = new ChartViewDatabase(Pr);
                laTableView.Closed += LaCtrl_Closed;
                laTableView.Content = chart;

                var MiddlePan1 = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();

                MiddlePan1.Children.Add(laTableView);
                laTableView.IsActive = true;
                laTableView.Title = "Chart Database";

                checkAccess();
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        private void MenuItem_SaveDatabeCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                DataTable tb = Pr.Db.GetAll();

                IEnumerable<string> columnNames = tb.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in tb.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString().Replace(',', '.'));
                    sb.AppendLine(string.Join(",", fields));
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                if (sfd.ShowDialog() == true)
                    io.File.WriteAllText(sfd.FileName, sb.ToString());
            }
            catch (Exception Ex)
            {
                if (PrCon.ApplicationError != null)
                    PrCon.ApplicationError(this, new ProjectEventArgs(Ex));
            }
        }

        public override string ToString()
        {
            return "Fenix Manager";
        }

        #endregion Internal Events
    }
}