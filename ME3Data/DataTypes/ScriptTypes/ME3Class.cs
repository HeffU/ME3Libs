using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Class : ME3State
    {
        public Int32 ClassFlags;

        public ME3Class OuterClass;

        public Int32 ConfigNameIndex;

        public Int32 InterfaceCount;
        public List<ME3Class> ImplementedInterfaces;

        public Int32 ComponentCount;
        public List<ME3Object> Components;

        public Int32 DLLBindIndex;

        public Int32 DefaultPropertyIndex;

        public Int32 FunctionRefCount;
        public List<ME3Function> FunctionRefs;

        private Int32 _OuterClassIndex;

        private Int32 _unkn1;
        private Int32 _unkn2;
        private Int32 _unkn3;

        private List<InterfaceMapEntry> _ImplInterfaces;

        private List<ComponentMapEntry> _Components;

        private List<Int32> _FunctionRefs;

        public ME3Class(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {
            _ImplInterfaces = new List<InterfaceMapEntry>();
            _Components = new List<ComponentMapEntry>();
            _FunctionRefs = new List<Int32>();
        }

        public bool Deserialize()
        {
            base.Deserialize();

            ClassFlags = Data.ReadInt32();

            _OuterClassIndex = Data.ReadInt32();
            ConfigNameIndex = Data.ReadInt32();

            InterfaceCount = Data.ReadInt32();
            for (int i = 0; i < InterfaceCount; i++)
            {
                var interfaceRef = new InterfaceMapEntry();
                interfaceRef.ObjectIndex = Data.ReadInt32();
                interfaceRef.TypeIndex = Data.ReadInt32();
                _ImplInterfaces.Add(interfaceRef);
            }

            ComponentCount = Data.ReadInt32();
            for (int i = 0; i < ComponentCount; i++)
            {
                var componentRef = new ComponentMapEntry();
                componentRef.NameRef.Index = Data.ReadInt32();
                componentRef.NameRef.ModNumber = Data.ReadInt32();
                componentRef.ObjectIndex = Data.ReadInt32();
                _Components.Add(componentRef);
            }

            DLLBindIndex = Data.ReadInt32();
            DefaultPropertyIndex = Data.ReadInt32();

            _unkn1 = Data.ReadInt32();
            _unkn2 = Data.ReadInt32();
            _unkn3 = Data.ReadInt32();

            FunctionRefCount = Data.ReadInt32();
            for (int i = 0; i < FunctionRefCount; i++)
            {
                _FunctionRefs.Add(Data.ReadInt32());
            }

            return true;
        }
    }
}
