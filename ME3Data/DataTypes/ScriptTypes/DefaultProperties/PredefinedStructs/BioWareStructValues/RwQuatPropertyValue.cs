using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.BioWareStructValues
{
    public class RwQuatPropertyValue : DefaultPropertyValue
    {
        public RwVector4PropertyValue Vector4;

        public RwQuatPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 12) { }

        public override bool Deserialize()
        {
            Vector4 = new RwVector4PropertyValue(Data, PCC);
            if (!Vector4.Deserialize())
                return false;

            return true;
        }
    }
}
