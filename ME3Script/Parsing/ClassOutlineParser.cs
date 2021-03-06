﻿using ME3Script.Analysis.Symbols;
using ME3Script.Compiling.Errors;
using ME3Script.Language;
using ME3Script.Language.Tree;
using ME3Script.Lexing;
using ME3Script.Lexing.Tokenizing;
using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ME3Script.Parsing
{
    public class ClassOutlineParser : StringParserBase
    {
        public ClassOutlineParser(TokenStream<String> tokens, MessageLog log = null)
        {
            Log = log ?? new MessageLog();
            Tokens = tokens;
        }

        public ASTNode ParseDocument()
        {
            return TryParseClass();
        }

        #region Parsers
        #region Statements

        public Class TryParseClass()
        {
            Func<ASTNode> classParser = () =>
                {
                    if (Tokens.ConsumeToken(TokenType.Class) == null)
                        return Error("Expected class declaration!");

                    var name = Tokens.ConsumeToken(TokenType.Word);
                    if (name == null)
                        return Error("Expected class name!");

                    var parentClass = TryParseParent();
                    if (parentClass == null)
                    {
                        Log.LogMessage("No parent class specified for " + name.Value + ", interiting from Object");
                        parentClass = new VariableType("Object", null, null);
                    }

                    var outerClass = TryParseOuter();

                    var specs = ParseSpecifiers(GlobalLists.ClassSpecifiers);

                    if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                        return Error("Expected semi-colon!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var variables = new List<VariableDeclaration>();
                    var types = new List<VariableType>();
                    while (CurrentTokenType == TokenType.InstanceVariable
                        || CurrentTokenType == TokenType.Struct
                        || CurrentTokenType == TokenType.Enumeration)
                    {
                        if (CurrentTokenType == TokenType.InstanceVariable)
                        {
                            var variable = TryParseVarDecl();
                            if (variable == null)
                                return Error("Malformed instance variable!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                            variables.Add(variable);
                        }
                        else
                        {
                            var type = TryParseEnum() ?? TryParseStruct() ?? new VariableType("INVALID", null, null);
                            if (type.Name == "INVALID")
                                return Error("Malformed type declaration!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                            types.Add(type);

                            if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                                return Error("Expected semi-colon!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                        }
                    }

                    List<Function> funcs = new List<Function>();
                    List<State> states = new List<State>();
                    List<OperatorDeclaration> ops = new List<OperatorDeclaration>();
                    ASTNode declaration;
                    do
                    {
                        declaration = (ASTNode)TryParseFunction() ?? 
                                        (ASTNode)TryParseOperatorDecl() ?? 
                                        (ASTNode)TryParseState() ?? 
                                        (ASTNode)null;
                        if (declaration == null && !Tokens.AtEnd())
                            return Error("Expected function/state/operator declaration!", 
                                CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                        if (declaration.Type == ASTNodeType.Function)
                            funcs.Add((Function)declaration);
                        else if (declaration.Type == ASTNodeType.State)
                            states.Add((State)declaration);
                        else
                            ops.Add((OperatorDeclaration)declaration);
                    } while (!Tokens.AtEnd());

                    // TODO: should AST-nodes accept null values? should they make sure they dont present any?
                    return new Class(name.Value, specs, variables, types, funcs, states, parentClass, outerClass, ops, name.StartPosition, name.EndPosition);
                };
            return (Class)Tokens.TryGetTree(classParser);
        }

        public VariableDeclaration TryParseVarDecl()
        {
            Func<ASTNode> declarationParser = () =>
                {
                    if (Tokens.ConsumeToken(TokenType.InstanceVariable) == null)
                        return null;

                    var specs = ParseSpecifiers(GlobalLists.VariableSpecifiers);

                    var type = TryParseEnum() ?? TryParseStruct() ?? TryParseType();
                    if (type == null)
                        return Error("Expected variable type or struct/enum type declaration!", 
                            CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var vars = ParseVariableNames();
                    if (vars == null)
                        return Error("Malformed variable names!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                        return Error("Expected semi-colon!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    return new VariableDeclaration(type, specs, vars, vars.First().StartPos, vars.Last().EndPos);
                };
            return (VariableDeclaration)Tokens.TryGetTree(declarationParser);
        }

        public Struct TryParseStruct()
        {
            Func<ASTNode> structParser = () =>
                {
                    if (Tokens.ConsumeToken(TokenType.Struct) == null)
                        return null;

                    var specs = ParseSpecifiers(GlobalLists.StructSpecifiers);

                    var name = Tokens.ConsumeToken(TokenType.Word);
                    if (name == null)
                        return Error("Expected struct name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var parent = TryParseParent();

                    if (Tokens.ConsumeToken(TokenType.LeftBracket) == null)
                        return Error("Expected '{'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var vars = new List<VariableDeclaration>();
                    do
                    {
                        var variable = TryParseVarDecl();
                        if (variable == null)
                            return Error("Malformed struct content!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                        vars.Add(variable);
                    } while (CurrentTokenType != TokenType.RightBracket && !Tokens.AtEnd());

                    if (Tokens.ConsumeToken(TokenType.RightBracket) == null)
                        return Error("Expected '}'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    return new Struct(name.Value, specs, vars, name.StartPosition, name.EndPosition, parent);
                };
            return (Struct)Tokens.TryGetTree(structParser);
        }

        public Enumeration TryParseEnum()
        {
            Func<ASTNode> enumParser = () =>
            {
                if (Tokens.ConsumeToken(TokenType.Enumeration) == null)
                    return null;

                var name = Tokens.ConsumeToken(TokenType.Word);
                if (name == null)
                    return Error("Expected enumeration name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                if (Tokens.ConsumeToken(TokenType.LeftBracket) == null)
                    return Error("Expected '{'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var identifiers = new List<VariableIdentifier>();
                do
                {
                    var ident = Tokens.ConsumeToken(TokenType.Word);
                    if (ident == null)
                        return Error("Expected non-empty enumeration!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    identifiers.Add(new VariableIdentifier(ident.Value, ident.StartPosition, ident.EndPosition));
                    if (Tokens.ConsumeToken(TokenType.Comma) == null && CurrentTokenType != TokenType.RightBracket)
                        return Error("Malformed enumeration content!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                } while (CurrentTokenType != TokenType.RightBracket);

                if (Tokens.ConsumeToken(TokenType.RightBracket) == null)
                    return Error("Expected '}'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                return new Enumeration(name.Value, identifiers, name.StartPosition, name.EndPosition);
            };
            return (Enumeration)Tokens.TryGetTree(enumParser);
        }

        public Function TryParseFunction()
        {
            Func<ASTNode> stubParser = () =>
                {
                    var specs = ParseSpecifiers(GlobalLists.FunctionSpecifiers);

                    if (Tokens.ConsumeToken(TokenType.Function) == null)
                        return null;

                    Token<String> returnType = null, name = null;

                    var firstString = Tokens.ConsumeToken(TokenType.Word);
                    if (firstString == null)
                        return Error("Expected function name or return type!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var secondString = Tokens.ConsumeToken(TokenType.Word);
                    if (secondString == null)
                        name = firstString;
                    else
                    {
                        returnType = firstString;
                        name = secondString;
                    }

                    VariableType retVarType = returnType != null ? 
                        new VariableType(returnType.Value, returnType.StartPosition, returnType.EndPosition) : null;

                    if (Tokens.ConsumeToken(TokenType.LeftParenth) == null)
                        return Error("Expected '('!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    var parameters = new List<FunctionParameter>();
                    while (CurrentTokenType != TokenType.RightParenth)
                    {
                        var param = TryParseParameter();
                        if (param == null)
                            return Error("Malformed parameter!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                        parameters.Add(param);
                        if (Tokens.ConsumeToken(TokenType.Comma) == null && CurrentTokenType != TokenType.RightParenth)
                            return Error("Unexpected parameter content!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                    }

                    if (Tokens.ConsumeToken(TokenType.RightParenth) == null)
                        return Error("Expected ')'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    CodeBody body = new CodeBody(null, CurrentPosition, CurrentPosition);
                    SourcePosition bodyStart = null, bodyEnd = null;
                    if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                    {
                        if (!ParseScopeSpan(TokenType.LeftBracket, TokenType.RightBracket, out bodyStart, out bodyEnd))
                            return Error("Malformed function body!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                        body = new CodeBody(null, bodyStart, bodyEnd);
                    }

                    return new Function(name.Value, retVarType, body, specs, parameters, name.StartPosition, name.EndPosition);
                };
            return (Function)Tokens.TryGetTree(stubParser);
        }

        public State TryParseState()
        {
            Func<ASTNode> stateSkeletonParser = () =>
            {
                var specs = ParseSpecifiers(GlobalLists.StateSpecifiers);

                if (Tokens.ConsumeToken(TokenType.State) == null)
                    return null;

                var name = Tokens.ConsumeToken(TokenType.Word);
                if (name == null)
                    return Error("Expected state name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var parent = TryParseParent();

                if (Tokens.ConsumeToken(TokenType.LeftBracket) == null)
                    return Error("Expected '{'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                List<Function> ignores = new List<Function>();
                if (Tokens.ConsumeToken(TokenType.Ignores) != null)
                {
                    do
                    {
                        VariableIdentifier variable = TryParseVariable();
                        if (variable == null)
                            return Error("Malformed ignore statement!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                        ignores.Add(new Function(variable.Name, null, null, null, null, variable.StartPos, variable.EndPos));
                    } while (Tokens.ConsumeToken(TokenType.Comma) != null);

                    if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                        return Error("Expected semi-colon!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                }

                var funcs = new List<Function>();
                Function func = TryParseFunction();
                while (func != null)
                {
                    funcs.Add(func);
                    func = TryParseFunction();
                }

                var bodyStart = Tokens.CurrentItem.StartPosition;
                while (Tokens.CurrentItem.Type != TokenType.RightBracket && !Tokens.AtEnd())
                {
                    Tokens.Advance();
                }
                var bodyEnd = Tokens.CurrentItem.StartPosition;

                if (Tokens.ConsumeToken(TokenType.RightBracket) == null)
                    return Error("Expected '}'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var body = new CodeBody(new List<Statement>(), bodyStart, bodyEnd);
                var parentState = parent != null ? new State(parent.Name, null, null, null, null, null, null, parent.StartPos, parent.EndPos) : null;
                return new State(name.Value, body, specs, parentState, funcs, ignores, null, name.StartPosition, name.EndPosition);
            };
            return (State)Tokens.TryGetTree(stateSkeletonParser);
        }

        public OperatorDeclaration TryParseOperatorDecl()
        {
            Func<ASTNode> operatorParser = () =>
            {
                var specs = ParseSpecifiers(GlobalLists.FunctionSpecifiers);

                var token = Tokens.ConsumeToken(TokenType.Operator) ??
                    Tokens.ConsumeToken(TokenType.PreOperator) ??
                    Tokens.ConsumeToken(TokenType.PostOperator) ??
                    new Token<String>(TokenType.INVALID);

                if (token.Type == TokenType.INVALID)
                    return null;

                Token<String> precedence = null;
                if (token.Type == TokenType.Operator)
                {
                    if (Tokens.ConsumeToken(TokenType.LeftParenth) == null)
                        return Error("Expected '('! (Did you forget to specify operator precedence?)", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    precedence = Tokens.ConsumeToken(TokenType.IntegerNumber);
                    if (precedence == null)
                        return Error("Expected an integer number!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    if (Tokens.ConsumeToken(TokenType.RightParenth) == null)
                        return Error("Expected ')'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                }

                Token<String> returnType = null, name = null;
                var firstString = TryParseOperatorIdentifier();
                if (firstString == null)
                    return Error("Expected operator name or return type!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var secondString = TryParseOperatorIdentifier();
                if (secondString == null)
                    name = firstString;
                else
                {
                    returnType = firstString;
                    name = secondString;
                }

                VariableType retVarType = returnType != null ?
                    new VariableType(returnType.Value, returnType.StartPosition, returnType.EndPosition) : null;

                if (Tokens.ConsumeToken(TokenType.LeftParenth) == null)
                    return Error("Expected '('!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var operands = new List<FunctionParameter>();
                while (CurrentTokenType != TokenType.RightParenth)
                {
                    var operand = TryParseParameter();
                    if (operand == null)
                        return Error("Malformed operand!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    operands.Add(operand);
                    if (Tokens.ConsumeToken(TokenType.Comma) == null && CurrentTokenType != TokenType.RightParenth)
                        return Error("Unexpected operand content!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                }

                if (token.Type == TokenType.Operator && operands.Count != 2)
                    return Error("In-fix operators requires exactly 2 parameters!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                else if (token.Type != TokenType.Operator && operands.Count != 1)
                    return Error("Post/Pre-fix operators requires exactly 1 parameter!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                if (Tokens.ConsumeToken(TokenType.RightParenth) == null)
                    return Error("Expected ')'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                CodeBody body = new CodeBody(null, CurrentPosition, CurrentPosition);
                SourcePosition bodyStart = null, bodyEnd = null;
                if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                {
                    if (!ParseScopeSpan(TokenType.LeftBracket, TokenType.RightBracket, out bodyStart, out bodyEnd))
                        return Error("Malformed operator body!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                    body = new CodeBody(null, bodyStart, bodyEnd);
                }

                // TODO: determine if operator should be a delimiter! (should only symbol-based ones be?)
                if (token.Type == TokenType.PreOperator)
                    return new PreOpDeclaration(name.Value, false, body, retVarType, operands.First(), specs, name.StartPosition, name.EndPosition);
                else if (token.Type == TokenType.PostOperator)
                    return new PostOpDeclaration(name.Value, false, body, retVarType, operands.First(), specs, name.StartPosition, name.EndPosition);
                else
                    return new InOpDeclaration(name.Value, Int32.Parse(precedence.Value), false, body, retVarType, 
                        operands.First(), operands.Last(), specs, name.StartPosition, name.EndPosition);
            };
            return (OperatorDeclaration)Tokens.TryGetTree(operatorParser);
        }

        #endregion

        #region Misc

        public FunctionParameter TryParseParameter()
        {
            Func<ASTNode> paramParser = () =>
            {
                var paramSpecs = ParseSpecifiers(GlobalLists.ParameterSpecifiers);

                var type = Tokens.ConsumeToken(TokenType.Word);
                if (type == null)
                    return Error("Expected parameter type!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                var variable = TryParseVariable();
                if (variable == null)
                    return Error("Expected parameter name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));

                return new FunctionParameter(
                    new VariableType(type.Value, type.StartPosition, type.EndPosition),
                    paramSpecs, variable, variable.StartPos, variable.EndPos);
            };
            return (FunctionParameter)Tokens.TryGetTree(paramParser);
        }

        public VariableType TryParseParent()
        {
            Func<ASTNode> parentParser = () =>
            {
                if (Tokens.ConsumeToken(TokenType.Extends) == null)
                    return null;
                var parentName = Tokens.ConsumeToken(TokenType.Word);
                if (parentName == null)
                {
                    Log.LogError("Expected parent name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                    return null;
                }
                return new VariableType(parentName.Value, parentName.StartPosition, parentName.EndPosition);
            };
            return (VariableType)Tokens.TryGetTree(parentParser);
        }

        public VariableType TryParseOuter()
        {
            Func<ASTNode> outerParser = () =>
            {
                if (Tokens.ConsumeToken(TokenType.Within) == null)
                    return null;
                var outerName = Tokens.ConsumeToken(TokenType.Word);
                if (outerName == null)
                {
                    Log.LogError("Expected outer class name!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                    return null;
                }
                return new VariableType(outerName.Value, outerName.StartPosition, outerName.EndPosition);
            };
            return (VariableType)Tokens.TryGetTree(outerParser);
        }

        public Specifier TryParseSpecifier(List<TokenType> category)
        {
            Func<ASTNode> specifierParser = () =>
                {
                    if (category.Contains(CurrentTokenType))
                    {
                        var token = Tokens.ConsumeToken(CurrentTokenType);
                        return new Specifier(token.Value, token.StartPosition, token.EndPosition);
                    }
                    return null;
                };
            return (Specifier)Tokens.TryGetTree(specifierParser);
        }

        #endregion
        #endregion
        #region Helpers

        public Token<String> TryParseOperatorIdentifier()
        {
            if (GlobalLists.ValidOperatorSymbols.Contains(CurrentTokenType)
                || CurrentTokenType == TokenType.Word)
                return Tokens.ConsumeToken(CurrentTokenType);

            return null;
        }

        public List<Specifier> ParseSpecifiers(List<TokenType> specifierCategory)
        {
            List<Specifier> specs = new List<Specifier>();
            Specifier spec = TryParseSpecifier(specifierCategory);
            while (spec != null)
            {
                specs.Add(spec);
                spec = TryParseSpecifier(specifierCategory);
            }
            return specs;
        }

        //TODO: unused?
        private List<Token<String>> ParseScopedTokens(TokenType scopeStart, TokenType scopeEnd)
        {
            var scopedTokens = new List<Token<String>>();
            if (Tokens.ConsumeToken(scopeStart) == null)
            {
                Log.LogError("Expected '" + scopeStart.ToString() + "'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                return null;
            }

            int nestedLevel = 1;
            while (nestedLevel > 0)
            {
                if (CurrentTokenType == TokenType.EOF)
                    return null; // ERROR: Scope ended prematurely, are your scopes unbalanced?
                if (CurrentTokenType == scopeStart)
                    nestedLevel++;
                else if (CurrentTokenType == scopeEnd)
                    nestedLevel--;

                scopedTokens.Add(Tokens.CurrentItem);
                Tokens.Advance();
            }
            // Remove the ending scope token:
            scopedTokens.RemoveAt(scopedTokens.Count - 1);
            return scopedTokens;
        }

        private bool ParseScopeSpan(TokenType scopeStart, TokenType scopeEnd, 
            out SourcePosition startPos, out SourcePosition endPos)
        {
            startPos = null;
            endPos = null;
            if (Tokens.ConsumeToken(scopeStart) == null)
            {
                Log.LogError("Expected '" + scopeStart.ToString() + "'!", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                return false;
            }
            startPos = Tokens.CurrentItem.StartPosition;

            int nestedLevel = 1;
            while (nestedLevel > 0)
            {
                if (CurrentTokenType == TokenType.EOF)
                {
                    Log.LogError("Scope ended prematurely, are your scopes unbalanced?", CurrentPosition, CurrentPosition.GetModifiedPosition(0, 1, 1));
                    return false;
                }
                if (CurrentTokenType == scopeStart)
                    nestedLevel++;
                else if (CurrentTokenType == scopeEnd)
                    nestedLevel--;

                // If we're at the end token, don't advance so we can check the position properly.
                if (nestedLevel > 0)
                    Tokens.Advance();
            }
            endPos = Tokens.CurrentItem.StartPosition;
            Tokens.Advance();
            return true;
        }

        #endregion
    }
}
