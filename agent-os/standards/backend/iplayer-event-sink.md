# IPlayer Event Sink

The engine communicates all world changes and notifications through the `IPlayer` interface to decouple core logic from player implementations.

- **Decoupling** — The engine must never distinguish between Human (UI) and AI players.
- **Unified Interface** — Both `Civ2Interface` (human/UI) and `AiPlayer` must implement `IPlayer`.
- **Event-Driven** — Use methods like `player.CityProductionComplete(city)` for notifications instead of direct UI/AI calls.
- **No-op Handling** — Implementations should use empty methods (no-ops) for irrelevant events (e.g., AI ignoring UI-only notifications).

```csharp
// Example usage in Engine
player.CivilDisorder(city);
```
