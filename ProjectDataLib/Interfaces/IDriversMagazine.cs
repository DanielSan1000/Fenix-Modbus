using System.Collections.ObjectModel;

namespace ProjectDataLib
{
    public interface IDriversMagazine
    {
        ObservableCollection<IDriverModel> Children { get; set; }
    }
}