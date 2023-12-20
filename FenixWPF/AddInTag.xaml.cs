using MahApps.Metro.Controls;
using ProjectDataLib;
using System;
using System.Windows;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for AddInTag.xaml
    /// </summary>
    public partial class AddInTag : MetroWindow
    {
        private Project Pr { get; set; }
        private ProjectContainer PrCon { get; set; }
        private InTag iTg { get; set; }

        //Ctor
        public AddInTag(Guid projId, ProjectContainer prCon)
        {
            try
            {
                InitializeComponent();

                PrCon = prCon;
                Pr = PrCon.getProject(projId);

                string nm = "InTag";
                for (int x = 0; ; x++)
                {
                    if (PrCon.GetAllITags().Exists(k => k.Name == nm))
                        nm = $"{nm}{x}";
                    else
                        break;
                }

                iTg = new InTag(PrCon, Pr, nm, "", TypeData.DOUBLE, "0");
                DataContext = iTg;
            }
            catch (Exception Ex)
            {
                PrCon.ApplicationError?.Invoke(this, new ProjectEventArgs(Ex));
            }
        }

        //Add InTag
        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrCon.AddIntTag(Pr.objId, iTg);
                Close();
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