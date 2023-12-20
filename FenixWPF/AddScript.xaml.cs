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
    /// Interaction logic for AddScript.xaml
    /// </summary>
    public partial class AddScript : MetroWindow
    {

        ProjectContainer PrCon { get; set; }
        Project Pr { get; set; }
        Guid Sel { get; set; }
        ElementKind ElKind { get; set; }

        //Ctor
        public AddScript(ProjectContainer pc, Project pr, Guid sel, ElementKind elKind)
        {
            try
            {
                InitializeComponent();

                PrCon = pc;
                Pr = pr;
                Sel = sel;
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
                fr.Filter = "Script Files (*.cs)|*.cs";

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
                        //Nazwa pliku
                        string nName = io.Path.GetFileName(s);

                        //Katalog docelowy
                        string TarDir = io.Path.GetDirectoryName(Pr.path) + PrCon.ScriptsCatalog;

                        //Jezeli istnieje
                        if (!io.Directory.Exists(TarDir))
                            io.Directory.CreateDirectory(TarDir);

                        //Kopiowanie pliku do dolferu
                        io.File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + PrCon.TemplateCatalog + "\\" + "Script.cs", TarDir + "\\" + nName + ".cs", true);

                        //Plik InFile
                        PrCon.AddScriptFile(Pr.objId, new ScriptFile(TarDir + "\\" + nName + ".cs"));
                    }

                    Close();
                }
                //intniejący
                else
                {

                    if (string.IsNullOrEmpty(TbAddFile.Text))
                    {
                        MessageBox.Show("Please fill File(s) name(s)!");
                        return;
                    }

                    foreach (string s in TbAddFile.Text.Split(';'))
                    {
                        //Nazwa pliku
                        string nName = io.Path.GetFileName(s);

                        //Katalog docelowy
                        string TarDir = io.Path.GetDirectoryName(Pr.path) + PrCon.ScriptsCatalog;

                        //Jezeli istnieje
                        if (!io.Directory.Exists(TarDir))
                            io.Directory.CreateDirectory(TarDir);

                        //Kopiowanie pliku do dolferu
                        io.File.Copy(s, TarDir + "\\" + nName, true);

                        //Plik InFile
                        PrCon.AddScriptFile(Pr.objId, new ScriptFile(TarDir + "\\" + nName));                   
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
    }
}
