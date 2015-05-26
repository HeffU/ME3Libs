﻿using ME3Script.Analysis.Visitors;
using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Language.Tree
{
    public class StateLabel : VariableIdentifier
    {
        public int StartOffset;

        public StateLabel(String name, int offset, SourcePosition start, SourcePosition end)
            : base(name, start, end)
        {
            StartOffset = offset;
            Type = ASTNodeType.StateLabel;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
    }
}
