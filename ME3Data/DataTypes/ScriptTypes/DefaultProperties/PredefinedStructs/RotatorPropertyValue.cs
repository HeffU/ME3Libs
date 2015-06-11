using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class RotatorPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue Pitch;
        public IntPropertyValue Yaw;
        public IntPropertyValue Roll;

        public RotatorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 12) { }

        public override bool Deserialize()
        {
            Pitch = new IntPropertyValue(Data, PCC);
            if (!Pitch.Deserialize())
                return false;
            Yaw = new IntPropertyValue(Data, PCC);
            if (!Yaw.Deserialize())
                return false;
            Roll = new IntPropertyValue(Data, PCC);
            if (!Roll.Deserialize())
                return false;

            return true;
        }
    }
}
