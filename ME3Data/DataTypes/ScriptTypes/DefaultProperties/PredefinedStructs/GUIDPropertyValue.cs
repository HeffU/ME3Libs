using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class GUIDPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue A;
        public IntPropertyValue B;
        public IntPropertyValue C;
        public IntPropertyValue D;

        public GUIDPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            A = new IntPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;
            B = new IntPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            C = new IntPropertyValue(Data, PCC);
            if (!C.Deserialize())
                return false;
            D = new IntPropertyValue(Data, PCC);
            if (!D.Deserialize())
                return false;

            return true;
        }
    }
}
