# Extended Data Dictionary

Use the `ExtendedData` dictionary to store state for scripts and mods on core objects (e.g., `Unit`, `Tile`, `City`) without modifying the core schema.

- **Purpose** — For script-specific or mod-specific data that is not part of the core engine logic.
- **Namespacing** — Always prefix keys with a script or mod name to prevent collisions (e.g., `scout_script:last_tile`).
- **Persistence** — Values in this dictionary are automatically serialized and persisted in save games.
- **Schema Integrity** — Avoid adding new class properties for data that is only used by specific scripts.

```csharp
// Example usage
unit.ExtendedData["ai_aggression:target_city_id"] = city.Id.ToString();
```
