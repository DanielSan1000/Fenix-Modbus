using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace ProjectDataLib
{
    /// <summary>
    /// Wybor timerow dla taga
    /// </summary>
    public class ScriptFileTimers : StringConverter
    {
        /// <summary>
        /// Właściwość wyświetla dane w zależności od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            ScriptFile tag = ((ScriptFile)context.Instance);

            //Dodanie wyświetlanych wartości
            List<String> buff = new List<string>() { String.Empty };
            buff.AddRange(tag.Proj.ScriptEng.Timers.Select(x => x.Name));

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(buff);
        }

        /// <summary>
        /// Możliwość edycji wartości w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Włączenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Blokada recznej edycji w propGrid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }
}