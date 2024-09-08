using ProjectDataLib;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FenixWPF
{
    public partial class DbExplorer : UserControl
    {
        public DbExplorer(ObservableCollection<TagDTO> context)
        {
            InitializeComponent();

            if (context != null)
            {
                myDataGrid.ItemsSource = context;
            }
        }
    }
}