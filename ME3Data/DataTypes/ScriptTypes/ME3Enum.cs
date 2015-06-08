using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Enum : ME3Field
    {
        public List<String> Names;

        protected List<NameReference> _NameRefs;

        public ME3Enum(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            var nameCount = Data.ReadInt32();
            _NameRefs = new List<NameReference>(nameCount);
            for (int n = 0; n < nameCount; n++)
            {
                _NameRefs.Add(Data.ReadNameRef());
                Names.Add(PCC.GetName(_NameRefs[n]));
            }

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            return result;
        }
    }
}
