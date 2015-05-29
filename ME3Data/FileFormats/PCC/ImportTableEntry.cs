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


        private NameReference _PCCNameRef;
        private NameReference _ClassNameRef;
        private Int32 _OuterIndex;
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

            _ObjectNameRef = Data.ReadNameRef();
            ObjectName = CurrentPCC.GetName(_ObjectNameRef);

            return true;
        }

        public bool LoadFromSource(PCCFile source)
        {
            SourcePCC = source;

            SourceEntry = SourcePCC.GetExportByName(ObjectName);
            if (SourceEntry == null)
                return false;

            Object = SourceEntry.Object;
            ObjectType = SourceEntry.ObjectType;

            var OuterEntry = SourcePCC.Exports[_OuterIndex];
            if (OuterEntry == null)
                return false;
            OuterObject = OuterEntry.Object;
            OuterName = OuterEntry.ObjectName;

            FullyLoaded = true;

            return true;
        }
    }
}
