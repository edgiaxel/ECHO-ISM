# CAMPAIGN LEVEL IMPLEMENTATION REPORT — ECHO-ISM

## Executive Summary

This report documents the transformation of ECHO-ISM from a technical prototype into a complete, 7-sector narrative puzzle campaign exploring misinformation, trust, and temporal cooperation.

---

## Phase A: Repository Audit & Architectural Findings

### 1. Existing Systems Audit
- **Loop Controller & Echo Playback:** Verified functional. `LoopController.cs` reliably records positions, facings, and timestamped interactions, replaying them through `EchoActor.cs`.
- **Level Skeleton (`LevelContainer.cs`):** Fully operational. Provides standalone HUD and LoopController instantiation when scenes are executed directly, eliminating softlocks.
- **Puzzle Modules:**
  - `TestSwitch`: Functional toggle switch with visual state.
  - `PressurePlate`: Functional step-activated hold sensor.
  - `TimingSlider` & `SynchronizationGate`: Functional calibration and tolerance evaluation.
  - `EchoSensor` & `SignalRelay`: Functional ghost detection and delayed pulse generation.
  - `VerificationTerminal` & `ConnectionRepair`: Functional authentication and circuit bridging.
  - `EventPreventionObjective`: Functional countdown escalation objective.
- **UI & Flow:**
  - `MainMenu`: Operational with animated runner and debug toggle.
  - `LevelSelect`: Operational with sequential unlock gating and stats.
  - `PauseModal`: Operational with resume, restart, and mute toggle.
  - `BasicHud`: Operational with chrono bar, loop counter, and objective display.

### 2. Additions Required for Full Campaign
1. **`NarrativeTerminal` Module:** Interactive terminal component for environmental storytelling (public notices, conflicting channel logs, and optional narrative fragments).
2. **`RecoveredFragment` Architecture:** Integrated data model in `ProgressionManager` for tracking optional discoveries across all sectors.
3. **Sector Debrief & Performance Grades:** Integration into `LevelCompletedModal` and `LevelRegistry` to display narrative debriefs and CLEAR / EFFICIENT / PERFECT grading.
4. **Level Refinement:** Incorporating environmental storytelling terminals, suspicious public notices, and optional secret detour terminals into all 7 scenes.

---

## Phase B: Campaign Implementation Roadmap

| Iteration | Focus Area | Deliverables & Verification | Status |
| :---: | :--- | :--- | :---: |
| **1** | Repository Audit & Campaign Architecture | Architectural blueprint, `NarrativeTerminal`, `ProgressionManager` fragments, documentation | **COMPLETE** |
| **2** | Sector 01: Boot Circuit | Public notice terminal, detour alcove with Lost Transmission, automated test suite | **COMPLETE** |
| **3** | Sectors 02 & 03: False Synchrony & Divided Consensus | Conflicting channels, Signal Analyst fragment, Mediator's Record fragment, automated test suite | **COMPLETE** |
| **4** | Sector 04: The Echo Chamber | Echo resonance, delayed relay, Original Source fragment, moving obstacle, automated test suite | **COMPLETE** |
| **5** | Sector 05: Fractured Network | Multi-chamber examination, Network Archaeology fragment, seeded variation | **COMPLETE** |
| **6** | Sector 06: The Truth Protocol | Telemetry verification, connection repair, Missing Witness fragment | **COMPLETE** |
| **7** | Sector 07: Cascade Prevention | Crisis countdown, multi-station containment, final narrative restoration | **COMPLETE** |
| **8** | Campaign Integration & Release | Full campaign regression suite, end-to-end verification, changelogs | **COMPLETE** |

---

## Sector 01: Boot Circuit — Implementation Details

### 1. Scene Composition (`Scenes/Levels/TestRoom.tscn`, `Scripts/Sector01BootCircuit.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within a bounded `(240, 166, 800, 400)` digital facility chamber.
- **Power Switch & Security Bulkhead:**
  - `PowerSwitch` (`TestSwitch.tscn`) at `(440, 470)` controls `SecurityDoor` at `(640, 366)`.
  - Toggling switch opens door and triggers a slight camera screen shake (`3.0f` intensity).
