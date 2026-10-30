# Gameplay Specification — ECHO-ISM

## 1. Design goals

- Make recorded past attempts the central mechanic.
- Keep movement and interaction simple.
- Make puzzles deterministic and learnable.
- Build levels from reusable modules.
- Use time pressure to create planning decisions, not arbitrary frustration.
- Ensure the player can see, understand, and correct mistakes.

## 2. Core loop

1. Enter a level and inspect its objective.
2. Start the loop.
3. Move Greg and interact with mechanisms.
4. The loop timer expires, or the player resets early.
5. The game stores the attempt as a recorded action track.
6. Greg returns to the loop's initial state.
7. Recorded echoes replay their tracks.
8. Greg performs a new track while earlier echoes act.
9. The puzzle evaluates its objective.
10. On success, proceed to the next variation; on failure, retry and revise the plan.

The first prototype may use a fixed-duration loop. Early reset can be added only after automatic reset and recording work reliably.

## 3. Loop and echo rules

### 3.1 Recording

Record the information required to reproduce an attempt. Prefer a deterministic event track over uncontrolled physics recording.

A track may contain:
- Loop-relative timestamp.
- Position or movement input.
- Facing direction.
- Interaction start and end.
- Target mechanism identifier.
- Interaction result where necessary.
- Animation/event markers for visual feedback.

The implementation should choose one canonical representation and document it. Avoid storing redundant values that can disagree.

### 3.2 Playback

- Echo playback starts at loop time zero.
- Echoes follow the recorded track without making new decisions.
- Playback uses a stable simulation clock or fixed-step update.
- Echoes visually indicate whether they are active, waiting, interacting, or finished.
- If the track references a missing or disabled object, fail safely and report the issue during development.
- Echoes must not silently change their recorded behavior because of a new obstacle state.

### 3.3 Reset

Resetting the loop:
- Returns the live Greg to the starting state.
- Resets loop-local puzzle state according to each module's reset policy.
- Preserves recorded tracks that are still within the echo limit.
- Resets the loop timer.
- Does not automatically create a permanent copy unless the game explicitly commits that attempt as an echo.

### 3.4 Finite limits

The game must not allow unlimited echoes by default.

Initial tuning values:
- Loop duration: **15 seconds** for the first prototype, configurable per level.
- Maximum active echoes: **3** initially, configurable per level.
- A new recorded attempt that exceeds the limit must follow an explicit policy: replace the oldest echo, reject the commit, or ask the player to choose. Prototype default: **replace the oldest echo**, with a clear UI indicator.
- Levels may impose a lower echo limit.
- A future advanced level may grant extra echoes as a deliberate mechanic, but no level should accidentally bypass the limit.

### 3.5 Ghost and collision rules

Default:
- Echoes are translucent and do not block Greg or one another.
- Echoes can activate only mechanisms marked as echo-compatible.
- Echoes do not physically push objects unless a puzzle explicitly enables that behavior.

Optional per-level variants:
- **Ghost:** no physical collision; can activate echo-compatible sensors.
- **Solid Echo:** can hold down a pressure plate or block a moving object.
- **Phase Echo:** passes through selected barriers and interacts with designated phase switches.
- **Anchor Echo:** remains at a recorded position or holds a mechanism after its recorded action, only if the module defines that rule.

Collision mode must be configured per echo or puzzle module. Do not globally enable collision for all echoes.

## 4. Interaction model

Use a small, consistent set of interactions.

- **Move:** keyboard movement, with a configurable speed.
- **Interact:** context-sensitive interaction within a clear range or at a grid position.
- **Activate:** use a switch, terminal, or mechanism.
- **Verify:** complete a verification action at a designated node.
- **Connect:** restore a route between two compatible nodes.
- **Reset/Commit:** end the current attempt and create an echo according to the current loop policy.

Use a single primary interaction key where possible. Do not require a separate key for every mechanism.

## 5. Puzzle module catalogue

Each module should be a reusable scene or component with explicit inputs, outputs, reset behavior, echo compatibility, and success conditions.

### M01 — Pressure Switch / Hold Plate
- A switch activates while an accepted actor occupies or interacts with it.
- An echo can hold it if the module is configured to accept echoes.
- Useful for holding a door open while the live player takes another route.
- Variants: momentary, toggle, multi-actor, timed release.

### M02 — Dual Mechanism Door
- Two mechanisms must be active together.
- The mechanisms may be in different parts of the room.
- An echo activates the first while Greg reaches the second.
- Variants: both held simultaneously, both activated in a sequence, one must remain active for a duration.

### M03 — Timed Door / Window
- A door opens only during a defined time window.
- The player must align an echo's recorded activation with the live player's arrival.
- Variants: short window, repeating window, window delayed by a switch, window extended by a second mechanism.

### M04 — Timing Slider
- A marker moves left and right along a track.
- The player presses an interaction key to stop it.
- Success depends on stopping inside a visible target zone.
- Variants: wide/narrow target, changing speed, reversed direction, moving target, echo-triggered start.
- Feedback must show whether the result was early, late, or accurate.

### M05 — Synchronization Gate
- Two or more signals must arrive within a synchronization tolerance.
- The player can record one signal and deliver another in a later loop.
- Variants: strict timing, forgiving timing, moving signal source, multiple gates in sequence.

### M06 — Route / Maze
- Obstacles and doors create alternate paths.
- The player may need to traverse both directions across separate attempts.
- Variants: one-way passage, rotating barrier, timed shortcut, phase-only route.

