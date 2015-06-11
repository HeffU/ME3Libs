using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class ScalePropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue Scale;
        public FloatPropertyValue SheerRate;
        public BytePropertyValue SheerAxis;

        public ScalePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 17) { }

        public override bool Deserialize()
        {
            Scale = new VectorPropertyValue(Data, PCC);
            if (!Scale.Deserialize())
                return false;
            SheerRate = new FloatPropertyValue(Data, PCC);
            if (!SheerRate.Deserialize())
                return false;
            SheerAxis = new BytePropertyValue(Data, PCC);
            if (!SheerAxis.Deserialize())
                return false;

            return true;
        }
    }
}
