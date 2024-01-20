using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;

namespace ProjectDataLib
{
    public class TagSecendaryAdress : Int32Converter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Tag tg = (Tag)context.Instance;

            IDriverModel idrv = ((Tag)context.Instance).idrv;

            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            List<int> opcje = new List<int>();

            if (mInf.AdresSize > tg.getSize())
            {
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                opcje.Add(0);
            }

            return new StandardValuesCollection(opcje.ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Tag tg = (Tag)context.Instance;

            IDriverModel idrv = ((Tag)context.Instance).idrv;

            MemoryAreaInfo mInf = idrv.MemoryAreaInf.Where(x => x.Name.Equals(tg.areaData)).ToArray()[0];

            List<int> opcje = new List<int>();
            if (mInf.AdresSize > tg.getSize())
            {
                for (int i = 0; i < mInf.AdresSize / tg.getSize(); i++)
                    opcje.Add(i);
            }
            else
            {
                opcje.Add(0);
            }

            if (value is string)
            {
                try
                {
                    int val = int.Parse((string)value);

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