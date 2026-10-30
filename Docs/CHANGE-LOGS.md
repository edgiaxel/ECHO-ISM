# Change Logs — ECHO-ISM

## Stage 0 — Project audit and baseline

- Audited the initial working tree on 2026-10-30. The repository has no commits; the project files and `Docs/` are untracked, and `origin` points to `git@github.com-personal:edgiaxel/ECHO-ISM.git`.
- The initial audit snapshot lacked the C# project files, main scene, and run-main-scene setting. Axel added them afterward; the current checkout contains `ECHO-ISM.csproj`, `ECHO-ISM.sln`, and `Scenes/Main.tscn`, and `project.godot` selects Main and enables C#.
- Inspected `project.godot`: project name is ECHO-ISM, feature version is 4.7, renderer is GL Compatibility, and the .NET assembly name is configured. Godot reports `4.7.2.stable.mono.official.ed1daf0bf`.
- .NET SDK 10.0.403 and runtime 10.0.14 are installed under `/home/axel/.dotnet`; `dotnet` is not on the shell PATH by default. The Debian APT commands were unnecessary, and `sudo` requires an unavailable password. With `DOTNET_ROOT` and PATH set to that install, `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors.
- Godot headless editor opened and initialized the project successfully with the .NET environment set. A headless run of the configured Main scene completed five frames with exit code 0 and no output errors. Axel also confirmed opening the project in Godot and selecting `Main.tscn` as the run scene.
- An initial headless editor launch without the .NET PATH reported `dotnet: not found`, could not load `hostfxr`, and crashed; rerunning with the correct PATH succeeded. That first attempt also printed `Cannot convert string "vlines2" to type Pixmap`. No warnings or errors appeared in the successful build, editor initialization, or game run.
- Stage 0 is preserved as commit `fd2ef69`; the verified project and checklist/changelog updates are being committed and pushed as the Stage 0 completion revision.


## Stage 1 — Skeleton application and reusable level container

- Kept `Scenes/Main.tscn` as the entry scene. `GameManager.cs` instantiates the configured test level under `LevelHost`; the HUD remains a separate scene and reads room name and objective from the level container.
- Added `Scenes/Levels/TestRoom.tscn` and `LevelContainer.cs` with editable room bounds, start position, loop duration, echo limit, objective, completion-condition field, and separate puzzle-module, resettable-state, and persistent-state roots. Completion and reset behaviors remain for later stages.
- Added drawn room boundaries, a spawn marker, and restrained signal traces. The slate background and blue accent follow the existing project icon; the geometry keeps the small test room legible without implying mechanics that are not implemented.
- Added a static high-contrast HUD for the room name and objective, plus an honest prototype note. No nonfunctional controls, timer, or echo count were added.
- Design Read: first room shell for players, using the documented digital-drawing direction; ENERGY 2 / RHYTHM 1 / MOTION 1. The single centered room makes the test space the focal point, the HUD sits above it, and motion is omitted until it communicates gameplay.
- Verification: `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors; the Godot 4.7.2 Mono editor initialized the project; the configured Main scene ran; a rendered frame showed the room and HUD with no clipping or missing resources. HUD text/background contrast measured 13.62:1 and 8.55:1; room trace/floor contrast measured 3.79:1.

## Stage 1 interface refinement and Stage 2 — Movement and interaction foundation

- Simplified the Stage 1 HUD to one short objective and a proximity-only interaction prompt. Removed the title, room-name label, and explanatory prototype note; retained the slate palette and blue signal accent from the existing art direction.
- Attempted to retrieve the three requested Stitch screens through the provided MCP endpoint. The endpoint rejected the provided API key, so no Stitch images or code were downloaded or added to the project.
- Added `Scenes/Characters/Greg.tscn` with a white outlined blob, facing eyes, four-directional WASD/arrow movement, and a physics collision shape. Movement runs in `_PhysicsProcess` at a configured speed.
- Added room-edge collision shapes derived from the level's `RoomBounds`, plus a fixed room camera and a 1152×648 viewport.
- Added `Scenes/Characters/Echo.tscn` as a translucent, outlined, non-colliding placeholder.
- Added the `IInteractable` contract and a test switch with stable ID `test-switch`. Greg selects the nearest switch in range, displays its prompt, toggles it with E, and emits the stable target ID for a future recording system. The switch position and objective text both show whether it is active.
- Verification: `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors. The Godot 4.7.2 Mono editor initialized the project, and Main launched without errors. A rendered frame confirmed the objective, room, Greg, and switch fit at 1152×648. HUD text contrast measured 13.74:1; the signal trace has 3.79:1 contrast against the room floor.
- Axel's hands-on checks for wall blocking, switch use, movement speed, and interaction feel remain open; no feel adjustments have been made without that review.
