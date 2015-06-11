using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class StrPropertyValue : DefaultPropertyValue
    {
        public String Value;

        public StrPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadString();
            return true;
        }
    }
}
