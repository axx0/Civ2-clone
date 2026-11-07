# Reveal movement before notifying animation listeners

ExecuteUnitMove previously revealed neighboring tiles but omitted the destination
itself. It notified animation listeners before revealing terrain or refreshing
map caches; movement into an unexplored destination could omit the owner's
notification entirely. The same method serves keyboard and Goto movement.

Movement now reveals the destination and neighbors to the owner, refreshes changed
map tiles (including source/destination), then emits movement notifications.
Distant terrain and other civilizations' exploration flags remain unchanged.
Notification recipients otherwise retain the existing visibility-flag rule.
This changes notification ordering intentionally so renderers see current map data.

## Validation

```sh
dotnet test Core.Tests/Core.Tests.csproj --filter FullyQualifiedName~MovementVisibilityTests
```

Three regressions cover ordinary/Goto orders, owner notification for an initially
unexplored destination, an observer with and without prior visibility, refreshed
map state before movement callbacks, and no exploration leak to distant tiles or
another civilization. All three fail against the original method.

This does not change pathfinding, the Goto destination visibility requirement,
or the distinction between explored terrain and current line of sight.
