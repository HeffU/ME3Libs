﻿using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3FixedArrayProperty : ME3Property
    {
        public ME3Property InnerProperty;
        public Int32 Count;

        private Int32 _InnerIndex;

        public ME3FixedArrayProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _InnerIndex = Data.ReadIndex();
            Count = Data.ReadInt32();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            InnerProperty = PCC.GetExportObject(_InnerIndex) as ME3Property;

            return result;
        }
    }
}
