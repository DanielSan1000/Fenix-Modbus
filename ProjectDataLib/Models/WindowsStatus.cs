using System;

namespace ProjectDataLib
{
    /// <summary>
    /// Klasa zarzadnia zamykaniem poloczen
    /// </summary>
    [Serializable]
    public class WindowsStatus
    {
        public int index;
        public Boolean Visible;
        public Boolean Live;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ind"></param>
        /// <param name="vis"></param>
        /// <param name="liv"></param>
        public WindowsStatus(int ind, Boolean vis, Boolean liv)
        {
            this.index = ind;
            this.Visible = vis;
            this.Live = liv;
        }

        public WindowsStatus()
        {
        }
    }
}