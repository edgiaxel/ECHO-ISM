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
- [ ] Create the main scene.
- [ ] Create a minimal game manager.
- [ ] Create the gameplay scene/container.
- [ ] Create a basic HUD container.
- [ ] Add placeholder geometry for one room.
- [ ] Add a simple title/start flow or direct entry into the test room.
- [ ] Keep scene ownership and script responsibilities clear.

### S1.2 Define the level container contract
- [ ] Define how a level declares its bounds or grid.
- [ ] Define the start position.
- [ ] Define the loop duration and echo limit.
- [ ] Define puzzle-module placement.
- [ ] Define objective and completion conditions.
- [ ] Define resettable versus persistent state.
- [ ] Document the contract in `Gameplay.md` without deleting existing content.

### S1.3 Validate the skeleton
- [ ] Run the game from the main scene.
- [ ] Confirm the room and HUD are visible.
- [ ] Confirm no missing-resource errors.
- [ ] Confirm the scene can be edited without changing gameplay scripts unnecessarily.

**Exit criteria:** A minimal, runnable skeleton exists, and one level can be assembled from a reusable container.

## Stage 2 — Greg movement and interaction foundation

### S2.1 Placeholder character
- [ ] Create Greg using basic 2D shapes.
- [ ] Add visible eyes or a clear facing indicator.
- [ ] Add a simple outline and readable silhouette.
- [ ] Create a distinct placeholder appearance for an echo.

### S2.2 Movement
- [ ] Implement four-directional movement.
- [ ] Keep movement consistent across frame rates.
- [ ] Prevent movement through ordinary solid walls.
- [ ] Confirm the camera and character scale work in the test room.
- [ ] Add basic movement feedback without polishing animation.

### S2.3 Interaction contract
- [ ] Define a common interaction interface or base component.
- [ ] Add a context-sensitive interaction prompt.
- [ ] Implement one simple test switch.
- [ ] Confirm the interaction works reliably near the object.
- [ ] Ensure the same interaction can later be recorded and replayed.

### S2.4 Review iteration
- [ ] Axel reviews movement speed, scale, and interaction feel.
- [ ] Apply only the approved adjustments.
- [ ] Run the project after changes.

**Exit criteria:** Greg can move around the room and activate one interactive object.

## Stage 3 — Loop timer, recording, and reset

### S3.1 Loop state
- [ ] Add configurable loop duration, initially 15 seconds.
- [ ] Display the timer.
- [ ] Define start, active, recording/commit, reset, and completion states.
- [ ] Define what happens when the timer expires.
- [ ] Prevent duplicate reset or commit events.

### S3.2 Record attempts
- [ ] Record movement and position on a stable loop-relative timeline.
- [ ] Record interaction events and their target identifiers.
- [ ] Record facing or animation state only where necessary.
- [ ] Add a development-only way to inspect the recorded track.
- [ ] Avoid redundant or contradictory event data.

### S3.3 Reset
- [ ] Return live Greg to the starting state.
- [ ] Reset loop-local mechanisms according to their reset policies.
- [ ] Reset the timer.
- [ ] Preserve the committed recording.
- [ ] Confirm resetting does not create unlimited echoes.

### S3.4 Validation
- [ ] Repeat the same attempt and verify equivalent recorded data.
- [ ] Test timer expiration and any manual reset path.
- [ ] Confirm no object remains accidentally active after reset.
- [ ] Inspect logs for errors.

**Exit criteria:** The game can record a complete attempt and reset to a consistent initial state.

## Stage 4 — Echo playback

### S4.1 First echo
- [ ] Spawn one echo from the previous committed attempt.
- [ ] Replay movement from the loop timeline.
- [ ] Replay interactions in the correct order.
- [ ] Make the echo visibly translucent.
- [ ] Ensure the echo does not collide with Greg by default.
- [ ] Ensure the echo can activate only compatible mechanisms.

### S4.2 Multiple echoes
- [ ] Add a configurable echo limit, initially three.
- [ ] Define and display the oldest-echo replacement policy.
- [ ] Ensure each echo uses its own recording.
- [ ] Keep playback timing synchronized.
- [ ] Handle an echo that finishes before the loop ends.
- [ ] Handle missing or invalid interaction targets safely.

### S4.3 Determinism tests
- [ ] Replay a recorded path multiple times.
- [ ] Verify interaction timestamps are stable.
- [ ] Verify echo count never exceeds the configured limit.
- [ ] Verify collision settings match the level/module configuration.
- [ ] Test overlapping echoes.

