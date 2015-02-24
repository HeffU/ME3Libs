using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script;

namespace Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void BasicTests()
        {
            var test = @"a.a";
            var lexer = new StringLexer(test);
            var tokens = lexer.LexData();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
