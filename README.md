# MsbtLib

A port of ascclemens's msbt-rs library to C#.

Currently, only UTF16 is supported, and only for labels, attributes, and texts.
The library was written using Legend of Zelda: Breath of the Wild's MSBT files.
Any other support would require MSBT files from other games for analysis.

## Usage

### Reading

To obtain an MSBT object, pass the constructor an open stream or byte array.

#### From a FileStream

```
using FileStream f_stream = File.OpenRead("C:/extracted_msbts/Msg_USen.product/ActorType/ArmorHead.msbt");
MSBT msbt = new(f_stream);
```

#### From a byte array from a [SarcFile](https://github.com/ArchLeaders/NCF-Library/blob/master/SarcLibrary/SarcFile.cs)

```
using Nintendo.Sarc;
using Yaz0Library;
...

SarcFile bootup = new(File.ReadAllBytes("C:/BOTW/Update/content/Pack/Bootup_USen.pack"));
SarcFile lang_pack = new(Yaz0.Decompress(bootup.Files["Msg_USen.product.ssarc"]));
MSBT msbt = new(lang_pack.Files["ActorType/ArmorHead.msbt"]);
```

### Editing

Once you've opened an MSBT, you can edit and write to it:

```
Dictionary<string, MsbtEntry> texts = msbt.GetTexts();
texts["Armor_001_Head_Name"].Attribute = "NewHylianHoodAttribute"; // Setting an attribute
texts["Armor_001_Head_Name"].Value = "New Hylian Hood Name"; // Setting localized text
texts["Armor_9001_Head_Name"] = new("", "New Head Armor Name"); // Adding a new entry
msbt.SetTexts(Texts);
```

NOTE: All BotW MSBTs have three sections: labels, attributes, texts. Setting an attribute 
will work for BotW even if another library/converter says the file does not have attributes.

NOTE 2: Other games' MSBTs may not have the attribute section. If you try to set an attribute 
other than the empty string ("") for an MSBT without attributes, an error will be thrown.

#### Controls

Text controls are implemented in MsbtLib using pseudo-HTML tags. Localized text contains 
these tags in-line, for example:

```
// with texts from ActorType/ArmorHead.msbt
Console.WriteLine(texts["Armor_063_Head_Desc"]);

/*
* Output:
* 
* Zora headgear made from dragon scales.
* Increases swimming speed and allows you to
* <color=Blue>spin</color> to attack underwater. A Great Fairy has
* increased its defense by three levels.
*/
```

Valid tags are:
* animation - `<animation=[name] />`
* auto advance - `<auto_advance=[num_frames] />`
* one choice - `<choice1=[uint16] />`†
* two choice - `<choice2 1=[uint16] 2=[uint16] cancel=[index] />`†
* three choice - `<choice3 1=[uint16] 2=[uint16] 3=[uint16] cancel=[index] >`†
* four choice - `<choice4 1=[uint16] 2=[uint16] 3=[uint16] 4=[uint16] cancel=[index] >`†
* font - `<font=[face] />` - Only `Normal` and `Hylian`
* icon - `<icon=[character] />` - Some characters require numbers, e.g. `A(10)`
* pause for a number of frames - `<pauseframes=[num_frames] />`
* pause for a duration - `<pauselength=[duration] />` - Only `Short`, `Long`, or `Longer`
* set text color - `<color=[color]>` - Only `Red`, `LightGreen1`, `Blue`, `Grey`,
    `LightGreen4`, `Orange`, or `LightGrey`
* reset text color to default - `</color>`
* sound - `<sound field_1=[uint8] field_2=[uint8] />`‡
* another sound type - `<sound2=[uint8] />`‡
* text size - `<textsize percent=[num] />` - Be sure to `<textsize percent=100 />` to reset
* variable - `<variable kind=[uint16] name=[name] />`† - `name` must correspond to a variable
    name in the executable

† - It is currently unknown how the game uses these `uint16`s  
‡ - It is currently unknown how the game uses these `uint8`s

### Writing

To write an MSBT file, use the Write() method. There are two options:

#### To a file

```
msbt.Write("C:/extracted_msbts/Msg_USen.product/ActorType/ArmorHead_edited.msbt");
```

#### To a byte array

Continuing from the "Reading from a byte array" example

```
using Nintendo.Sarc;
using Yaz0Library;
...

lang_pack.Files["ActorType/ArmorHead.msbt"] = msbt.Write();
bootup.Files["Msg_USen.product.ssarc"] = Yaz0.Compress(lang_pack.ToBinary());
File.WriteAllBytes(bootup.ToBinary());
```

## Contributing

-   Issues: <https://github.com/GingerAvalanche/MsbtLib/issues>
-   Source: <https://github.com/GingerAvalanche/MsbtLib>

Currently, only BotW is extensively supported, as I do not have MSBT files from 
other games to analyze. If you find that MsbtLib doesn't work for another game, 
please consider making a pull request, or working with me to determine the 
structure of your game's MSBT files so that I can do it.

## License

This software is licensed under the terms of the GNU General Public License, version 3
or later. The source is publicly available on [GitHub](https://github.com/GingerAvalanche/MsbtLib). 
The License is included in the source.
