using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3ScriptStruct : ME3Struct
    {
        public Int32 StructFlags;
        public List<ME3DefaultProperty> MemberDefaultProperties;


        private NameReference _UnknownData;

        public ME3ScriptStruct(ObjectReader data, ExportTableEntry exp, PCCFile pccObj)
            : base(data, exp, pccObj)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            StructFlags = Data.ReadInt32();
            // Seen flags:
            // 110101
            if ((StructFlags & 0x30) != 0)
            {
                //_UnknownData = Data.ReadNameRef();
                //var unkn = PCC.GetName(_UnknownData);
            }

            DeserializeMemberProperties();

            return result;
        }

        public bool DeserializeMemberProperties()
        {
            MemberDefaultProperties = new List<ME3DefaultProperty>();
            var current = new ME3DefaultProperty(Data, PCC);

            while (current.Deserialize())
            {
                MemberDefaultProperties.Add(current);
                current = new ME3DefaultProperty(Data, PCC);
            }

            return true;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            return result;
        }
    }
}
