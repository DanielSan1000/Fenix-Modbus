using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using io = System.IO;
using wf = System.Windows.Forms;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddProject.xaml
    /// </summary>
    public partial class AddProject : MetroWindow
    {
        private ProjectContainer PrCon;
        private Project Pr;

        public AddProject(ProjectContainer prCon)
        {
            try
            {
                InitializeComponent();

                PrCon = prCon;
                Pr = new Project(PrCon, "Project", Environment.UserName, "Company", "");
                DataContext = Pr;
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Copy Dir
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            io.DirectoryInfo dir = new io.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new io.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            io.DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!io.Directory.Exists(destDirName))
            {
                io.Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            io.FileInfo[] files = dir.GetFiles();
            foreach (io.FileInfo file in files)
            {
                string temppath = io.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (io.DirectoryInfo subdir in dirs)
                {
                    string temppath = io.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        //Save
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Dialog zapisu
                wf.SaveFileDialog sfd = new wf.SaveFileDialog();
                sfd.Filter = "Fenix files (*.psx)|*.psx|All files (*.*)|*.*";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (PrCon.saveProject(Pr, sfd.FileName))
                    {
                        //Dodanie projektu
                        Pr.path = sfd.FileName;
                        PrCon.addProject(Pr);

                        //Dodaj pliki szablonowe jesli sfdjest aktywna opcja
                        if ((bool)ChkHttpTemplates.IsChecked)
                            DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + PrCon.HttpCatalog, io.Path.GetDirectoryName(Pr.path) + PrCon.HttpCatalog, true);

                        //Dodanie wszytkich dzieci plikow
                        io.DirectoryInfo gt = new io.DirectoryInfo(io.Path.GetDirectoryName(Pr.path) + "\\Http");
                        var SubDir = (from x in gt.GetDirectories() select new CusFile(x)).ToList();
                        SubDir.AddRange(from x in gt.GetFiles() select new CusFile(x));
                        ((ITreeViewModel)Pr.WebServer1).Children = new ObservableCollection<object>(SubDir);

                        string[] files1 = io.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + PrCon.TemplateCatalog);
                        foreach (string f in files1)
                        {
                            //Nazwa pliku
                            string nName = io.Path.GetFileName(f);

                            //Katalog docelowy
                            string TarDir = io.Path.GetDirectoryName(Pr.path) + PrCon.ScriptsCatalog;

                            //Jezeli istnieje
                            if (!io.Directory.Exists(TarDir))
                                io.Directory.CreateDirectory(TarDir);

                            //Kopiowanie pliku do dolferu
                            io.File.Copy(f, TarDir + "\\" + nName, true);

                            //Utworzenie pliku
                            ScriptFile file = new ScriptFile(TarDir + "\\" + nName);

                            //Dodanie do bufora
                            PrCon.AddScriptFile(Pr.objId, file);
                        }

                        Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex)); ;
            }
        }

        //Close
        private void Button_Close_Click(object sender, RoutedEventArgs e)
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
    }
}