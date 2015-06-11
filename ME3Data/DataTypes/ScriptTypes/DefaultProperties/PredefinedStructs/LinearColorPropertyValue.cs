using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class LinearColorPropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue B;
        public FloatPropertyValue G;
        public FloatPropertyValue R;
        public FloatPropertyValue A;

        public LinearColorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            B = new FloatPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            G = new FloatPropertyValue(Data, PCC);
            if (!G.Deserialize())
                return false;
            R = new FloatPropertyValue(Data, PCC);
            if (!R.Deserialize())
                return false;
            A = new FloatPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;

            return true;
        }
    }
}
