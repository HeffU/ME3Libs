﻿using ME3Script.Analysis.Visitors;
using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Language.Tree
{
    public class Struct : VariableType
    {
        public List<Specifier> Specifiers;
        public VariableType Parent;
        public List<VariableDeclaration> Members;
        public DefaultPropertiesBlock DefaultProperties;

        public Struct(String name, List<Specifier> specs,
            List<VariableDeclaration> members,
            SourcePosition start, SourcePosition end, VariableType parent = null)
            : base(name, start, end)
        {
            Type = ASTNodeType.Struct;
            Specifiers = specs;
            Members = members;
            Parent = parent;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }

        public bool SameOrSubStruct(String name)
        {
            String nodeName = this.Name.ToLower();
            String inputName = name.ToLower();
            if (nodeName == inputName)
                return true;
            Struct current = this;
            while (current.Parent != null)
            {
                if (current.Parent.Name.ToLower() == inputName)
                    return true;
                current = (Struct)current.Parent;
            }
            return false;
        }
    }
}
