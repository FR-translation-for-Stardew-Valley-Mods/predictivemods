# Roadmap

Read this document to peer into the future of these very mods! Spooky.

## Version 1.0.0 release

* Check on balancing
* Additional testing: Windows/Mac? other common mods? multiplayer?
* Publish Nexus pages
* Create ModDrop pages
* Wait for CustomTV second page fix to release in PyTK
* Announce in places

---

## Shopping channel/experience

### PredictiveCore.Shopping

* ListSalesForDate
* ListKrobusForDate
* ListPierreForDate
* ListSandyForDate
* ListTravelingCartForDate

### PublicAccessTV.ShoppingChannel

* name: "Shop the Valley"
* host: Lewis
* condition: two hearts with Lewis
* introductory event: TBD
* content:
	* today's wallpaper and flooring at Pierre's General Store
	* today's shirt at Sandy's Oasis (if bus repaired)
	* today's items at the Traveling Cart

### ScryingOrb.ShoppingExperience

* offering categories: Fruit, Vegetable or Forage
* offering exceptions: Sap, Salmonberry, Blackberry or Spring Onion
* input: date
* content: as for "Shop the Valley", plus Krobus's Fish/Cooking item (if Krobus met)

## Tailoring channel

### PredictiveCore.Tailoring

* ChooseRandomClothing

### PublicAccessTV.TailoringChannel

* name: "Fashion Showcase"
* hosts: Emily and Haley
* conditions:
	* sewing machine access
	* two hearts with Emily
* introductory event: TBD
* content:
	* random tailoring recipe
	* fashion commentary from Haley (connect to mods like Seasonal Villager Outfits?)

## Version 1.1.0 release

---

## Item Finder experience

### PredictiveCore.ItemFinder

* FindItem
* FindItemByCombat
* FindItemByCooking
* FindItemByCrafting
* FindItemByFarming
* FindItemByFishing
* FindItemByForaging
* FindItemBySpecial

### PredictiveCore supporting methods

* Garbage.FindItemInGarbage
* Geodes.FindItemInGeodes
* NightEvents.FindItemByNightEvents
* Mining.FindItemByMining
* Shopping.FindItemByShopping
* Tailoring.FindItemByTailoring

### ScryingOrb.ItemFinderExperience

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

## Version 1.2.0 release

---

## Future ideas

### Public Access TV channels (non-predictive)

* Arts (hosts: Elliott, Evelyn, Leah)
* Crafting (hosts: Maru, Robin)
* Desert (hosts: Pam, Sandy)
* History (hosts: Gunther, Penny)
* Music (hosts: Abigail, Sam, Sebastian)
* Ranching (hosts: Marnie, Shane)
* Wellness (hosts: Alex, Caroline, Harvey)

## Not planned

### Public Access TV channels

* ~~GeodesChannel~~ (who would know this in advance?)
* ~~ItemFinderChannel~~ (wouldn't fit with TV interface)

### Scrying Orb experiences

* ~~MoviesExperience~~ (why would the spirits care?)
* ~~TailoringExperience~~ (no future component)
* ~~TrainsExperience~~ (Demetrius already knows the future schedule)
