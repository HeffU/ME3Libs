using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
    public class MatrixPropertyValue : DefaultPropertyValue
    {
        public Vector4PropertyValue PlaneX;
        public Vector4PropertyValue PlaneY;
        public Vector4PropertyValue PlaneZ;
        public Vector4PropertyValue PlaneW;

        public MatrixPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 64) { }

        public override bool Deserialize()
        {
            PlaneX = new Vector4PropertyValue(Data, PCC);
            if (!PlaneX.Deserialize())
                return false;
            PlaneY = new Vector4PropertyValue(Data, PCC);
            if (!PlaneY.Deserialize())
                return false;
            PlaneZ = new Vector4PropertyValue(Data, PCC);
            if (!PlaneZ.Deserialize())
                return false;
            PlaneW = new Vector4PropertyValue(Data, PCC);
            if (!PlaneW.Deserialize())
                return false;

            return true;
        }
    }
}
