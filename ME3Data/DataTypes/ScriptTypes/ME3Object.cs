using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Object
    {
        // Network
        public int NetIndex;

        // Default properties for anything except 'Class' types
        public List<ME3DefaultProperty> DefaultProperties;

        protected PCCFile PCC;
        protected ObjectReader Data;

        protected ExportTableEntry ExportEntry;

        public ME3Object(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
        {
            PCC = pcc;
            Data = data;
            ExportEntry = exp;
        }

        public bool Deserialize()
        {
            NetIndex = Data.ReadIndex();

            if (false) // ExportEntry.ClassName != "Class"
            {
                return DeserializeDefaultProperties();
            }

            return true;
        }

        public bool DeserializeDefaultProperties()
        {
            DefaultProperties = new List<ME3DefaultProperty>();
            var current = new ME3DefaultProperty(Data, PCC);

            while (current.Deserialize())
            {
                DefaultProperties.Add(current);
                current = new ME3DefaultProperty(Data, PCC);
            }

            return true;
        }
    }
}