### M07 — Verification Terminal
- Greg performs a verification interaction at a terminal.
- Completion changes a message from unverified to verified.
- A connected output node accepts only verified information.
- Variants: terminal requires a held switch, terminal must be activated during a time window, two terminals must be checked in sequence.
- This is a fictional information puzzle, not a real-world fact-checking simulator.

### M08 — Signal Relay
- A signal must pass through a series of nodes.
- Nodes may need activation in a specific order or during overlapping windows.
- Echoes can operate one or more relay nodes while Greg reaches another.
- Variants: split route, delayed signal, limited active nodes.

### M09 — Moving Obstacle
- A barrier or platform follows a deterministic cycle.
- The player must time movement or use an echo to control its state.
- Variants: periodic movement, switch-controlled movement, reversible direction.

### M10 — Echo-Sensitive Sensor
- Detects the live player, any echo, or a specified echo class according to configuration.
- Variants: one echo only, two echoes simultaneously, echoes in sequence, live player excluded.

### M11 — Connection Repair
- Connect two or more separated nodes to restore a route.
- Connection may require a verified signal or a switch held by an echo.
- Variants: shortest route, ordered route, multi-branch route.

### M12 — Event Prevention
- A deterministic countdown leads to a harmful system event.
- The player must perform a defined set of interventions before the countdown completes.
- The event is represented abstractly through warning lights, separation, system instability, or broken connections; do not depict graphic harm.
- Variants: one intervention delays the event, another permanently prevents it, multiple interventions are required.

## 6. Timing design

Timing puzzles must be learnable and fair.

- Display the timing bar and target zone clearly.
- Use consistent units and stable simulation timing.
- Start with a generous target zone; tighten it only in later variants.
- Avoid requiring frame-perfect input in the introductory stages.
- Give immediate feedback on early/late/accurate attempts.
- If an echo must perform an action at a particular time, display its recorded timestamp or an alignment cue during planning.
- Provide a deterministic retry with the same initial conditions unless variation is the explicit challenge.

## 7. Modular level structure

Treat each level as a composition of reusable containers, similar to connecting small building blocks.

### Level container

A level container should define:
- Level identifier and display name.
- Starting position.
- Loop duration and echo limit.
- Tile/grid dimensions or room bounds.
- Puzzle module instances.
- Connections between module outputs and inputs.
- Echo collision profile.
- Objective conditions.
- Reset policy.
- Optional variant parameters.
- Completion and failure feedback.

### Module contract

Each module should expose:
- A stable identifier.
- Required configuration values.
- Accepted actor types.
- Input signals/events.
- Output signals/events.
- Reset behavior.
- Completion state.
- Visual state for idle, active, failed, and completed.

Do not build a generic visual scripting framework prematurely. Begin with a small typed configuration or scene-based composition that is easy to inspect in Godot.

## 8. Level variation and recombination

Start with handcrafted, deterministic templates. Procedural generation comes later.

Variation dimensions:
- Number of mechanisms.
- Relative positions and route lengths.
- Loop duration.
- Echo limit.
- Number of required simultaneous actions.
- Timing-window width.
- Timing-slider speed.
- Door activation rule.
- Echo collision profile.
- Order of objectives.
- Whether the player must travel out and back.
- Number of information nodes.
- Whether a signal must be verified before it can pass.
- Moving-obstacle period.
- Number of echoes needed.

### Composition rules

- Every generated or remixed level must be solvable under its configured limits.
- Do not combine modules if their contracts conflict.
- Preserve a clear start, objective, and completion condition.
- Introduce one new mechanic at a time before combining mechanics.
- Use seeded variation for reproducible debugging.
- Keep a handcrafted fallback level available.
- Do not call a level procedural merely because its objects are randomly positioned.
- Randomization must preserve puzzle validity and teachable difficulty.

## 9. Suggested progression

### Set A — Learn the echo
1. One switch and one door.
2. Echo holds a switch while Greg moves through the door.
3. Two switches on separate routes.
4. Door requires both switches at once.

### Set B — Learn timing
1. A generous timing-slider target.
2. Timing slider opens a timed door.
3. Echo activates the slider while Greg traverses the route.
4. Two timed mechanisms must overlap.

### Set C — Learn information routing
1. One verification terminal and one output node.
2. Echo verifies while Greg reaches the output.
3. A relay requires two signals in sequence.
4. A misinformation warning event must be prevented before the final timer expires.

### Set D — Combine systems
1. Dual mechanism plus moving obstacle.
2. Verification terminal plus timed gate.
3. Three echoes coordinate across separate mechanisms.
4. A final room combines previously learned modules without introducing an entirely new rule.

## 10. Win, failure, and feedback

Victory:
- The required mechanisms are in their target state.
- The information route is verified where required.
- The relevant event is prevented or the system is stabilized.
- The game presents clear success feedback and unlocks the next level.

Failure:
- The timer expires without progress if the level defines that condition.
- The player reaches a failed event state.
- A timing action fails, if the module requires a successful timing window.

Failure should usually allow a quick retry. Avoid long reloads, punitive progress loss, or unnecessary narrative interruption.

## 11. Prototype acceptance tests

- An attempt can be recorded and replayed.
- Reset returns the live player and resettable objects to their correct initial state.
- An echo reproduces movement and interactions in a stable order.
- Echo count never exceeds the configured limit.
- Default echoes do not collide with the live player.
- Echo-compatible mechanisms respond to echoes.
- Non-compatible mechanisms ignore echoes.
- A dual-mechanism puzzle can be solved with at least one echo.
- A timing puzzle displays useful feedback.
- The same recorded tracks and initial state produce the same result.
- Victory and failure are both reachable and visibly communicated.
