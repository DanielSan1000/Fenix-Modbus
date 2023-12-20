using System.Collections.Generic;

namespace ProjectDataLib
{
    /// <summary>
    /// Wszystkie dane
    /// </summary>
    public class GraphElement
    {
        /// <summary>
        /// Name
        /// </summary>
        public string label;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="lab"></param>
        public GraphElement(string lab)
        {
            label = lab;
        }

        /// <summary>
        /// Dane prywante
        /// </summary>
        private List<object[]> dane = new List<object[]>();

        /// <summary>
        /// Publiczne dane
        /// </summary>
        public List<object[]> data
        { get { return dane; } }

        /// <summary>
        /// Add point to
        /// </summary>
        /// <param name="d"></param>
        public void addDataPoint(object[] d)
        {
            dane.Add(d);
        }

        /// <summary>
        /// Usun element o numerze;
        /// </summary>
        /// <param name="nr"></param>
        public void removeElement(int nr)
        {
            if (dane.Count > 0)
                dane.RemoveAt(nr);
        }
    }
}