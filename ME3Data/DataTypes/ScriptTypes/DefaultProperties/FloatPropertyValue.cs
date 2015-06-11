using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class FloatPropertyValue : DefaultPropertyValue
    {
        public float Value;

        public FloatPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Value = Data.ReadFloat();
            return true;
        }
    }
}
