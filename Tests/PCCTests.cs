using System;
using System.Linq;
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
            var loaded = new List<PCCFile>();

            var corepath = @"G:\Code\ME3\temp\Core.pcc";
            var corestream = new FileStream(corepath, FileMode.Open);
            var corepcc = new PCCFile(new PCCStreamReader(corestream), "Core");
            corepcc.Deserialize();
            loaded.Add(corepcc);

            var enginepath = @"G:\Code\ME3\temp\Engine.pcc";
            var enginestream = new FileStream(enginepath, FileMode.Open);
            var enginepcc = new PCCFile(new PCCStreamReader(enginestream), "Engine");
            enginepcc.Deserialize();
            enginepcc.LoadDependencies(loaded);
            loaded.Add(enginepcc);

            var SFXGamepath = @"G:\Code\ME3\temp\SFXGame.pcc";
            var SFXGamestream = new FileStream(SFXGamepath, FileMode.Open);
            var SFXGamepcc = new PCCFile(new PCCStreamReader(SFXGamestream), "SFXGame");
            SFXGamepcc.Deserialize();
            SFXGamepcc.LoadDependencies(loaded);
            loaded.Add(SFXGamepcc);

            var path = @"G:\Code\ME3\temp\test.pcc";
            var stream = new FileStream(path, FileMode.Open);
            var pcc = new PCCFile(new PCCStreamReader(stream), "test");

            Assert.IsTrue(pcc.Deserialize());
            var deps = pcc.ImportPackages;

            foreach (var dep in deps.Where(x => !loaded.Any(p => p.Name == x)))
            {
                var depPath = @"G:\Code\ME3\temp\" + dep + ".pcc";
                var depStream = new FileStream(depPath, FileMode.Open);
                var depPCC = new PCCFile(new PCCStreamReader(depStream), dep);
                Assert.IsTrue(depPCC.Deserialize());
                loaded.Add(depPCC);
            }

            pcc.LoadDependencies(loaded);

            Assert.IsTrue(pcc.ResolveLinks());

            stream.Close();
        }
    }
}
