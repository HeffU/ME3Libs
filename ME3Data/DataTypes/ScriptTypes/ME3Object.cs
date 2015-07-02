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

        public virtual String Name { get { return ExportEntry.ObjectName; } }

        public ExportTableEntry ExportEntry;

        protected PCCFile PCC;
        protected ObjectReader Data;

        public ME3Object(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
        {
            PCC = pcc;
            Data = data;
            ExportEntry = exp;
        }

        public virtual bool Deserialize()
        {
            NetIndex = Data.ReadIndex();

            if (!String.Equals(ExportEntry.ClassName, "Class", StringComparison.OrdinalIgnoreCase))
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

        public virtual bool ResolveLinks()
        {
            return true;
        }
    }
}
