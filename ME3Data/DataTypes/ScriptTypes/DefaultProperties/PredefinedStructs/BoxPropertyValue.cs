using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class BoxPropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue Min;
        public VectorPropertyValue Max;
        public BytePropertyValue IsValid;

        public BoxPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 25) { }

        public override bool Deserialize()
        {
            Min = new VectorPropertyValue(Data, PCC);
            if (!Min.Deserialize())
                return false;
            Max = new VectorPropertyValue(Data, PCC);
            if (!Max.Deserialize())
                return false;
            IsValid = new BytePropertyValue(Data, PCC);
            if (!IsValid.Deserialize())
                return false;

            return true;
        }
    }
}
