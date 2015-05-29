using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Field : ME3Object
    {
        public ME3Field SuperField;
        public ME3Field NextField;

        protected int _SuperIndex;
        protected int _NextIndex;

        public ME3Field(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
        }

        public bool Deserialize()
        {
            base.Deserialize();

            _SuperIndex = Data.ReadIndex();
            _NextIndex = Data.ReadIndex();

            return true;
        }
    }
}
