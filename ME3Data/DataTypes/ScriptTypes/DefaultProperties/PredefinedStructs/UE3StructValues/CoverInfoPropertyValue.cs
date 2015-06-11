using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class CoverInfoPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue CoverLinkIndex;
        public IntPropertyValue SlotIndex;

        public CoverInfoPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 8) { }

        public override bool Deserialize()
        {
            CoverLinkIndex = new IntPropertyValue(Data, PCC);
            if (!CoverLinkIndex.Deserialize())
                return false;
            SlotIndex = new IntPropertyValue(Data, PCC);
            if (!SlotIndex.Deserialize())
                return false;

            return true;
        }
    }
}
