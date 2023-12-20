using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProjectDataLib;
using MahApps.Metro.Controls;

namespace FenixWPF
{

    /// <summary>
    /// Interaction logic for AddInTag.xaml
    /// </summary>
    public partial class AddInTag : MetroWindow
    {
        Project Pr { get; set; }
        ProjectContainer PrCon { get; set; }
        InTag iTg { get; set; }

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
