using System;
using System.Xml.Serialization;

namespace ProjectDataLib
{
    public class XMLVersion
    {
        private Version ver_ = new Version();

        public XMLVersion()
        { }

        public XMLVersion(Version c)
        { ver_ = c; }

        public Version ToVersion()
        {
            return ver_;
        }

        public void FromVersion(Version c)
        {
            ver_ = c;
        }

        public static implicit operator Version(XMLVersion x)
        {
            return x.ToVersion();
        }

        public static implicit operator XMLVersion(Version c)
        {
            return new XMLVersion(c);
        }

        [XmlAttribute]
        public string Ver
        {
            get { return ver_.ToString(); }
            set
            {
                try
                {
                    ver_ = new Version(value);
                }
                catch (Exception)
                {
                    ver_ = new Version();
                }
            }
        }
    }
}