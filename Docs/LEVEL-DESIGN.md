# LEVEL DESIGN SPECIFICATION — ECHO-ISM

## Campaign Overview

ECHO-ISM is a 2D top-down puzzle adventure where Greg, an abstract digital blob, coordinates with ghostly copies of his past attempts across time loops to investigate and restore a malfunctioning, manipulated digital society ("The Network").

The visual presentation contrasts cute, minimal blob citizens and fluid procedural animations against dark, increasingly destabilized digital network chambers.

---

## Sector 01: Boot Circuit

- **Chapter:** ACT I — The First Distortion
- **Difficulty:** ★☆☆☆☆ (Tutorial / Foundational)
- **Target Playtime:** 2–3 minutes
- **Loop Duration:** 15.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 1 loop)
- **Core Mechanics:** `TestSwitch`, `Door`, `ExitGoal`, Facility Navigation
- **Scene File:** `res://Scenes/Levels/TestRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [START] (360, 366)                                          |
|    G                                                        |
|    |                                                        |
|    v                                                        |
| [POWER SWITCH] (440, 470)                                   |
|    |                                                        |
|    +----------------------> [SECURITY DOOR] (640, 366)      |
|                                     |                       |
|                                     v                       |
| [PUBLIC NOTICE TERMINAL] -----> [EXIT GOAL] (920, 366)      |
|                                                             |
| [DETOUR NORTH] ------> [OPTIONAL FRAGMENT: Lost Transmission]|
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Introduction:** Greg spawns near the west entrance; the central security bulkhead blocks access to the eastern sector.
2. **First Interaction:** Player moves to the Power Switch at (440, 470) and toggles it with `[E]`, energizing the circuit trace and opening the bulkhead door.
3. **Environmental Reading:** Passing through the aperture, Greg encounters a malfunctioning Public Notice Terminal:
   > **SYSTEM STATUS:** NORMAL  
   > **CITIZEN REPORTS:** CONFLICTING  
   > **NETWORK INTEGRITY:** 64%  
   > *Please disregard all previous warnings.*
4. **Optional Discovery (Lost Transmission):** An alcove along the north perimeter reveals a hidden terminal:
   > *"...they told us everything was fine. But why did the warning system activate?"*
5. **Exit:** Reaching the glowing exit terminal at (920, 366) clears the sector.

---

## Sector 02: False Synchrony

- **Chapter:** ACT I — Conflicting Signals
- **Difficulty:** ★★☆☆☆ (Timing & Precision)
- **Target Playtime:** 3–5 minutes
- **Loop Duration:** 15.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 2 loops)
- **Core Mechanics:** `TimingSlider`, `PressurePlate`, `SynchronizationGate`, `TimedDoor`
- **Scene File:** `res://Scenes/Levels/TimingRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [START]                                                     |
|    G                                                        |
|    +-------------------+--------------------+               |
|    |                                        |               |
|    v (Route A)                              v (Route B)     |
| [TIMING SLIDER] (Signal A)              [PRESSURE PLATE]    |
| (Channel A: Evacuation Request)         (Channel B: No Evac)|
|    |                                        |               |
|    +-------------------> [SYNC GATE] <------+               |
|                               |                             |
|                          [TIMED DOOR]                       |
|                               |                             |
|                           [EXIT GOAL]                       |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Forked Channels:** The facility splits into Route A (timing calibration) and Route B (pressure hold).
2. **Conflicting Signals:** Channel A displays *"CITIZENS REQUESTING EVACUATION"*; Channel B displays *"NO EVACUATION REQUIRED"*.
3. **Temporal Synchronization:** Live Greg stops the slider in the target zone (Signal A), while an Echo or timed sprint maintains the pressure plate (Signal B).
4. **Gate Unlock:** Both signals trigger the Synchronization Gate within its tolerance window, opening the timed bulkhead door.
5. **Optional Discovery (Signal Analyst):** A secondary terminal reveals conflicting timestamps: the two contradictory reports were recorded hours apart and presented as simultaneous.

---

## Sector 03: Divided Consensus

