using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.BioWareStructValues
{
    public class BioRwBoxPropertyValue : DefaultPropertyValue
    {
        public RwVector3PropertyValue Min; // TODO: figure out if this is really the correct syntax
        public RwVector3PropertyValue Max; // data seemed to match with the BoxProprtyValue but who knows.
        public BytePropertyValue IsValid;

        public BioRwBoxPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 25) { }

        public override bool Deserialize()
        {
            Min = new RwVector3PropertyValue(Data, PCC);
            if (!Min.Deserialize())
                return false;
            Max = new RwVector3PropertyValue(Data, PCC);
            if (!Max.Deserialize())
                return false;
            IsValid = new BytePropertyValue(Data, PCC);
            if (!IsValid.Deserialize())
                return false;

            return true;
        }
    }
}
