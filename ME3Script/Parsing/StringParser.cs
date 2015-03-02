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

                    return node;
                };
            return Tokens.TryGetTree(parser);
        }

        public void ParseDefaultProperties()
        {
            throw new NotImplementedException();
        }

        private AbstractSyntaxTree TryParseVariable(TokenType delimiter)
        {
            Func<AbstractSyntaxTree> parser = () =>
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

                    if (variableName != null && Tokens.ConsumeToken(delimiter) != null && IsValidType(type))
                    {
                        return new VariableNode(scope, variableName, type.Value, specifiers);
                    }
                    return null;
                };
            return Tokens.TryGetTree(parser);
        }

        private AbstractSyntaxTree TryParseStruct()
        {
            return null;
        }

        private AbstractSyntaxTree TryParseEnum()
        {
            return null;
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
                AbstractSyntaxTree tree = 
                    TryParseVariable(TokenType.SemiColon) ??
                    TryParseStruct() ??
                    TryParseEnum();
                
                if (tree == null)
                    break; // ERROR: ?
                properties.Add(tree);
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
            // TODO: check symbol validity?
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

        private bool IsValidType(Token<String> token)
        {
            return Types.SymbolExists(token.Value);
        }

        private bool RegisterType(String name, AbstractSyntaxTree tree)
        {
            return Types.TryRegisterType(name, tree);
        }

        #endregion
    }
}