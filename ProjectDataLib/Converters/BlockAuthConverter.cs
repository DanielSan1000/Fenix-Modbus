using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace ProjectDataLib
{
    public class BlockAuthConverter : StringConverter
    {
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[]
            {
                "Anonymous",
                "Basic"
            });
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (AuthenticationSchemes)Enum.Parse(typeof(AuthenticationSchemes), (string)value);
        }
    }
}