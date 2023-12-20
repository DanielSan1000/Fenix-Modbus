using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace ProjectDataLib
{
    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class MemoryAreaModbus : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(idrv.MemoryAreaInf.Select(x => x.Name).ToArray());
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