using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;

namespace ProjectDataLib
{
    [Serializable]
    public class S7DriverParam
    {
        /// <summary>
        /// Nazwa Portu COM
        /// </summary>
        private IPAddress Ip_;

        [Category("Target")]
        [DisplayName("Adress Ip")]
        public String Ip
        {
            get { return Ip_.ToString(); }
            set
            {
                try { Ip_ = IPAddress.Parse(value); }
                catch (Exception Ex) { MessageBox.Show(Ex.Message); }
            }
        }

        private ushort Port_;

        [Category("Target")]
        [DisplayName("Port")]
        public ushort Port
        {
            set { Port_ = value; }
            get { return Port_; }
        }

        private int Rack_;

        [Category("Target")]
        [DisplayName("Rack")]
        public int Rack
        {
            get { return Rack_; }
            set { Rack_ = value; }
        }

        private int Slot_;

        [Category("Target")]
        [DisplayName("Slot")]
        public int Slot
        {
            get { return Slot_; }
            set { Slot_ = value; }
        }

        private int Timeout_;

        [Category("Time")]
        [DisplayName("Timeout [ms]")]
        public int Timeout
        {
            get { return Timeout_; }
            set { Timeout_ = value; }
        }

        private int ReplyTime_;

        [Category("Time")]
        [DisplayName("Reply Time [ms]")]
        public int ReplyTime
        {
            get { return ReplyTime_; }
            set { ReplyTime_ = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public S7DriverParam()
        {
            Ip_ = IPAddress.Parse("127.0.0.1");
            Port_ = 102;
            Rack_ = 0;
            Slot_ = 2;
            Timeout_ = 1000000;
            ReplyTime_ = 1500;
        }
    }
}