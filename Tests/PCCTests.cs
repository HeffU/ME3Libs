using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ME3Data.FileFormats.PCC;
using ME3Data.Utility;

namespace Tests
{
    [TestClass]
    public class PCCTests
    {
        [TestMethod]
        public void TestLoadUncompressed()
        {
            var path = @"G:\Code\ME3\temp\test.pcc";
            var stream = new FileStream(path, FileMode.Open);
            var pcc = new PCCFile(new PCCStreamReader(stream));

            Assert.IsTrue(pcc.Deserialize());

            stream.Close();
        }
    }
}
