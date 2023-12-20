using System;

namespace ProjectDataLib
{
    /// <summary>
    /// Klasa dostepna dla scryptow
    /// </summary>
    public class ScriptModel
    {
        /// <summary>
        /// Project
        /// </summary>
        private Project Pr { get; set; }

        private String FileName { get; set; }

        /// <summary>
        /// Kontener projektowy
        /// </summary>
        private ProjectContainer PrCon { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="?"></param>
        public ScriptModel()
        {
        }

        /// <summary>
        /// Inicjalizacja
        /// </summary>
        /// <param name="pr"></param>
        public void Init(Project pr, String name)
        {
            this.Pr = pr;
            this.PrCon = pr.PrCon;
            this.FileName = name;
        }

        /// <summary>
        /// Start Skrypt
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stop skryptu
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Wykonaywany cyklicznie
        /// </summary>
        public virtual void Cycle()
        {
        }

        /// <summary>
        /// Ustawia wartość taga
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetTag(string name, object value)
        {
            Pr.SetTag(name, value);
        }

        /// <summary>
        /// Zapis w oknie diagnostycznym
        /// </summary>
        /// <param name="s"></param>
        public void Write(object s)
        {
            Pr.Write(this, Convert.ToString(s));
        }

        /// <summary>
        /// Pobiera wartosc taga
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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