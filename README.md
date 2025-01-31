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

#### From a byte array

Example: from a [SarcFile](https://github.com/ArchLeaders/NCF-Library/blob/master/SarcLibrary/SarcFile.cs)

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
Console.WriteLine(texts["Armor_063_Head_Desc"].Value);

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
* Ruby = `<Ruby 0=[uint16] 1=[uint16] 2=[uint16] />`*
* Font - `<Font='[face]'>` - Only `Normal` and `Hylian`
* Reset font - `</Font>` - Alternative to `<Font='Normal'>`
* Font size - `<FontSize percent=[num] />` - Be sure to `<textsize percent=100 />` to reset
* Set text color - `<Color=[color]>` - Only `Red`, `Green`, `Cyan`, `Grey`, `Azure`, `Orange`, or `Gold`
* Reset text color to default - `</Color>`
* Page break - `<br />` or `<br>` - breaks to a clean dialogue statement, like using \n multiple times
* Pause for a number of frames - `<Delay=[num_frames] />`
* Text speed - `<TextSpeed=[uint32] />`*
* No scroll - `<NoScroll />`*
* Auto advance - `<AutoAdvance=[num_frames] />`
* Two choice - `<Choice2 0=[key] 1=[key] cancel=[index] />`†
* Three choice - `<Choice3 0=[key] 1=[key] 2=[key] cancel=[index] />`†
* Four choice - `<Choice4 0=[key] 1=[key] 2=[key] 3=[key] cancel=[index] />`†
* Icon - `<Icon=[character] />` - Some characters require numbers, e.g. `A(10)`
* Three choice on flag - `<Choice3OnFlag type=[type](#) name0='[name]' resp0=[key] name1='[name]' resp1=[key] name2='[name]' resp2=[key] default=[index] cancel=[index] />`
* Five flags - Too long to put here, and not well understood. See an example in an existing file.
* One choice - `<Choice1=[key] />`†
* Variable - `<Variable kind=[uint16] name='[name]' />`♠ - `name` must correspond to a variable name in the game flags
* Sound - `<Sound field_1=[uint8] field_2=[uint8] />`‡
* FourZero - `<FourZero str='[name]' />`*
* Another sound type - `<Sound2=[uint8] />`‡
* Animation - `<Animation='[name]' />`
* FourThree - `<FourThree />`*
* Pause for a duration - `<PauseLength=[duration] />` - Only `Short`, `Long`, or `Longer`
* Info - `<Info gender=[uint8] definite=[uint8] indefinite=[uint8] plural=[uint8] />` - These u8s correspond to GrammarArticle.msbt
* Definite - `<Definite />` - Inserts a language-based definite article based on an Info
* Indefinite - `<Indefinite />` - Inserts a language-based indefinite article based on an Info
* Capitalize - `<Capitalize />` - Capitalizes the next letter, often used with Variable
* Downcase - `<Downcase />` - Downcases the next letter, often used with Variable
* Gender - `<Gender masc='[str]' femme='[str]' neut='[str]' />` - Direct strings to be used for each gender phrasing
* Pluralize - `<Pluralize 0='[str]' 1='[str]' 2='[str]' />` - Direct strings to be used for one, multiple, or many, used with Variable
* LongVowel - `<LongVowel 0='[str]' 1='[str]' />` - Used for Korean*
* LongVowel2 - `<LongVowel2 0='[str]' 1='[str]' />` - Used for Korean*

\* - These tags are not well understood at this time  
† - These `key`s are keys for other localized strings. Normally those keys are read from 
the same MSBT, but in the case of shops, they are read from the shop NPC's MSBT. The keys 
are read as `%04d`-formatted strings, e.g. 4 is read as "0004". The `index`es are for which 
choice represents a cancellation.  
‡ - It is currently unknown how the game uses these `uint8`s  
♠ - It is currently unknown how the game uses these `uint16`s

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
