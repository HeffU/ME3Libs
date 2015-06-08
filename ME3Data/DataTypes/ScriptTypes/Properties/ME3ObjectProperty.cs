using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3ObjectProperty : ME3Property
    {
        /// <summary>
        /// The object this property refers to.
        /// Only valid if IsNativeImport is false.
        /// </summary>
        public ME3Object Object;

        /// <summary>
        /// True if this object is an import in Core.pcc, thus being imported from the VM itself.
        /// </summary>
        public bool IsNativeImport;

        private Int32 _ObjectIndex;

        public ME3ObjectProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _ObjectIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_ObjectIndex);
            if (entry != null)
            {
                Object = PCC.GetObjectEntry(_ObjectIndex).Object as ME3Object;
            }

            IsNativeImport = Object == null ? false : true;

            return result;
        }
    }
}
