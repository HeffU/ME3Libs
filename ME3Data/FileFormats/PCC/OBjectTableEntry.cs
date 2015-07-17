using ME3Data.DataTypes;
using ME3Data.DataTypes.ScriptTypes;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.FileFormats.PCC
{
    public class ObjectTableEntry
    {
        /// <summary>
        /// Reference to the PCC file that has this specific table entry.
        /// </summary>
        public PCCFile CurrentPCC;

        /// <summary>
        /// The name of the object pointed to by this entry.
        /// </summary>
        public String ObjectName;

        /// <summary>
        /// The name of this object's class.
        /// </summary>
        public String ClassName;

        /// <summary>
        /// The name of this object's outer object.
        /// </summary>
        public String OuterName;

        /// <summary>
        /// The object of this entry. 
        /// Only valid if object is an export or the import's PCC file has been loaded beforehand.
        /// </summary>
        public ME3Object Object;

        /// <summary>
        /// The type of this object.
        /// Only valid if object is an export or the import's PCC file has been loaded beforehand.
        /// </summary>
        public ME3Object ObjectType;

        /// <summary>
        /// The object's outer object.
        /// Only valid if object is an export or the import's PCC file has been loaded beforehand.
        /// </summary>
        public ME3Object OuterObject;


        protected ObjectReader Data;
        protected Int32 _OuterIndex;

        public ObjectTableEntry(PCCFile current, ObjectReader data)
        {
            CurrentPCC = current;
            Data = data;
        }

        public String GetOuterTreeString()
        {
            var outer = CurrentPCC.GetObjectEntry(_OuterIndex);
            if (outer == null)
                return String.Empty;
            String str = outer.ObjectName;
            while (outer._OuterIndex != 0)
            {
                outer = CurrentPCC.GetObjectEntry(outer._OuterIndex);
                str = outer.ObjectName + "." + str;
            }
            return str;
        }

        public ObjectTableEntry GetOuterOfType(String type)
        {
            var outer = CurrentPCC.GetObjectEntry(_OuterIndex);
            if (outer == null)
                return null;
            while (type.ToLower() != outer.ClassName.ToLower())
            {
                if (outer._OuterIndex == 0)
                {
                    return null;
                }
                outer = CurrentPCC.GetObjectEntry(outer._OuterIndex);
            }
            return outer;
        }
    }
}
