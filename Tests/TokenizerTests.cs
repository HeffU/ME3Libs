using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ME3Script;
using System.Globalization;
using System.Collections.Generic;
using ME3Script.Lexing.Tokenizing;

namespace Tests
{
    [TestClass]
    public class TokenizableDataStreamTests
    {
        [TestMethod]
        public void TestStreamTokenizer()
        {
            String source = "123456789ABCDEF";
            Func<List<String>> input = () => source.ToCharArray()
                .Select(i => i.ToString(CultureInfo.InvariantCulture))
                .ToList();
            TokenizableDataStream<String> data = new TokenizableDataStream<String>(input);

            // Take base snapshot for later match
            data.PushSnapshot();
            // Match CurrentItem result
            Assert.AreEqual(data.CurrentItem, "1");
            // Match advance
            data.Advance(3);
            Assert.AreEqual(data.CurrentItem, "4");
            // Snapshot and advance and then pop and ensure result
            data.PushSnapshot();
            data.Advance();
            data.PopSnapshot();
            Assert.AreEqual(data.CurrentItem, "4");
            // Snapshot, advance, discard snapshot and ensure result
            data.PushSnapshot();
            data.Advance(3);
            data.DiscardSnapshot();
            Assert.AreEqual(data.CurrentItem, "7");
            // Pop snapshot and assert that we are once again at the start
            data.PopSnapshot();
            Assert.AreEqual(data.CurrentItem, "1");
            // Test AtEnd for non-eof
            Assert.IsFalse(data.AtEnd());
            // Advance too far and ensure that AtEnd returns
            data.PushSnapshot();
            data.Advance(23);
            Assert.IsTrue(data.AtEnd());
            // Assert that currentItem is null when out of range
            Assert.IsNull(data.CurrentItem);
            // Pop back to start and assert that LookAhead returns desired item
            data.PopSnapshot();
            Assert.AreEqual(data.LookAhead(4), "5");
            // Assert that LookAhead returns null once out of range
            Assert.IsNull(data.LookAhead(42));
        }
    }
}
