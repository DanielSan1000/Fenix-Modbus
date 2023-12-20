using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;

namespace ProjectDataLib
{
    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class TagSecendaryAdress : Int32Converter
    {
        /// <summary>
        /// Wlasciwość wyswietla dane w zaleznoci od rodzaju sterownika
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;

            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Obszar pamięci
            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            //Lista dostępnych opcji
            List<int> opcje = new List<int>();

            //Wypalnianie Secand Adress
            if (mInf.AdresSize > tg.getSize())
            {
                //Dodanie elementu do wyboru
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                //Brak opcji
                opcje.Add(0);
            }

            //Wyswietlenie dostępnych obszarow pamieci dla Taga
            return new StandardValuesCollection(opcje.ToArray());
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
            return true;
        }

        /// <summary>
        /// Proba konwersji
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;

            //Pobranie interfejsu sterownika
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            //Obszar pamięci
            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            //Lista dostępnych opcji
            List<int> opcje = new List<int>();
            if (mInf.AdresSize > tg.getSize())
            {
                //Dodanie elementu do wyboru
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                //Brak opcji
                opcje.Add(0);
            }

            //Konwersja
            if (value is string)
            {
                try
                {
                    //Konwersja pobranej wartości
                    int val = int.Parse((string)value);

                    //Sprawdzenie zakresu
                    if (val < 0 || val > opcje.Count - 1)
                        return 0;
                    else
                        return val;
                }
                catch (Exception)
                {
                    return 0;
                }
            }

            return 0;
        }
    }
}