using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3State : ME3Struct
    {
        // Function masks
        public Int32 ProbeMask;
        public Int64 IgnoreMask;

        public Int16 LabelTableOffset;
        public StateFlags StateFlags;

        public Int32 FunctionMapCount;
        public List<ME3Function> FunctionMap;

        private Int32 _unknown;

        private List<FunctionMapEntry> _FunctionMap;

        public ME3State(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
            _FunctionMap = new List<FunctionMapEntry>();
        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _unknown = Data.ReadInt32();

            ProbeMask = Data.ReadInt32();
            IgnoreMask = Data.ReadInt64();

            LabelTableOffset = Data.ReadInt16();
            StateFlags = (StateFlags)Data.ReadInt32();

            FunctionMapCount = Data.ReadInt32();
            for (int i = 0; i < FunctionMapCount; i++)
            {
                var funcEntry = new FunctionMapEntry();
                funcEntry.NameRef.Index = Data.ReadInt32();
                funcEntry.NameRef.ModNumber = Data.ReadInt32();
                funcEntry.ObjectIndex = Data.ReadInt32();
                _FunctionMap.Add(funcEntry);
            }

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            FunctionMap = new List<ME3Function>();
            foreach (var funcEntry in _FunctionMap)
            {
                var func = PCC.GetExportObject(funcEntry.ObjectIndex) as ME3Function;
                if (func == null)
                    return false;
                FunctionMap.Add(func);
            }

            return result;
        }
    }
}
