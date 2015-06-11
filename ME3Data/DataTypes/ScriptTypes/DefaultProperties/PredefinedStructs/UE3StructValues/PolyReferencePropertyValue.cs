using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.UE3StructValues
{
    public class PolyReferencePropertyValue : DefaultPropertyValue
    {
        public ActorReferencePropertyValue Actor;
        public IntPropertyValue PolyID;

        public PolyReferencePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 24) { }

        public override bool Deserialize()
        {
            Actor = new ActorReferencePropertyValue(Data, PCC);
            if (!Actor.Deserialize())
                return false;
            PolyID = new IntPropertyValue(Data, PCC);
            if (!PolyID.Deserialize())
                return false;

            return true;
        }
    }
}