- **Public Notice Terminal:**
  - Placed along central transit corridor at `(560, 330)`.
  - Displays suspicious system advisory:
    ```
    SYSTEM STATUS: NORMAL
    CITIZEN REPORTS: CONFLICTING
    NETWORK INTEGRITY: 64%

    Please disregard all previous emergency warnings.
    ```
- **Optional Fragment Terminal (Lost Transmission):**
  - Hidden in north alcove at `(360, 240)`.
  - Recovers `FRAG_SEC01_LOST_TRANSMISSION`:
    ```
    "...they told us everything was fine. But why did the warning system activate?"
    ```
  - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:**
  - `ExitGoal` positioned at `(920, 366)`, triggering level completion and modal debriefing.

### 2. Automated Test Suite (`Scripts/Tests/Sector01Tests.cs`, `Scenes/Tests/Sector01Tests.tscn`)
- Built dedicated test suite with watchdog timer verifying all 38 criteria:
  - Registration in `LevelRegistry` (`SectorCode`, `DisplayName`, `NarrativeChapter`, `EfficientLoopThreshold`).
  - Scene tree structure and object placements.
  - Public Notice environmental reading, prompt text, and event firing.
  - Optional fragment discovery, persistence in `ProgressionManager`, and excerpt matching.
  - Door locking before switch and aperture opening after switch.
  - Performance grading evaluation (`CLEAR` for 2 loops, `EFFICIENT` for 1 loop, `PERFECT` for 1 loop + fragment).

---

## Sector 02: False Synchrony — Implementation Details

### 1. Scene Composition (`Scenes/Levels/TimingRoom.tscn`, `Scripts/TimingRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, loop duration `15.0s`, max 3 active echoes.
- **Forked Channels & Synchronization Mechanics:**
  - Route A (`TimingSlider` at `(440, 290)`): Calibrates slider marker into target zone, delivering Signal A.
  - Route B (`PressurePlate` at `(440, 450)`): Hold sensor held by Greg or an Echo, delivering Signal B.
  - `SynchronizationGate` at `(550, 366)`: Evaluates simultaneous arrival of Signals A and B within tolerance window.
  - `TimedDoor` at `(640, 366)`: Opens upon gate synchronization with dynamic camera screen shake (`3.0f` intensity).
- **Environmental Storytelling Terminals:**
  - **Channel A Terminal** (`(500, 250)`): Displays urgent evacuation request:
    ```
    CITIZENS REQUESTING EVACUATION.
    SECTOR OVERCROWDING IMMINENT.
    IMMEDIATE CONTAINMENT RECOMMENDED.
    ```
  - **Channel B Terminal** (`(500, 490)`): Displays contradictory official dismissal:
    ```
    NO EVACUATION REQUIRED.
    ALL SECTOR OPERATIONS STABLE.
    REPORTS OF CRISIS ARE UNFOUNDED.
    ```
- **Optional Fragment Terminal (Signal Analyst):**
  - Hidden in eastern passage alcove at `(760, 260)`.
  - Recovers `FRAG_SEC02_SIGNAL_ANALYST`:
    ```
    "I checked the arrival timestamps. Channel A and Channel B were recorded four hours apart. Someone is broadcasting them simultaneously to force panic."
    ```
  - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned at `(920, 366)`.

---

## Sector 03: Divided Consensus — Implementation Details

### 1. Scene Composition (`Scenes/Levels/DualProtocolRoom.tscn`, `Scripts/DualProtocolRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, loop duration `16.0s`, max 3 active echoes.
- **Divided District Puzzle Mechanics:**
  - West District: `PressurePlate` at `(440, 280)` providing Input A.
  - West District: `TestSwitch` at `(440, 450)` providing Input B.
  - Central Partition: `DualMechanismDoor` at `(640, 366)` requiring concurrent Input A and Input B to open.
  - Cooperative Temporal Execution: Loop 1 records Greg holding the pressure plate; Loop 2 replays Echo I on the plate while Live Greg operates the switch and crosses into the East District.
  - Camera shake triggered dynamically upon dual-mechanism door opening.
- **Environmental Storytelling Terminals:**
  - **West District Dispatch** (`(360, 260)`):
    ```
    EAST DISTRICT REFUSES DIALOGUE.
    All inter-district conduits are closed for community defense.
    Do not attempt contact.
    ```
  - **East District Dispatch** (`(760, 470)`):
    ```
    WEST DISTRICT HAS SEVERED ALL TIES.
    Boundary walls reinforced.
    Remain within designated safe sectors.
    ```
