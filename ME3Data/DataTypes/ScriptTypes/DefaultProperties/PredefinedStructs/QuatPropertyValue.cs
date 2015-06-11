using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class QuatPropertyValue : DefaultPropertyValue
    {
        public Vector4PropertyValue Vector4;

        public QuatPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            Vector4 = new Vector4PropertyValue(Data, PCC);
            if (!Vector4.Deserialize())
                return false;

            return true;
        }
    }
}
