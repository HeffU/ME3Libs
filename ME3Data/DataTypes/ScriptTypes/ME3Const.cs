using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Const : ME3Field
    {
        public String Value;

        public ME3Const(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();
            Value = Data.ReadString();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            return result;
        }
    }
}
