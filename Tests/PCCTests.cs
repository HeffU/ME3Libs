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
            Assert.IsTrue(corepcc.DeserializeTables());
            Assert.IsTrue(corepcc.DeserializeObjects());
            corepcc.ResolveLinks();
            loaded.Add(corepcc);

            var enginepath = @"G:\Code\ME3\temp\Engine.pcc";
            var enginestream = new FileStream(enginepath, FileMode.Open);
            var enginepcc = new PCCFile(new PCCStreamReader(enginestream), "Engine");
            Assert.IsTrue(enginepcc.DeserializeTables());
            Assert.IsTrue(enginepcc.DeserializeObjects());
            Assert.IsTrue(enginepcc.ResolveLinks());
            enginepcc.LoadDependencies(loaded);
            loaded.Add(enginepcc);

            var frameworkpath = @"G:\Code\ME3\temp\GameFramework.pcc";
            var frameworkstream = new FileStream(frameworkpath, FileMode.Open);
            var frameworkpcc = new PCCFile(new PCCStreamReader(frameworkstream), "GameFramework");
            Assert.IsTrue(frameworkpcc.DeserializeTables());
            Assert.IsTrue(frameworkpcc.DeserializeObjects());
            Assert.IsTrue(frameworkpcc.ResolveLinks());
            frameworkpcc.LoadDependencies(loaded);
            loaded.Add(frameworkpcc);

            var onlinepath = @"G:\Code\ME3\temp\SFXOnlineFoundation.pcc";
            var onlinestream = new FileStream(onlinepath, FileMode.Open);
            var onlinepcc = new PCCFile(new PCCStreamReader(onlinestream), "SFXOnlineFoundation");
            Assert.IsTrue(onlinepcc.DeserializeTables());
            Assert.IsTrue(onlinepcc.DeserializeObjects());
            Assert.IsTrue(onlinepcc.ResolveLinks());
            onlinepcc.LoadDependencies(loaded);
            loaded.Add(onlinepcc);

            var uipath = @"G:\Code\ME3\temp\GFxUI.pcc";
            var uistream = new FileStream(uipath, FileMode.Open);
            var uipcc = new PCCFile(new PCCStreamReader(uistream), "GFxUI");
            Assert.IsTrue(uipcc.DeserializeTables());
            Assert.IsTrue(uipcc.DeserializeObjects());
            Assert.IsTrue(uipcc.ResolveLinks());
            uipcc.LoadDependencies(loaded);
            loaded.Add(uipcc);

            var wwisepath = @"G:\Code\ME3\temp\WwiseAudio.pcc";
            var wwisestream = new FileStream(wwisepath, FileMode.Open);
            var wwisepcc = new PCCFile(new PCCStreamReader(wwisestream), "WwiseAudio");
            Assert.IsTrue(wwisepcc.DeserializeTables());
            Assert.IsTrue(wwisepcc.DeserializeObjects());
            Assert.IsTrue(wwisepcc.ResolveLinks());
            wwisepcc.LoadDependencies(loaded);
            loaded.Add(wwisepcc);

            var SFXGamepath = @"G:\Code\ME3\temp\SFXGame.pcc";
            var SFXGamestream = new FileStream(SFXGamepath, FileMode.Open);
            var SFXGamepcc = new PCCFile(new PCCStreamReader(SFXGamestream), "SFXGame");
            Assert.IsTrue(SFXGamepcc.DeserializeTables());
            Assert.IsTrue(SFXGamepcc.DeserializeObjects());
            Assert.IsTrue(enginepcc.ResolveLinks());
            SFXGamepcc.LoadDependencies(loaded);
            loaded.Add(SFXGamepcc);

            var path = @"G:\Code\ME3\temp\SFXWeapon_Pistol_Carnifex.pcc";
            var stream = new FileStream(path, FileMode.Open);
            var pcc = new PCCFile(new PCCStreamReader(stream), "SFXWeapon_Pistol_Carnifex");

            Assert.IsTrue(pcc.DeserializeTables());
            Assert.IsTrue(pcc.DeserializeObjects());
            var deps = pcc.ImportPackageNames;

            foreach (var dep in deps.Where(x => !loaded.Any(p => p.Name == x)))
            {
                var depPath = @"G:\Code\ME3\temp\" + dep + ".pcc";
                var depStream = new FileStream(depPath, FileMode.Open);
                var depPCC = new PCCFile(new PCCStreamReader(depStream), dep);
                Assert.IsTrue(depPCC.DeserializeTables());
                Assert.IsTrue(depPCC.DeserializeObjects());
                loaded.Add(depPCC);
            }

            pcc.LoadDependencies(loaded);

            Assert.IsTrue(pcc.ResolveLinks());

            stream.Close();
        }
    }
}
