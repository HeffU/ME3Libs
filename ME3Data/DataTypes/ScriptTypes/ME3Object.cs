using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Object
    {
        // Network
        public int NetIndex;

        // Default properties for anything except 'Class' types
        public List<ME3DefaultProperty> DefaultProperties;

        public ME3Object()
        {

        }
    }
}
