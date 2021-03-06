﻿using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3InterfaceProperty : ME3Property
    {
        public ME3Object Interface;
        public String InterfaceName = "Unknown";

        private Int32 _InterfaceIndex;

        public ME3InterfaceProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _InterfaceIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_InterfaceIndex);
            if (entry != null)
            {
                InterfaceName = entry.ObjectName;
                Interface = PCC.GetObjectEntry(_InterfaceIndex).Object as ME3Object;
            }
            if (Interface == null) // TODO
                return true; // this should be false, but until native objects are handled by the library this will have to do.

            return result;
        }
    }
}
