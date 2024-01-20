using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    internal class NoName : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            return " ";
        }
    }
}