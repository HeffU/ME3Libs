using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script;

namespace Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestLexerGetToken()
        {
            String source = "+ \n \"asd";
            var lexer = new StringLexer(source);

            // Match that the first token is a +
            Assert.AreEqual(lexer.GetNextToken().Type, TokenType.Add);
            // Ensure that whitespace is returned
            Assert.AreEqual(lexer.GetNextToken().Type, TokenType.WhiteSpace);
            // Assert that an invalid token is found on a non-terminated string
            Assert.AreEqual(lexer.GetNextToken().Type, TokenType.INVALID);
            // Assert that after an invalid token the stream advances
            Assert.AreEqual(lexer.GetNextToken().Value, "asd");
            // Assert that EOF is returned at the end of the stream
            Assert.AreEqual(lexer.GetNextToken().Type, TokenType.EOF);
        }

        [TestMethod]
        public void TestLexerOutput()
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
