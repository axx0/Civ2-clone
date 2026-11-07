# Mouse presses and Goto

A held mouse button previously emitted MouseDown on every frame, resetting the
Goto timer continuously. With a 15 ms threshold, behavior depended on frame rate.
BaseControl now emits one MouseDown per press and one Click on release. Leaving
and re-entering a control cancels its pending click as before. A protected input
reader permits native-window-free regression tests.

Goto now uses a monotonic 350 ms hold threshold. Hold the left button over an
explored destination, then release to issue Goto. The route starts from the unit's
actual location rather than map focus. Right clicks do not start the hold timer;
leaving the map cancels it. Cleanup targets the mode that received the gesture,
even if execution changes modes. Invalid active units and the current tile do
not issue routes.

A failed route shows a message instead of silently falling through to selection
or a city window. Unexplored destinations remain unsupported by the existing
pathfinder; use movement keys to explore first. This fix does not give the
pathfinder access to hidden terrain or implement speculative exploration routes.

## Validation

```sh
dotnet test RaylibUI.Tests/RaylibUI.Tests.csproj
```

Six tests cover left/right holds over one or 120 frames, exactly one release,
repeat gestures and leaving/re-entering the control. Two long-hold cases fail
against the original repeated-press logic. UI tests have their own project and
do not depend on the engine's pre-existing test-fixture compilation repairs.

Related upstream report: https://github.com/axx0/Civ2-clone/issues/137.
Movement visibility is a separate change described in movement-visibility.md.
