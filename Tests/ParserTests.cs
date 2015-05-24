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
using ME3Script.Language.Util;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        MessageLog log = new MessageLog();

        [TestMethod]
        public void TestParseSpecifier()
        {
            var source = "coerce native";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            Assert.AreEqual(parser.TryParseSpecifier(GlobalLists.ParameterSpecifiers).Value.ToLower(), "coerce");
            Assert.IsNull(parser.TryParseSpecifier(GlobalLists.ParameterSpecifiers));

            return;
        }

        [TestMethod]
        public void TestParseParameter()
        {
            var source = "coerce out int one float two[5] optional )";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            Assert.AreEqual(parser.TryParseParameter().Name.ToLower(), "one");
            Assert.AreEqual(parser.TryParseParameter().IsStaticArray, true);
            Assert.IsNull(parser.TryParseParameter());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected parameter type!");

            return;
        }

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
                "{ var float a, b, c; };\n" +
                "var private struct transient twoStruct extends testStruct\n" +
                "{\n" +
                "   var etestnumeration num;\n" +
                "} structA, structB;\n" +
                "function float funcB( testStruct one, float two ) \n" +
                "{\n" +
                "   return one.a + 0.33 * (0.66 + 0.1) * 1.5;\n" +
                "}\n" +
                "private simulated function float MyFunc( out testStruct one, coerce optional float two ) \n" +
                "{\n" +
                "   return one.b + funcB(one, two);\n" +
                "}\n" +
                "auto state MyState\n" +
                "{\n" +
                "ignores MyFunc;\n" +
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

            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            var symbols = new SymbolTable();

            Class obj = new Class("Object", null, null, null, null, null, null, null, null, null, null);
            obj.OuterClass = obj;
            symbols.PushScope(obj.Name);
            symbols.AddSymbol(obj.Name, obj);

            VariableType integer = new VariableType("int", null, null);
            symbols.AddSymbol(integer.Name, integer);
            VariableType floatingpoint = new VariableType("float", null, null);
            symbols.AddSymbol(floatingpoint.Name, floatingpoint);

            InOpDeclaration plus_float = new InOpDeclaration("+", 20, false, null, floatingpoint, new FunctionParameter(floatingpoint, null, null, null, null),
                new FunctionParameter(floatingpoint, null, null, null, null), null, null, null);
            symbols.AddOperator(plus_float);

            InOpDeclaration mult_float = new InOpDeclaration("*", 16, false, null, floatingpoint, new FunctionParameter(floatingpoint, null, null, null, null),
                new FunctionParameter(floatingpoint, null, null, null, null), null, null, null);
            symbols.AddOperator(mult_float);

            Class node = (Class)parser.ParseDocument();
            var ClassValidator = new ClassValidationVisitor(log, symbols);
            node.AcceptVisitor(ClassValidator);

            symbols.GoDirectlyToStack(node.GetInheritanceString());
            foreach (Function f in node.Functions)
            {
                symbols.PushScope(f.Name);
                var p = new CodeBodyParser(new TokenStream<String>(new StringLexer(source)), f.Body, symbols, f, log);
                var b = p.ParseBody();
                symbols.PopScope();
            }

            return;
        }
    }
}
