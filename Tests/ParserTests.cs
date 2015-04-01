using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script.Parsing;
using ME3Script.Lexing;
using System.Collections.Generic;
using ME3Script.Lexing.Tokenizing;
using ME3Script.Language.Tree;
using ME3Script.Utilities;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void BasicClassTest()
        {
            var source = 
                "class Test extends Actor within Object Deprecated Transient; \n" +
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
                "final static operator(254) int add( coerce float left, coerce float right )\n" +
                "{\n" +
                "   all the dragons\n" +
                "}\n" +
                "\n" +
                "\n" +
                "\n";

            var parser = new StringParser(new TokenStream<String>(new StringLexer(source)));

            Class node = (Class)parser.ParseDocument();
            return;
        }
    }
}
