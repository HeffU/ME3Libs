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
        public Int32 ProbeMask; // TODO: understand probe functions in UE3
        public Int64 IgnoreMask;

        // Relative to ByteScript start
        public Int16 LabelTableOffset;
        public StateFlags StateFlags;

        public Int32 FunctionMapCount;
        public List<ME3Function> FunctionMap; // Should be a map by name for all functions available in this state

        public List<ME3Function> DefinedFunctions;

        public List<LabelTableEntry> LabelTable;

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

            _unknown = Data.ReadInt32(); // Not always zero, looks like flags or mask?

            ProbeMask = Data.ReadInt32();
            IgnoreMask = Data.ReadInt64();

            LabelTableOffset = Data.ReadInt16();

            // This can't be processed here unless the bytecode is also processed for virtual address space
            /*if (LabelTableOffset >= 0) 
            {
                LabelTable = new List<LabelTableEntry>();
                var tableReader = new ObjectReader(ByteScript);
                tableReader.ReadRawData(LabelTableOffset);

                var NameRef = tableReader.ReadNameRef();
                var Offset = tableReader.ReadUInt32();
                while (Offset != 0x0000FFFF)
                {
                    var entry = new LabelTableEntry();
                    entry.NameRef = NameRef;
                    entry.Name = PCC.GetName(NameRef);
                    entry.Offset = Offset;
                    LabelTable.Add(entry);

                    NameRef = tableReader.ReadNameRef();
                    Offset = tableReader.ReadUInt32();
                }
            }*/

            StateFlags = (StateFlags)Data.ReadInt32();

            FunctionMapCount = Data.ReadInt32();
            for (int i = 0; i < FunctionMapCount; i++)
            {
                var funcEntry = new FunctionMapEntry();
                funcEntry.NameRef.Index = Data.ReadInt32();
                funcEntry.NameRef.ModNumber = Data.ReadInt32();
                funcEntry.FunctionObjectIndex = Data.ReadInt32();
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
                var func = PCC.GetExportObject(funcEntry.FunctionObjectIndex) as ME3Function;
                if (func == null)
                    return false;
                FunctionMap.Add(func);
            }

            DefinedFunctions = new List<ME3Function>();
            foreach (var member in Members)
            {
                if (member.GetType() == typeof(ME3Function))
                {
                    DefinedFunctions.Add(member as ME3Function);
                }
            }

            return result;
        }
    }
}
