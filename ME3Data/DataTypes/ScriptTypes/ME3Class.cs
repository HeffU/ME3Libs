﻿using ME3Data.FileFormats.PCC;
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
        public ClassFlags ClassFlags;

        public ME3Class OuterClass;

        public Int32 ConfigNameIndex;

        public Int32 InterfaceCount;
        public List<ME3Class> ImplementedInterfaces;

        public Int32 ComponentCount;
        public List<ME3Object> Components;

        public List<ME3State> States;

        public Int32 DLLBindIndex;

        public Int32 DefaultPropertyIndex;

        public Int32 FunctionRefCount;
        public List<ME3Function> FunctionRefs;
        //public List<String> FunctionRefs;

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

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            ClassFlags = (ClassFlags)Data.ReadInt32();

            _OuterClassIndex = Data.ReadInt32();
            ConfigNameIndex = Data.ReadInt32();

            InterfaceCount = Data.ReadInt32();
            for (int i = 0; i < InterfaceCount; i++)
            {
                var interfaceRef = new InterfaceMapEntry();
                interfaceRef.ClassIndex = Data.ReadInt32();
                interfaceRef.PropertyPointer = Data.ReadInt32();
                _ImplInterfaces.Add(interfaceRef);
            }

            ComponentCount = Data.ReadInt32();
            for (int i = 0; i < ComponentCount; i++)
            {
                var componentRef = new ComponentMapEntry();
                componentRef.NameRef = Data.ReadNameRef();
                componentRef.ComponentObjectIndex = Data.ReadInt32();
                _Components.Add(componentRef);
            }

            DLLBindIndex = Data.ReadInt32(); 

            _unkn1 = Data.ReadInt32();
            _unkn2 = Data.ReadInt32();
            _unkn3 = Data.ReadInt32();
            DefaultPropertyIndex = Data.ReadInt32();

            FunctionRefCount = Data.ReadInt32();
            for (int i = 0; i < FunctionRefCount; i++)
            {
                _FunctionRefs.Add(Data.ReadInt32());
            }

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            OuterClass = PCC.GetExportObject(_OuterClassIndex) as ME3Class;

            ImplementedInterfaces = new List<ME3Class>();
            Components = new List<ME3Object>();
            FunctionRefs = new List<ME3Function>();

            foreach (var interfaceRef in _ImplInterfaces) // TODO: overhaul to objectableentry, or provide native object support.
            {
                var obj = PCC.GetExportObject(interfaceRef.ClassIndex);
                if (obj != null)
                    ImplementedInterfaces.Add(obj as ME3Class);
            }

            foreach (var component in _Components) // TODO: overhaul to objectableentry, or provide native object support.
            {
                var obj = PCC.GetExportObject(component.ComponentObjectIndex);
                if (obj != null)
                    Components.Add(obj);
            }

            foreach (var funcRef in _FunctionRefs) // TODO: overhaul
            {
                var entry = PCC.GetObjectEntry(funcRef);
                if (entry != null) // TODO: this shoud probably never happen.
                {
                    ME3Function func = entry.Object as ME3Function;
                    FunctionRefs.Add(func);
                }
            }

            States = new List<ME3State>();
            foreach (var state in Members.Where(s => typeof(ME3State) == s.GetType()))
                States.Add(state as ME3State);

            return result;
        }
    }
}
