using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProjectDataLib
{
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
        public List<String> assmemblyPath = new List<string>();

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
        private Boolean saveData(string path)
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
        private Boolean openData(string path)
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

            foreach (string s in assmemblyPath)
            {
                if (!string.IsNullOrEmpty(s))
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
}