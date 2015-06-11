using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class NamePropertyValue : DefaultPropertyValue
    {
        public String Name;

        public NamePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 8) { }

        public override bool Deserialize()
        {
            Name = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }
}
