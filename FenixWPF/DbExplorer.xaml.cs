using ProjectDataLib;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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