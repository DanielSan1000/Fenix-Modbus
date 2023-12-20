using System;

namespace ProjectDataLib
{
    /// <summary>
    /// Klasa informacyjna
    /// </summary>
    public class ProjectEventArgs : EventArgs
    {
        /// <summary>
        /// Element przesyłany
        /// </summary>
        public object element;

        public object element0;
        public object element1;
        public object element2;

        /// <summary>
        /// Konstruktor1
        /// </summary>
        /// <param name="obj"></param>
        public ProjectEventArgs(object obj)
        {
            this.element = obj;
        }

        /// <summary>
        /// Konstrukor2
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        public ProjectEventArgs(object obj, object obj0)
        {
            this.element = obj;
            this.element0 = obj0;
        }

        /// <summary>
        /// Konstruktor 3
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        /// <param name="obj1"></param>
        public ProjectEventArgs(object obj, object obj0, object obj1)
        {
            this.element = obj;
            this.element0 = obj0;
            this.element1 = obj1;
        }

        /// <summary>
        /// Konstruktor4
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj0"></param>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public ProjectEventArgs(object obj, object obj0, object obj1, object obj2)
        {
            this.element = obj;
            this.element0 = obj0;
            this.element1 = obj1;
            this.element2 = obj2;
        }
    }
}