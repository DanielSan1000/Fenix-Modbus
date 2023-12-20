using System.ComponentModel;

namespace ProjectDataLib
{
    /// <summary>
    /// Formatowanie roznuch pozycji na Proprty Grid
    /// </summary>
    public class BlockAdressVisibility : Int32Converter
    {
        /// <summary>
        /// Mozliwosc edycji wartosci w polu
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //Pobranie Taga
            Tag tg = (Tag)context.Instance;
            return !tg.idrv.AuxParam[1];
        }
    }
}