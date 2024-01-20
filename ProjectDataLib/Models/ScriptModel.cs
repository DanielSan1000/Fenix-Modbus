using System;

namespace ProjectDataLib
{
    public class ScriptModel
    {
        private Project Pr { get; set; }

        private String FileName { get; set; }

        private ProjectContainer PrCon { get; set; }

        public ScriptModel()
        {
        }

        public void Init(Project pr, String name)
        {
            this.Pr = pr;
            this.PrCon = pr.PrCon;
            this.FileName = name;
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Cycle()
        {
        }

        public void SetTag(string name, object value)
        {
            Pr.SetTag(name, value);
        }

        public void Write(object s)
        {
            Pr.Write(this, Convert.ToString(s));
        }

        public Object GetTag(string name)
        {
            return Pr.GetTag(name);
        }

        public override string ToString()
        {
            return "Script: " + FileName;
        }
    }
}