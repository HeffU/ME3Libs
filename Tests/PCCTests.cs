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
            corepcc.ResolveLinks();
            loaded.Add(corepcc);

            var enginepath = @"G:\Code\ME3\temp\Engine.pcc";
            var enginestream = new FileStream(enginepath, FileMode.Open);
            var enginepcc = new PCCFile(new PCCStreamReader(enginestream), "Engine");
            enginepcc.Deserialize();
            enginepcc.ResolveLinks();
            enginepcc.LoadDependencies(loaded);
            loaded.Add(enginepcc);

            var frameworkpath = @"G:\Code\ME3\temp\GameFramework.pcc";
            var frameworkstream = new FileStream(frameworkpath, FileMode.Open);
            var frameworkpcc = new PCCFile(new PCCStreamReader(frameworkstream), "GameFramework");
            frameworkpcc.Deserialize();
            frameworkpcc.ResolveLinks();
            frameworkpcc.LoadDependencies(loaded);
            loaded.Add(frameworkpcc);

            var onlinepath = @"G:\Code\ME3\temp\SFXOnlineFoundation.pcc";
            var onlinestream = new FileStream(onlinepath, FileMode.Open);
            var onlinepcc = new PCCFile(new PCCStreamReader(onlinestream), "SFXOnlineFoundation");
            onlinepcc.Deserialize();
            onlinepcc.ResolveLinks();
            onlinepcc.LoadDependencies(loaded);
            loaded.Add(onlinepcc);

            var uipath = @"G:\Code\ME3\temp\GFxUI.pcc";
            var uistream = new FileStream(uipath, FileMode.Open);
            var uipcc = new PCCFile(new PCCStreamReader(uistream), "GFxUI");
            uipcc.Deserialize();
            uipcc.ResolveLinks();
            uipcc.LoadDependencies(loaded);
            loaded.Add(uipcc);

            var wwisepath = @"G:\Code\ME3\temp\WwiseAudio.pcc";
            var wwisestream = new FileStream(wwisepath, FileMode.Open);
            var wwisepcc = new PCCFile(new PCCStreamReader(wwisestream), "WwiseAudio");
            wwisepcc.Deserialize();
            wwisepcc.ResolveLinks();
            wwisepcc.LoadDependencies(loaded);
            loaded.Add(wwisepcc);

            var SFXGamepath = @"G:\Code\ME3\temp\SFXGame.pcc";
            var SFXGamestream = new FileStream(SFXGamepath, FileMode.Open);
            var SFXGamepcc = new PCCFile(new PCCStreamReader(SFXGamestream), "SFXGame");
            SFXGamepcc.Deserialize();
            enginepcc.ResolveLinks();
            SFXGamepcc.LoadDependencies(loaded);
            loaded.Add(SFXGamepcc);

            var path = @"G:\Code\ME3\temp\SFXWeapon_Pistol_Carnifex.pcc";
            var stream = new FileStream(path, FileMode.Open);
            var pcc = new PCCFile(new PCCStreamReader(stream), "SFXWeapon_Pistol_Carnifex");

            Assert.IsTrue(pcc.Deserialize());
            var deps = pcc.ImportPackageNames;

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
