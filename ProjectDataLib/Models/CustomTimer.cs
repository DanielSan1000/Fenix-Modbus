using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    [Serializable]
    public class CustomTimer : IComparable<CustomTimer>
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CustomTimer()
        {
            Name_ = "Timer";
            Time_ = 1000;
            Delay_ = 0;
        }

        private string Name_;

        /// <summary>
        /// Nazwa Timera
        /// </summary>
        [Category("Design"), DisplayName("01 Timer Name")]
        public string Name
        {
            get { return Name_; }
            set { Name_ = value; }
        }

        private int Time_;

        /// <summary>
        /// Czas zegara
        /// </summary>
        [Category("Design"), DisplayName("02 Time [ms]")]
        public int Time
        {
            get { return Time_; }
            set { Time_ = value; }
        }

        private int Delay_;

        /// <summary>
        /// Opoznienie startu
        /// </summary>
        [Category("Design"), DisplayName("03 Delay [ms]")]
        public int Delay
        {
            get { return Delay_; }
            set { Delay_ = value; }
        }

        /// <summary>
        /// Sortowanie
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        int IComparable<CustomTimer>.CompareTo(CustomTimer other)
        {
            return this.Time_.CompareTo(other.Time_);
        }
    }
}