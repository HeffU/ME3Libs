using ME3Data.DataTypes;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.FileFormats.PCC
{
    public class PCCFile
    {
        /// <summary>
        /// List of all Export objects in this PCC.
        /// </summary>
        public List<ExportTableEntry> Exports { public get; private set; }

        /// <summary>
        /// List of all Import objects in this PCC.
        /// </summary>
        public List<ImportTableEntry> Imports { public get; private set; }

        /// <summary>
        /// List of all names in this PCC.
        /// </summary>
        public List<String> Names { public get; private set; }


        private ObjectReader Data;

        public PCCFile(ObjectReader data)
        {
            Data = data;
            Exports = new List<ExportTableEntry>();
            Imports = new List<ImportTableEntry>();
            Names = new List<String>();
        }

        public bool Deserialize()
        {
            return true;
        }

        public ExportTableEntry GetExportByName(String name)
        {

            return Exports.FirstOrDefault(x => String.Equals(x.ObjectName, name, StringComparison.OrdinalIgnoreCase));
        }

        public String GetName(NameReference reference)
        {
            return reference.Index >= 0 && reference.Index < Names.Count ? Names[reference.Index] : null;
        }
    }
}
