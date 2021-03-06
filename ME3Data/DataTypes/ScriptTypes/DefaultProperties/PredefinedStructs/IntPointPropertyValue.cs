﻿using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs
{
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
