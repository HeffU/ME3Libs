using ME3Data.DataTypes;
using ME3Data.DataTypes.ScriptTypes;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.FileFormats.PCC
{
    public class ExportTableEntry : ObjectTableEntry
    {
        /// <summary>
        /// Name of this object's super object;
        /// </summary>
        public ME3Object SuperName;

        /// <summary>
        /// The flags of this export object.
        /// </summary>
        public ObjectFlags ObjectFlags;

        /// <summary>
        /// Size of the object in bytes
        /// </summary>
        public UInt32 Size;

        /// <summary>
        /// Offset for the object relative to the start of the PCCFile
        /// </summary>
        public UInt32 FileOffset;

        /// <summary>
        /// Flags for this export entry.
        /// </summary>
        public ExportFlags ExportFlags;

        /// <summary>
        /// Size of an export table entry.
        /// </summary>
        public static UInt32 SizeInBytes = 68;

        /// <summary>
        /// True if the export entry is generated for a fully native import.
        /// </summary>
        public bool FullyNative = false;


        private Int32 _ClassIndex;
        private Int32 _SuperIndex;
        private NameReference _ObjectNameRef;
        private Int32 _ArchetypeIndex;

        public ExportTableEntry(PCCFile current, ObjectReader data)
            :base(current, data)
        {

        }

        public int Deserialize()
        {

            // TODO: Handle dependencies
            // 2nd pass after this to resolve names etc?
            _ClassIndex = Data.ReadInt32();
            ClassName = CurrentPCC.GetClassName(_ClassIndex);
            _SuperIndex = Data.ReadInt32();
            _OuterIndex = Data.ReadInt32();

            _ObjectNameRef = Data.ReadNameRef();
            ObjectName = CurrentPCC.GetName(_ObjectNameRef);
            if (_ObjectNameRef.ModNumber > -1)
                ObjectName += "_" + _ObjectNameRef.ModNumber;
            _ArchetypeIndex = Data.ReadInt32();
            ObjectFlags = (ObjectFlags)Data.ReadUInt64();

            Size = Data.ReadUInt32();
            FileOffset = Data.ReadUInt32();
            ExportFlags = (ExportFlags)Data.ReadUInt32();

            // TODO: save / figure these out!
            var netObjectCount = Data.ReadInt32(); // Skip netObjectCount
            Data.ReadRawData(netObjectCount * 4); // Skip netObjects

            Data.ReadRawData(16); // Skip package GUID's
            Data.ReadRawData(4); // Skip package flags

            return (int)SizeInBytes + 4 * netObjectCount;
        }

        public bool ResolveLinks()
        {
            OuterName = CurrentPCC.GetObjectEntry(_OuterIndex).ObjectName;
            return true;
        }
    }
}
