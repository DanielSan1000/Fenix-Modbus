using System;
using System.ComponentModel;
using System.Globalization;

namespace ProjectDataLib
{
    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid dla rodzaju sterowników
    /// </summary>
    public class EmptyConverter : StringConverter
    {
        /// <summary>
        /// Zabepzievznie
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Analiza tego co jest do wyswietlenia
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //Zabezpieczenie przeciwko System.String
            if (destinationType.FullName == "System.String" || destinationType.FullName == "(Collections)")
                return String.Empty;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}