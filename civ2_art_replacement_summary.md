# Civ2 Art Replacement Summary

This inventory compares the classic rules data and hard-coded original asset buckets against `RaylibUI/FOSSart`. Filename mismatches are counted as covered when a clear mapping exists, with notes in the CSV.

## Coverage Counts

| Category | Covered | Missing |
|---|---:|---:|
| Advance | 89 | 11 |
| Improvement | 34 | 4 |
| Original Asset Bucket | 0 | 20 |
| Special Resource | 20 | 2 |
| Terrain | 10 | 1 |
| Unit | 47 | 15 |
| Wonder | 28 | 0 |

## Missing Items

### Advance
- Plumbing
- User Def Tech A - User/extra slots can be left blank until used by scenarios.
- User Def Tech B - User/extra slots can be left blank until used by scenarios.
- User Def Tech C - User/extra slots can be left blank until used by scenarios.
- Extra Advance 1 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 2 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 3 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 4 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 5 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 6 - User/extra slots can be left blank until used by scenarios.
- Extra Advance 7 - User/extra slots can be left blank until used by scenarios.

### Improvement
- SS Structural - Spaceship/capitalization entries need explicit art decisions.
- SS Component - Spaceship/capitalization entries need explicit art decisions.
- SS Module - Spaceship/capitalization entries need explicit art decisions.
- (Capitalization) - Spaceship/capitalization entries need explicit art decisions.

### Original Asset Bucket
- Road connection masks - Need scalable road overlays for 9 connection masks.
- Railroad connection masks - Need scalable railroad overlays for 9 connection masks.
- River masks - Need scalable river overlays for 16 masks plus 4 river mouths.
- Coastline masks - Need scalable coastline overlays for 32 mini coast pieces.
- Forest variation masks - Need 16 forest edge/variant tiles, or a compositing strategy.
- Hill variation masks - Need 16 hill edge/variant tiles, or a compositing strategy.
- Mountain variation masks - Need 16 mountain edge/variant tiles, or a compositing strategy.
- Irrigation overlay - Need map overlay art.
- Farmland overlay - Need map overlay art.
- Mine overlay - Need map overlay art.
- Pollution overlay - Need map overlay art.
- Goody hut - Need map overlay art.
- City sprites - Need city style/size/walled/capital variants.
- Unit shields and flags - Need owner-colorizable shields, shield backs, HP shields, flags.
- Battle animation frames - Need 8 replacement combat animation frames.
- Advisor backgrounds - Need replacements for city report, defense, attitude, trade, science, world wonders.
- Throne room layers - Need replacements for pv.dll throne room layers and section masks.
- Civilopedia terrain backgrounds - Need replacements for cv.dll ocean/river/continent/improvement pages.
- Dialog/startup illustrations - Need replacements for Intro.dll scenario/new-game pictures.
- UI chrome and icons - Need buttons, close/zoom, gold/science/trade, food/shield/trade, grid/view icons, wallpaper/borders.

### Special Resource
- Peat
- Wheat

### Terrain
- Forest

### Unit
- Archers
- Legion
- Mech. Inf.
- AEGIS Cruiser
- Extra Land - Extra/test slots need art only if enabled by a scenario/ruleset.
- Extra Ship - Extra/test slots need art only if enabled by a scenario/ruleset.
- Extra Air - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 1 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 2 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 3 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 4 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 5 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 6 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 7 - Extra/test slots need art only if enabled by a scenario/ruleset.
- Test Unit 8 - Extra/test slots need art only if enabled by a scenario/ruleset.

## Original Source Buckets

- **Advance:** ICONS sprite sheet: 36x20 technology icons at x=343+37*category, y=211+21*epoch; Civilopedia also uses advanceCategories.
- **Improvement:** ICONS sprite sheet: 36x20 building icons at x=343+37*column, y=1+21*row.
- **Wonder:** ICONS sprite sheet for small wonder icon; Tiles.dll/worldWonders for advisor background.
- **Unit:** UNITS sprite sheet: 64x48-ish per unit plus flag/shield marker pixels.
- **Terrain:** TERRAIN1/TERRAIN2 sprite sheets: base tiles, special-resource tiles, rivers, coasts, roads, railroads, improvements.
- **Special Resource:** TERRAIN1 special1/special2 slots, plus resource rules entries.
- **Leader:** PEOPLE sprite sheet for citizen/people portraits; leader diplomacy portraits are not yet represented as a first-class rules image table.
- **UI/Advisor/Screen:** ICONS, CITIES, PEOPLE, Tiles.dll, Intro.dll, pv.dll, cv.dll, local buttons.png/explorer_icons.png.
