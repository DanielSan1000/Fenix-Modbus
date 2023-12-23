using System;

namespace ProjectDataLib
{
    [Serializable]
    public class WindowsStatus
    {
        public int index;
        public Boolean Visible;
        public Boolean Live;

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