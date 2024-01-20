using System.ComponentModel;

namespace ProjectDataLib
{
    public class BlockAdressVisibility : Int32Converter
    {
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            Tag tg = (Tag)context.Instance;
            return !tg.idrv.AuxParam[1];
        }
    }
}