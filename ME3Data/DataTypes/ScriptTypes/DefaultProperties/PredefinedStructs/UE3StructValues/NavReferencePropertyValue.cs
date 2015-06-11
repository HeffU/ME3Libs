using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class NavReferencePropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue NavPointIndex;
        public GUIDPropertyValue NavPointGUID;

        public NavReferencePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 20) { }

        public override bool Deserialize()
        {
            NavPointIndex = new IntPropertyValue(Data, PCC);
            if (!NavPointIndex.Deserialize())
                return false;
            NavPointGUID = new GUIDPropertyValue(Data, PCC);
            if (!NavPointGUID.Deserialize())
                return false;

            return true;
        }
    }
}
