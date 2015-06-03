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

        public VectorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 12) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC);
            if (!Y.Deserialize())
                return false;
            Z = new FloatPropertyValue(Data, PCC);
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

        public ColorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            B = new BytePropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            G = new BytePropertyValue(Data, PCC);
            if (!G.Deserialize())
                return false;
            R = new BytePropertyValue(Data, PCC);
            if (!R.Deserialize())
                return false;
            A = new BytePropertyValue(Data, PCC);
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

        public LinearColorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            B = new FloatPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            G = new FloatPropertyValue(Data, PCC);
            if (!G.Deserialize())
                return false;
            R = new FloatPropertyValue(Data, PCC);
            if (!R.Deserialize())
                return false;
            A = new FloatPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;

            return true;
        }
    }

    public class VectorPairPropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue A;
        public VectorPropertyValue B;

        public VectorPairPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 24) { }

        public override bool Deserialize()
        {
            A = new VectorPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;
            B = new VectorPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;

            return true;
        }
    }

    public class Vector4PropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue X;
        public FloatPropertyValue Y;
        public FloatPropertyValue Z;
        public FloatPropertyValue W;

        public Vector4PropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC);
            if (!Y.Deserialize())
                return false;
            Z = new FloatPropertyValue(Data, PCC);
            if (!Z.Deserialize())
                return false;
            W = new FloatPropertyValue(Data, PCC);
            if (!Z.Deserialize())
                return false;

            return true;
        }
    }

    public class Vector2DPropertyValue : DefaultPropertyValue
    {
        public FloatPropertyValue X;
        public FloatPropertyValue Y;

        public Vector2DPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 8) { }

        public override bool Deserialize()
        {
            X = new FloatPropertyValue(Data, PCC);
            if (!X.Deserialize())
                return false;
            Y = new FloatPropertyValue(Data, PCC);
            if (!Y.Deserialize())
                return false;

            return true;
        }
    }

    public class RotatorPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue Pitch;
        public IntPropertyValue Yaw;
        public IntPropertyValue Roll;

        public RotatorPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 12) { }

        public override bool Deserialize()
        {
            Pitch = new IntPropertyValue(Data, PCC);
            if (!Pitch.Deserialize())
                return false;
            Yaw = new IntPropertyValue(Data, PCC);
            if (!Yaw.Deserialize())
                return false;
            Roll = new IntPropertyValue(Data, PCC);
            if (!Roll.Deserialize())
                return false;

            return true;
        }
    }

    public class GUIDPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue A;
        public IntPropertyValue B;
        public IntPropertyValue C;
        public IntPropertyValue D;

        public GUIDPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            A = new IntPropertyValue(Data, PCC);
            if (!A.Deserialize())
                return false;
            B = new IntPropertyValue(Data, PCC);
            if (!B.Deserialize())
                return false;
            C = new IntPropertyValue(Data, PCC);
            if (!C.Deserialize())
                return false;
            D = new IntPropertyValue(Data, PCC);
            if (!D.Deserialize())
                return false;

            return true;
        }
    }

    public class SpherePropertyValue : DefaultPropertyValue
    {
        public Vector4PropertyValue Vector4;

        public SpherePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            Vector4 = new Vector4PropertyValue(Data, PCC);
            if (!Vector4.Deserialize())
                return false;

            return true;
        }
    }

    public class PlanePropertyValue : DefaultPropertyValue
    {
        public Vector4PropertyValue Vector4;

        public PlanePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            Vector4 = new Vector4PropertyValue(Data, PCC);
            if (!Vector4.Deserialize())
                return false;

            return true;
        }
    }

    public class ScalePropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue Scale;
        public FloatPropertyValue SheerRate;
        public BytePropertyValue SheerAxis;

        public ScalePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 17) { }

        public override bool Deserialize()
        {
            Scale = new VectorPropertyValue(Data, PCC);
            if (!Scale.Deserialize())
                return false;
            SheerRate = new FloatPropertyValue(Data, PCC);
            if (!SheerRate.Deserialize())
                return false;
            SheerAxis = new BytePropertyValue(Data, PCC);
            if (!SheerAxis.Deserialize())
                return false;

            return true;
        }
    }

    public class BoxPropertyValue : DefaultPropertyValue
    {
        public VectorPropertyValue Min;
        public VectorPropertyValue Max;
        public BytePropertyValue IsValid;

        public BoxPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 25) { }

        public override bool Deserialize()
        {
            Min = new VectorPropertyValue(Data, PCC);
            if (!Min.Deserialize())
                return false;
            Max = new VectorPropertyValue(Data, PCC);
            if (!Max.Deserialize())
                return false;
            IsValid = new BytePropertyValue(Data, PCC);
            if (!IsValid.Deserialize())
                return false;

            return true;
        }
    }

    public class QuatPropertyValue : DefaultPropertyValue
    {
        public Vector4PropertyValue Vector4;

        public QuatPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 16) { }

        public override bool Deserialize()
        {
            Vector4 = new Vector4PropertyValue(Data, PCC);
            if (!Vector4.Deserialize())
                return false;

            return true;
        }
    }

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

    public class IntPointPropertyValue : DefaultPropertyValue
    {
        public IntPropertyValue X;
        public IntPropertyValue Y;

        public IntPointPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 8) { }

        public override bool Deserialize()
        {
            X = new IntPropertyValue(Data, PCC);
            if (!X.Deserialize())
                return false;
            Y = new IntPropertyValue(Data, PCC);
            if (!Y.Deserialize())
                return false;

            return true;
        }
    }
}
