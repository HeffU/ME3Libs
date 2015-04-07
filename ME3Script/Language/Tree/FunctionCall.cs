﻿using ME3Script.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Language.Tree
{
    public class FunctionCall : Expression
    {
        public SymbolReference Function;
        public List<Expression> Parameters;

        public FunctionCall(SymbolReference func, List<Expression> parameters, SourcePosition start, SourcePosition end)
            : base(ASTNodeType.FunctionCall, start, end)
        {
            Function = func;
            Parameters = parameters;
        }
    }
}