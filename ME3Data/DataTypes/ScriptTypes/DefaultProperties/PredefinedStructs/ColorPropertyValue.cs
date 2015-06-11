using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class ColorPropertyValue : DefaultPropertyValue
    {
        public BytePropertyValue B;
        public BytePropertyValue G;
        public BytePropertyValue R;
        public BytePropertyValue A;

        public ColorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            B = new BytePropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            G = new BytePropertyValue(Data, PCC);
            if (!G.Deserialize())
                return false;
            R = new BytePropertyValue(Data, PCC);
            if (!R.Deserialize())
                return false;
            A = new BytePropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;

            return true;
        }
    }
}
