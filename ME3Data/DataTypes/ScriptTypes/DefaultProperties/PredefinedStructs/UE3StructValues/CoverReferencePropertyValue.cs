using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class CoverReferencePropertyValue : NavReferencePropertyValue
    {
        public IntPropertyValue SlotIndex;
        public IntPropertyValue Direction;

        public CoverReferencePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc) 
        {
            Size = 28;
        }

        public override bool Deserialize()
        {
            base.Deserialize();

            SlotIndex = new IntPropertyValue(Data, PCC);
            if (!SlotIndex.Deserialize())
                return false;
            Direction = new IntPropertyValue(Data, PCC);
            if (!Direction.Deserialize())
                return false;

            return true;
        }
    }
}
