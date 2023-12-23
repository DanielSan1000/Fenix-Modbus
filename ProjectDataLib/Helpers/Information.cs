using System;

namespace ProjectDataLib
{
    //Klasa sluzaca do wyswietlania informacji ze skryptów
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