**Exit criteria:** Multiple past versions of Greg reliably replay their actions and can activate echo-compatible mechanisms.

## Stage 5 — First complete puzzle

### S5.1 Build the teaching room
- [ ] Add a switch and a door.
- [ ] Require the switch to remain active while Greg traverses the door.
- [ ] Make the switch echo-compatible.
- [ ] Add a clear objective and completion state.
- [ ] Ensure the room cannot be completed by simply walking through in one loop.

### S5.2 Validate the puzzle
- [ ] Complete the room using one echo.
- [ ] Retry after a deliberately failed attempt.
- [ ] Confirm the failure teaches the necessary timing or route.
- [ ] Check that the puzzle is not dependent on frame-perfect movement.
- [ ] Confirm the solution is understandable without a long tutorial.

### S5.3 Review iteration
- [ ] Axel reviews the puzzle's fun, clarity, and pacing.
- [ ] Adjust only after feedback.
- [ ] Keep a known-good version of the room.

**Exit criteria:** The central mechanic is necessary, understandable, and satisfying in at least one complete puzzle.

## Stage 6 — Reusable puzzle modules

### S6.1 Module interface
- [ ] Define module identity, configuration, accepted actors, state, reset behavior, and outputs.
- [ ] Keep the implementation simple; avoid building an unnecessary general-purpose framework.
- [ ] Make module states visible in the scene and during gameplay.

### S6.2 Implement core modules
- [ ] Pressure switch / hold plate.
- [ ] Dual mechanism door.
- [ ] Timed door / window.
- [ ] Timing slider.
- [ ] Synchronization gate.
- [ ] Moving obstacle.
- [ ] Verification terminal.
- [ ] Signal relay.
- [ ] Echo-sensitive sensor.
- [ ] Connection repair.
- [ ] Event-prevention objective.

### S6.3 Module tests
- [ ] Test each module independently.
- [ ] Test reset behavior.
- [ ] Test live-player and echo compatibility.
- [ ] Test at least one valid and one invalid configuration.
- [ ] Confirm visual states are readable.
- [ ] Document any intentionally deferred module rather than deleting it from the plan.

**Exit criteria:** The core modules have explicit contracts and can be reused in a level.

## Stage 7 — Timing and precision challenges

### S7.1 Timing slider
- [ ] Add a moving marker and visible target zone.
- [ ] Stop the marker on player input.
- [ ] Provide early/late/accurate feedback.
- [ ] Make speed and target width configurable.
- [ ] Support echo-triggered timing actions where appropriate.

### S7.2 Synchronization
- [ ] Add a mechanism that requires two events within a tolerance window.
- [ ] Display a useful timing cue.
- [ ] Ensure simulation timing is deterministic.
- [ ] Add a forgiving introductory variant.
- [ ] Add a more demanding variant for later progression.

### S7.3 Validation
- [ ] Test on repeated runs.
- [ ] Ensure success is not dependent on unstable frame timing.
- [ ] Verify the player can understand why the timing action failed.

**Exit criteria:** At least one timing module and one synchronization puzzle work reliably.

## Stage 8 — Level templates and remixing

### S8.1 Handcrafted templates
- [ ] Create a small library of reusable room templates.
- [ ] Define each template's intended mechanic and difficulty.
- [ ] Keep a known-solvable example of each template.
- [ ] Separate layout data from reusable module logic where practical.

### S8.2 Variation parameters
- [ ] Vary mechanism placement.
- [ ] Vary route length and direction.
- [ ] Vary loop duration and echo limit.
- [ ] Vary timing-window width and mechanism period.
- [ ] Vary objective order.
- [ ] Vary required number of simultaneous actions.
- [ ] Vary echo collision profile only when the puzzle explains the rule.

### S8.3 Deterministic remixing
- [ ] Add seeded variation only after templates are reliable.
- [ ] Validate that each selected configuration is solvable.
- [ ] Keep the seed available for reproduction and debugging.
- [ ] Preserve a handcrafted fallback level.
- [ ] Reject invalid combinations rather than silently generating broken levels.

### S8.4 Progression review
- [ ] Introduce one new mechanic at a time.
- [ ] Combine mechanics only after their individual rules are understood.
- [ ] Confirm later variants are harder because of meaningful decisions, not arbitrary timing.
- [ ] Have Axel review the level progression.

**Exit criteria:** Several distinct level variations can be produced from tested templates and deterministic parameters.

## Stage 9 — Narrative and information-resilience integration

