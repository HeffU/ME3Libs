
using ME3Data.DataTypes.ScriptTypes;
using ME3Data.FileFormats.PCC;
using System;

namespace ME3Data.DataTypes
{
    public struct NameReference
    {
        public Int32 Index;
        public Int32 ModNumber; // -1 if it's not pointing to anything, // >= 0 if numeric
                                // Default properties have their own standard, 0 <= numeric <= 0x1F(?) < secondaryName
                                // Where secondaryName is a new namereference, unsure of purpose, usually components.
    }

    public struct FunctionMapEntry
    {
        public NameReference NameRef;
        public Int32 FunctionObjectIndex;
    }

    public struct InterfaceMapEntry
    {
        public Int32 ClassIndex; // index of the interface class
        public Int32 PropertyPointer; // index of some property from interface vtable? null for non-native interfaces
    }

    public struct ComponentMapEntry
    {
        public NameReference NameRef; // template name of component in default props
        public Int32 ComponentObjectIndex; // component template itself
    }

    public struct LabelTableEntry
    {
        public NameReference NameRef;
        public String Name;
        public UInt32 Offset; // standard bytescript MemOffs
    }

    public struct ObjectStackFrame // TODO: figure this out
    {
        public Int32 NodeIndex;
        public ObjectTableEntry Node;      // Object
        public Int32 StateNodeIndex;
        public ObjectTableEntry StateNode; // Object->Class
        public UInt64 ProbeMask;
        public Int16 LatentActionIndex;
        public ObjectTableEntry LatentAction;
        public Int32 Unkn1;
        public Int32 Unkn2;
        //public Int32 UnknCount; Most likely NetIndex
    }
}