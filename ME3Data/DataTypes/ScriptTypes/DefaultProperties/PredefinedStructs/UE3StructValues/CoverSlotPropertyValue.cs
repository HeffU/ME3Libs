using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class CoverSlotPropertyValue : DefaultPropertyValue
    {
        public byte[] Values; // TODO: try to match with info from beyondunreal, both UT3/UDK

        public CoverSlotPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Values = Data.ReadRawData((int)Size);

            return true;
        }
    }
}
