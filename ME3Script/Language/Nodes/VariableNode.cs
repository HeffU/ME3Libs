﻿using ME3Script.Lexing.Tokenizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Language.Nodes
{
    public class VariableNode : TypeDeclarationNode
    {
        public bool IsLocal { get { return Type == TokenType.LocalVariable; } }
        public bool IsInstance { get { return Type == TokenType.InstanceVariable; } }

        public String Name { get; private set; }

        private List<TokenType> Specifiers;

        public VariableNode(TokenType type, String name, String typeName, List<TokenType> specifiers) : base(type, typeName)
        {
            Name = name;
            Specifiers = specifiers;
        }

        public bool HasSpecifier(TokenType spec)
        {
            return Specifiers.Contains(spec);
        }
    }
}