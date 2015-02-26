using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ME3Script;
using ME3Script.Lexing.Matching.StringMatchers;
using System.Collections.Generic;
using ME3Script.Lexing.Tokenizing;

namespace Tests
{
    [TestClass]
    public class MatcherTests
    {
        private List<KeywordMatcher> Delimiters = new List<KeywordMatcher>();

        [TestInitialize]
        public void Initialize()
        {
            Delimiters = new List<KeywordMatcher>
            {
                new KeywordMatcher("{", TokenType.LeftBracket, null),
                new KeywordMatcher("}", TokenType.RightBracket, null),
                new KeywordMatcher("[", TokenType.LeftSqrBracket, null),
                new KeywordMatcher("]", TokenType.RightSqrBracket, null),
                new KeywordMatcher("==", TokenType.Equals, null),    
                new KeywordMatcher("+=", TokenType.AddAssign, null),   
                new KeywordMatcher("-=", TokenType.SubAssign, null),   
                new KeywordMatcher("*=", TokenType.MulAssign, null),   
                new KeywordMatcher("/=", TokenType.DivAssign, null),      
                new KeywordMatcher("!=", TokenType.NotEquals, null),  
                new KeywordMatcher("~=", TokenType.ApproxEquals, null), 
                new KeywordMatcher(">>", TokenType.RightShift, null),    
                new KeywordMatcher("<<", TokenType.LeftShift, null),
                new KeywordMatcher("<=", TokenType.LessOrEquals, null),
                new KeywordMatcher(">=", TokenType.GreaterOrEquals, null),
                new KeywordMatcher("**", TokenType.Power, null), 
                new KeywordMatcher("&&", TokenType.And, null),   
                new KeywordMatcher("||", TokenType.Or, null),         
                new KeywordMatcher("^^", TokenType.Xor, null),
                new KeywordMatcher("<", TokenType.LessThan, null),    
                new KeywordMatcher(">", TokenType.GreaterThan, null),         
                new KeywordMatcher("%", TokenType.Modulo, null),
                new KeywordMatcher("$=", TokenType.StrConcatAssign, null),
                new KeywordMatcher("$", TokenType.StrConcat, null),
                new KeywordMatcher("@=", TokenType.StrConcAssSpace, null),
                new KeywordMatcher("@", TokenType.StrConcatSpace, null),
                new KeywordMatcher("-", TokenType.Subract, null),      
                new KeywordMatcher("+", TokenType.Add, null),        
                new KeywordMatcher("*", TokenType.Multiply, null),   
                new KeywordMatcher("/", TokenType.Divide, null),  
                new KeywordMatcher("=", TokenType.Assign, null),  
                new KeywordMatcher("~", TokenType.BinaryNegate, null), 
                new KeywordMatcher("&", TokenType.BinaryAnd, null),    
                new KeywordMatcher("|", TokenType.BinaryOr, null),     
                new KeywordMatcher("^", TokenType.BinaryXor, null),     
                new KeywordMatcher("?", TokenType.Conditional, null),   
                new KeywordMatcher(":", TokenType.Colon, null)         
            };
        }

        [TestMethod]
        public void TestWhiteSpaceMatcher()
        {
            WhiteSpaceMatcher matcher = new WhiteSpaceMatcher();
            StringTokenizer data = new StringTokenizer(" ,   \n   test ");

            // Match a single whitespace
            Assert.AreEqual(matcher.MatchNext(data).Type, TokenType.WhiteSpace);
            Assert.AreEqual(data.CurrentItem, ",");
            data.Advance();
            // Match a longer mixed whitespace and check that all of it is discarded
            Assert.IsNull(matcher.MatchNext(data).Value);
            Assert.AreEqual(data.CurrentItem, "t");
            data.Advance(4);
            // Match EOF whitespace
            Assert.IsNotNull(matcher.MatchNext(data));

            Assert.IsTrue(data.AtEnd());
        }

