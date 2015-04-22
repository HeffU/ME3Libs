using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script.Parsing;
using ME3Script.Lexing;
using System.Collections.Generic;
using ME3Script.Lexing.Tokenizing;
using ME3Script.Language.Tree;
using ME3Script.Utilities;
using ME3Script.Analysis.Visitors;
using ME3Script.Compiling.Errors;
using ME3Script.Analysis.Symbols;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void BasicClassTest()
        {
            var source = 
                "class Test Deprecated Transient; \n" +
                "var enum ETestnumeration {\n" +
                "     TEST_value1,\n" +
                "     TEST_value2,\n" +
                "     TEST_value3,\n" +
                "} inlineNumeration, testnum2;\n" +
                "var private deprecated int X; \n" +
                "VAR INT Y, Z; \n" +
                "var ETestnumeration testnum;\n" +
                "struct transient testStruct\n" +
                "{ var int a, b, c; };\n" +
                "var private struct transient twoStruct extends testStruct\n" +
                "{\n" +
                "   var etestnumeration num;\n" +
                "} structA, structB;\n" +
                "private simulated function testStruct MyFunc( out int one, coerce optional float two ) \n" +
                "{\n" +
                "   herebedragons\n" +
                "}\n" +
                "auto state MyState\n" +
                "{\n" +
                "ignores MyFunc, dragon;\n" +
                "function StateFunc()\n" +
                "{\n" +
                "}\n" +
                "\n" +
                "Begin:\n" +
                "       moredragons\n" +
                "}\n" +
                "\n" +
                "final static operator(254) int >>>( coerce float left, coerce float right )\n" +
                "{\n" +
                "   all the dragons\n" +
                "}\n" +
                "\n" +
                "\n" +
                "\n";

            var log = new MessageLog();
            var parser = new StringParser(new TokenStream<String>(new StringLexer(source)), log);
            var symbols = new SymbolTable();

            Class obj = new Class("Object", null, null, null, null, null, null, null, null, null, null);
            obj.OuterClass = obj;
            symbols.PushScope(obj.Name);
            symbols.AddSymbol(obj.Name, obj);

            VariableType integer = new VariableType("int", null, null);
            symbols.AddSymbol(integer.Name, integer);
            VariableType floatingpoint = new VariableType("float", null, null);
            symbols.AddSymbol(floatingpoint.Name, floatingpoint);

            Class node = (Class)parser.ParseDocument();
            var ClassValidator = new ClassValidationVisitor(log, symbols);
            node.AcceptVisitor(ClassValidator);
            return;
        }
    }
}
