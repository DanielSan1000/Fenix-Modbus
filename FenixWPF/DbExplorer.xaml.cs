using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            if(context != null)
                View.ItemsSource = context.DefaultView;
        }
    }
}
