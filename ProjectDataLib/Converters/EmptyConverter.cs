using System;
using System.ComponentModel;
using System.Globalization;

namespace ProjectDataLib
{
    public class EmptyConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.FullName == "System.String" || destinationType.FullName == "(Collections)")
                return String.Empty;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}