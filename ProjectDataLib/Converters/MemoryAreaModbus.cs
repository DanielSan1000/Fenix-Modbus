using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace ProjectDataLib
{
    public class MemoryAreaModbus : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDriverModel idrv = ((Tag)context.Instance).idrv;

            return new StandardValuesCollection(idrv.MemoryAreaInf.Select(x => x.Name).ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }
    }
}