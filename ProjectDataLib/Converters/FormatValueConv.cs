using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjectDataLib
{
    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid dla rodzaju sterowników
    /// </summary>
    public class FormatValueConv : StringConverter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            List<String> lista = new List<string>();
            lista.Add("{0:0.0}");
            lista.Add("{0:0.00}");
            lista.Add("{0:0.000}");
            lista.Add("{0:00.000}");
            lista.Add("{0:000.000}");
            lista.Add("{0:0000.0000}");
            lista.Add("{0:ASCII}");

            if (tg.TypeData_ == TypeData.BYTE)
                lista.Add("{0:X4}");

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(lista.ToArray());
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            if (tg.TypeData_ == TypeData.BIT)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Wlaczenie parametru
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            if (tg.TypeData_ == TypeData.BIT)
                return false;
            else
                return true;
        }
    }
}