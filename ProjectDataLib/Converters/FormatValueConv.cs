using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjectDataLib
{
    public class FormatValueConv : StringConverter
    {
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

            return new StandardValuesCollection(lista.ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            ITag tg = (ITag)context.Instance;

            if (tg.TypeData_ == TypeData.BIT)
                return true;
            else
                return false;
        }

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