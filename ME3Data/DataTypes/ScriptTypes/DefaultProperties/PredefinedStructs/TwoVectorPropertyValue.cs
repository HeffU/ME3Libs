using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class TwoVectorsPropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue A;
        public VectorPropertyValue B;

        public TwoVectorsPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 24) { }

        public override bool Deserialize()
        {
            A = new VectorPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;
            B = new VectorPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;

            return true;
        }
    }
}
