using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Object
    {
        // Network
        public int NetIndex = -1; // only valid if !HasStack
        // Stack
        public ObjectStackFrame Stack; // only valid if HasStack

        // Default properties for anything except 'Class' types
        public List<ME3DefaultProperty> DefaultProperties;

        public virtual String Name { get { return ExportEntry.ObjectName; } }

        public ExportTableEntry ExportEntry;

        protected PCCFile PCC;
        protected ObjectReader Data;

        public ME3Object(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
        {
            PCC = pcc;
            Data = data;
            ExportEntry = exp;
        }

        public virtual bool Deserialize()
        {
            // TODO
            if (ExportEntry.ObjectFlags.HasFlag(ObjectFlags.HasStack))
                DeserializeStack();
            
            NetIndex = Data.ReadIndex();

            if (!String.Equals(ExportEntry.ClassName, "Class", StringComparison.OrdinalIgnoreCase)
                // Work-around for totally undocumented class that does not follow standard unreal default property structure.
                && !ExportEntry.ClassName.Contains("Component"))
            {
                return DeserializeDefaultProperties();
            }

            return true;
        }

        public bool DeserializeDefaultProperties()
        {
            DefaultProperties = new List<ME3DefaultProperty>();
            var current = new ME3DefaultProperty(Data, PCC);

            while (current.Deserialize())
            {
                DefaultProperties.Add(current);
                current = new ME3DefaultProperty(Data, PCC);
            }

            return true;
        }

        public bool DeserializeStack()
        {
            Stack = new ObjectStackFrame();
            Stack.NodeIndex = Data.ReadIndex();
            Stack.Node = PCC.GetObjectEntry(Stack.NodeIndex);
            Stack.StateNodeIndex = Data.ReadIndex();
            Stack.StateNode = PCC.GetObjectEntry(Stack.StateNodeIndex);
            Stack.ProbeMask = Data.ReadUInt64();
            Stack.LatentActionIndex = Data.ReadInt16();
            Stack.LatentAction = PCC.GetObjectEntry(Stack.LatentActionIndex);
            Stack.Unkn1 = Data.ReadInt32();
            Stack.Unkn2 = Data.ReadInt32();

            return true;
        }

        public virtual bool ResolveLinks()
        {
            return true;
        }
    }
}
