using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    /// <summary>
    /// Klasa inforujaca o danym obszarze pamięci w sterowniku
    /// </summary>
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

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Nazwa Obszaru pamięci</param>
        /// <param name="moduleSizeReq">Rozmiar w bitach ządania</param>
        /// <param name="moduleSizeRes">Odpowiedz w bitach sterownika</param>
        public MemoryAreaInfo(string name, int adresSize, int ctCode)
        {
            //Nazwa
            Name = name;

            //Rozmiar jednostkowego rejestru
            AdresSize = adresSize;

            //Kod
            fctCode = ctCode;
        }

        private string Name_;

        /// <summary>
        /// Nazwa obszaru
        /// </summary>
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

        /// <summary>
        /// Kod fukcji danego bszaru
        /// </summary>
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

        /// <summary>
        /// Rozmiar elementu zapytania
        /// </summary>
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