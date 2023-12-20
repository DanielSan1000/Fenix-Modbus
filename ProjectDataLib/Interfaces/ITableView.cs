using System.Collections.ObjectModel;

namespace ProjectDataLib
{
    public interface ITableView
    {
        ObservableCollection<ITag> Children { get; set; }
    }
}