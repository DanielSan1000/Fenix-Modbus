using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    internal class DanConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is Object)
                return " ";

            return base.ConvertTo(context, culture, value, destType);
        }
    }
}