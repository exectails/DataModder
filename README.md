DataModder
=============================================================================

DataModder is a tool to create a modded Mabinogi data folder, based on mod
scripts written in Lua. It allows the user to create and update data mods
without having to extract and modify the files themselves. It was designed
for XML file modding in particular, allowing the modification of attributes
of specific XML elements.

How it works
-----------------------------------------------------------------------------

DataModder looks for mods in its mod folder (DataModder/mods/). Each mod
needs its own folder and a main.lua, which serves as the entry point.
In each of those scripts any number of modifications can be specified,
which are then executed by the program.

For all modifications it automatically uses the latest version of the file
from the pack files.

Examples
-----------------------------------------------------------------------------

### XML files

The most special feature of DataModder is the ability to modify XML files
mostly without having to manually write and update XML code. You can modify
and remove attributes, remove elements, or add completely new ones.

The selection of elements for whatever purpose utilizes XPath, a standardized
query language for selecting nodes in XML documents. For more information
check [Wikipedia](https://en.wikipedia.org/wiki/XPath).

#### Attributes

The following example modifies the attribute "Text_Name0" on a collection
of items that match the path to "Just some book". 

```lua
loadxmlfile('db/itemdb.xml')
setattributes('//Mabi_Item[@ID=1000]', 'Text_Name0', 'Just some book')
```

The same code can be used to add attributes if they don't exist yet, and if
the value is `nil` the attribute is removed.

#### Removing elements

The following example removes all `Mabi_Item` elements in the itemdb.xml
that match the given path.

```lua
loadxmlfile('db/itemdb.xml')
removeelements('//Mabi_Item[@ID=221066]')
```

#### Adding elements

To add a new element you simply specifiy the XML code and where it should
be added. In the example below it's added at the end of the `Items` node,
which is the parent of all `Mabi_Item` elements in the itemdb.xml.

```lua
loadxmlfile('db/itemdb.xml')
addelement('/Items', '<Mabi_Item ID="424242" Name="test" />')
```

### Replacing Files

Is XML modification not viable, or the file in question not an XML file,
you can replace files either with other files inside the pack files or new
ones inside the mod folder.

The first parameter is the path the file will be copied to in the data
folder, the second is the path it's copied from. DataModder first checks
the mod's folder, and if the file doesn't exist there, it tries to find
it in the pack folder.

```lua
-- Replace a file with a new file in the mod's folder
replacefile('db/cutscene/cutscene_waterfall.xml', 'my_improved_waterfall.xml')

-- Replace an image with a different one from the packs
replacefile('gfx/image/npc/npcportrait_manus_us.dds', 'gfx/image/npc/npcportrait_manus.dds')
```

### Organization

To organize mods a little you can use `include` and/or `require`, which can
be used to include scripts from the same mod folder.

```lua
-- main.lua

include('item_mods.lua')
include('url_mods.lua')
include('ebil_hack_mods.lua')
```

### More

For more examples, check the examples folder (DataModder/examples/).

Links
-----------------------------------------------------------------------------
* GitHub: https://github.com/exectails/DataModder
