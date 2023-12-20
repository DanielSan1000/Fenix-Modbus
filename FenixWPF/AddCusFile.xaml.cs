using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Linq;
using System.Windows;
using io = System.IO;
using wf = System.Windows.Forms;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddInFile.xaml
    /// </summary>
    public partial class AddCusFile : MetroWindow
    {
        private ProjectContainer PrCon { get; set; }
        private Project Pr { get; set; }
        private String Path { get; set; }
        private ElementKind ElKind { get; set; }

        //Ctor
        public AddCusFile(ProjectContainer pc, Project pr, string path, ElementKind elKind)
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
                wf.OpenFileDialog fr = new wf.OpenFileDialog();
                fr.Multiselect = true;
                fr.Filter = "All Files (*.*)|*.*";

                if (fr.ShowDialog() == wf.DialogResult.OK)
                {
                    if (fr.CheckPathExists)
                        TbAddFile.Text = fr.FileNames.Aggregate((p, k) => (p + ";" + k));
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
                    if (string.IsNullOrEmpty(TbNewFile.Text))
                    {
                        MessageBox.Show("Please fill File(s) name(s)!");
                        return;
                    }

                    foreach (string s in TbNewFile.Text.Split(';'))
                    {
                        if (io.File.Exists(Path + "\\" + s))
                        {
                            MessageBox.Show(String.Format("File: [{0}] already exist in this location!", Path + "\\" + s));
                            return;
                        }
                        else
                            io.File.Create(Path + "\\" + s).Close();
                    }

                    Close();
                }
                //Plik
                else
                {
                    if (string.IsNullOrEmpty(TbAddFile.Text))
                    {
                        MessageBox.Show("Please fill File(s) name(s)!");
                        return;
                    }

                    foreach (string s in TbAddFile.Text.Split(';'))
                    {
                        if (io.File.Exists(Path + "//" + io.Path.GetFileName(s)))
                        {
                            MessageBox.Show(string.Format("File: [{0}] exist in target location!", s));
                            return;
                        }
                        else if (!io.File.Exists(s))
                        {
                            MessageBox.Show(string.Format("File: [{0}] not exist!", s));
                            return;
                        }
                        else if (Path + "//" + io.Path.GetFileName(s) == s)
                        {
                            MessageBox.Show(string.Format("Yout try copy File1: [{0}] to File2: [{1}]! Not allowed!", s, Path + "//" + io.Path.GetFileName(s)));
                            return;
                        }
                        else
                            io.File.Copy(s, Path + "//" + io.Path.GetFileName(s));

                        Close();
                    }
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
    }
}