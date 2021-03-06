﻿using ME3Data.DataTypes;
using ME3Data.Utility;
using ME3Script.Analysis.Visitors;
using ME3Script.Language.ByteCode;
using ME3Script.Language.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Decompiling
{
    public partial class ME3ByteCodeDecompiler
    {
        public Statement DecompileStatement()
        {
            StartPositions.Push((UInt16)Position);
            var token = CurrentByte;

            switch (token)
            {
                // return [expression];
                case (byte)StandardByteCodes.Return:
                    return DecompileReturn();

                // switch (expression)
                case (byte)StandardByteCodes.Switch:
                    return DecompileSwitch();

                // case expression :
                case (byte)StandardByteCodes.Case:
                    return DecompileCase();

                // if (expression) // while / for / do until
                case (byte)StandardByteCodes.JumpIfNot:
                    return DecompileConditionalJump();

                // continue
                case (byte)StandardByteCodes.Jump: // TODO: UDK seems to compile this from break when inside ForEach, handle?
                    return DecompileJump();

                // continue (iterator)
                case (byte)StandardByteCodes.IteratorNext:
                    PopByte(); // pop iteratornext token
                    return DecompileJump();

                // break;
                case (byte)StandardByteCodes.IteratorPop:
                    return DecompileIteratorPop();

                // stop;
                case (byte)StandardByteCodes.Stop:
                    PopByte();
                    var stopStatement = new StopStatement(null, null);
                    StatementLocations.Add(StartPositions.Pop(), stopStatement);
                    return stopStatement;

                // Goto label
                case (byte)StandardByteCodes.GotoLabel:
                    PopByte();
                    var labelExpr = DecompileExpression();
                    var func = new SymbolReference(null, null, null, "goto");
                    var call = new FunctionCall(func, new List<Expression>() { labelExpr }, null, null);
                    var gotoLabel = new ExpressionOnlyStatement(null, null, call);
                    StatementLocations.Add(StartPositions.Pop(), gotoLabel);
                    return gotoLabel;

                // assignable expression = expression;
                case (byte)StandardByteCodes.Let:
                case (byte)StandardByteCodes.LetBool:
                case (byte)StandardByteCodes.LetDelegate:
                    return DecompileAssign();

                // [skip x bytes]
                case (byte)StandardByteCodes.Skip: // TODO: this should never occur as statement, possibly remove?
                    PopByte();
                    ReadUInt16();
                    StartPositions.Pop();
                    return DecompileStatement();

                case (byte)StandardByteCodes.Nothing:
                    PopByte();
                    StartPositions.Pop();
                    return DecompileStatement(); // TODO, should probably have a nothing expression or statement, this is ugly

                // foreach IteratorFunction(...)
                case (byte)StandardByteCodes.Iterator:
                    return DecompileForEach();

                // foreach arrayName(valuevariable[, indexvariable])
                case (byte)StandardByteCodes.DynArrayIterator:
                    return DecompileForEach(isDynArray: true);

                case (byte)StandardByteCodes.LabelTable:
                    DecompileLabelTable();
                    StartPositions.Pop();
                    return DecompileStatement();

                #region unsupported

                case (byte)StandardByteCodes.OptIfLocal: // TODO: verify, handle syntax
                    return DecompileConditionalJump(isOpt: true);

                case (byte)StandardByteCodes.OptIfInstance: // TODO: verify, handle syntax
                    return DecompileConditionalJump(isOpt: true);

                #endregion

                default:
                    var expr = DecompileExpression();
                    if (expr != null)
                    {
                        var statement = new ExpressionOnlyStatement(null, null, expr);
                        StatementLocations.Add(StartPositions.Pop(), statement);
                        return statement;
                    }

                    // ERROR!
                    break;
            }

            return null;
        }

        private void DecompileLabelTable()
        {
            PopByte();
            var name = ReadNameRef();
            var ofs = ReadUInt32();
            while (ofs != 0x0000FFFF) // ends with non-ref + max-offset
            {
                var entry = new LabelTableEntry();
                entry.NameRef = name;
                entry.Name = PCC.GetName(name);
                entry.Offset = ofs;
                LabelTable.Add(entry);

                name = ReadNameRef();
                ofs = ReadUInt32();
            }
        }

        #region Decompilers
        public ReturnStatement DecompileReturn()
        {
            PopByte();

            Expression expr = null;
            if (CurrentIs(StandardByteCodes.ReturnNullValue))
            {
                // TODO: research this a bit, seems to be the zero-equivalent value for the return type.
                PopByte();
                var retVal = ReadObject();
                expr = new SymbolReference(null, null, null, "null"); // TODO: faulty obv, kind of illustrates the thing though.
            }
            else if(CurrentIs(StandardByteCodes.Nothing))
            {
                PopByte();
            }
            else
            {
                expr = DecompileExpression();
                if (expr == null && PopByte() != (byte)StandardByteCodes.Nothing)
                    return null; //ERROR ?
            }

            var statement = new ReturnStatement(null, null, expr);
            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public Statement DecompileConditionalJump(bool isOpt = false) // TODO: guess for loop, probably requires a large restructure
        {
            PopByte();
            var scopeStartOffset = StartPositions.Pop();
            Statement statement = null;
            bool hasElse = false;
            var scopeStatements = new List<Statement>();

            UInt16 scopeEndJmpOffset = 0;
            UInt16 afterScopeOffset = 0;
            Expression conditional = null;

            if (isOpt)
            {
                var obj = ReadObject();
                var optCheck = Convert.ToBoolean(ReadByte());
                afterScopeOffset = ReadUInt16();

                String special = (optCheck ? "" : "!") + obj.ObjectName;
                conditional = new SymbolReference(null, null, null, special);
            }
            else
            {
                afterScopeOffset = ReadUInt16();
                conditional = DecompileExpression();
            }

            if (conditional == null)
                return null;

            if (afterScopeOffset < scopeStartOffset) // end of do_until detection
            {
                scopeStartOffset = afterScopeOffset;
                var outerScope = Scopes[CurrentScope.Peek()];
                var startStatement = StatementLocations[afterScopeOffset];
                StatementLocations.Remove(afterScopeOffset);
                var index = outerScope.IndexOf(startStatement);
                scopeStatements = new List<Statement>(outerScope.Skip(index));
                outerScope.RemoveRange(index, outerScope.Count - index);
                statement = new DoUntilLoop(conditional, new CodeBody(scopeStatements, null, null), null, null);
            }

            Scopes.Add(scopeStatements);
            CurrentScope.Push(Scopes.Count - 1);
            while (Position < afterScopeOffset)
            {
                if (CurrentIs(StandardByteCodes.Jump))
                {
                    var contPos = (UInt16)Position;
                    PopByte();
                    scopeEndJmpOffset = ReadUInt16();
                    if (scopeEndJmpOffset == scopeStartOffset)
                    {
                        statement = new WhileLoop(conditional, new CodeBody(scopeStatements, null, null), null, null);
                        break;
                    }
                    else if (Position < afterScopeOffset) // if we are not at the end of the scope, this is a continue statement in a loop rather than an else statement
                    {
                        var cont = new ContinueStatement(null, null); 
                        StatementLocations.Add(contPos, cont);
                        scopeStatements.Add(cont);
                    }
                    else if (ForEachScopes.Count != 0 && scopeEndJmpOffset == ForEachScopes.Peek())
                    {
                        var breakStatement = new BreakStatement(null, null);
                        StatementLocations.Add(contPos, breakStatement);
                        scopeStatements.Add(breakStatement);
                    }
                    else
                    {
                        hasElse = true;
                    }

                    continue;
                }

                var current = DecompileStatement();
                if (current == null)
                    return null; // ERROR ?

                scopeStatements.Add(current);
            }
            CurrentScope.Pop();


            List<Statement> elseStatements = new List<Statement>();
            if (hasElse)
            {
                var endElseOffset = scopeEndJmpOffset;
                Scopes.Add(elseStatements);
                CurrentScope.Push(Scopes.Count - 1);
                while (Position < endElseOffset)
                {
                    var current = DecompileStatement();
                    if (current == null)
                        return null; // ERROR ?

                    elseStatements.Add(current);
                }
                CurrentScope.Pop();
            }

            statement = statement ?? new IfStatement(conditional, new CodeBody(scopeStatements, null, null),
                        null, null, elseStatements.Count != 0 ? new CodeBody(elseStatements, null, null) : null);
            StatementLocations.Add(scopeStartOffset, statement);
            return statement;
        }

        public Statement DecompileForEach(bool isDynArray = false) // TODO: guess for loop, probably requires a large restructure
        {
            PopByte();
            var scopeStatements = new List<Statement>();

            var iteratorFunc = DecompileExpression();
            if (iteratorFunc == null)
                return null;

            Expression dynArrVar = null;
            Expression dynArrIndex = null;
            bool unknByte = false;
            if (isDynArray)
            {
                dynArrVar = DecompileExpression();
                unknByte = Convert.ToBoolean(ReadByte());
                dynArrIndex = DecompileExpression();
            }

            var scopeEnd = ReadUInt16(); // MemOff
            ForEachScopes.Push(scopeEnd);

            Scopes.Add(scopeStatements);
            CurrentScope.Push(Scopes.Count - 1);
            while (Position < Size)
            {
                if (CurrentIs(StandardByteCodes.IteratorNext) && PeekByte == (byte)StandardByteCodes.IteratorPop)
                {
                    PopByte(); // IteratorNext
                    PopByte(); // IteratorPop
                    break;
                }

                var current = DecompileStatement();
                if (current == null)
                    return null; // ERROR ?

                scopeStatements.Add(current);
            }
            CurrentScope.Pop();
            ForEachScopes.Pop();

            if (isDynArray)
            {
                var builder = new CodeBuilderVisitor(); // what a wonderful hack, TODO.
                iteratorFunc.AcceptVisitor(builder);
                var arrayName = new SymbolReference(null, null, null, builder.GetCodeString());
                var parameters = new List<Expression>() { dynArrVar, dynArrIndex };
                iteratorFunc = new FunctionCall(arrayName, parameters, null, null);
            }

            var statement = new ForEachLoop(iteratorFunc, new CodeBody(scopeStatements, null, null), null, null);
            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public SwitchStatement DecompileSwitch()
        {
            PopByte();
            var objIndex = ReadObject();
            var unknByte = ReadByte();
            var expr = DecompileExpression();
            var scopeStatements = new List<Statement>();
            UInt16 endOffset = 0xFFFF; // set it at max to begin with, so we can begin looping

            Scopes.Add(scopeStatements);
            CurrentScope.Push(Scopes.Count - 1);
            while (Position < endOffset && Position < Size)
            {
                if (CurrentIs(StandardByteCodes.Jump)) // break detected, save the endOffset
                {                                    // executes for all occurences, to handle them all.
                    StartPositions.Push((UInt16)Position);
                    PopByte();
                    endOffset = ReadUInt16();
                    var breakStatement = new BreakStatement(null, null);
                    StatementLocations.Add(StartPositions.Pop(), breakStatement);
                    scopeStatements.Add(breakStatement);
                    continue;
                }

                var current = DecompileStatement();
                if (current == null)
                    return null; // ERROR ?

                scopeStatements.Add(current);
                if (current is DefaultStatement && endOffset == 0xFFFF)
                    break; // If no break was detected, we end the switch rather than include the rest of ALL code in the default.
            }
            CurrentScope.Pop();

            var statement = new SwitchStatement(expr, new CodeBody(scopeStatements, null, null), null, null);
            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public Statement DecompileCase()
        {
            PopByte();
            var offs = ReadUInt16(); // MemOff
            Statement statement = null;

            if (offs == (UInt16)0xFFFF)
            {
                statement = new DefaultStatement(null, null);
            }
            else 
            {
                var expr = DecompileExpression();
                if (expr == null)
                    return null; //ERROR ?

                statement = new CaseStatement(expr, null, null);
            }

            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public AssignStatement DecompileAssign()
        {
            PopByte();

            var left = DecompileExpression();
            if (left == null || !typeof(SymbolReference).IsAssignableFrom(left.GetType()))
                return null; //ERROR ?

            var right = DecompileExpression();
            if (right == null)
                return null; //ERROR ?

            var statement = new AssignStatement(left as SymbolReference, right, null, null);
            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public Statement DecompileJump()
        {
            PopByte();
            var jumpOffs = ReadUInt16(); // discard jump destination
            Statement statement = null;

            if (ForEachScopes.Count != 0 && jumpOffs == ForEachScopes.Peek()) // A jump to the IteratorPop of a ForEach means break afaik.
                statement = new BreakStatement(null, null);
            else
                statement = new ContinueStatement(null, null);

            StatementLocations.Add(StartPositions.Pop(), statement);
            return statement;
        }

        public Statement DecompileIteratorPop()
        {
            PopByte();
            if (CurrentIs(StandardByteCodes.Return)) // Any return inside a ForEach seems to call IteratorPop before the return, maybe breaks the loop?
            {
                return DecompileReturn();
            }
            else
            {
                var statement = new BreakStatement(null, null);
                StatementLocations.Add(StartPositions.Pop(), statement);
                return statement;
            }
        }

        #endregion

        #region Unsupported Decompilers

        #endregion
    }
}
