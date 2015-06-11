using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class IntPropertyValue : DefaultPropertyValue
    {
        public Int32 Value;

        public IntPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Value = Data.ReadInt32();
            return true;
        }
    }
}
