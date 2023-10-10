# TheLastBroadcast

Streaming JSON File Locations:
./Assets/StreamingAssets/*

These files are all used as the defaults for instantiating save data for new games
save.json - Current scene, max health, max charge, collectible counts, unlocked abilities, unlocked radio powers
items.json - holds all item data; loaded on game launch for reference when the player picks up a specific item by checking the ID value
lore.json - (Currently unused in working project) Stores all lore text values and dynamically loads them into the save data manager at launch for reference when player finds a lore item (works similarly to items.json)


Item Atlas:
./Assets/Materials/Inventory/*

Inventory Atlas points to the Items_HiRes folder. Any asset updates must be added to this folder and match the corresponding name in the items.json file (will need to change this to an ID value going forward). Once an asset has been added/changed, the atlas MUST be packaged again to use the latest version of files in the folder. All item assets should use Mesh Type > Full Rect to ensure no overlapping at run time. 

The preferred sprite size should be 256x256 but can be modified in the editor to fit into the atlas.
