using System.ComponentModel;

namespace ProjectDataLib
{
    public class BlockConverter : UInt16Converter
    {
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            Device dev = (Device)context.Instance;

            return !dev.idrv.AuxParam[0];
        }
    }
}