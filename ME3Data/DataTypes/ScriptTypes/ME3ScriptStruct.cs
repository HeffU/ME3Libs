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


        public ME3ScriptStruct(ObjectReader data, ExportTableEntry exp, PCCFile pccObj)
            : base(data, exp, pccObj)
        {

        }

        public bool Deserialize()
        {
            base.Deserialize();

            StructFlags = Data.ReadInt32();
            DeserializeDefaultProperties();

            return true;
        }
    }
}
