using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class ActorReferencePropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue ActorIndex;
        public GUIDPropertyValue ActorGUID;

        public ActorReferencePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 20) { }

        public override bool Deserialize()
        {
            ActorIndex = new IntPropertyValue(Data, PCC);
            if (!ActorIndex.Deserialize())
                return false;
            ActorGUID = new GUIDPropertyValue(Data, PCC);
            if (!ActorGUID.Deserialize())
                return false;

            return true;
        }
    }
}
