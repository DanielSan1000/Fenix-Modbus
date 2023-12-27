using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProjectDataLib
{
    [Serializable]
    [Synchronization]
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

        [Browsable(false), JsonIgnore, XmlIgnore]
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
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += ITag_PropertyChanged;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ((INotifyPropertyChanged)e.OldItems[0]).PropertyChanged -= ITag_PropertyChanged;
                removeDataITagElement(((ITag)e.OldItems[0]).Name);
            }
        }

        private void ITag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                ITag tg = (ITag)sender;

                if (tg.TypeData_ == TypeData.CHAR)
                {
                    addDataElement(tg.Name, Char.GetNumericValue((char)tg.Value).ToString(), DateTime.Now);
                }
                else if (tg.TypeData_ == TypeData.BIT)
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
        private void addDataElement(string name, string value, DateTime tm)
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
        private void removeDataITagElement(string name)
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
                                      MessageBoxButtons.OKCancel) == DialogResult.OK)
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

        public ObservableCollection<TagDTO> GetAllObservableCollection()
        {

            ObservableCollection<TagDTO> observableCollection = new ObservableCollection<TagDTO>();
            string sql = "select * from tags";
            SQLiteCommand command = new SQLiteCommand(sql, DbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TagDTO tag = new TagDTO()
                {
                    Name = reader["name"].ToString(),
                    Stamp = DateTime.Parse(reader["stamp"].ToString()),
                    Value = double.Parse(reader["value"].ToString())
                };

                observableCollection.Add(tag);
            }

            return observableCollection;
        }

        //GetRange
        public DatabaseValues GetRange(ITag tg, DateTime from, DateTime to)
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

        [XmlIgnore]
        [Browsable(false)]
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

        [Browsable(false), XmlIgnore()]
        private SQLiteConnection DbConnection;

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
}