        [TestMethod]
        public void TestNumberMatcher()
        {
            NumberMatcher matcher = new NumberMatcher(Delimiters);
            StringTokenizer data = new StringTokenizer("1 454 12h 0xa 0x 0xA12 0x1G 1.0 0.33 0x22.3 123.13a 1xA");

            // Match a single integer
            Assert.AreEqual(matcher.MatchNext(data).Value, "1");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match several chars
            Assert.AreEqual(matcher.MatchNext(data).Type, TokenType.IntegerNumber);
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Try to match a number that is actually a word
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "1");
            data.Advance(4);
            // Match a hexadecimal number
            Assert.IsNotNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // fail to match a partial hex number
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "0");
            data.Advance(3);
            // Match and assert value of hexadecimal number converted to decimal
            Assert.AreEqual(matcher.MatchNext(data).Value, (0xA12).ToString("D"));
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // fail to match a malformed hex number
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "0");
            data.Advance(5);
            // Match a floating point number
            Assert.AreEqual(matcher.MatchNext(data).Type, TokenType.FloatingNumber);
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match a float and assert value
            Assert.AreEqual(matcher.MatchNext(data).Value, "0.33");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Fail parsing a malformed hex-float
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "0");
            data.Advance(7);
            // Fail parsing a malformed float-hex
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "1");
            data.Advance(8);
            // Fail parsing a malformed hex prefix
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "1");
            data.Advance(3);

            Assert.IsTrue(data.AtEnd());
        }

        [TestMethod]
        public void TestWordMatcher()
        {
            WordMatcher matcher = new WordMatcher(Delimiters);
            StringTokenizer data = new StringTokenizer("test \"'test test2+end");

            // Match a basic word
            Assert.IsNotNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Ensure that string/name tokens are not considered 
            // as they are exempt from the delimiters.
            Assert.IsNull(matcher.MatchNext(data));
            data.Advance();
            Assert.IsNull(matcher.MatchNext(data));
            data.Advance();
            Assert.AreEqual(matcher.MatchNext(data).Value, "test");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match a word containing a number
            Assert.AreEqual(matcher.MatchNext(data).Value, "test2");
            Assert.AreEqual(data.CurrentItem, "+");
            data.Advance();
            // Ensure token type and EOF handling
            Assert.AreEqual(matcher.MatchNext(data).Type, TokenType.Word);

            Assert.IsTrue(data.AtEnd());
        }

        [TestMethod]
        public void TestKeywordMatcher()
        {
            KeywordMatcher matcher = new KeywordMatcher("invalid", TokenType.INVALID, Delimiters, false);
            StringTokenizer data = new StringTokenizer("+=+//= invalid=invalidinvalid$=");

            // Match a basic delimiter keyword
            Assert.AreEqual(Delimiters.Find(m => m.Keyword == "+=").MatchNext(data).Type, TokenType.AddAssign);
            Assert.AreEqual(data.CurrentItem, "+");
            data.Advance();
            // Assert that the matched token contains the value
            Assert.AreEqual(Delimiters.Find(m => m.Keyword == "/").MatchNext(data).Value, "/");
            Assert.AreEqual(data.CurrentItem, "/");
            data.Advance(3);
            // Match against a keyword rather than short operator as well as delimiter
            Assert.IsNotNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "=");
            data.Advance();
            // Assert that non-delimiters will prevent a match
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "i");
            data.Advance(14);
            // Ensure EOF with keywords
            Assert.IsNotNull(Delimiters.Find(m => m.Keyword == "$=").MatchNext(data));

            Assert.IsTrue(data.AtEnd());
        }

        [TestMethod]
        public void TestStringLiteralMatcher()
        {
            StringLiteralMatcher matcher = new StringLiteralMatcher();
            StringTokenizer data = new StringTokenizer("\"str\" \"\"\"test\" \"\\\"\" \"\\n\" \"eof");

            // Match basic string
            Assert.IsNotNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match empty string
            Assert.AreEqual(matcher.MatchNext(data).Value, "");
            Assert.AreEqual(data.CurrentItem, "\"");
            // Ensure non-empty string contents
            Assert.AreEqual(matcher.MatchNext(data).Value, "test");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match escape char
            Assert.AreEqual(matcher.MatchNext(data).Value, "\\\"");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Include but dont react to other escape chars
            Assert.AreEqual(matcher.MatchNext(data).Value, "\\n");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Ensure discarding of string that reaches eof without closing quote
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "\"");

        }

        [TestMethod]
        public void TestNameLiteralMatcher()
        {
            NameLiteralMatcher matcher = new NameLiteralMatcher();
            StringTokenizer data = new StringTokenizer("'A' 'name_0xA2' '' 'n\\ame' 'Bob bobson' '\"Bob\"' 'eof");

            // Match basic name
            Assert.IsNotNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match name with underscore and digit
            Assert.AreEqual(matcher.MatchNext(data).Value, "name_0xA2");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Match empty name
            Assert.AreEqual(matcher.MatchNext(data).Value, "");
            Assert.AreEqual(data.CurrentItem, " ");
            data.Advance();
            // Fail escape char
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "'");
            data.Advance(8);
            // Fail whitespace
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "'");
            data.Advance(13);
            // Fail quotes
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "'");
            data.Advance(8);
            // Ensure discarding of name that reaches eof without closing single-quote
            Assert.IsNull(matcher.MatchNext(data));
            Assert.AreEqual(data.CurrentItem, "'");
        }
    }
}
