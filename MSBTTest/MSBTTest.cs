using Microsoft.Extensions.FileSystemGlobbing;
using MsbtLib;
using System.Diagnostics;

namespace MSBTTests
{
    [TestClass]
    public class MSBTTest
    {
        [TestMethod]
        public void ReadAndTestMSBT_NoChanges()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Assert.IsNotNull(texts);
            Assert.AreEqual(334, texts.Count);
            Assert.AreEqual("Zora Helm", texts["Armor_063_Head_Name"].Value);
        }
        [TestMethod]
        public void ReadAndTestMSBT_NoChanges_2()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorUpper.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            texts.Where(kvp => kvp.Key.StartsWith("Armor_5")).Select(kvp => kvp.Value.Value).ToList().ForEach(s => Trace.WriteLine(s));
        }
        [TestMethod]
        public void ReadAndLogMSBT_Color()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["Armor_063_Head_Desc"].Value);
        }
        [TestMethod]
        public void ReadAndLogMSBT_Icon()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\LayoutMsg\MessageTipsPauseMenu_00.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["0001"].Value);
        }
        [TestMethod]
        public void ReadAndLogMSBT_AutoAdvance()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\DemoMsg\Demo006_0.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["Talk00"].Value);
        }
        [TestMethod]
        public void ReadAndLogMSBT_Variable()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\Item.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["Obj_ArrowBundle_A_10_Name"].Value);
            msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\EventFlowMsg\Npc_AncientAssistant001.msbt", FileMode.Open, FileAccess.Read));
            texts = msbt.GetTexts();
            Trace.WriteLine(texts["Com_Talk_10"].Value);
        }
        [TestMethod]
        public void ReadAndLogMSBT_PausesAndSound()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\EventFlowMsg\BalladOfHeroZora.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["Npc_Zora003_SD_000"].Value);
        }
        [TestMethod]
        public void ReadAndLogMSBT_PausesAndSoundAndColorAndTextSizeAndChoice()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\EventFlowMsg\BalladOfHeroZora.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            Trace.WriteLine(texts["Npc_ZoraB001_SD_000"].Value);
        }
        [TestMethod]
        public void GetAndSetTexts_NoChanges()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            msbt.SetTexts(texts);
            texts = msbt.GetTexts();
            Assert.IsNotNull(texts);
            Assert.AreEqual(334, texts.Count);
            Assert.AreEqual("Zora Helm", texts["Armor_063_Head_Name"].Value);
        }
        [TestMethod]
        public void ReadAndWriteMSBT_NoChanges()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            msbt.Write(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead_written.msbt");
            msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead_written.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            msbt.SetTexts(texts);
            texts = msbt.GetTexts();
            Assert.IsNotNull(texts);
            Assert.AreEqual(334, texts.Count);
            Assert.AreEqual("Zora Helm", texts["Armor_063_Head_Name"].Value);
        }
        [TestMethod]
        public void ReadAndWriteMSBT_GetAndSetTexts_NoChanges()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts1 = msbt.GetTexts();
            Assert.IsNotNull(texts1);
            Assert.AreEqual(334, texts1.Count);
            msbt.SetTexts(texts1);
            msbt.Write(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead_written.msbt");
            msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\ActorType\ArmorHead_written.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts2 = msbt.GetTexts();
            Assert.IsNotNull(texts2);
            Assert.AreEqual(334, texts2.Count);
            foreach (string key in texts1.Keys)
            {
                Assert.AreEqual(texts1[key].Attribute, texts2[key].Attribute);
                Assert.AreEqual(texts1[key].Value, texts2[key].Value);
            }
            foreach (string key in texts2.Keys)
            {
                Assert.AreEqual(texts1[key].Attribute, texts2[key].Attribute);
                Assert.AreEqual(texts1[key].Value, texts2[key].Value);
            }
        }
        [TestMethod]
        public void ReadAndWriteMSBT_GetAndSetTexts_AddOne()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\DemoMsg\Demo006_0.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            texts["Talk01"] = new("", "Okay, actually, that was kind of mean. I'm sorry.<auto_advance=30 />");
            msbt.SetTexts(texts);
            msbt.Write(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\DemoMsg\Demo006_0_written.msbt");
            msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\DemoMsg\Demo006_0_written.msbt", FileMode.Open, FileAccess.Read));
            texts = msbt.GetTexts();
            Trace.WriteLine(texts["Talk01"].Value);
        }
        [TestMethod]
        public void MakeAndWriteMSBT()
        {
            Msbt msbt = new(Endianness.Big, UtfEncoding.Utf16);
            msbt.CreateLbl1();
            msbt.CreateAtr1();
            msbt.CreateTxt2();
            Dictionary<string, MsbtEntry> texts = new()
            {
                ["Talk00"] = new("", "Hey CEO!<auto_advance=30 />"),
                ["Talk01"] = new("", "This MSBT was made from scratch.<auto_advance=30 />"),
                ["Talk02"] = new("", "Not a single byte in this file was loaded from an existing MSBT file.<auto_advance=30 />"),
                ["Talk03"] = new("Random_Attribute", "There <color=Red>shouldn't</color> be anything wrong with the <color=Blue>ATR1 section</color>, but you <textsize percent=125 />never know!<textsize percent=100 /><auto_advance=30 />"),
                ["Talk04"] = new("Another_Random_Attribute", "Mostly because we... y'know, haven't figured out what all the bits mean, heh.<auto_advance=30 />"),
                ["Talk05"] = new("", "Anyway, this is the <color=Red>end</color> of my story. No more auto advancing."),
            };
            msbt.SetTexts(texts);
            msbt.Write(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\DemoMsg\Demo006_0_fromscratch.msbt");
        }
        [TestMethod]
        public void TestAllMSBTs()
        {
            Matcher matcher = new();
            matcher.AddInclude("**/*.msbt");
            IEnumerable<string> files = matcher.GetResultsInFullPath(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\");
            foreach (string file in files)
            {
                Msbt msbt = new(File.ReadAllBytes(file));
                Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
                Assert.IsNotNull(texts);
            }
        }
        [TestMethod]
        public void CheckAllMSBTsForKey5502()
        {
            Matcher matcher = new();
            matcher.AddInclude("**/*.msbt");
            IEnumerable<string> files = matcher.GetResultsInFullPath(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\");
            foreach (string file in files)
            {
                Msbt msbt = new(File.ReadAllBytes(file));
                Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
                if (texts.ContainsKey("5502"))
                {
                    Trace.WriteLine(Path.GetFileName(file));
                    Trace.WriteLine(texts["5502"].Value);
                }
            }
        }
        [TestMethod]
        public void TestZora003_Talk08()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\EventFlowMsg\Npc_Zora003.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
            msbt.SetTexts(texts);
            File.WriteAllBytes(@"E:\Users\chodn\Documents\CemuShit\botw2.0\Msg_USen.product\EventFlowMsg\Npc_Zora003_backup.msbt", msbt.Write());
        }
        [TestMethod]
        public void TestTotkMsbt()
        {
            Msbt msbt = new(File.Open(@"E:\Users\chodn\Documents\ISOs - Switch\InstantTips_00.msbt", FileMode.Open, FileAccess.Read));
        }
    }
}