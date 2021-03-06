﻿using ME3Script.Analysis.Symbols;
using ME3Script.Compiling.Errors;
using ME3Script.Language.Tree;
using ME3Script.Language.Util;
using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Analysis.Visitors
{
    public class ClassValidationVisitor : IASTVisitor
    {
        private SymbolTable Symbols;
        private MessageLog Log;
        private bool Success;

        public ClassValidationVisitor(MessageLog log, SymbolTable symbols)
        {
            Log = log;
            Symbols = symbols;
            Success = true;
        }

        private bool Error(String msg, SourcePosition start = null, SourcePosition end = null)
        {
            Log.LogError(msg, start, end);
            Success = false;
            return false;
        }

        public bool VisitNode(Class node)
        {
            // TODO: allow duplicate names as long as its in different packages!
            if (Symbols.SymbolExists(node.Name, ""))
                return Error("A class named '" + node.Name + "' already exists!", node.StartPos, node.EndPos);

            Symbols.AddSymbol(node.Name, node);

            ASTNode parent;
            if (!Symbols.TryGetSymbol(node.Parent.Name, out parent, ""))
                return Error("No parent class named '" + node.Parent.Name + "' found!", node.Parent.StartPos, node.Parent.EndPos);
            if (parent != null)
            {
                if (parent.Type != ASTNodeType.Class)
                    return Error("Parent named '" + node.Parent.Name + "' is not a class!", node.Parent.StartPos, node.Parent.EndPos);
                else if ((parent as Class).SameOrSubClass(node.Name)) // TODO: not needed due to no forward declarations?
                    return Error("Extending from '" + node.Parent.Name + "' causes circular extension!", node.Parent.StartPos, node.Parent.EndPos);
                else
                    node.Parent = parent as Class;
            }

            ASTNode outer;
            if (node.OuterClass != null)
            {
                if (!Symbols.TryGetSymbol(node.OuterClass.Name, out outer, ""))
                    return Error("No outer class named '" + node.OuterClass.Name + "' found!", node.OuterClass.StartPos, node.OuterClass.EndPos);
                else if (outer.Type != ASTNodeType.Class)
                    return Error("Outer named '" + node.OuterClass.Name + "' is not a class!", node.OuterClass.StartPos, node.OuterClass.EndPos);
                else if (node.Parent.Name == "Actor")
                    return Error("Classes extending 'Actor' can not be inner classes!", node.OuterClass.StartPos, node.OuterClass.EndPos);
                else if (!(outer as Class).SameOrSubClass((node.Parent as Class).OuterClass.Name))
                    return Error("Outer class must be a sub-class of the parents outer class!", node.OuterClass.StartPos, node.OuterClass.EndPos);
            }
            else
            {
                outer = (node.Parent as Class).OuterClass;
            }
            node.OuterClass = outer as Class;

            // TODO(?) validate class specifiers more than the initial parsing?

            Symbols.GoDirectlyToStack((node.Parent as Class).GetInheritanceString());
            Symbols.PushScope(node.Name);

            foreach (VariableType type in node.TypeDeclarations)
            {
                type.Outer = node;
                Success = Success && type.AcceptVisitor(this);
            }
            foreach (VariableDeclaration decl in node.VariableDeclarations.ToList())
            {
                decl.Outer = node;
                Success = Success && decl.AcceptVisitor(this);
            }
            foreach (OperatorDeclaration op in node.Operators)
            {
                op.Outer = node;
                Success = Success && op.AcceptVisitor(this);
            }
            foreach (Function func in node.Functions)
            {
                func.Outer = node;
                Success = Success && func.AcceptVisitor(this);
            }
            foreach (State state in node.States)
            {
                state.Outer = node;
                Success = Success && state.AcceptVisitor(this);
            }

            Symbols.PopScope();
            Symbols.RevertToObjectStack();

            node.Declaration = node;

            return Success;
        }


        public bool VisitNode(VariableDeclaration node)
        {
            ASTNode nodeType;
            if (node.VarType.Type == ASTNodeType.Struct || node.VarType.Type == ASTNodeType.Enumeration)
            {
                // Check type, if its a struct or enum, visit that first.
                node.VarType.Outer = node.Outer;
                Success = Success && node.VarType.AcceptVisitor(this);
                // Add the type to the list of types in the class.
                NodeUtils.GetContainingClass(node).TypeDeclarations.Add(node.VarType);
                nodeType = node.VarType;
            }
            else if (!Symbols.TryGetSymbol(node.VarType.Name, out nodeType, NodeUtils.GetOuterClassScope(node)))
            {
                return Error("No type named '" + node.VarType.Name + "' exists in this scope!", node.VarType.StartPos, node.VarType.EndPos);
            }
            else if (!typeof(VariableType).IsAssignableFrom(nodeType.GetType()))
            {
                return Error("Invalid variable type, must be a class/struct/enum/primitive.", node.VarType.StartPos, node.VarType.EndPos);
            }

            if (node.Outer.Type == ASTNodeType.Class)
            {
                int index = NodeUtils.GetContainingClass(node).VariableDeclarations.IndexOf(node);
                foreach (VariableIdentifier ident in node.Variables)
                {
                    if (Symbols.SymbolExistsInCurrentScope(ident.Name))
                        return Error("A member named '" + ident.Name + "' already exists in this class!", ident.StartPos, ident.EndPos);
                    Variable variable = new Variable(node.Specifiers, ident, nodeType as VariableType, ident.StartPos, ident.EndPos);
                    variable.Outer = node.Outer;
                    Symbols.AddSymbol(variable.Name, variable);
                    NodeUtils.GetContainingClass(node).VariableDeclarations.Insert(index++, variable);
                }
                NodeUtils.GetContainingClass(node).VariableDeclarations.Remove(node);
            } 
            else if (node.Outer.Type == ASTNodeType.Struct)
            {
                int index = (node.Outer as Struct).Members.IndexOf(node);
                foreach (VariableIdentifier ident in node.Variables)
                {
                    if (Symbols.SymbolExistsInCurrentScope(ident.Name))
                        return Error("A member named '" + ident.Name + "' already exists in this struct!", ident.StartPos, ident.EndPos);
                    Variable variable = new Variable(node.Specifiers, ident, nodeType as VariableType, ident.StartPos, ident.EndPos);
                    variable.Outer = node.Outer;
                    Symbols.AddSymbol(variable.Name, variable);
                    (node.Outer as Struct).Members.Insert(index++, variable);
                }
                (node.Outer as Struct).Members.Remove(node);
            }

            return Success;
        }

        public bool VisitNode(VariableType node)
        {
            // This should never be called.
            throw new NotImplementedException();
        }

        public bool VisitNode(Struct node)
        {
            if (Symbols.SymbolExistsInCurrentScope(node.Name))
                return Error("A member named '" + node.Name + "' already exists in this class!", node.StartPos, node.EndPos);

            Symbols.AddSymbol(node.Name, node);
            // TODO: add in package / global namespace.
            // If a symbol with that name exists, overwrite it with this symbol from now on.
            // damn this language...

            if (node.Parent != null)
            {
                ASTNode parent;
                if (!Symbols.TryGetSymbol(node.Parent.Name, out parent, NodeUtils.GetOuterClassScope(node)))
                    Error("No parent struct named '" + node.Parent.Name + "' found!", node.Parent.StartPos, node.Parent.EndPos);
                if (parent != null)
                {
                    if (parent.Type != ASTNodeType.Struct)
                        Error("Parent named '" + node.Parent.Name + "' is not a struct!", node.Parent.StartPos, node.Parent.EndPos);
                    else if ((parent as Struct).SameOrSubStruct(node.Name)) // TODO: not needed due to no forward declarations?
                        Error("Extending from '" + node.Parent.Name + "' causes circular extension!", node.Parent.StartPos, node.Parent.EndPos);
                    else
                        node.Parent = parent as Struct;
                }
            }

            Symbols.PushScope(node.Name);

            // TODO: can all types of variable declarations be supported in a struct?
            // what does the parser let through?
            var unprocessed = node.Members.ToList();
            foreach (VariableDeclaration decl in node.Members.ToList())
            {
                decl.Outer = node;
                Success = Success && decl.AcceptVisitor(this);
            }

            Symbols.PopScope();

            node.Declaration = node;

            return Success;
        }

        public bool VisitNode(Enumeration node)
        {
            if (Symbols.SymbolExistsInCurrentScope(node.Name))
                return Error("A member named '" + node.Name + "' already exists in this class!", node.StartPos, node.EndPos);

            Symbols.AddSymbol(node.Name, node);
            // TODO: add in package / global namespace.
            // If a symbol with that name exists, overwrite it with this symbol from now on.
            // damn this language...
            Symbols.PushScope(node.Name);

            foreach (VariableIdentifier enumVal in node.Values)
            {
                enumVal.Outer = node;
                if (enumVal.Type != ASTNodeType.VariableIdentifier)
                    Error("An enumeration member must be a simple(name only) variable.", enumVal.StartPos, enumVal.EndPos);
                Symbols.AddSymbol(enumVal.Name, enumVal);
            }

            Symbols.PopScope();

            // Add enum values at the class scope so they can be used without being explicitly qualified.
            foreach (VariableIdentifier enumVal in node.Values)
                Symbols.AddSymbol(enumVal.Name, enumVal);

            node.Declaration = node;

            return Success;
        }

        public bool VisitNode(Function node)
        {
            if (Symbols.SymbolExistsInCurrentScope(node.Name))
                return Error("The name '" + node.Name + "' is already in use in this class!", node.StartPos, node.EndPos);

            Symbols.AddSymbol(node.Name, node);
            ASTNode returnType = null;
            if (node.ReturnType != null)
            {
                if (!Symbols.TryGetSymbol(node.ReturnType.Name, out returnType, NodeUtils.GetOuterClassScope(node)))
                {
                    return Error("No type named '" + node.ReturnType.Name + "' exists in this scope!", node.ReturnType.StartPos, node.ReturnType.EndPos);
                }
                else if (!typeof(VariableType).IsAssignableFrom(returnType.GetType()))
                {
                    return Error("Invalid return type, must be a class/struct/enum/primitive.", node.ReturnType.StartPos, node.ReturnType.EndPos);
                }
            }

            Symbols.PushScope(node.Name);
            foreach (FunctionParameter param in node.Parameters)
            {
                param.Outer = node;
                Success = Success && param.AcceptVisitor(this);
            }
            Symbols.PopScope();

            if (Success == false)
                return Error("Error in function parameters.", node.StartPos, node.EndPos);

            ASTNode func;
            if (Symbols.TryGetSymbol(node.Name, out func, "") // override functions in parent classes only (or current class if its a state)
                && func.Type == ASTNodeType.Function)
            {   // If there is a function with this name that we should override, validate the new functions declaration
                Function original = func as Function;
                if (original.Specifiers.Contains(new Specifier("final", null, null)))
                    return Error("Function name overrides a function in a parent class, but the parent function is marked as final!", node.StartPos, node.EndPos);
                if (node.ReturnType != original.ReturnType)
                    return Error("Function name overrides a function in a parent class, but the functions do not have the same return types!", node.StartPos, node.EndPos);
                if (node.Parameters.Count != original.Parameters.Count)
                    return Error("Function name overrides a function in a parent class, but the functions do not have the same amount of parameters!", node.StartPos, node.EndPos);
                for (int n = 0; n < node.Parameters.Count; n++)
                {
                    if (node.Parameters[n].Type != original.Parameters[n].Type)
                        return Error("Function name overrides a function in a parent class, but the functions do not ahve the same parameter types!", node.StartPos, node.EndPos);
                }
            }

            return Success;
        }

        public bool VisitNode(FunctionParameter node)
        {
            ASTNode paramType;
            if (!Symbols.TryGetSymbol(node.VarType.Name, out paramType, NodeUtils.GetOuterClassScope(node)))
            {
                return Error("No type named '" + node.VarType.Name + "' exists in this scope!", node.VarType.StartPos, node.VarType.EndPos);
            }
            else if (!typeof(VariableType).IsAssignableFrom(paramType.GetType()))
            {
                return Error("Invalid parameter type, must be a class/struct/enum/primitive.", node.VarType.StartPos, node.VarType.EndPos);
            }
            node.VarType = paramType as VariableType;

            if (Symbols.SymbolExistsInCurrentScope(node.Name))
                return Error("A parameter named '" + node.Name + "' already exists in this function!", 
                    node.Variables.First().StartPos, node.Variables.First().EndPos);

            Symbols.AddSymbol(node.Variables.First().Name, node);
            return Success;
        }

        public bool VisitNode(State node)
        {
            if (Symbols.SymbolExistsInCurrentScope(node.Name))
                return Error("The name '" + node.Name + "' is already in use in this class!", node.StartPos, node.EndPos);

            ASTNode overrideState;
            bool overrides = Symbols.TryGetSymbol(node.Name, out overrideState, NodeUtils.GetOuterClassScope(node))
                && overrideState.Type == ASTNodeType.State;

            if (node.Parent != null)
            {
                if (overrides)
                    return Error("A state is not allowed to both override a parent class's state and extend another state at the same time!", node.StartPos, node.EndPos);

                ASTNode parent;
                if (!Symbols.TryGetSymbolFromCurrentScope(node.Parent.Name, out parent))
                    Error("No parent state named '" + node.Parent.Name + "' found in the current class!", node.Parent.StartPos, node.Parent.EndPos);
                if (parent != null)
                {
                    if (parent.Type != ASTNodeType.State)
                        Error("Parent named '" + node.Parent.Name + "' is not a state!", node.Parent.StartPos, node.Parent.EndPos);
                    else
                        node.Parent = parent as State;
                }
            }

            int numFuncs = node.Functions.Count;
            Symbols.PushScope(node.Name);
            foreach (Function ignore in node.Ignores)
            {
                ASTNode original;
                if (!Symbols.TryGetSymbol(ignore.Name, out original, "") || original.Type != ASTNodeType.Function)
                    return Error("No function to ignore named '" + ignore.Name + "' found!", ignore.StartPos, ignore.EndPos);
                Function header = original as Function;
                Function emptyOverride = new Function(header.Name, header.ReturnType, new CodeBody(null, null, null), header.Specifiers, header.Parameters, ignore.StartPos, ignore.EndPos);
                node.Functions.Add(emptyOverride);
                Symbols.AddSymbol(emptyOverride.Name, emptyOverride);
            }

            foreach (Function func in node.Functions.GetRange(0, numFuncs))
            {
                func.Outer = node;
                Success = Success && func.AcceptVisitor(this);
            }
            //TODO: check functions overrides:
            //if the state overrides another state, we should be in that scope as well whenh we check overrides maybe?
            //if the state has a parent state, we should be in that scope
            //this is a royal mess, check that ignores also look-up from parent/overriding states as we are not sure if symbols are in the scope

            // if the state extends a parent state, use that as outer in the symbol lookup
            // if the state overrides another state, use that as outer
            // both of the above should apply to functions as well as ignores.

            //TODO: state code/labels

            Symbols.PopScope();
            return Success;
        }

        public bool VisitNode(OperatorDeclaration node)
        {
            ASTNode returnType = null;
            if (node.ReturnType != null)
            {
                if (!Symbols.TryGetSymbol(node.ReturnType.Name, out returnType, NodeUtils.GetOuterClassScope(node)))
                {
                    return Error("No type named '" + node.ReturnType.Name + "' exists in this scope!", node.ReturnType.StartPos, node.ReturnType.EndPos);
                }
                else if (!typeof(VariableType).IsAssignableFrom(returnType.GetType()))
                {
                    return Error("Invalid return type, must be a class/struct/enum/primitive.", node.ReturnType.StartPos, node.ReturnType.EndPos);
                }
            }

            Symbols.PushScope(node.OperatorKeyword);
            if (node.Type == ASTNodeType.InfixOperator)
            {
                var op = node as InOpDeclaration;
                op.LeftOperand.Outer = node;
                Success = Success && op.LeftOperand.AcceptVisitor(this);
                op.RightOperand.Outer = node;
                Success = Success && op.RightOperand.AcceptVisitor(this);
            }
            else if (node.Type == ASTNodeType.PrefixOperator)
            {
                var op = node as PreOpDeclaration;
                op.Operand.Outer = node;
                Success = Success && op.Operand.AcceptVisitor(this);
            }
            else if (node.Type == ASTNodeType.PostfixOperator)
            {
                var op = node as PostOpDeclaration;
                op.Operand.Outer = node;
                Success = Success && op.Operand.AcceptVisitor(this);
            }
            Symbols.PopScope();

            if (Success == false)
                return Error("Error in operator parameters.", node.StartPos, node.EndPos);

            if (Symbols.OperatorSignatureExists(node))
                return Error("An operator with identical signature to '" + node.OperatorKeyword + "' already exists!", node.StartPos, node.EndPos);

            Symbols.AddOperator(node);
            return Success;
        }

        #region Unused
        public bool VisitNode(CodeBody node)
        { throw new NotImplementedException(); }
        public bool VisitNode(StateLabel node)
        { throw new NotImplementedException(); }

        public bool VisitNode(Variable node)
        { throw new NotImplementedException(); }
        public bool VisitNode(VariableIdentifier node)
        { throw new NotImplementedException(); }

        public bool VisitNode(DoUntilLoop node)
        { throw new NotImplementedException(); }
        public bool VisitNode(ForLoop node)
        { throw new NotImplementedException(); }
        public bool VisitNode(ForEachLoop node)
        { throw new NotImplementedException(); }
        public bool VisitNode(WhileLoop node)
        { throw new NotImplementedException(); }

        public bool VisitNode(SwitchStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(CaseStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(DefaultStatement node)
        { throw new NotImplementedException(); }

        public bool VisitNode(AssignStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(BreakStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(ContinueStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(IfStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(ReturnStatement node)
        { throw new NotImplementedException(); }
        public bool VisitNode(StopStatement node)
        { throw new NotImplementedException(); }

        public bool VisitNode(ExpressionOnlyStatement node)
        { throw new NotImplementedException(); }

        public bool VisitNode(InOpReference node)
        { throw new NotImplementedException(); }
        public bool VisitNode(PreOpReference node)
        { throw new NotImplementedException(); }
        public bool VisitNode(PostOpReference node)
        { throw new NotImplementedException(); }

        public bool VisitNode(FunctionCall node)
        { throw new NotImplementedException(); }

        public bool VisitNode(ArraySymbolRef node)
        { throw new NotImplementedException(); }
        public bool VisitNode(CompositeSymbolRef node)
        { throw new NotImplementedException(); }
        public bool VisitNode(SymbolReference node)
        { throw new NotImplementedException(); }

        public bool VisitNode(BooleanLiteral node)
        { throw new NotImplementedException(); }
        public bool VisitNode(FloatLiteral node)
        { throw new NotImplementedException(); }
        public bool VisitNode(IntegerLiteral node)
        { throw new NotImplementedException(); }
        public bool VisitNode(NameLiteral node)
        { throw new NotImplementedException(); }
        public bool VisitNode(StringLiteral node)
        { throw new NotImplementedException(); }

        public bool VisitNode(ConditionalExpression node)
        { throw new NotImplementedException(); }
        public bool VisitNode(CastExpression node)
        { throw new NotImplementedException(); }

        public bool VisitNode(DefaultPropertiesBlock node)
        { throw new NotImplementedException(); }
        #endregion
    }
}
