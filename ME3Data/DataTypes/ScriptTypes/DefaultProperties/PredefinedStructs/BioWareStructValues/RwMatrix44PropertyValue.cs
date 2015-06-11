using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.BioWareStructValues
{
    public class RwMatrix44PropertyValue : DefaultPropertyValue
    {
        public RwVector4PropertyValue PlaneX;
        public RwVector4PropertyValue PlaneY;
        public RwVector4PropertyValue PlaneZ;
        public RwVector4PropertyValue PlaneW;

        public RwMatrix44PropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 64) { }

        public override bool Deserialize()
        {
            PlaneX = new RwVector4PropertyValue(Data, PCC);
            if (!PlaneX.Deserialize())
                return false;
            PlaneY = new RwVector4PropertyValue(Data, PCC);
            if (!PlaneY.Deserialize())
                return false;
            PlaneZ = new RwVector4PropertyValue(Data, PCC);
            if (!PlaneZ.Deserialize())
                return false;
            PlaneW = new RwVector4PropertyValue(Data, PCC);
            if (!PlaneW.Deserialize())
                return false;

            return true;
        }
    }
}
