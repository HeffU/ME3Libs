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
    public class ImportTableEntry : ObjectTableEntry
    {
        /// <summary>
        /// The PCC file that this import belongs to.
        /// Only valid if said file has been loaded beforehand.
        /// </summary>
        public PCCFile SourcePCC;

        /// <summary>
        /// Name of the PCC file that this import belongs to.
        /// </summary>
        public String SourcePCCName;

        /// <summary>
        /// Export entry in the source PCC.
        /// </summary>
        public ExportTableEntry SourceEntry;

        /// <summary>
        /// True if the source PCC has been deserialized and this object is available in full.
        /// </summary>
        public bool FullyLoaded;

        /// <summary>
        /// Size of an import table entry.
        /// </summary>
        public static UInt32 SizeInBytes = 28;

        private NameReference _PCCNameRef;
        private NameReference _ClassNameRef;
        private NameReference _ObjectNameRef;

        public ImportTableEntry(PCCFile current, ObjectReader data)
            :base(current, data)
        {

        }

        public bool Deserialize()
        {
            _PCCNameRef = Data.ReadNameRef();
            SourcePCCName = CurrentPCC.GetName(_PCCNameRef);

            _ClassNameRef = Data.ReadNameRef();
            ClassName = CurrentPCC.GetName(_ClassNameRef);

            _OuterIndex = Data.ReadInt32();
            // Do in links or loadfromsource:
            //OuterName = CurrentPCC.GetObjectEntry(_OuterIndex).ObjectName;

            _ObjectNameRef = Data.ReadNameRef();
            ObjectName = CurrentPCC.GetName(_ObjectNameRef);

            return true;
        }

        public bool LoadFromSource(PCCFile source)
        {
            SourcePCC = source;

            var entry = PCCFile.GetExportFromImport(this);
            if (entry == null)
                return false;
            SourceEntry = entry as ExportTableEntry;

            Object = SourceEntry.Object;
            ObjectType = SourceEntry.ObjectType;

            OuterObject = entry.OuterObject;
            OuterName = entry.OuterName;

            FullyLoaded = true;

            return true;
        }
    }
}