- **Chapter:** ACT I — A Society Splits
- **Difficulty:** ★★☆☆☆ (Cooperation & Dual Mechanisms)
- **Target Playtime:** 4–6 minutes
- **Loop Duration:** 16.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 2 loops)
- **Core Mechanics:** `PressurePlate`, `TestSwitch`, `DualMechanismDoor`
- **Scene File:** `res://Scenes/Levels/DualProtocolRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [WEST DISTRICT]                      [EAST DISTRICT]        |
| [START] G                            [EXIT GOAL]            |
|   |                                                         |
|   v                                    ^                    |
| [PRESSURE PLATE]                       |                    |
| (Announcement: East Refuses)      [POWER SWITCH]            |
|   |                               (Announcement: West Refus)|
|   +-------------> [DUAL MECHANISM DOOR] <---+               |
|                       (Dividing Wall)                       |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Divided Transit:** A solid central dividing barrier isolates the West and East transit districts.
2. **First Echo Recording:** Loop 1: Greg records a path to the West District pressure plate and holds position.
3. **Echo Playback:** Loop 2: Echo I replays the hold, while Live Greg runs through the unlocked aperture into the East District to flip the master switch.
4. **Dual Verification:** Both inputs register concurrently, permanently opening the transit passage to the exit.
5. **Optional Discovery (The Mediator's Record):** A forgotten station log:
   > *"We used to share the same station. I don't remember when we stopped talking."*

---

## Sector 04: The Echo Chamber

- **Chapter:** ACT II — Repeating the Past
- **Difficulty:** ★★★☆☆ (Delayed Routing & Echo Resonance)
- **Target Playtime:** 5–7 minutes
- **Loop Duration:** 18.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 3 loops)
- **Core Mechanics:** `EchoSensor`, `SignalRelay`, `Door`, `MovingObstacle`
- **Scene File:** `res://Scenes/Levels/RelaySyncRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [START] G                                                   |
|   |                                                         |
|   v                                                         |
| [ECHO RESONANCE SENSOR] (Responds ONLY to Echo ghost)       |
|   |                                                         |
|   v                                                         |
| [SIGNAL RELAY] (Configurable delay: 2.0s pulse)             |
|   |                                                         |
|   v                                                         |
| [CORRIDOR WITH MOVING HAZARD] (Patrolling barrier)          |
|   |                                                         |
|   v                                                         |
| [TIMED SECURITY BULKHEAD] ---> [EXIT GOAL]                  |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Echo Sensing:** Live Greg cannot activate the Echo Sensor; he must record a ghost loop passing through the sensor zone.
2. **Delayed Transmission:** In Loop 2, Echo I trips the sensor, firing into the Signal Relay. The relay delays the pulse, giving Live Greg time to sprint into position.
3. **Patrol Timing:** Live Greg coordinates passage past a slowly patrolling MovingObstacle while the door is briefly held open.
4. **Optional Discovery (Original Source):** An archive terminal displays three versions of the same message (Original, Repeated, Edited), demonstrating how repetition stripped out crucial context.

---

## Sector 05: Fractured Network

