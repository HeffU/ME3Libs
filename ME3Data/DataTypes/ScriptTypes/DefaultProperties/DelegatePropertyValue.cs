using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties
{
    public class DelegatePropertyValue : DefaultPropertyValue
    {
        public Int32 OuterIndex; // Object that contains the assigned delegate
        public String DelegateValue;
        public String DelegateName;

        public DelegatePropertyValue(ObjectReader data, PCCFile pcc, String name)
            : base(data, pcc, 12)
        {
            DelegateName = name;//name.Substring(2, name.Length - 12);
        }

        public override bool Deserialize()
        {
            OuterIndex = Data.ReadIndex();
            DelegateValue = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }
}
