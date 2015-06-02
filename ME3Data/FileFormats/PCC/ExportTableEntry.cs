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
        public UInt64 ObjectFlags;

        /// <summary>
        /// Size of the object in bytes
        /// </summary>
        public UInt32 Size;

        /// <summary>
        /// Offset for the object relative to the start of the PCCFile
        /// </summary>
        public UInt32 FileOffset;


        private Int32 _ClassIndex;
        private Int32 _SuperIndex;
        private Int32 _OuterIntex;
        private NameReference _ObjectNameRef;
        private Int32 _ArchetypeIndex;

        public ExportTableEntry(PCCFile current, ObjectReader data)
            :base(current, data)
        {

        }

        public bool Deserialize()
        {

            // TODO: Handle dependencies
            // 2nd pass after this to resolve names etc?
            _ClassIndex = Data.ReadInt32();
            _SuperIndex = Data.ReadInt32();
            _OuterIntex = Data.ReadInt32();
            _ObjectNameRef = Data.ReadNameRef();
            _ArchetypeIndex = Data.ReadInt32();
            ObjectFlags = Data.ReadUInt64();

            return true;
        }
    }
}
