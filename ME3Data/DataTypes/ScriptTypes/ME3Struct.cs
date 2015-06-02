using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Struct : ME3Field
    {
        public Int32 FirstChildIndex;

        // Members
        public List<ME3Object> Members;
        // By type:
        public List<ME3Struct> Structs;
        public List<ME3Enum> Enums;
        public List<ME3Property> Variables;
        public List<ME3Const> Constants;

        // Script-related
        public Int32 ByteScriptSize;
        public Int32 DataScriptSize;
        public byte[] DataScript;

        public ME3Struct(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
        }

        public bool Deserialize()
        {
            base.Deserialize();

            FirstChildIndex = Data.ReadIndex();

            ByteScriptSize = Data.ReadInt32();
            DataScriptSize = Data.ReadInt32();
            DataScript = Data.ReadRawData(DataScriptSize);

            return true;
        }

        public void LinkMembers()
        {

        }
    }
}
