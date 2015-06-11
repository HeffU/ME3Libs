using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class BytePropertyValue : DefaultPropertyValue
    {
        public byte Value;

        public BytePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 1) { }

        public override bool Deserialize()
        {
            Value = Data.ReadByte();
            return true;
        }
    }
}
