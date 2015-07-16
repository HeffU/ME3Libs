
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
        public Int32 ObjectIndex;
    }

    public struct InterfaceMapEntry
    {
        public Int32 ObjectIndex;
        public Int32 TypeIndex;
    }

    public struct ComponentMapEntry
    {
        public NameReference NameRef;
        public Int32 ObjectIndex; // unknown?
    }

    public struct LabelTableEntry
    {
        public NameReference NameRef;
        public String Name;
        public UInt32 Offset; // standard bytescript MemOffs
    }
}