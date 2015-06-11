using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.BioWareValues
{
    public class BioMask4PropertyValue : DefaultPropertyValue
    {
        public byte Mask;

        public BioMask4PropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 1) { }

        public override bool Deserialize()
        {
            Mask = Data.ReadByte();
            return true;
        }
    }
}
