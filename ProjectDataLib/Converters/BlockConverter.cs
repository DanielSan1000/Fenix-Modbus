using System.ComponentModel;

namespace ProjectDataLib
{
    /// <summary>
    /// Blokowanie rozszerzonej adresaxji
    /// </summary>
    public class BlockConverter : UInt16Converter
    {
        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //Pobranie danych
            Device dev = (Device)context.Instance;

            //Rozszerzona adresacja
            return !dev.idrv.AuxParam[0];
        }
    }
}