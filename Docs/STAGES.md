# Development Stages — ECHO-ISM

## Workflow rules

- Work in order, but allow short feedback iterations within a stage.
- Each stage must have a clear acceptance checklist.
- Do not start a later stage while a critical acceptance criterion from the current stage remains unverified.
- Axel may review or revise a stage vision before it is marked complete.
- Keep all future stages documented. Never remove them because they are not part of the current sprint.
- Coding agents must follow `AGENTS.md`, especially its documentation-preservation rules.
- Each stage should end with a playable or verifiably testable result where practical.

$antislop $antislop-code $antislop-human $antislop-ui 

Read README.md, /Docs AGENT.md, GAME-CONCEPT.md, GAMEPLAY.md, CHANGE-LOGS.md, and STAGES.md. ONLY change the checklist and DO NOT DELETE the content. All document in Docs/ is absolute except CHANGE-LOGS.md you can change it based on the development progress.
Add a limit if a test takes longer than expected, kill it and just skip it and finish the prompt.
 
For the level design read LEVEL-DESIGN.md, CAMPAIGN-PROGRESSION.md. Update LEVEL-IMPLEMENTATION-REPORT.md and CHANGE-LOGS.md.

---

## Stage 0 — Project audit and baseline

### S0.1 Inspect the existing project
- [x] Inspect `project.godot` and current project settings.
- [x] Confirm the project opens in Godot 4.7.2 .NET.
- [x] Confirm the C# project/solution setup and .NET SDK are available.
- [x] Check the Git working tree before making changes.
- [x] Record the actual current project state; do not assume gameplay files exist.

### S0.2 Establish a stable baseline
- [x] Run the empty project.
- [x] Record any warnings or errors.
- [x] Confirm the project can be launched from the editor.
- [x] Commit or otherwise preserve a clean baseline according to Axel's workflow.

**Exit criteria:** The project opens and runs in the intended Godot .NET environment, and its baseline state is understood.

## Stage 1 — Skeleton application and reusable level container

### S1.1 Create the application skeleton
- [x] Create the main scene.
- [x] Create a minimal game manager.
- [x] Create the gameplay scene/container.
- [x] Create a basic HUD container.
- [x] Add placeholder geometry for one room.
- [x] Add a simple title/start flow or direct entry into the test room.
- [x] Keep scene ownership and script responsibilities clear.

### S1.2 Define the level container contract
- [x] Define how a level declares its bounds or grid.
- [x] Define the start position.
- [x] Define the loop duration and echo limit.
- [x] Define puzzle-module placement.
- [x] Define objective and completion conditions.
- [x] Define resettable versus persistent state.
- [x] Document the contract in `Gameplay.md` without deleting existing content.

### S1.3 Validate the skeleton
- [x] Run the game from the main scene.
- [x] Confirm the room and HUD are visible.
- [x] Confirm no missing-resource errors.
- [x] Confirm the scene can be edited without changing gameplay scripts unnecessarily.

**Exit criteria:** A minimal, runnable skeleton exists, and one level can be assembled from a reusable container.

## Stage 2 — Greg movement and interaction foundation

### S2.1 Placeholder character
- [x] Create Greg using basic 2D shapes.
- [x] Add visible eyes or a clear facing indicator.
- [x] Add a simple outline and readable silhouette.
- [x] Create a distinct placeholder appearance for an echo.

### S2.2 Movement
- [x] Implement four-directional movement.
- [x] Keep movement consistent across frame rates.
- [x] Prevent movement through ordinary solid walls.
- [x] Confirm the camera and character scale work in the test room.
- [x] Add basic movement feedback without polishing animation.

### S2.3 Interaction contract
- [x] Define a common interaction interface or base component.
- [x] Add a context-sensitive interaction prompt.
- [x] Implement one simple test switch.
- [x] Confirm the interaction works reliably near the object.
- [x] Ensure the same interaction can later be recorded and replayed.

### S2.4 Review iteration
- [x] Axel reviews movement speed, scale, and interaction feel.
- [x] Apply only the approved adjustments.
- [x] Run the project after changes.

**Exit criteria:** Greg can move around the room and activate one interactive object.

## Stage 3 — Loop timer, recording, and reset

### S3.1 Loop state
- [x] Add configurable loop duration, initially 15 seconds.
- [x] Display the timer.
- [x] Define start, active, recording/commit, reset, and completion states.
- [x] Define what happens when the timer expires.
- [x] Prevent duplicate reset or commit events.

### S3.2 Record attempts
- [x] Record movement and position on a stable loop-relative timeline.
- [x] Record interaction events and their target identifiers.
- [x] Record facing or animation state only where necessary.
- [x] Add a development-only way to inspect the recorded track.
- [x] Avoid redundant or contradictory event data.

### S3.3 Reset
- [x] Return live Greg to the starting state.
- [x] Reset loop-local mechanisms according to their reset policies.
- [x] Reset the timer.
- [x] Preserve the committed recording.
- [x] Confirm resetting does not create unlimited echoes.

