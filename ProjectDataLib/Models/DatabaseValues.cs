using System.Collections.Generic;

namespace ProjectDataLib
{
    public class DatabaseValues
    {
        public DatabaseValues()
        {
            TimePts = new List<double>();
            ValPts = new List<double>();
        }

        public string Name { get; set; }
        public List<double> TimePts { get; set; }
        public List<double> ValPts { get; set; }
    }
}