using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class StructPropertyValue : DefaultPropertyValue
    {
        public String StructName;
        public List<DefaultPropertyValue> MemberValues;

        public byte[] Value;
        public List<ME3DefaultProperty> MemberProperties;

        public StructPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size, String name)
            : base(data, pcc, size)
        {
            StructName = name;
        }

        public override bool Deserialize()
        {
            MemberProperties = new List<ME3DefaultProperty>();
            var current = new ME3DefaultProperty(Data, PCC);

            while (current.Deserialize())
            {
                MemberProperties.Add(current);
                current = new ME3DefaultProperty(Data, PCC);
            }
            return true;
        }
    }
}