### S3.4 Validation
- [x] Repeat the same attempt and verify equivalent recorded data.
- [x] Test timer expiration and any manual reset path.
- [x] Confirm no object remains accidentally active after reset.
- [x] Inspect logs for errors.

**Exit criteria:** The game can record a complete attempt and reset to a consistent initial state.

## Stage 4 — Echo playback

### S4.1 First echo
- [x] Spawn one echo from the previous committed attempt.
- [x] Replay movement from the loop timeline.
- [x] Replay interactions in the correct order.
- [x] Make the echo visibly translucent.
- [x] Ensure the echo does not collide with Greg by default.
- [x] Ensure the echo can activate only compatible mechanisms.

### S4.2 Multiple echoes
- [x] Add a configurable echo limit, initially three.
- [x] Define and display the oldest-echo replacement policy.
- [x] Ensure each echo uses its own recording.
- [x] Keep playback timing synchronized.
- [x] Handle an echo that finishes before the loop ends.
- [x] Handle missing or invalid interaction targets safely.

### S4.3 Determinism tests
- [x] Replay a recorded path multiple times.
- [x] Verify interaction timestamps are stable.
- [x] Verify echo count never exceeds the configured limit.
- [x] Verify collision settings match the level/module configuration.
- [x] Test overlapping echoes.

**Exit criteria:** Multiple past versions of Greg reliably replay their actions and can activate echo-compatible mechanisms.

## Stage 5 — First complete puzzle

### S5.1 Build the teaching room
- [x] Add a switch and a door.
- [x] Require the switch to remain active while Greg traverses the door.
- [x] Make the switch echo-compatible.
- [x] Add a clear objective and completion state.
- [x] Ensure the room cannot be completed by simply walking through in one loop.

### S5.2 Validate the puzzle
- [x] Complete the room using one echo.
- [x] Retry after a deliberately failed attempt.
- [x] Confirm the failure teaches the necessary timing or route.
- [x] Check that the puzzle is not dependent on frame-perfect movement.
- [x] Confirm the solution is understandable without a long tutorial.

### S5.3 Review iteration
- [x] Axel reviews the puzzle's fun, clarity, and pacing.
- [x] Adjust only after feedback.
- [x] Keep a known-good version of the room.

**Exit criteria:** The central mechanic is necessary, understandable, and satisfying in at least one complete puzzle.

## Stage 6 — Reusable puzzle modules

### S6.1 Module interface
- [x] Define module identity, configuration, accepted actors, state, reset behavior, and outputs.
- [x] Keep the implementation simple; avoid building an unnecessary general-purpose framework.
- [x] Make module states visible in the scene and during gameplay.

### S6.2 Implement core modules
- [x] Pressure switch / hold plate.
- [x] Dual mechanism door.
- [x] Timed door / window.
- [x] Timing slider.
- [x] Synchronization gate.
- [x] Moving obstacle.
- [x] Verification terminal.
- [x] Signal relay.
- [x] Echo-sensitive sensor.
- [x] Connection repair.
- [x] Event-prevention objective.

### S6.3 Module tests
- [x] Test each module independently.
- [x] Test reset behavior.
- [x] Test live-player and echo compatibility.
- [x] Test at least one valid and one invalid configuration.
- [x] Confirm visual states are readable.
- [x] Document any intentionally deferred module rather than deleting it from the plan.

**Exit criteria:** The core modules have explicit contracts and can be reused in a level.

## Stage 7 — Timing and precision challenges

### S7.1 Timing slider
- [x] Add a moving marker and visible target zone.
- [x] Stop the marker on player input.
- [x] Provide early/late/accurate feedback.
- [x] Make speed and target width configurable.
- [x] Support echo-triggered timing actions where appropriate.

### S7.2 Synchronization
- [x] Add a mechanism that requires two events within a tolerance window.
- [x] Display a useful timing cue.
- [x] Ensure simulation timing is deterministic.
- [x] Add a forgiving introductory variant.
- [x] Add a more demanding variant for later progression.

### S7.3 Validation
- [x] Test on repeated runs.
- [x] Ensure success is not dependent on unstable frame timing.
- [x] Verify the player can understand why the timing action failed.

**Exit criteria:** At least one timing module and one synchronization puzzle work reliably.

## Stage 8 — Level templates and remixing

### S8.1 Handcrafted templates
- [x] Create a small library of reusable room templates.
- [x] Define each template's intended mechanic and difficulty.
- [x] Keep a known-solvable example of each template.
- [x] Separate layout data from reusable module logic where practical.

### S8.2 Variation parameters
- [x] Vary mechanism placement.
- [x] Vary route length and direction.
- [x] Vary loop duration and echo limit.
- [x] Vary timing-window width and mechanism period.
- [x] Vary objective order.
- [x] Vary required number of simultaneous actions.
- [x] Vary echo collision profile only when the puzzle explains the rule.

