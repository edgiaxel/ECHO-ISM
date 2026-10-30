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

## Stage 2 Stitch UI integration and verification pass

- Retrieved all three Stitch screens via the Stitch MCP server at `https://stitch.googleapis.com/mcp` using project ID `12186110244768219801`:
  1. ECHO-ISM Design System & Art Direction Document (`5a9a3a71c9ee4095a7b17a8838a6a680`)
  2. ECHO-ISM Modular Room Blueprints (`cf1eb0af371041f98bf4003bf37940b3`)
  3. ECHO-ISM Gameplay HUD 1152x648 (`1cbbafd0451b40a69cd3999da0a64b37`)
  Saved screenshots and HTML specifications into `StitchAssets/`.
- Integrated the Stitch design system into `Scenes/UI/BasicHud.tscn`:
  - Added a top perimeter bar with Room 01 badge, Sector tag, loop chrono timer display (`15.00s`), and loop/echo indicators (`LOOP 01/03`, `ECHOES [I] [ ] [ ]`).
  - Added a primary objective card in the top-left area showing active mission status with high contrast.
  - Added a context-sensitive interaction prompt badge with `[E]` key indicator and action label that appears when Greg is within interaction range and hides when out of range.
  - Added a bottom controls hint bar (`WASD Move · E Interact · R Reset Loop`).
  - Kept UI concise, readable, and free of filler text per antislop and antislop-human guidelines.
- Refined game visuals in accordance with the Stitch design system:
  - Updated Greg (`GregCharacter.cs`) with electric azure fill (`#00D2FF`), dark border (`#081B33`), directional eyes, and bright white specular highlight dots.
  - Updated TestSwitch (`TestSwitch.cs`) with dark slate base (`#0E1626`), responsive borders, active mint (`#10B981`) and idle amber (`#F59E0B`) indicators.
  - Updated RoomGeometry (`RoomGeometry.cs`) with deep surface background (`#090E1A`), crisp layered boundaries (`#1E293B` and `#334155`), and cyan signal paths (`#00D2FF`).
- Verification:
  - `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors.
  - Headless physics test confirmed solid wall collision stops Greg at the room boundary.
  - Interaction test confirmed the prompt appears within range and pressing interact toggles switch state and objective text.
  - Contrast verified using `contrast-check.py`: `#F8FAFC` on `#0E1626` is 17.28:1 (PASS); `#00D2FF` on `#0E1626` is 10.04:1 (PASS); `#94A3B8` on `#0E1626` is 7.05:1 (PASS); `#818CF8` on `#0E1626` is 6.06:1 (PASS).
  - Recorded a verified 1152x648 frame at `/tmp/echo-ism-s2-stitch00000000.png`.

## Stage 3: Loop timer, recording, and reset

- Implemented `LoopController` (`Scripts/LoopController.cs`) to manage loop states: `Ready`, `Active`, `Committing`, `Resetting`, and `Completed`.
- Configurable loop duration initialized to 15.0 seconds from `LevelContainer`, displayed as countdown on the HUD with an animated progress bar.
- Implemented state guards preventing duplicate reset or commit events during active transitions.
- Automatic reset triggers on timer expiration (`CurrentTime >= LoopDuration`); manual reset triggers via the `R` key (`reset_loop` action).
- Implemented attempt recording data structures (`Scripts/Recording/RecordedAttempt.cs`):
  - Records position and facing direction per physics frame on a stable loop-relative timeline.
  - Records interaction events with timestamp and target `StableId` (`test-switch`).
- Implemented `IResettable` contract (`Scripts/IResettable.cs`) and integrated it into `TestSwitch.cs` to reset mechanisms upon loop reset.
- Reset sequence returns Greg to level start position and default facing, clears prompts, resets mechanisms, resets the timer to 0, and increments attempt counter.
- Enforced finite echo pool policy: committed recordings are preserved up to `MaximumActiveEchoes` (3), replacing the oldest recording upon exceeding the limit.
- Added development-only track inspector (`DevInspector`) toggleable with `F1` and printable diagnostics via `GetRecordedTracksSummary()`.
- Verification:
  - `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors.
  - Executed automated validation suite verifying initial states, movement frame logging, interaction event recording, manual reset, duplicate event prevention, max echo pool cap, timer expiration reset, and data repeatability.
  - Recorded a verified in-engine frame at `/tmp/echo-ism-stage300000000.png`.

## Stage 4: Echo playback

- Implemented `EchoActor` (`Scripts/EchoActor.cs`) inheriting `GregCharacter`:
  - Translucent spectral cyan-violet appearance (`#818CF8` at 0.55 alpha, `#38BDF8` outline) with overhead Roman numeral tag (`I`, `II`, `III`).
  - Ghost collision configuration: `CollisionLayer = 0` and `CollisionMask = 0`, ensuring non-blocking movement that passes through Greg and other echoes without physics jitter.
  - Deterministic timeline playback: samples recorded frames via binary search by timestamp and updates world position and facing direction in lockstep with `CurrentTime`.
  - Sequential interaction replay: replays timestamped interaction events on target mechanisms in chronological order.
  - Safe error handling: safely logs and skips missing or disabled targets without desyncing playback.
  - Handled early finish: when an echo reaches the end of its recording before the loop finishes, it sets `IsPlaybackFinished` and holds its final position without disappearing.
- Updated `IInteractable` contract (`Scripts/IInteractable.cs`):
  - Added `AcceptsEchoes` property to explicitly declare whether a mechanism accepts echo actors.
  - Implemented on `TestSwitch.cs` to filter echo interactions.
- Updated `LoopController` (`Scripts/LoopController.cs`):
  - Manages active echo instances under an `EchoHost` container in the level.
  - Spawns synchronized echo actors for all committed attempts upon loop reset.
  - Enforces configurable echo limit (3) with oldest-echo replacement policy when exceeding capacity.
- Verification:
  - `dotnet build ECHO-ISM.sln` succeeded with 0 warnings and 0 errors.
  - Executed automated validation suite verifying first echo spawning, movement timeline playback, interaction replay, translucent appearance, ghost collision, mechanism compatibility filter, dual/triple echo synchronization, oldest-echo replacement policy, safe missing-target handling, and overlapping echo stability.
  - Recorded in-engine frame at `/tmp/echo-ism-stage400000000.png`.
