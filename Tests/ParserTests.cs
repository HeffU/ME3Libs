using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script.Parsing;
using ME3Script.Lexing;
using ME3Script.Language.Types;
using System.Collections.Generic;
using ME3Script.Lexing.Tokenizing;
using ME3Script.Language.Nodes;

namespace Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void BasicClassTest()
        {
            var source = 
                "class Test extends Actor Deprecated Transient; \n" +
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
                "} structA, structB;\n";

            List<AbstractType> types = new List<AbstractType>
            {
                new BasicDataType("int", TokenType.Int)
            };
            TypeManager.InitializeGlobalNamespace(types);

            var typeMgr = new TypeManager();
            var parser = new StringParser(new StringLexer(source), typeMgr);

            ClassNode node = parser.ParseClassSkeleton() as ClassNode;
            return;
        }
    }
}
