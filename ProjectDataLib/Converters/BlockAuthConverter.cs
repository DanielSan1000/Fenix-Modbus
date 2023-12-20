using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace ProjectDataLib
{
    /// <summary>
    /// Blokowanie autoryacji
    /// </summary>
    public class BlockAuthConverter : StringConverter
    {
        /// <summary>
        /// Kolekcja stadardowa
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[]
            {
                "Anonymous",
                "Basic"
            });
        }

        /// <summary>
        /// Czy pokazywac standardowe wartosci
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Powrot do typu
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        /// <summary>
        /// Konwetuj dane
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (AuthenticationSchemes)Enum.Parse(typeof(AuthenticationSchemes), (string)value);
        }
    }
}