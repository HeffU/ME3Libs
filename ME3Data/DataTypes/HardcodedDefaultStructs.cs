using ME3Data.DataTypes.ScriptTypes;
using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes
{
    public class VectorPropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue X;
        public FloatPropertyValue Y;
        public FloatPropertyValue Z;

        public VectorPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC, 4);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC, 4);
            if (!Y.Deserialize())
                return false;
            Z = new FloatPropertyValue(Data, PCC, 4);
            if (!Z.Deserialize())
                return false;

            return true;
        }
    }

    public class ColorPropertyValue : DefaultPropertyValue
    {
        public BytePropertyValue B;
        public BytePropertyValue G;
        public BytePropertyValue R;
        public BytePropertyValue A;

        public ColorPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            B = new BytePropertyValue(Data, PCC, 1);
            if (!B.Deserialize())
                return false;
            G = new BytePropertyValue(Data, PCC, 1);
            if (!G.Deserialize())
                return false;
            R = new BytePropertyValue(Data, PCC, 1);
            if (!R.Deserialize())
                return false;
            A = new BytePropertyValue(Data, PCC, 1);
            if (!A.Deserialize())
                return false;

            return true;
        }
    }

    public class LinearColorPropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue B;
        public FloatPropertyValue G;
        public FloatPropertyValue R;
        public FloatPropertyValue A;

        public LinearColorPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            B = new FloatPropertyValue(Data, PCC, 4);
            if (!B.Deserialize())
                return false;
            G = new FloatPropertyValue(Data, PCC, 4);
            if (!G.Deserialize())
                return false;
            R = new FloatPropertyValue(Data, PCC, 4);
            if (!R.Deserialize())
                return false;
            A = new FloatPropertyValue(Data, PCC, 4);
            if (!A.Deserialize())
                return false;

            return true;
        }
    }

    public class VectorPairPropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue A;
        public VectorPropertyValue B;

        public VectorPairPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            A = new VectorPropertyValue(Data, PCC, 4);
            if (!A.Deserialize())
                return false;
            B = new VectorPropertyValue(Data, PCC, 4);
            if (!B.Deserialize())
                return false;

            return true;
        }
    }

    public class Vector4PropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue V;
        public FloatPropertyValue W;

        public Vector4PropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            V = new VectorPropertyValue(Data, PCC, 4);
            if (!V.Deserialize())
                return false;
            W = new FloatPropertyValue(Data, PCC, 4);
            if (!W.Deserialize())
                return false;

            return true;
        }
    }

    public class Vector2DPropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue X;
        public FloatPropertyValue Y;

        public Vector2DPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC, 4);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC, 4);
            if (!Y.Deserialize())
                return false;

            return true;
        }
    }
}