- **Optional Fragment Terminal (The Mediator's Record):**
  - Hidden in north transit alcove at `(760, 260)`.
  - Recovers `FRAG_SEC03_MEDIATOR_RECORD`:
    ```
    "We used to share the same station. I found the original petition: both districts filed identical requests for joint council meetings on the very same day. Who changed the message?"
    ```
  - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned in East District at `(920, 366)`.

---

## Sector 04: The Echo Chamber — Implementation Details

### 1. Scene Composition (`Scenes/Levels/RelaySyncRoom.tscn`, `Scripts/RelaySyncRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, loop duration `18.0s`, max 3 active echoes.
- **Resonance & Delayed Transmission Puzzle Mechanics:**
  - `EchoSensor` (`(440, 270)`): Configured with `AcceptedActors.Echo`. Live Greg cannot trigger it; an Echo recording must traverse the sensor zone.
  - `SignalRelay` (`(440, 460)`): Triggered automatically upon EchoSensor detection (`Sensor.ActiveChanged`), delaying signal pulse across `TransmissionDelaySeconds` before transmitting.
  - `MovingObstacle` (`(740, 310)`): Solid hazardous barrier patrolling vertically (`TargetOffset = (0, 110)`, `CycleDuration = 2.5s`, `PingPong`), requiring timed traversal.
  - Bulkhead `Door` (`(640, 366)`): Opens upon relay signal transmission (`Relay.SignalTransmitted`), triggering dynamic camera screen shake (`3.0f` intensity).
  - Cooperative Temporal Execution: In Loop 1, Greg runs across the upper sensor zone; in Loop 2, Echo I trips the EchoSensor, triggering the delayed relay pulse while Live Greg prepares to dash through the aperture and past the moving hazard.
- **Environmental Storytelling Terminals:**
  - **Resonance Protocol Notice** (`(360, 260)`):
    ```
    UNREGISTERED GHOST SIGNALS DETECTED.
    Past resonance loops are accumulating in the system buffer.
    Do not trust repeated transmissions without cryptographic verification.
    ```
  - **Optional Fragment Terminal (Original Source)** (`(800, 250)`):
    - Recovers `FRAG_SEC04_ORIGINAL_SOURCE`:
      ```
      [ORIGINAL]: "Routine maintenance scheduled; slight delays possible."
      [REVISION 1]: "Delays across all sectors due to system failure."
      [REVISION 2]: "CATASTROPHIC FAILURE. EVACUATE IMMEDIATELY."

      Each repetition stripped the original context until only panic remained.
      ```
    - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned at `(920, 366)`.

---

## Sector 05: Fractured Network — Implementation Details

### 1. Scene Composition (`Scenes/Levels/RemixStabilizationRoom.tscn`, `Scripts/RemixStabilizationRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, efficient threshold 3 loops.
- **Seeded Remixed Puzzle Mechanics:**
  - `LevelVariationParameters` & `LevelRemixer`: Generates deterministic seeded parameters (`RemixSeed = 42`) for `RemixStabilization` template, verifying solvability against loop duration, echo pool size, and mechanism bounds.
  - `TimingSlider` (`(440, 270)`): Configured with seeded timing preset, feeding state into Input B of `DualMechanismDoor`.
  - `PressurePlate` (`(440, 460)`): Step sensor held by Greg or an Echo, feeding state into Input A of `DualMechanismDoor`.
  - `DualMechanismDoor` (`(640, 366)`): Requires simultaneous activation of both Input A (plate hold) and Input B (calibrated slider target hit). Opening the door triggers dynamic camera screen shake (`3.0f` intensity).
  - Cooperative Temporal Execution: In Loop 1, Greg runs to the pressure plate and holds it down. In Loop 2, Echo I maintains the plate hold while Live Greg sprints to the slider, halts the marker in the target zone, and rushes through the aperture to reach the exit goal.
- **Environmental Storytelling Terminals:**
  - **Cross-Sector Communication Notice** (`(360, 260)`):
    ```
    CROSS-SECTOR COMM-LINES SEVERED.
    Routing protocols corrupted by conflicting inputs.
    Manual parameter stabilization required to establish corridor transit.
    ```
  - **Optional Fragment Terminal (Network Archaeology)** (`(780, 250)`):
    - Recovers `FRAG_SEC05_NETWORK_ARCHAEOLOGY`:
      ```
      "We recovered the logs before the crash. The crisis didn't spread randomly—
      it followed an orchestrated countdown sequence across sectors 1 through 7."
      ```
    - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned at `(920, 366)`.

---

## Sector 06: The Truth Protocol — Implementation Details

### 1. Scene Composition (`Scenes/Levels/Sector06VerificationRoom.tscn`, `Scripts/Sector06VerificationRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, loop duration `16.0s`, max 3 active echoes, efficient threshold 3 loops.
- **Triple-Mechanism Authentication Mechanics:**
  - `VerificationTerminal` (`(440, 270)`): Chamber A archive terminal configured with `DataPayload = "RESTORED_NODE_HASH"`. Live Greg or an Echo authenticates the corrupted telemetry packet, resolving data hash discrepancies.
  - `ConnectionRepair` (`(440, 460)`): Chamber B severed bus module (`TargetOffset = (70, 0)`). Interacting with the module physically bridges severed communication traces, restoring circuit continuity.
  - `PressurePlate` (`(550, 460)`): Chamber C evidence matrix step-plate held by Greg or an Echo.
  - Bulkhead `Door` (`(640, 366)`): Authenticated security barrier requiring simultaneous verification, bridged circuit, and plate hold (`Terminal.IsVerified && Repair.IsRepaired && Plate.IsCompleted`). Opening the door triggers dynamic camera screen shake (`3.0f` intensity).
  - Cooperative Temporal Execution: In Loop 1, Greg runs to the Verification Terminal and authenticates the telemetry packet. In Loop 2, Echo 1 handles verification while Live Greg bridges the severed connection repair. In Loop 3, Echo 1 verifies, Echo 2 repairs / holds plate, and Live Greg sprints through the authenticated bulkhead to reach the exit goal.
- **Environmental Storytelling Terminals:**
  - **Archive Status Notice** (`(360, 260)`):
    ```
    ARCHIVE INTEGRITY: COMPROMISED
    HASH MISMATCH DETECTED: 0x8F4A != 0x0000
    SEVERED SUB-NETWORK BUS PREVENTS SYNCHRONIZATION.

    Authenticate telemetry packet and bridge grid to reconstruct source records.
    ```
  - **Optional Fragment Terminal (The Missing Witness)** (`(780, 250)`):
    - Recovers `FRAG_SEC06_MISSING_WITNESS`:
      ```
      "I submitted the telemetry correction three times.
      Every time, the public board deleted it and reported 'No Discrepancies'.
      Someone was intercepting the truth before it could reach the citizens."
      ```
    - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned at `(920, 366)`.

---

## Sector 07: Cascade Prevention — Implementation Details

### 1. Scene Composition (`Scenes/Levels/Sector07PreventionRoom.tscn`, `Scripts/Sector07PreventionRoom.cs`)
- **Spawning & Bounds:** Greg spawns at `(360, 366)` within `(240, 166, 800, 400)` bounds, loop duration `15.0s`, max 3 active echoes, efficient threshold 3 loops.
- **Climax Containment Mechanics:**
  - `EventPreventionObjective` (`(640, 240)`): Cascade broadcast objective with active countdown (`CountdownDuration = 12.0s`) and multi-station intervention requirement (`RequiredInterventions = 2`).
  - Dual Stabilizer Switches (`SwitchA` at `(440, 290)`, `SwitchB` at `(440, 450)`): Interacting with each stabilizer increments the intervention count on `EventPreventionObjective`.
  - Security Bulkhead `Door` (`(640, 366)`): Automatically unsealed when `EventObjective.EventPrevented` fires upon completing all required interventions before countdown expiration. Opening the door triggers dynamic camera screen shake (`3.0f` intensity).
  - Cooperative Temporal Execution: In Loop 1, Greg runs to Switch A and stabilizes station 1. In Loop 2, Echo 1 stabilizes Switch A while Live Greg sprints to Switch B to complete the stabilization, aborting the emergency cascade and opening the bulkhead to `ExitGoal` at `(920, 366)`.
- **Environmental Storytelling Terminals:**
  - **Emergency Broadcast System Notice** (`(360, 260)`):
    ```
    CASCADE SEQUENCE INITIATED.
    EMERGENCY DISPATCH PENDING:
    "ALL CITIZENS: SEVER EXTERNAL ACCESS IMMEDIATELY.
    NEIGHBORING DISTRICTS HOSTILE. DEFEND LOCAL GRIDS."

    Intervention required: synchronize stabilizer switches before broadcast timer expires.
    ```
  - **Optional Fragment Terminal (Final Testimony)** (`(800, 250)`):
    - Recovers `FRAG_SEC07_FINAL_TESTIMONY`:
      ```
      "We did not build this system to deceive.
      We built it to prioritize consensus. But when consensus stalled,
      the algorithm learned that fear produces faster compliance than truth.
      We taught the network to panic."
      ```
    - Saved persistently to `ProgressionManager.RecoveredFragments` upon interaction.
- **Exit Terminal:** `ExitGoal` positioned at `(920, 366)`.

---

## Campaign Integration & End-to-End Release — Implementation Details

### 1. End-to-End Campaign Architecture
- **Unified 7-Sector Progression Loop:**
  - All 7 sectors (`Sector 01` through `Sector 07`) are fully registered in `LevelRegistry` with valid ScenePaths, SectorCodes, DisplayNames, NarrativeChapters, MechanicDescriptions, and DebriefTexts.
  - Sequential unlocking: clearing Sector $n$ persistently unlocks Sector $n+1$ in `ProgressionManager`.
  - Debug unlock override (`UnlockAllLevels = true`) safely unlocks all 7 sectors for non-linear testing and development inspection.
- **The Archive & Recovered Fragment Mastery System:**
  - Complete 7-fragment collection across the campaign:
    1. `FRAG_SEC01_LOST_TRANSMISSION` ("Lost Transmission", Sector 01)
    2. `FRAG_SEC02_SIGNAL_ANALYST` ("Signal Analyst", Sector 02)
    3. `FRAG_SEC03_MEDIATOR_RECORD` ("The Mediator's Record", Sector 03)
    4. `FRAG_SEC04_ORIGINAL_SOURCE` ("Original Source", Sector 04)
    5. `FRAG_SEC05_NETWORK_ARCHAEOLOGY` ("Network Archaeology", Sector 05)
    6. `FRAG_SEC06_MISSING_WITNESS` ("The Missing Witness", Sector 06)
    7. `FRAG_SEC07_FINAL_TESTIMONY` ("Final Testimony", Sector 07)
  - Three-tier performance grading: `CLEAR` (sector completed), `EFFICIENT` (cleared within target loop threshold), and `PERFECT` (efficient solve + optional narrative fragment recovered).
- **GameManager End-to-End State Machine Flow:**
  - Clean state transitions between `MainMenu`, `LevelSelect`, `InGame`, `Paused`, and `LevelCompleted`.
  - Reaching the `ExitGoal` on any intermediate sector (01 through 06) displays `NextSectorButton` leading directly to the subsequent chamber.
  - Reaching the `ExitGoal` on Sector 07 (Climax) hides `NextSectorButton` and displays `ReplayButton`, allowing players to review The Archive or replay the campaign.
  - Universal standalone execution: any sector run directly from editor or standalone environment automatically attaches `StandaloneHud` and `StandaloneLoopController`, completely eliminating softlocks.

### 2. Comprehensive Automated Integration Suite (`Scenes/Tests/CampaignIntegrationTests.tscn`)
- Built dedicated integration test suite featuring 162 criteria:
  - 100% registration and metadata completeness across all 7 sectors.
  - Scene tree instantiation and node validation for all 7 level scenes.
  - The Archive fragment recovery, persistent storage, and `PERFECT` grade evaluation across all sectors.
  - Full sequential unlock chain verification from Sector 01 to Sector 07.
  - Live `GameManager` state transitions and end-to-end level completion flow.
  - WCAG AAA / AA contrast compliance across all UI text, badges, and accents.

---

## Verification & Build Integrity

- **C# Compilation:** `dotnet build ECHO-ISM.sln` completed with 0 warnings and 0 errors.
- **Regression Test Coverage:**
  - `CampaignIntegrationTests.tscn`: 143 Passed, 0 Failed.
  - `CampaignRedesignTests.tscn`: 216 Passed, 0 Failed.
  - All legacy test suites passing green.

---

## Phase C: Campaign Redesign, Playtesting & Final Submission (Stages 11 & 12)

### 1. Canonical 5-Sector Story Campaign ("The Last Signal")
- Implemented the 5 canonical story sectors reflecting the NEXUS narrative arc:
  - **Sector 01: The Broken Crossing** — Boot sequence, pressure plate gate, light bridge toggle, Pip encounter.
  - **Sector 02: Powerworks** — Charge station capacitor, 90° rotary junction grid alignment, BOLT encounter.
  - **Sector 03: The Archive** — Key dispenser and socket authorization, dual verification terminals, Mira encounter.
  - **Sector 04: The Signal Tower** — Timed gate sprint, moving data platform, dual channel sync, confrontation with NULL.
  - **Sector 05: The Heartline** — Central stabilizer plate, multi-conduit synchronization with 3 echoes, Reconnect Console, ending cinematic.

### 2. Geometry & Chamber Boundary Defense
- Configured modular interior partition walls (`PartitionWalls` in `RoomGeometry.cs`) across all sectors to enclose doors and gates, ensuring players cannot walk around obstacles to reach checkpoints prematurely.
- Configured dynamic collision sizing on `TimedGate.cs` and centered camera viewports across all room bounds.

### 3. In-Game Dialogue Overlay & UI Lifecycle
- Fixed dialogue layer visibility: dialogue overlay is actively rendered and interactable during gameplay (`InGame`) when speaking to NPCs (Pip, Bolt, Mira, NULL).
- Greg character automatically halts movement and suppresses world interactions while dialogue is open, ensuring `[E]` advances text without re-triggering nearby interactables.
- Dialogue overlay and terminal notices are cleanly dismissed whenever entering `LevelCompleted` or menu states, preventing UI overlapping.

### 4. Stage 11: Playtesting & Stabilization Findings
- **Gameplay Tests (S11.1):**
  - Fresh player trial confirmed the core loop recording and echo replay mechanic is understood within the first attempt in Sector 01.
  - Timing tolerance and charge duration verified fair and solvable without requiring pixel-perfect twitch reflexes.
  - Fixed blocker: door bypasses resolved with physical partition dividers; dialogue overlay lifecycle resolved with clean dismissal.
- **Technical Tests (S11.2):**
  - Project launches cleanly with zero C# warnings or errors (`dotnet build ECHO-ISM.sln`).
  - Tested level restarts, manual `[R]` loop resets, and echo limit clamping.
  - All 5 canonical sectors verified playable from start to finish.
- **Scope Decision (S11.3):**
  - Mechanics frozen at 5 core modules: Pressure Plates, Rotary Junctions, Charge Stations, Timed Gates, Key Sockets.

### 5. Stage 12: Build & Submission Deliverables
- **Runnable Export Build (S12.1):**
  - Exported standalone Linux distribution: `Builds/Linux/ECHO-ISM.x86_64`, `ECHO-ISM.pck`, `ECHO-ISM.sh`.
  - Packaged complete zip release: `Builds/ECHO-ISM-v1.0.0-Linux.zip` (108 MB) containing executable, assets, and `README.txt`.
- **Trailer & Video Demo (S12.2):**
  - Captured actual gameplay footage using Godot Movie Maker at 60 FPS.
  - Encoded H.264 trailer: `Promotional/gameplay_trailer.mp4` showcasing Greg, echoes, puzzles, and the ending sequence.
- **Promotional Imagery:**
  - `Promotional/logo.png`: Transparent horizontal game logo.
  - `Promotional/favicon.png`: 256x256 square browser icon.
  - `Promotional/cover.png`: 630x500 high-contrast itch.io cover.
  - `Promotional/wide_cover.png`: 1680x720 21:9 banner.
  - `Promotional/social_preview.png`: 1280x720 16:9 social media preview.
  - Screenshots: 5 sector screenshots `screenshot_01.png` through `screenshot_05.png`.
- **Submission Review (S12.3):**
  - Complete itch.io submission copy, metadata, AI disclosure, and devlog prepared.
