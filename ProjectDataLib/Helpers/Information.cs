using System;

namespace ProjectDataLib
{
    public class Information : Exception
    {
        public Information(string s) : base(s)
        { }

        public override string ToString()
        {
            return "-";
        }
    }
}