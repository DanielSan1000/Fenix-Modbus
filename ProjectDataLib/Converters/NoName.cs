using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    /// <summary>
    /// Moja klasa zmiany nazwy parametrów dla parametrow sterownika
    /// </summary>
    internal class NoName : ExpandableObjectConverter
    {
        /// <summary>
        /// Zmiana wyświetlanej nazwy
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            return " ";
        }
    }
}