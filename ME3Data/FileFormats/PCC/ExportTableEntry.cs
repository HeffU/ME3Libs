using ME3Data.DataTypes.ScriptTypes;
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
    }
}
