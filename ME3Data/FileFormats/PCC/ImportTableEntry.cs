using ME3Data.DataTypes.ScriptTypes;
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
        /// Export entry in the source PCC.
        /// </summary>
        public ExportTableEntry SourceEntry;
    }
}
