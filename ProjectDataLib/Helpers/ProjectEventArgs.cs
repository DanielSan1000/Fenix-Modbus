using System;

namespace ProjectDataLib
{
    public class ProjectEventArgs : EventArgs
    {
        public object element;

        public object element0;
        public object element1;
        public object element2;

        public ProjectEventArgs(object obj)
        {
            this.element = obj;
        }

        public ProjectEventArgs(object obj, object obj0)
        {
            this.element = obj;
            this.element0 = obj0;
        }

        public ProjectEventArgs(object obj, object obj0, object obj1)
        {
            this.element = obj;
            this.element0 = obj0;
            this.element1 = obj1;
        }

        public ProjectEventArgs(object obj, object obj0, object obj1, object obj2)
        {
            this.element = obj;
            this.element0 = obj0;
            this.element1 = obj1;
            this.element2 = obj2;
        }
    }
}