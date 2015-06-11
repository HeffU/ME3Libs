using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.BioWareStructValues
{
    public class RwVector2PropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue X;
        public FloatPropertyValue Y;

        public RwVector2PropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 12) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC);
            if (!Y.Deserialize())
                return false;

            return true;
        }
    }
}
