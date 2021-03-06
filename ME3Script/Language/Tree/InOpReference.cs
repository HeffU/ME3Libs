﻿using ME3Script.Analysis.Visitors;
using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Language.Tree
{
    public class InOpReference : Expression
    {
        public InOpDeclaration Operator;
        public Expression LeftOperand;
        public Expression RightOperand;

        public InOpReference(InOpDeclaration op, Expression lhs, Expression rhs, SourcePosition start, SourcePosition end)
            : base(ASTNodeType.InOpRef, start, end) 
        {
            Operator = op;
            LeftOperand = lhs;
            RightOperand = rhs;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }

        public override VariableType ResolveType()
        {
            return Operator.ReturnType;
        }
    }
}
