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
        public void TestParseOpDecl()
        {
            var source =                 
                "final static operator(254) int >>>( coerce float left, coerce float right )\n" +
                "{\n" +
                "   all the dragons\n" +
                "}\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var decl = parser.TryParseOperatorDecl();
            Assert.IsNotNull(decl);
            Assert.AreEqual(decl.OperatorKeyword.ToLower(), ">>>");
            Assert.AreEqual((decl as InOpDeclaration).Precedence, 254);
            Assert.AreEqual(decl.ReturnType.Name.ToLower(), "int");
            Assert.AreEqual((decl as InOpDeclaration).RightOperand.Name.ToLower(), "right");

            source = "final static operator int \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '('! (Did you forget to specify operator precedence?)");

            source = "final static operator(asd) int \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected an integer number!");

            source = "final static operator(54 int \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected ')'!");

            source = "final static operator(54) (\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected operator name or return type!");

            source = "final static operator(254) int >>>\n {";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '('!");

            source = "final static operator(254) int >>>( coerce left, coerce float right )\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed operand!");

            source = "final static operator(254) int >>>( coerce float left coerce float right )\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Unexpected operand content!");

            source = "final static operator(254) int >>>( coerce float left )\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "In-fix operators requires exactly 2 parameters!");

            source = "final static preoperator int >>>( coerce float left, coerce float right )\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Post/Pre-fix operators requires exactly 1 parameter!");

            source = "final static operator(254) int >>>( coerce float left, coerce float right )\n { asdasd";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseOperatorDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed operator body!");

            return;
        }

        [TestMethod]
        public void TestParseState()
        {
            var source =
                "auto state MyState\n" +
                "{\n" +
                "ignores MyFunc;\n" +
                "function StateFunc()\n" +
                "{\n" +
                "}\n" +
                "\n" +
                "Begin:\n" +
                "       moredragons\n" +
                "}\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var state = parser.TryParseState();
            Assert.IsNotNull(state);
            Assert.AreEqual(state.Name.ToLower(), "mystate");
            Assert.AreEqual(state.Specifiers[0].Value.ToLower(), "auto");
            Assert.AreEqual(state.Ignores[0].Name.ToLower(), "myfunc");

            source = "auto state { \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected state name!");

            source = "auto state MyState ignores \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '{'!");

            source = "auto state MyState { ignores ; \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed ignore statement!");

            source = "auto state MyState { ignores one, ; \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed ignore statement!");

            source = "auto state MyState { ignores one, two } \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected semi-colon!");

            source = "auto state MyState {  \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseState());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '}'!");

            return;
        }

        [TestMethod]
        public void TestParseFunction()
        {
            var source =
                "private simulated function float MyFunc( out testStruct one, coerce optional float two ) \n" +
                "{\n" +
                "   return one.b + funcB(one, two);\n" +
                "}\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var func = parser.TryParseFunction();
            Assert.IsNotNull(func);
            Assert.AreEqual(func.Name.ToLower(), "myfunc");
            Assert.AreEqual(func.Specifiers[0].Value.ToLower(), "private");
            Assert.AreEqual(func.Parameters[1].Name.ToLower(), "two");
            Assert.AreEqual(func.ReturnType.Name.ToLower(), "float");

            source = "private simulated function ( \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseFunction());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected function name or return type!");

            source = "function int funcA ) \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseFunction());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '('!");

            source = "function int funcA ( out int, ) \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseFunction());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed parameter!");

            source = "function int funcA ( out int one two ) \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseFunction());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Unexpected parameter content!");

            source = "function int funcA ( out int one, int two) \n {";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseFunction());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed function body!");

            return;
        }

        [TestMethod]
        public void TestParseEnum()
        {
            var source =
                "enum ETestnumeration {\n" +
                "     TEST_value1,\n" +
                "     TEST_value2,\n" +
                "     TEST_value3,\n" +
                "};\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var enumeration = parser.TryParseEnum();
            Assert.IsNotNull(enumeration);
            Assert.AreEqual(enumeration.Name.ToLower(), "etestnumeration");
            Assert.AreEqual(enumeration.Values[1].Name.ToLower(), "test_value2");

            source = "enum {\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseEnum());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected enumeration name!");

            source = "enum test \n test_value1, \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseEnum());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '{'!");

            source = "enum test {\n } \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseEnum());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected non-empty enumeration!");

            source = "enum test {\n int value } \n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseEnum());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed enumeration content!");

            return;
        }

        [TestMethod]
        public void TestParseStruct()
        {
            var source =
                "struct transient testStruct\n" +
                "{ var float a, b, c; };\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var s = parser.TryParseStruct();
            Assert.IsNotNull(s);
            Assert.AreEqual(s.Name.ToLower(), "teststruct");
            Assert.AreEqual(s.Members[0].Variables[1].Name.ToLower(), "b");

            source = "struct transient {\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseStruct());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected struct name!");

            source = "struct transient testStruct \n var float a; }";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseStruct());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected '{'!");

            source = "struct transient testStruct { \n var a; }";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseStruct());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed struct content!");

            return;
        }

        [TestMethod]
        public void TestParseVarDecl()
        {
            var source =
                "var private struct transient twoStruct extends testStruct\n" +
                "{\n" +
                "   var etestnumeration num;\n" +
                "} structA, structB;";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var decl = parser.TryParseVarDecl();
            Assert.IsNotNull(decl);
            Assert.AreEqual(decl.Specifiers[0].Value.ToLower(), "private");
            Assert.AreEqual(decl.VarType.Name.ToLower(), "twostruct");
            Assert.AreEqual(decl.Variables[1].Name.ToLower(), "structb");

            source = "var ;\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseVarDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected variable type or struct/enum type declaration!");

            source = "var int ;\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseVarDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed variable names!");

            source = "var int test\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseVarDecl());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected semi-colon!");

            return;
        }

        [TestMethod]
        public void TestParseClass()
        {
            var source =
                "class Test Deprecated Transient; \n" +
                "var private deprecated int X; \n" +
                "struct transient testStruct\n" +
                "{ var float a, b, c; };\n" +
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
                "Begin:\n" +
                "       moredragons\n" +
                "}\n" +
                "final static operator(254) int >>>( coerce float left, coerce float right )\n" +
                "{\n" +
                "   all the dragons\n" +
                "}\n" +
                "\n";
            var parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);

            var decl = parser.TryParseClass();
            Assert.IsNotNull(decl);
            Assert.AreEqual(decl.Name.ToLower(), "test");
            Assert.AreEqual(decl.Specifiers[1].Value.ToLower(), "transient");
            Assert.AreEqual(decl.VariableDeclarations[0].Variables[0].Name.ToLower(), "x");
            Assert.AreEqual(decl.TypeDeclarations[0].Name.ToLower(), "teststruct");
            Assert.AreEqual(decl.Functions[0].Name.ToLower(), "myfunc");
            Assert.AreEqual(decl.States[0].Name.ToLower(), "mystate");
            Assert.AreEqual(decl.Operators[0].OperatorKeyword.ToLower(), ">>>");
            Assert.AreEqual(log.Messages[log.Messages.Count - 1].Message.Substring(0, 10), "No parent ");

            source = "test ;\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected class declaration!");

            source = "class ;\n";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected class name!");

            source = "class Test deprecated\n var int local;";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected semi-colon!");

            source = "class Test deprecated; \n var int ;";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed instance variable!");

            source = "class Test deprecated; \n struct fail;";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Malformed type declaration!");

            source = "class Test deprecated; \n struct fail { var int one; } struct next;";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected semi-colon!");

            source = "class Test deprecated; \n struct fail { var int one; }; function myfunc() {} var int fail;";
            parser = new ClassOutlineParser(new TokenStream<String>(new StringLexer(source)), log);
            Assert.IsNull(parser.TryParseClass());
            Assert.AreEqual(log.AllErrors[log.AllErrors.Count - 1].Message, "Expected function/state/operator declaration!");

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
                "   local float c;" +
                "   one.a = 1.3 * c;" +
                "   while (true)" +
                "   {" +
                "       c = c + c;" +
                "   }" +
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

            var CodeBuilder = new CodeBuilderVisitor();
            node.AcceptVisitor(CodeBuilder);
            Console.Write(CodeBuilder.GetCodeString());

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
