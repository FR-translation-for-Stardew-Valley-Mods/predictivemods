# Roadmap

Read this document to peer into the future of these very mods! Spooky.

## PredictiveCore

### Shopping

* ListSalesForDate
* ListKrobusForDate
* ListPierreForDate
* ListSandyForDate
* ListTravelingCartForDate

### Tailoring

* ChooseRandomClothing

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
* Shopping.FindItemByShopping
* Tailoring.FindItemByTailoring

## PublicAccessTV

* Add introductory events/dialogue/mail/etc. for hosts:
	* Trains/Demetrius
	* Tailoring/Emily
	* Shopping/Lewis
	* Garbage/Linus
	* Mining/Marlon

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

### Channels not planned

* ~~GeodesChannel~~ (who would know this in advance?)
* ~~ItemFinderChannel~~ (wouldn't fit with TV interface)

## ScryingOrb

### ShoppingExperience

* offering categories: Fruit, Vegetable or Forage
* offering exceptions: Sap, Salmonberry, Blackberry or Spring Onion
* input: date
* content: as for "Shop the Valley", plus Krobus's Fish/Cooking item (if Krobus met)

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

### Experiences not planned

* ~~MoviesExperience~~ (why would the spirits care?)
* ~~TailoringExperience~~ (no future component)
* ~~TrainsExperience~~ (Demetrius already knows the future schedule)
