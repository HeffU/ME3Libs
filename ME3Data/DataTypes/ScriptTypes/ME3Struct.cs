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
        public byte[] ByteScript;
        public Int32 DataScriptSize;
        public byte[] DataScript;

        public ME3Struct(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            FirstChildIndex = Data.ReadIndex();

            ByteScriptSize = Data.ReadInt32();
            ByteScript = new byte[ByteScriptSize];
            DataScriptSize = Data.ReadInt32();
            DataScript = Data.ReadRawData(DataScriptSize);

            if (DataScriptSize > 0)
                Array.Copy(DataScript, 0, ByteScript, ByteScriptSize - DataScriptSize, DataScriptSize);

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            Members = new List<ME3Object>();
            Structs = new List<ME3Struct>();
            Enums = new List<ME3Enum>();
            Variables = new List<ME3Property>();
            Constants = new List<ME3Const>();

            ME3Field obj = PCC.GetExportObject(FirstChildIndex) as ME3Field;
            while (obj != null)
            {
                Members.Add(obj);
                if (obj.GetType().IsSubclassOf(typeof(ME3Property)))
                {
                    Variables.Add(obj as ME3Property);
                } 
                else if (String.Compare(obj.ExportEntry.ClassName, "enum", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Enums.Add(obj as ME3Enum);
                } 
                else if (String.Compare(obj.ExportEntry.ClassName, "const", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Constants.Add(obj as ME3Const);
                }
                else if (String.Compare(obj.ExportEntry.ClassName, "Struct", StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(obj.ExportEntry.ClassName, "ScriptStruct", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Structs.Add(obj as ME3Struct);
                }
                obj = obj.NextField;
            }

            return result;
        }
    }
}
