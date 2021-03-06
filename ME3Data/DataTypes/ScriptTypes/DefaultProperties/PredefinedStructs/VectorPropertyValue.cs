﻿using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
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
}
