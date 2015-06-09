using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class PCCTests
    {
        [TestMethod]
        public void TestLoadUncompressed()
        {
            //var path = @"G:\Code\ME3\temp\Core.pcc";
            var path = @"G:\Code\ME3\temp\test.pcc";
            var stream = new FileStream(path, FileMode.Open);
            var pcc = new PCCFile(new PCCStreamReader(stream), "test");

            Assert.IsTrue(pcc.Deserialize());
            var deps = pcc.ImportPackages;
            var loaded = new List<PCCFile>();
            foreach (var dep in deps)
            {
                var depPath = @"G:\Code\ME3\temp\" + dep + ".pcc";
                var depStream = new FileStream(depPath, FileMode.Open);
                var depPCC = new PCCFile(new PCCStreamReader(depStream), dep);
                Assert.IsTrue(depPCC.Deserialize());
                loaded.Add(depPCC);
            }

            stream.Close();
        }
    }
}
