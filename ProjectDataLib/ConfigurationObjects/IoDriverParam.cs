using System;
using System.ComponentModel;
using System.IO.Ports;

namespace ProjectDataLib
{
    [Serializable]
    public class IoDriverParam
    {
        private string PortName_;

        [Category("COM Settings")]
        [DisplayName("Port Name")]
        public string PortName
        {
            get { return PortName_; }
            set { PortName_ = value; }
        }

        private int BaundRate_;

        [Category("COM Settings")]
        [DisplayName("Baund Rate[B/s]")]
        public int BoundRate
        {
            set { BaundRate_ = value; }
            get { return BaundRate_; }
        }

        private Parity parity_;

        [Category("COM Settings")]
        [DisplayName("Parity")]
        public Parity parity
        {
            get { return parity_; }
            set { parity_ = value; }
        }

        private int DataBits_;

        [Category("COM Settings")]
        [DisplayName("Data Bits")]
        public int DataBits
        {
            get { return DataBits_; }
            set { DataBits_ = value; }
        }

        private StopBits stopBits_;

        [Category("COM Settings")]
        [DisplayName("Stop Bits")]
        public StopBits stopBits
        {
            get { return stopBits_; }
            set { stopBits_ = value; }
        }

        private int Timeout_;

        [Category("COM Settings")]
        [DisplayName("Timeout [ms]")]
        public int Timeout
        {
            get { return Timeout_; }
            set { Timeout_ = value; }
        }

        private int ReplyTime_;

        [Category("COM Settings")]
        [DisplayName("Reply Time [ms]")]
        public int ReplyTime
        {
            get { return ReplyTime_; }
            set { ReplyTime_ = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public IoDriverParam()
        {
            //Port Name
            PortName_ = "COM1";
            BaundRate_ = 9600;
            parity_ = Parity.None;
            DataBits_ = 8;
            stopBits_ = StopBits.One;
            Timeout_ = 1000;
            ReplyTime_ = 1500;
        }
    }
}