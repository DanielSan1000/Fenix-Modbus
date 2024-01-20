using System;
using System.ComponentModel;

namespace ProjectDataLib
{
    public class DriverKind : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            String[] sterowniki = ((Connection)context.Instance).gConf.getDrvLst();

            return new StandardValuesCollection(sterowniki);
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