### S8.3 Deterministic remixing
- [x] Add seeded variation only after templates are reliable.
- [x] Validate that each selected configuration is solvable.
- [x] Keep the seed available for reproduction and debugging.
- [x] Preserve a handcrafted fallback level.
- [x] Reject invalid combinations rather than silently generating broken levels.

### S8.4 Progression review
- [x] Introduce one new mechanic at a time.
- [x] Combine mechanics only after their individual rules are understood.
- [x] Confirm later variants are harder because of meaningful decisions, not arbitrary timing.
- [x] Have Axel review the level progression.

**Exit criteria:** Several distinct level variations can be produced from tested templates and deterministic parameters.

## Stage 9 — Narrative and information-resilience integration

### S9.1 Environmental storytelling
- [x] Add short contextual messages or visual clues.
- [x] Show how misinformation disrupts connections and system stability.
- [x] Keep the player objective clear without requiring extensive reading.
- [x] Use fictional claims and fictional groups.

### S9.2 Preventive mechanics
- [x] Make verification an actual puzzle action.
- [x] Make restoring communication a meaningful objective.
- [x] Show a visible consequence when the player prevents an escalation event.
- [x] Avoid portraying disagreement alone as dangerous.
- [x] Avoid glamorizing manipulation or depicting graphic harm.

### S9.3 Narrative review
- [x] Check that the story follows the actual gameplay.
- [x] Remove or revise any text that promises mechanics the game does not implement.
- [x] Confirm the theme is present in player decisions, not only in exposition.

**Exit criteria:** The theme is communicated through mechanics and outcomes, with concise supporting narrative.

## Stage 10 — Visual, UI, and audio refinement

### S10.1 Visual refinement
- [x] Refine Greg's blob silhouette and eye expressions.
- [x] Establish a consistent palette and outline style.
- [x] Improve switch, door, terminal, and connection visuals.
- [x] Refine echo appearance and overlap readability.
- [x] Add restrained glow and signal effects.

### S10.2 UI/UX
- [x] Improve timer, loop count, echo count, and objective readability.
- [x] Make interaction prompts consistent.
- [x] Add pause and retry flows.
- [x] Verify controls are displayed clearly.
- [x] Check keyboard navigation and text readability where relevant.

### S10.3 Audio
- [x] Add interaction, switch, echo, timing, door, and success sounds.
- [x] Add restrained ambient music.
- [x] Confirm the game remains understandable when muted.
- [x] Add volume controls if time permits.
- [x] Verify asset licensing and attribution.

### S10.4 Performance and readability
- [x] Ensure effects do not obscure puzzle information.
- [x] Check for excessive flashing or screen shake.
- [x] Confirm the game runs smoothly on the target machine.

**Exit criteria:** The game has a consistent visual/audio identity without compromising puzzle clarity.

## Stage 11 — Playtesting and stabilization

### S11.1 Gameplay tests
- [x] Ask a fresh player to try the first room without a verbal explanation.
- [x] Observe whether they understand the echo mechanic.
- [x] Record where they get confused.
- [x] Test the timing modules.
- [x] Test all intended completion and failure states.
- [x] Fix blockers before adding new features.

### S11.2 Technical tests
- [x] Launch from a clean project state.
- [x] Check for missing resources and C# build errors.
- [x] Test restarting levels repeatedly.
- [x] Test echo limits and reset state.
- [x] Test every included level.
- [x] Confirm no documentation or project files were unintentionally removed.

### S11.3 Final scope decision
- [x] Freeze new mechanics.
- [x] Prioritize bugs and usability issues.
- [x] Remove unfinished features from the playable build only if they are not required; do not delete their documented future plans.
- [x] Confirm the submitted build matches the actual implementation.

**Exit criteria:** The included experience is stable and can be completed by someone other than its creator.

## Stage 12 — Build, submission, and promotional video

### S12.1 Build
- [x] Export a runnable build using the correct Godot .NET export templates.
- [x] Test the exported build on a clean launch.
- [x] Include controls and a concise description.
- [x] Verify all required files are included.
- [x] Keep a backup of the final build and source state.

### S12.2 Trailer / progress video
- [x] Capture actual gameplay footage.
- [x] Show the echo mechanic clearly.
- [x] Show a puzzle that requires multiple attempts.
- [x] Explain the prevention theme through player actions.
- [x] Use Remotion and/or Hyperframes for titles, captions, and editing if useful.
- [x] Do not present unimplemented mockups as actual gameplay.

### S12.3 Submission review
- [x] Confirm the submission description matches the game.
- [x] Disclose AI use as required by the jam.
- [x] Check asset credits and licenses.
- [x] Check the official submission requirements and deadline.
- [x] Submit with enough time to recover from upload or export problems.

**Exit criteria:** A tested playable build and required submission materials are ready.

---

## Change log

- Initial documentation plan: define a reusable, deterministic echo-puzzle foundation before expanding into level variation, narrative, polish, and submission.
- This file is an ongoing plan. Completed checklists should be updated in place; future stages must remain present.
