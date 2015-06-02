using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3DefaultProperty
    {
        public String Name;
        public String TypeName;

        // Common data
        public UInt32 Size;
        public UInt32 ArrayIndex;

        // Struct only
        public String MemberName;

        // Enum instance
        public String EnumName;

        // Bool only
        public Boolean BoolValue;

        //---

        private NameReference NameRef;
        private NameReference TypeNameRef;

        private PCCFile PCC;
        private ObjectReader Data;

        public ME3DefaultProperty(ObjectReader data, PCCFile pccObj)
        {
            Data = data;
            PCC = pccObj;
        }

        public bool Deserialize()
        {
            NameRef = Data.ReadNameRef();

            
            if (String.Equals(PCC.GetName(NameRef), "None", StringComparison.OrdinalIgnoreCase))
                return false; 

            TypeNameRef = Data.ReadNameRef();

            Size = Data.ReadUInt32();
            ArrayIndex = Data.ReadUInt32();

            // Todo: read data depending on type.
            var skip = Data.ReadRawData((int)Size);

            return true;
        }
    }
}