- **Chapter:** ACT II — Information Overload
- **Difficulty:** ★★★★☆ (Multi-Chamber Examination)
- **Target Playtime:** 6–8 minutes
- **Loop Duration:** 16.0–20.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 3 loops)
- **Core Mechanics:** `TestSwitch`, `PressurePlate`, `TimingSlider`, `SynchronizationGate`, `EchoSensor`, `MovingObstacle`
- **Scene File:** `res://Scenes/Levels/RemixStabilizationRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| Chamber 1: Power Matrix (Switch + Timing Slider)            |
| Chamber 2: Resonance Gate (Echo Sensor + Delayed Relay)     |
| Chamber 3: Central Bus (Dual Pressure Plates + Hazard Bar)  |
| Chamber 4: Gateway (Exit Goal)                              |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Multi-Agent Planning:** Player must allocate distinct duties across 3 successive loops:
   - Echo I: Sustains Chamber 1 calibration.
   - Echo II: Activates Chamber 2 resonance sensor.
   - Live Greg: Navigates Chamber 3, timing movements with the hazard obstacle.
2. **Seeded Variations:** Supports reproducible puzzle variants (A, B, C, D) with varied switch offsets and relay delays validated by `LevelVariationParameters.ValidateSolvability()`.
3. **Optional Discovery (Network Archaeology):** Reconstructed logs show how misinformation was seeded across subsystems on a precise timetable.

---

## Sector 06: The Truth Protocol

- **Chapter:** ACT II — Evidence and Context
- **Difficulty:** ★★★★☆ (Authentication & Reconstruction)
- **Target Playtime:** 6–9 minutes
- **Loop Duration:** 16.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 3 loops)
- **Core Mechanics:** `VerificationTerminal`, `ConnectionRepair`, `PressurePlate`, `Door`
- **Scene File:** `res://Scenes/Levels/Sector06VerificationRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [CHAMBER A: ARCHIVE]                                        |
| [VERIFICATION TERMINAL] (Detects hash mismatch / corruption)|
|   |                                                         |
|   v                                                         |
| [CHAMBER B: SEVERED GRID]                                   |
| [CONNECTION REPAIR] (Physically restores broken traces)     |
|   |                                                         |
|   v                                                         |
| [CHAMBER C: EVIDENCE MATRIX]                                |
| [ECHO PRESSURE PLATE] ---> [AUTHENTICATED BULKHEAD]         |
|                                  |                          |
|                                  v                          |
|                             [EXIT GOAL]                     |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Data Mismatch:** The Verification Terminal identifies that stored network reports are corrupted and missing context.
2. **Severed Channel:** Moving to Chamber B, Greg interacts with `ConnectionRepair`, physically bridging severed network wires.
3. **Evidence Reconstruction:** Echo holds the Chamber C pressure plate, enabling Live Greg to submit the verified telemetry packet.
4. **Major Narrative Reveal:** Reconstructed files reveal the reports were not contradictory—they described distinct historical events that bad actors intentionally conflated.
5. **Optional Discovery (The Missing Witness):** A suppressed citizen testimony revealing that earlier corrections were intercepted and deleted before reaching the public board.

---

## Sector 07: Cascade Prevention

- **Chapter:** ACT III — Before the Breaking Point
- **Difficulty:** ★★★★★ (Campaign Climax)
- **Target Playtime:** 8–12 minutes
- **Loop Duration:** 15.0 seconds
- **Max Echoes:** 3 (Efficient Threshold: 3 loops)
- **Core Mechanics:** `EventPreventionObjective`, Dual Stabilizer Switches, `PressurePlate`, `SignalRelay`, `SynchronizationGate`, `ExitGoal`
- **Scene File:** `res://Scenes/Levels/Sector07PreventionRoom.tscn`

### Spatial Layout & Blueprints

```text
+-------------------------------------------------------------+
| [CASCADE BROADCAST HUB]                                     |
| [EVENT PREVENTION TERMINAL] (Countdown to network broadcast)|
|                                                             |
| [STABILIZER SWITCH A] (Live Greg intervention)              |
| [STABILIZER SWITCH B] (Echo 1 synchronized intervention)    |
| [DELAYED BYPASS RELAY] (Echo 2 pulse)                       |
|                                                             |
| ---> [CASCADE OVERRIDE CONFIRMED] ---> [EXIT GOAL]          |
+-------------------------------------------------------------+
```

### Gameplay Progression
1. **Impending Crisis:** The automated broadcast terminal initiates a cascade countdown to broadcast a fabricated emergency alert that will pit the districts against one another.
2. **Multi-Station Containment:**
   - Greg must coordinate with two Echoes across three designated stations.
   - Stabilizer A and B must be activated within the required intervention window.
3. **Escalation Halted:** Completing all interventions terminates the countdown, cancels the broadcast, and restores authentic public network logs.
4. **Campaign Conclusion:**
   > *EMERGENCY BROADCAST CANCELLED.*  
   > *ORIGINAL REPORTS RESTORED.*  
   > *COMMUNICATION CHANNELS REOPENING.*  
   > *"INFORMATION CANNOT REPLACE TRUST. BUT WITHOUT INFORMATION, TRUST CANNOT BE REBUILT."*
5. **Optional Discovery (Final Testimony):** The architect's confession explaining how fear was weaponized when communication broke down.
