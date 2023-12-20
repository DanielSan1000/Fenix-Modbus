using System.Data;
using System.Windows.Controls;

namespace FenixWPF
{
    /// <summary>
    /// Interaction logic for DbExplorer.xaml
    /// </summary>
    public partial class DbExplorer : UserControl
    {
        public DbExplorer(DataTable context)
        {
            InitializeComponent();

            if (context != null)
                View.ItemsSource = context.DefaultView;
        }
    }
}