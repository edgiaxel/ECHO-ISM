# ECHO-ISM

> **A time-loop puzzle game about cooperation, information integrity, and preventing a digital society from collapsing.**

ECHO-ISM is a 2D top-down puzzle game built with **Godot 4.7.2 .NET / C#**. The player controls **Greg**, a small abstract blob in a miniature digital city. When misinformation destabilizes the city's systems and relationships, Greg must restore trustworthy connections and prevent a chain of harmful events.

The central mechanic is **recorded past attempts**: actions performed during one loop are replayed by ghostly echoes in later loops. The player coordinates with earlier versions of Greg to operate mechanisms, deliver verified information, open routes, and solve challenges that cannot be completed in one attempt.

## Current project status

The initial repository is a minimal Godot project. The documentation defines the intended design and staged implementation; it does **not** claim that the gameplay systems are already implemented.

## Project goals

- Make the past-self echo mechanic immediately understandable and satisfying.
- Build puzzle challenges from reusable, deterministic modules.
- Use small, handcrafted templates that can be recombined into varied levels.
- Communicate the theme through player actions and consequences, not long exposition.
- Use a distinctive 2D digital-drawing style with simple shapes and expressive blob characters.
- Keep the project buildable and testable at every development stage.

## Technology

- **Engine:** Godot 4.7.2 .NET
- **Language:** C#
- **Presentation:** 2D top-down
- **Source control:** Git
- **Anti-slop workflow:** use the repository's Anti-Slop instructions or tooling when available; prefer small, reviewable, tested changes over large speculative rewrites.
- **Promotional video:** Remotion and/or Hyperframes may be used for editing a trailer or progress video. They are not part of the gameplay runtime.

## Documentation map

- [`AGENTS.md`](AGENTS.md) — non-negotiable instructions for coding agents.
- [`GameConcept.md`](GameConcept.md) — premise, narrative, player fantasy, and intended experience.
- [`Gameplay.md`](Gameplay.md) — core loop, echo rules, puzzle modules, level variation, and progression.
- [`Design.md`](Design.md) — visual direction, UI/UX, effects, audio, and accessibility.
- [`Stages.md`](Stages.md) — staged implementation workflow and acceptance checklists.

## Getting started

1. Open the project using the **Godot .NET editor** compatible with Godot 4.7.2.
2. Confirm that the installed .NET SDK and Godot C# support are working.
3. Read `AGENTS.md` before asking a coding agent to modify the project.
4. Follow `Stages.md` in order. Do not skip the skeleton and core-mechanic validation stages.
5. Keep each change small enough to run and verify before proceeding.

## Current implementation principle

**Prove the loop before expanding the world.** First create the project skeleton and a single playable test room. Validate recording, resetting, echo playback, and one puzzle that genuinely requires an earlier attempt. Add more puzzle modules and remixing only after that mechanic is reliable.
