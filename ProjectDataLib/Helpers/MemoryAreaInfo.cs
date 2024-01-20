using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    [Serializable]
    public class MemoryAreaInfo : INotifyPropertyChanged
    {
        [field: NonSerialized]
        private PropertyChangedEventHandler propChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                propChanged += value;
            }

            remove
            {
                propChanged -= value;
            }
        }

        public MemoryAreaInfo(string name, int adresSize, int ctCode)
        {
            Name = name;

            AdresSize = adresSize;

            fctCode = ctCode;
        }

        private string Name_;

        public string Name
        {
            get { return Name_; }
            set
            {
                Name_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private int fctCode_;

        public int fctCode
        {
            get { return fctCode_; }
            set
            {
                fctCode_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(fctCode)));
            }
        }

        private int AdresSize_;

        public int AdresSize
        {
            get { return AdresSize_; }
            set
            {
                AdresSize_ = value;
                propChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdresSize)));
            }
        }
    }
}