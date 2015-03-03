﻿using ME3Script.Language;
using ME3Script.Language.Nodes;
using ME3Script.Language.Types;
using ME3Script.Lexing;
using ME3Script.Lexing.Tokenizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Parsing
{
    public class StringParser
    {
        private TokenStream<String> Tokens;
        private TypeManager Types;

        private TokenType CurrentTokenType { get { return Tokens.CurrentItem.Type; } }

        private List<TokenType> VariableSpecifiers = new List<TokenType>
        {
            TokenType.ConfigSpecifier,
            TokenType.GlobalConfigSpecifier,
            TokenType.LocalizedSpecifier,
            TokenType.ConstSpecifier,
            TokenType.PrivateSpecifier,
            TokenType.ProtectedSpecifier,
            TokenType.PrivateWriteSpecifier,
            TokenType.ProtectedWriteSpecifier,
            TokenType.RepNotifySpecifier,
            TokenType.DeprecatedSpecifier,
            TokenType.InstancedSpecifier,
            TokenType.DatabindingSpecifier,
            TokenType.EditorOnlySpecifier,
            TokenType.NotForConsoleSpecifier,
            TokenType.EditConstSpecifier,
            TokenType.EditFixedSizeSpecifier,
            TokenType.EditInlineSpecifier,
            TokenType.EditInlineUseSpecifier,
            TokenType.NoClearSpecifier,
            TokenType.InterpSpecifier,
            TokenType.InputSpecifier,
            TokenType.TransientSpecifier,
            TokenType.DuplicateTransientSpecifier,
            TokenType.NoImportSpecifier,
            TokenType.NativeSpecifier,
            TokenType.ExportSpecifier,
            TokenType.NoExportSpecifier,
            TokenType.NonTransactionalSpecifier,
            TokenType.PointerSpecifier,
            TokenType.InitSpecifier,
            TokenType.RepRetrySpecifier,
            TokenType.AllowAbstractSpecifier
        };

        private List<TokenType> ClassSpecifiers = new List<TokenType>
        {
            TokenType.AbstractSpecifier,
            TokenType.ConfigSpecifier,
            TokenType.DependsOnSpecifier,
            TokenType.ImplementsSpecifier,
            TokenType.InstancedSpecifier,
            TokenType.ParseConfigSpecifier,
            TokenType.PerObjectConfigSpecifier,
            TokenType.PerObjectLocalizedSpecifier,
            TokenType.TransientSpecifier,
            TokenType.NonTransientSpecifier,
            TokenType.DeprecatedSpecifier
        };

        private List<TokenType> BasicSymbols = new List<TokenType>
        {
            TokenType.Byte,
            TokenType.Int,
            TokenType.Bool,
            TokenType.Float,
            TokenType.String
        };

        // Update!
        private List<TokenType> ClassKeywords = new List<TokenType>
        {
            TokenType.Object,
            TokenType.Actor
        };

        private List<TokenType> PropertyTypes = new List<TokenType>
        {
            TokenType.InstanceVariable,
            TokenType.Struct,
            TokenType.Enumeration
        };

        public StringParser(StringLexer lexer, TypeManager symbols)
        {
            Tokens = new TokenStream<String>(lexer);
            Types = symbols;
        }

        public AbstractSyntaxTree ParseClass()
        {
            throw new NotImplementedException();
        }

        public AbstractSyntaxTree ParseClassSkeleton()
        {
            Func<AbstractSyntaxTree> parser = () =>
                {
                    Token<String> classToken = Tokens.ConsumeToken(TokenType.Class);
                    if (classToken == null)
                        return null; // ERROR: Malformed file, class keyword expected.

                    Token<String> nameToken = Tokens.ConsumeToken(TokenType.Word);
                    if (nameToken == null)
                        return null; // ERROR: Malformed file, class name expected.
                    if (IsValidType(nameToken))
                        return null; // ERROR: A class with that name already exists!

                    String parent = ParseParentClass();
                    if (parent == null)
                        return null; // ERROR: Class must have a parent class!
                    // TODO: support optional within clause here.

                    var specifiers = ParseTokensFromList(ClassSpecifiers);

                    if (CurrentTokenType != TokenType.SemiColon)
                        return null; // ERROR: ';' expected after class declaration header.
                    Tokens.Advance();

                    List<AbstractSyntaxTree> properties = ParseProperties(PropertyTypes);

                    var node = new ClassNode(classToken.Type, nameToken.Value, parent, specifiers);
                    node.AddProperties(properties);
                    
                    return RegisterType(node.TypeName, node);
                };
            return Tokens.TryGetTree(parser);
        }

        public void ParseDefaultProperties()
        {
            throw new NotImplementedException();
        }

        // Function from hell... Refactor!
        private List<AbstractSyntaxTree> TryParseVariables()
        {
            if (CurrentTokenType != TokenType.InstanceVariable)
                return null;
            var variables = new List<AbstractSyntaxTree>();
            Tokens.PushSnapshot();
            Tokens.Advance();
            List<TokenType> specifiers = new List<TokenType>();
            if (CurrentTokenType != TokenType.Enumeration)
                specifiers = ParseTokensFromList(VariableSpecifiers);

            Token<String> type = null;
            if (CurrentTokenType == TokenType.Enumeration || CurrentTokenType == TokenType.Struct)
            {
                var tree = TryParseEnum() ?? TryParseStruct();
                if (tree == null)
                {
                    Tokens.PopSnapshot();
                    return null; // ERROR: Malformed declaration (?)
                }
                else
                {
                    type = new Token<String>(TokenType.Word, (tree as TypeDeclarationNode).TypeName);
                    variables.Add(tree);
                }
            }
            else
            {
                type = Tokens.CurrentItem;
                Tokens.Advance();
                if (!IsValidType(type))
                {
                    Tokens.PopSnapshot();
                    return null; // ERROR: unknown variable type!
                }
            }

            List<String> variableNames = TokensToDelimitedStrings(
                ParseTokensBefore(TokenType.SemiColon), TokenType.Comma);
            if (variableNames.Count == 0)
            {
                Tokens.PopSnapshot();
                return null; // ERROR: One or more variable names expected!
            }

            //TODO: check if names are taken!
            foreach (String name in variableNames)
            {
                variables.Add(new VariableNode(TokenType.InstanceVariable, name, type.Value, specifiers));
            }

            Tokens.DiscardSnapshot();
            return variables;
            /*Func<AbstractSyntaxTree> parser = () =>
                {
                    TokenType scope = TokenType.StructMember;
                    if (CurrentTokenType == TokenType.InstanceVariable
                        || CurrentTokenType == TokenType.LocalVariable)
                    {
                        scope = CurrentTokenType;
                        Tokens.Advance();
                    }
                    List<TokenType> specifiers = ParseTokensFromList(VariableSpecifiers);
                    Token<String> type = Tokens.CurrentItem;
                    Tokens.Advance();
                    String variableName = Tokens.ConsumeToken(TokenType.Word).Value;

                    if (variableName != null && IsValidType(type)
                        && Tokens.ConsumeToken(delimiter) != null)
                    {
                        return new VariableNode(scope, variableName, type.Value, specifiers);
                    }
                    return null;
                };
            return Tokens.TryGetTree(parser);*/
        }

        private AbstractSyntaxTree TryParseStruct()
        {
            return null;
        }

        private AbstractSyntaxTree TryParseEnum()
        {
            Func<AbstractSyntaxTree> parser = () =>
            {
                var enumToken = Tokens.ConsumeToken(TokenType.Enumeration);
                if (enumToken == null)
                    return null;

                String enumName = Tokens.ConsumeToken(TokenType.Word).Value;
                //TODO: check that type is valid etc!
                var contentValues = ParseScopedTokens(
                    TokenType.LeftBracket, TokenType.RightBracket);
                var enumValues = TokensToDelimitedStrings(contentValues, TokenType.Comma);
                if (enumValues.Count == 0)
                    return null; // ERROR: enum has no values!
                return RegisterType(enumName, 
                    new EnumerationNode(TokenType.Enumeration, enumName, enumValues));
            };
            return Tokens.TryGetTree(parser);
        }

        #region Helpers

        private List<TokenType> ParseTokensFromList(List<TokenType> typeList)
        {
            var tokens = new List<TokenType>();
            while (typeList.Contains(CurrentTokenType))
            {
                tokens.Add(CurrentTokenType);
                Tokens.Advance();
            }
            return tokens;
        }

        private List<AbstractSyntaxTree> ParseProperties(List<TokenType> typeList)
        {
            var properties = new List<AbstractSyntaxTree>();
            while (typeList.Contains(CurrentTokenType))
            {
                List<AbstractSyntaxTree> trees = TryParseVariables() ?? new List<AbstractSyntaxTree>();
                if (trees.Count == 0)
                {
                    var tree = TryParseStruct() ?? TryParseEnum();
                    if (tree != null)
                        trees.Add(tree);
                }

                if (trees.Count == 0)
                    break; // ERROR: Property declaration expected!
                if (Tokens.ConsumeToken(TokenType.SemiColon) == null)
                    break; // ERROR: Semicolon expected after class property declaration!
                properties.AddRange(trees);
            }
            return properties;
        }

        private List<AbstractSyntaxTree> Properties(List<TokenType> typeList)
        {
            var properties = new List<AbstractSyntaxTree>();
            while (typeList.Contains(CurrentTokenType))
            {

            }
            return properties;
        }

        private String ParseParentClass()
        {
            if (Tokens.ConsumeToken(TokenType.Extends) == null)
                return null; // ERROR: Expected 'extends' keyword!
            var parent = ParseTokenFromList(ClassKeywords) ?? Tokens.ConsumeToken(TokenType.Word);
            if (parent == null)
                return null; // ERROR: Expected parent class name!
            // TODO: check identifier validity?
            return parent.Value;
        }

        private Token<String> ParseTokenFromList(List<TokenType> types)
        {
            Token<String> token = null;
            if (!types.Contains(CurrentTokenType))
                return null; // ERROR?

            foreach (var type in types)
            {
                token = Tokens.ConsumeToken(type);
                if (token != null)
                    return token;
            }
            return null;
        }

        private List<String> TokensToDelimitedStrings(List<Token<String>> tokens, TokenType delimiter)
        {
            List<String> strings = new List<String>();
            IEnumerator<Token<String>> iterator = tokens.GetEnumerator();
            iterator.MoveNext();
            do
            {
                var token = iterator.Current;
                if (token.Type != TokenType.Word)
                    return null; // ERROR?
                bool end = !iterator.MoveNext();
                if ((!end && iterator.Current.Type == delimiter) || end)
                    strings.Add(token.Value);
                else 
                    return null;  // ERROR?
            } while (iterator.MoveNext());

            return strings;
        }

        private List<Token<String>> ParseScopedTokens(TokenType scopeStart, TokenType scopeEnd)
        {
            var scopedTokens = new List<Token<String>>();
            if (Tokens.ConsumeToken(scopeStart) == null)
                return null; // ERROR: expected 'scopeStart' at start of a scope

            int nestedLevel = 1;
            while(nestedLevel > 0)
            {
                if (Tokens.AtEnd())
                    return null; // ERROR: Scope ended prematurely, are your scoped unbalanced?
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

        private List<Token<String>> ParseTokensBefore(TokenType delimiter)
        {
            var scopedTokens = new List<Token<String>>();
            while (CurrentTokenType != delimiter)
            {
                scopedTokens.Add(Tokens.CurrentItem);
                Tokens.Advance();
            }
            return scopedTokens;
        }

        private bool IsValidType(Token<String> token)
        {
            return Types.SymbolExists(token.Value);
        }

        private AbstractSyntaxTree RegisterType(String name, AbstractSyntaxTree tree)
        {
            return Types.TryRegisterType(name, tree) ? tree : null;
        }

        #endregion
    }
}
