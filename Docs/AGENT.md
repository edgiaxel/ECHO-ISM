# AGENTS.md — Rules for Coding Agents

These instructions are mandatory for Greg, Gemini, Antigravity, or any other coding agent working on ECHO-ISM.

## 1. Project identity and technical constraints

- Project name: **ECHO-ISM**.
- Engine: **Godot 4.7.2 .NET**.
- Language: **C#** for gameplay code unless a specific Godot resource is more appropriate in the editor.
- Perspective: **2D top-down**.
- Main character: **Greg**, an abstract blob with expressive eyes.
- Core mechanic: actions from previous loops are recorded and replayed as ghost echoes that help the current Greg solve a puzzle.
- Theme: preventing digital-society collapse by restoring information integrity, connection, and cooperation.
- Do not add combat, realistic human characters, or a large dialogue-driven campaign without explicit approval.

## 2. Documentation preservation — CRITICAL

**Never delete documentation content. Never replace an entire documentation file just to update a checklist or status.**

When updating documentation:

1. Read the entire relevant file before editing.
2. Preserve every existing heading, section, requirement, design decision, future stage, and checklist item unless Axel explicitly asks to remove or revise it.
3. Update checklist markers in place (`[ ]` to `[x]`) only when the acceptance criterion has actually been met and verified.
4. Make narrow, additive edits. If a requirement changes, preserve the old context where useful and record the change in a clearly labeled decision or changelog section.
5. Never remove stages because they are not part of the current sprint. Future stages must remain in `Stages.md`.
6. Never truncate a file, regenerate it from a summary, or replace it with a shorter version.
7. Before finishing, inspect the diff and confirm no unrelated documentation content disappeared.
8. If a requested change appears to conflict with existing documentation, explain the conflict and ask Axel instead of silently choosing.
9. Do not mark an item complete merely because code was written. It must be run or tested as appropriate.
10. Do not rename or relocate documentation files without explicit approval.

This rule applies to `README.md`, `AGENTS.md`, `GameConcept.md`, `Gameplay.md`, `Design.md`, `Stages.md`, and any future documentation.

## 3. Workflow rules

- Read `README.md`, `GameConcept.md`, `Gameplay.md`, `Design.md`, and the current position in `Stages.md` before implementing a feature.
- Work only on the current stage and its explicitly approved sub-stage.
- Use the smallest change that proves the intended behavior.
- Prefer reusable scenes, components, and puzzle modules over copying an entire level implementation.
- Keep puzzle logic deterministic wherever practical. The same state and action sequence should produce the same result.
- Use placeholder shapes before creating detailed art.
- Do not add external dependencies unless necessary and approved.
- Do not perform broad refactors while implementing a gameplay feature.
- Preserve existing project settings, import settings, and user work.
- Never overwrite project files without inspecting them first.
- Keep runtime gameplay independent from Remotion and Hyperframes. Those tools are only for promotional video production.
- Use Anti-Slop guidance/tooling if present in the project environment. Do not invent commands for a tool whose instructions have not been inspected.

## 4. Required implementation cycle

For every stage or sub-stage:

1. **Inspect:** read relevant files, scenes, scripts, project settings, and current Git status.
2. **Plan:** state the intended files and smallest implementation approach.
3. **Implement:** make only the scoped changes.
4. **Run:** launch the project or run the most relevant automated checks.
5. **Verify:** test the acceptance criteria directly.
6. **Review:** inspect the diff, including documentation changes.
7. **Report:** summarize files changed, test results, known limitations, and the next checklist items.

Do not claim a feature works unless it has been run or otherwise verified.

## 5. Echo-system invariants

Unless Axel explicitly approves a design change:

- An echo replays a recorded attempt; it does not make fresh decisions.
- Echo playback must be deterministic and tied to the loop timeline.
- Echoes are visual ghosts and do not block the player by default.
- Echoes may activate designated echo-compatible mechanisms.
- Echoes do not automatically affect every object. Each interactable declares which actors and interactions it accepts.
- A loop has a finite duration and the game has a finite echo limit.
- Resetting must not silently create unlimited permanent echoes.
- Level-specific collision or solidity is an explicit mechanic configured per echo or puzzle module, not a global accidental behavior.
- The player must be able to understand why a puzzle succeeded or failed.

## 6. Scope and quality guardrails

- Do not implement procedural generation before a handcrafted template can be completed and tested.
- Do not add random outcomes to puzzles whose solution depends on timing.
- Do not add story dialogue as a substitute for a missing mechanic.
- Do not add a feature only because it is easy to generate; it must support the design.
- Prefer a small number of well-tested puzzle modules over many shallow gimmicks.
- Avoid premature abstraction. Extract a reusable component after a real repeated use case is understood.
- Ensure the game can be played with keyboard controls and clear on-screen feedback.
- Keep AI-generated content, code, and assets under human review; disclose AI use where the jam submission requires it.

## 7. Documentation checklist protocol

When a stage is completed:

- Update only the relevant existing checklist items in `Stages.md`.
- Add a concise implementation note under that stage if useful.
- Do not delete the acceptance criteria.
- Do not remove future stages or rewrite them as completed.
- If new work is discovered, append a new checklist item or sub-stage without erasing existing items.
- If a stage is blocked, leave its unchecked items in place and explain the blocker.

## 8. Definition of done

A feature is done only when:

- Its intended behavior is implemented.
- The project runs in the target Godot .NET environment.
- The relevant acceptance criteria have been tested.
- Known limitations are documented.
- Existing project files and documentation have been preserved.
- The final diff contains no unrelated destructive changes.
