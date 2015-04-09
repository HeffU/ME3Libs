﻿using ME3Script.Language.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Script.Analysis.Symbols
{
    public class SymbolTable
    {
        private Dictionary<String, Dictionary<String, ASTNode>> Cache;
        private LinkedList<Dictionary<String, ASTNode>> Scopes;
        private LinkedList<String> ScopeNames;

        public String CurrentScopeName
        {
            get
            {
                if (ScopeNames.Count == 0)
                    return "";
                return ScopeNames.Last();
            }
        }

        public SymbolTable()
        {
            ScopeNames = new LinkedList<String>();
            Scopes = new LinkedList<Dictionary<String, ASTNode>>();
            Cache = new Dictionary<String, Dictionary<String, ASTNode>>();
        }

        public void PushScope(String name)
        {
            String fullName = (CurrentScopeName == "" ? "" : CurrentScopeName + ".") + name;
            Dictionary<String, ASTNode> scope;
            bool cached = Cache.TryGetValue(fullName, out scope);
            if (!cached)
                scope = new Dictionary<String, ASTNode>();

            Scopes.AddLast(scope);
            ScopeNames.AddLast(fullName);
            
            if (!cached)
                Cache.Add(fullName, scope);
        }

        public void PopScope()
        {
            if (Scopes.Count == 0)
                throw new InvalidOperationException();

            Scopes.RemoveLast();
            ScopeNames.RemoveLast();
        }

        public bool TryGetSymbol(String symbol, out ASTNode node, String outerScope)
        {
            return TryGetSymbolInternal(symbol, out node, Scopes) ||
                TryGetSymbolInScopeStack(symbol, out node, outerScope);
        }

        public bool SymbolExists(String symbol, String outerScope)
        {   
            ASTNode dummy;
            return TryGetSymbol(symbol, out dummy, outerScope);
        }

        public bool TryGetSymbolInScopeStack(String symbol, out ASTNode node, String lowestScope)
        {
            LinkedList<Dictionary<String, ASTNode>> stack;
            node = null;
            if (!TryBuildSpecificScope(lowestScope, out stack))
                return false;

            return TryGetSymbolInternal(symbol, out node, stack);
        }

        private bool TryBuildSpecificScope(String lowestScope, out LinkedList<Dictionary<String, ASTNode>> stack)
        {
            var names = lowestScope.Split('.');
            stack = new LinkedList<Dictionary<String, ASTNode>>();
            Dictionary<String, ASTNode> currentScope;
            foreach (string scopeName in names)
            {
                if (Cache.TryGetValue(scopeName, out currentScope))
                    stack.AddLast(currentScope);
                else
                    return false;
            }
            return stack.Count > 0;
        }

        private bool TryGetSymbolInternal(String symbol, out ASTNode node, LinkedList<Dictionary<String, ASTNode>> stack)
        {
            LinkedListNode<Dictionary<String, ASTNode>> it;
            for (it = stack.Last; it != null; it = it.Previous)
            {
                if (it.Value.TryGetValue(symbol, out node))
                    return true;
            }
            node = null;
            return false;
        }

        public bool SymbolExistsInCurrentScope(String symbol)
        {
            return Scopes.Last().ContainsKey(symbol);
        }

        public void AddSymbol(String symbol, ASTNode node)
        {
            Scopes.Last().Add(symbol, node);
        }

        public bool TryAddSymbol(String symbol, ASTNode node)
        {
            if (!SymbolExistsInCurrentScope(symbol))
            {
                AddSymbol(symbol, node);
                return true;
            }
            return false;
        }
    }
}