### S9.1 Environmental storytelling
- [ ] Add short contextual messages or visual clues.
- [ ] Show how misinformation disrupts connections and system stability.
- [ ] Keep the player objective clear without requiring extensive reading.
- [ ] Use fictional claims and fictional groups.

### S9.2 Preventive mechanics
- [ ] Make verification an actual puzzle action.
- [ ] Make restoring communication a meaningful objective.
- [ ] Show a visible consequence when the player prevents an escalation event.
- [ ] Avoid portraying disagreement alone as dangerous.
- [ ] Avoid glamorizing manipulation or depicting graphic harm.

### S9.3 Narrative review
- [ ] Check that the story follows the actual gameplay.
- [ ] Remove or revise any text that promises mechanics the game does not implement.
- [ ] Confirm the theme is present in player decisions, not only in exposition.

**Exit criteria:** The theme is communicated through mechanics and outcomes, with concise supporting narrative.

## Stage 10 — Visual, UI, and audio refinement

### S10.1 Visual refinement
- [ ] Refine Greg's blob silhouette and eye expressions.
- [ ] Establish a consistent palette and outline style.
- [ ] Improve switch, door, terminal, and connection visuals.
- [ ] Refine echo appearance and overlap readability.
- [ ] Add restrained glow and signal effects.

### S10.2 UI/UX
- [ ] Improve timer, loop count, echo count, and objective readability.
- [ ] Make interaction prompts consistent.
- [ ] Add pause and retry flows.
- [ ] Verify controls are displayed clearly.
- [ ] Check keyboard navigation and text readability where relevant.

### S10.3 Audio
- [ ] Add interaction, switch, echo, timing, door, and success sounds.
- [ ] Add restrained ambient music.
- [ ] Confirm the game remains understandable when muted.
- [ ] Add volume controls if time permits.
- [ ] Verify asset licensing and attribution.

### S10.4 Performance and readability
- [ ] Ensure effects do not obscure puzzle information.
- [ ] Check for excessive flashing or screen shake.
- [ ] Confirm the game runs smoothly on the target machine.

**Exit criteria:** The game has a consistent visual/audio identity without compromising puzzle clarity.

## Stage 11 — Playtesting and stabilization

### S11.1 Gameplay tests
- [ ] Ask a fresh player to try the first room without a verbal explanation.
- [ ] Observe whether they understand the echo mechanic.
- [ ] Record where they get confused.
- [ ] Test the timing modules.
- [ ] Test all intended completion and failure states.
- [ ] Fix blockers before adding new features.

### S11.2 Technical tests
- [ ] Launch from a clean project state.
- [ ] Check for missing resources and C# build errors.
- [ ] Test restarting levels repeatedly.
- [ ] Test echo limits and reset state.
- [ ] Test every included level.
- [ ] Confirm no documentation or project files were unintentionally removed.

### S11.3 Final scope decision
- [ ] Freeze new mechanics.
- [ ] Prioritize bugs and usability issues.
- [ ] Remove unfinished features from the playable build only if they are not required; do not delete their documented future plans.
- [ ] Confirm the submitted build matches the actual implementation.

**Exit criteria:** The included experience is stable and can be completed by someone other than its creator.

## Stage 12 — Build, submission, and promotional video

### S12.1 Build
- [ ] Export a runnable build using the correct Godot .NET export templates.
- [ ] Test the exported build on a clean launch.
- [ ] Include controls and a concise description.
- [ ] Verify all required files are included.
- [ ] Keep a backup of the final build and source state.

### S12.2 Trailer / progress video
- [ ] Capture actual gameplay footage.
- [ ] Show the echo mechanic clearly.
- [ ] Show a puzzle that requires multiple attempts.
- [ ] Explain the prevention theme through player actions.
- [ ] Use Remotion and/or Hyperframes for titles, captions, and editing if useful.
- [ ] Do not present unimplemented mockups as actual gameplay.

### S12.3 Submission review
- [ ] Confirm the submission description matches the game.
- [ ] Disclose AI use as required by the jam.
- [ ] Check asset credits and licenses.
- [ ] Check the official submission requirements and deadline.
- [ ] Submit with enough time to recover from upload or export problems.

**Exit criteria:** A tested playable build and required submission materials are ready.

---

## Change log

- Initial documentation plan: define a reusable, deterministic echo-puzzle foundation before expanding into level variation, narrative, polish, and submission.
- This file is an ongoing plan. Completed checklists should be updated in place; future stages must remain present.
