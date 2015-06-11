using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class EnumPropertyValue : DefaultPropertyValue
    {
        public String EnumName;
        public String EnumValue;

        public EnumPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size, String name)
            : base(data, pcc, size)
        {
            EnumName = name;
        }


        public override bool Deserialize()
        {
            EnumValue = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }
}
