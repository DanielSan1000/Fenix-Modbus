using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Windows;
using io = System.IO;
using wf = System.Windows.Forms;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddFolder.xaml
    /// </summary>
    public partial class AddFolder : MetroWindow
    {
        private ProjectContainer PrCon { get; set; }
        private Project Pr { get; set; }
        private String Path { get; set; }
        private ElementKind ElKind { get; set; }

        //Ctor
        public AddFolder(ProjectContainer pc, Project pr, string path, ElementKind elKind)
        {
            try
            {
                InitializeComponent();

                PrCon = pc;
                Pr = pr;
                Path = path;
                ElKind = elKind;
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //File
        private void Button_File_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wf.FolderBrowserDialog fr = new wf.FolderBrowserDialog();

                if (fr.ShowDialog() == wf.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(fr.SelectedPath))
                        TbAddFile.Text = fr.SelectedPath;
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //OK
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Nowy
                if ((bool)Ch1.IsChecked)
                {
                    string s = TbNewFile.Text;
                    if (string.IsNullOrEmpty(s))
                    {
                        MessageBox.Show("Please fill Directory name!");
                        return;
                    }
                    else if (io.Directory.Exists(Path + "\\" + s))
                    {
                        MessageBox.Show(String.Format("Dir: [{0}] already exist in this location!", Path + "\\" + s));
                        return;
                    }
                    else
                        io.Directory.CreateDirectory(Path + "\\" + s);

                    Close();
                }

                //
                else
                {
                    string s = TbAddFile.Text;
                    if (string.IsNullOrEmpty(s))
                    {
                        MessageBox.Show("Please fill Directory name!");
                        return;
                    }
                    else
                    {
                        if (MessageBox.Show("Do you want to overwrite similar files?", "Attention", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            DirectoryCopy(s, Path, true);
                    }

                    Close();
                }
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Cancel
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
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

        //Copy Dir
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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
                file.CopyTo(temppath, false);
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
    }
}