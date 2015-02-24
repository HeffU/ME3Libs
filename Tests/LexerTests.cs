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
            var test = "class test <<< \"str\"'name' 0.3 ";
            var lexer = new StringLexer(test);
            var tokens = lexer.LexData();
            var enumerator = tokens.GetEnumerator();
            enumerator.MoveNext();

            // Match a keyword
            Assert.AreEqual(enumerator.Current.Type, TokenType.Class);
            enumerator.MoveNext();
            // Match a word
            Assert.AreEqual(enumerator.Current.Value, "test");
            enumerator.MoveNext();
            // Match a delimiter with higher priority
            Assert.AreEqual(enumerator.Current.Type, TokenType.LeftShift);
            enumerator.MoveNext();
            // Match another delimiter
            Assert.AreEqual(enumerator.Current.Type, TokenType.LessThan);
            enumerator.MoveNext();
            // Match a string
            Assert.AreEqual(enumerator.Current.Value, "str");
            enumerator.MoveNext();
            // Match a name
            Assert.AreEqual(enumerator.Current.Value, "name");
            enumerator.MoveNext();
            // Match a number
            Assert.AreEqual(enumerator.Current.Value, "0.3");
            enumerator.MoveNext();
            // Assert that we are at the end of stream
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}
