
using System;

namespace ME3Data.DataTypes
{
    public struct NameReference
    {
        public Int32 Index;
        public Int32 ModNumber;
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
        public UInt32 Offset; // Relative to DataScript start (?)
    }
}