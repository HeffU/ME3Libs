using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class AimTransformPropertyValue : DefaultPropertyValue
    {
        public QuatPropertyValue Quaternion;
        public VectorPropertyValue Translation;

        public AimTransformPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 28) { }

        public override bool Deserialize()
        {
            Quaternion = new QuatPropertyValue(Data, PCC);
            if (!Quaternion.Deserialize())
                return false;
            Translation = new VectorPropertyValue(Data, PCC);
            if (!Translation.Deserialize())
                return false;

            return true;
        }
    }
}
