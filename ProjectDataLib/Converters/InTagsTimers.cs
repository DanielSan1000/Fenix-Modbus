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
    public class InTagsTimers : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            InTag tag = ((InTag)context.Instance);

            //Dodanie wyswitlanych warotosci
            List<String> buff = new List<string>() { String.Empty };
            buff.AddRange(tag.Proj.InternalTagsDrv.Timers.Select(x => x.Name));

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(buff);
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Wlaczenie parametru
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