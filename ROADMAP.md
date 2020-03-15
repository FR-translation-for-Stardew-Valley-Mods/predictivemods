# Roadmap

Read this document to peer into the future of these very mods! Spooky.

## PredictiveCore

### Mining

* ListFloorsForDate
* FindMushroomFloors
* FindDinosaurFloors

### Shopping

* ListSalesForDate
* ListKrobusForDate
* ListPierreForDate
* ListSandyForDate
* ListTravelingCartForDate
* FindItemByShopping

### Tailoring

* ChooseRandomClothing
* FindItemByTailoring

### ItemFinder

* FindItem
* FindItemByCombat
* FindItemByCooking
* FindItemByCrafting
* FindItemByFarming
* FindItemByFishing
* FindItemByForaging
* FindItemBySpecial

#### Supporting methods

* Garbage.FindItemInGarbage
* Geodes.FindItemInGeodes
* NightEvents.FindItemByNightEvents
* Mining.FindItemByMining

## PublicAccessTV

* Add introductory events/dialogue/mail/etc. for hosts:
	* Demetrius
	* Emily
	* Lewis
	* Linus
	* Marlon

### MiningChannel

* name: "Mining Advisory"
* hosts: Marlon and Gil
* conditions:
	* "Initiation" quest completed
	* SVE only: also two hearts with Marlon
* content:
	* monster, slime and quarry infestations
	* mushroom floors (all, if 80th floor reached)
	* Skull Cavern dinosaur floors (up to N, if Skull Cavern reached)

### ShoppingChannel

* name: "Shop the Valley"
* host: Lewis
* condition: two hearts with Lewis
* content:
	* today's wallpaper and flooring at Pierre's General Store
	* today's shirt at Sandy's Oasis (if bus repaired)
	* today's/this week's items at the Traveling Cart

### TailoringChannel

* name: "Fashion Showcase"
* hosts: Emily and Haley
* conditions:
	* sewing machine access
	* two hearts with Emily
* content:
	* random tailoring recipe
	* fashion commentary from Haley (connect to mods like Seasonal Villager Outfits?)
 
### Future channel ideas

* ArtsChannel (hosts: Elliott, Evelyn, Leah)
* CraftingChannel (hosts: Maru, Robin)
* DesertChannel (hosts: Pam, Sandy)
* HistoryChannel (hosts: Gunther, Penny)
* MusicChannel (hosts: Abigail, Sam, Sebastian)
* RanchingChannel (hosts: Marnie, Shane)
* WellnessChannel (hosts: Alex, Caroline, Harvey)
* Channels not planned: ~~GeodesChannel~~ (who would know this in advance?), ~~ItemFinderChannel~~ (wouldn't fit with TV interface)

## ScryingOrb

* Add MetaExperience for offering a Scrying Orb to the Scrying Orb ;)
* LuckyPurpleExperience: on second attempt, reduce daily luck
* GarbageExperience: randomize list order
* Experiences not planned: ~~MoviesExperience~~ (why would the spirits care?), ~~TailoringExperience~~ (no future component), ~~TrainsExperience~~ (Demetrius already knows the future schedule)

### MiningExperience

* criterion: 1st floor reached
* offering: Copper Ore, Iron Ore, Gold Ore, Iridium Ore or Coal
* Specific Day
	* input: date
	* content: as for "Mining Advisory"
* Upcoming Mushroom Floors
	* criterion: 80th floor reached
	* content: days (within next week) and floors when mushrooms may be present
* Upcoming Dinosaur Floors
	* criterion: Skull Cavern reached
	* content: days (within next week) and floors (within those explored/first 50) when dinosaurs may be present

### ShoppingExperience

* offering categories: Fruit, Vegetable or Forage
* offering exceptions: Sap, Salmonberry, Blackberry or Spring Onion
* input: date
* content: as for "Shopper's Report", plus Krobus's Fish/Cooking item (if Krobus met)

### ItemFinderExperience

* offering categories: Animal Product, Artisan Goods, Cooking or Flower
* offering exceptions: Fried Egg, Magic Rock Candy, Salmonberry Wine, Tulip, Void Egg or Void Mayonnaise
* criterion: any item (needs UI; ref. CJB Item Spawner?)
* content:
	* days, stores and prices when item will be sold
	* numbers and types of upcoming geodes to contain item
	* days and garbage cans when item will be trashed
	* cooking, crafting and tailoring recipes for item
	* mining, combat, foraging and fishing drops of item
	* farming of item from seeds/trees/animals
	* special ways of getting item
