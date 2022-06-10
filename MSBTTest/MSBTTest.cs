using MsbtLib;
using System.Diagnostics;

namespace MSBTTests
{
    [TestClass]
    public class MSBTTest
    {
        [TestMethod]
        public void ReadAndLogMSBT_NoChanges()
        {
            MSBT msbt = new(File.Open("E:\\Users\\chodn\\Documents\\CemuShit\\ArmorHead.msbt", FileMode.Open, FileAccess.Read));
            Dictionary<string, string> texts = msbt.GetTexts();
            Assert.IsNotNull(texts);
            Assert.AreEqual(334, texts.Count);
            Assert.AreEqual("Zora Helm", texts["Armor_063_Head_Name"]);
            Trace.WriteLine(texts["Armor_063_Head_Desc"]);
        }
    }
}