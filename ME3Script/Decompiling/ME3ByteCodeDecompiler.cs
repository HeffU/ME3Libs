﻿using ME3Data.DataTypes.ScriptTypes;
using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using ME3Script.Language.ByteCode;
using ME3Script.Language.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Decompiling
{
    //TODO: most likely cleaner to convert to stack-based solution like the tokenstream, investigate.
    public partial class ME3ByteCodeDecompiler : ObjectReader 
    {
        private ME3Struct DataContainer;
        private PCCFile PCC { get { return DataContainer.ExportEntry.CurrentPCC; } }

        private Byte CurrentByte { get { return _data[Position]; } } // TODO: meaningful error handling here..
        private Byte PopByte() { return ReadByte(); }
        private Byte PeekByte { get { return Position < Size ? _data[Position + 1] : (byte)0; } }
        private Byte PrevByte { get { return Position > 0 ? _data[Position - 1] : (byte)0; } }

        private Dictionary<UInt16, Statement> StatementLocations;
        private Stack<UInt16> StartPositions;
        private Stack<List<Statement>> Scopes;

        private bool CurrentIs(StandardByteCodes val)
        {
            return CurrentByte == (byte)val;
        }

        public ME3ByteCodeDecompiler(ME3Struct dataContainer)
            :base(dataContainer.ByteScript)
        {
            DataContainer = dataContainer;
        }

        public CodeBody Decompile()
        {
            Position = DataContainer.ByteScriptSize - DataContainer.DataScriptSize;
            var statements = new List<Statement>();
            StatementLocations = new Dictionary<UInt16, Statement>();
            StartPositions = new Stack<UInt16>();

            Scopes.Push(statements);
            while (Position < Size && !CurrentIs(StandardByteCodes.EndOfScript))
            {
                var current = DecompileStatement();
                if (current == null)
                    return null; // ERROR!

                statements.Add(current);
            }
            Scopes.Pop();

            return null;
        }
    }
}