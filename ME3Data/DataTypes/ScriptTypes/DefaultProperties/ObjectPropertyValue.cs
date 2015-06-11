using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class ObjectPropertyValue : DefaultPropertyValue
    {
        public ME3Object Object { get { return PCC.GetExportObject(Index); } }
        public Int32 Index;

        public ObjectPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Index = Data.ReadIndex();
            return true;
        }
    }
}
