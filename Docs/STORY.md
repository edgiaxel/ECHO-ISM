1. The story: The Last Signal
Yuurei: I want to change the premise slightly. Greg shouldn't just be repairing a collection of terminals. He should be trying to save someone, and that someone should give the journey a reason to exist.

Yuuri: What if the city has a little community of blobs who depend on a shared communication network? They have different jobs, help one another, and live peacefully. Then one false emergency broadcast causes everything to go wrong...
The premise
The setting is NEXUS, a miniature digital city powered by a central network called the Heartline.

The Heartline connects the city's districts, allowing residents to communicate, exchange information, and operate shared infrastructure.

One day, an anonymous signal spreads throughout NEXUS:
"The central network has been compromised. The other districts are deliberately shutting us out. Do not trust their messages."

The message is false, but it contains enough believable fragments to create confusion.

Residents begin disconnecting communication relays. Transport systems stop coordinating. Emergency routes become inaccessible. A power-routing system begins overloading.

The city is approaching a cascading failure.

Greg, a small maintenance blob, discovers that the original emergency signal was manipulated. He has a special ability: Chrono Echo, which records his actions and allows previous versions of himself to repeat them.

He cannot repair the whole network in one attempt. He must coordinate with his past selves to recover evidence, restore communication, and prevent the city's central system from collapsing.
Main characters
Character	Appearance	Role
Greg	Cyan blob	Main protagonist; maintenance and investigation
Pip	Green blob	Courier and Greg's friend; knows the city's routes
Mira	Yellow blob	Archive keeper; preserves original network records
BOLT	Orange geometric blob	Power-station operator; initially distrusts Greg
NULL	Dark violet signal entity	Antagonist manipulating the communication network

Yuume: Pip is important. Don't make every character a terminal with eyes. Give the player someone to care about.
The antagonist
NULL isn't a giant monster that Greg has to fight.

NULL is a corrupted signal intelligence that manipulates messages, removes context, impersonates network notifications, and selectively distributes information.

It exploits existing uncertainty between districts.

Its objective is to disconnect the Heartline and take control of the city's central routing system.

The player gradually discovers that the messages causing conflict are connected to the same source.

Yuuko: This is also a better thematic representation. The antagonist manipulates information, but the player wins through verification, communication, and cooperation. The game doesn't need to teach the player how to manipulate people.
The emotional arc
1. Greg discovers that the city is in danger.
2. Greg tries to repair the network alone and realizes that he cannot.
3. Greg meets Pip and starts investigating the source of the false broadcast.
4. Greg gathers evidence and earns the trust of other residents.
5. Greg discovers NULL's involvement.
6. Greg coordinates multiple echoes to restore the Heartline.
7. The city reconnects, and the residents begin rebuilding their community.
The ending should show the residents working together, not Greg simply pressing a giant button and saving everything.

2. Campaign structure — five substantial levels
Yuusei: Five levels. Not seven tiny rooms. We have a deadline, not a studio production cycle.

Yuura: AND THEY SHOULD FEEL LIKE PLACES! Not rectangles floating in the void!

I propose a connected, predominantly vertical progression through NEXUS. Each level is a larger environment made from smaller puzzle sections.

The player should travel through multiple connected spaces, backtrack when appropriate, and gradually unlock new routes.
Level 01 — Boot Sequence: The Broken Crossing
Story: Greg discovers the city's emergency network is failing. Pip is trapped on the other side of a damaged crossing.

Environment: A maintenance station connected to a broken bridge, a power room, and an exit corridor.

Puzzle sequence:
1. Greg enters the maintenance station.
2. A pressure plate opens the first security gate.
3. Greg records an attempt holding the plate.
4. The echo keeps the gate mechanism active while Greg takes the alternate route.
5. Greg reaches the power switch and restores the bridge.
6. Greg crosses and finds Pip.
Mechanics introduced:
- Echo cooperation.
- Pressure plates.
- Doors and gates.
- Alternate routes.
- Basic multi-loop planning.
Story moment: Pip explains that residents in other districts have received a warning claiming that the central network is deliberately isolating them.

Cutscene: Greg and Pip examine a damaged broadcast terminal. A short fragment of the original message appears, but its source is missing.
Level 02 — Powerworks: The Rotating Grid
Story: Greg and Pip need to restore power to the communication relay. BOLT, the station operator, refuses to help because he believes another district sabotaged the power system.

Environment: A large industrial power station with connected rooms, rotating platforms, energy conduits, and locked maintenance doors.

Puzzle sequence:
1. Enter the lower power chamber.
2. Activate a charging mechanism.
3. Route energy through a rotating junction.
4. Use an echo to hold a power switch while Greg reaches the rotation controls.
5. Align three power conduits.
6. Restore the upper lift.
7. Reach BOLT.
Mechanics introduced:
Mechanic	Function
Rotary junction	Redirects an energy path
Charging station	Requires holding an activation point
Energy conduit	Transfers power between connected nodes
Multi-position switch	Rotates between different states
Lift	Opens when the correct power route is established

Custom puzzle: The player must activate a charging station, then use an echo to maintain its charge while the current Greg rotates the junction into the correct position.

Story moment: BOLT initially refuses to cooperate. Greg discovers that the original power shutdown command was issued from an unauthorized source.

BOLT agrees to help after seeing the evidence.
Level 03 — The Archive: Truth Protocol
Story: Greg and Pip travel to the information archive to recover the original broadcast and establish what actually happened.

Environment: A library-like digital archive with multiple floors, storage chambers, rotating data bridges, and sealed records.

Puzzle sequence:
1. Reach the archive entrance.
2. Find the missing authentication key.
3. Activate two archive terminals using separate echoes.
4. Align the data relay.
5. Recover the original broadcast.
6. Compare it with the altered message.
7. Deliver the verified evidence to the central communication node.
Mechanics introduced:
- Data keys.
- Locked storage containers.
- Sequential switches.
- Signal routing.
- Verification terminals.
- Multi-echo synchronization.
Custom puzzle: A keycard is accessible from one route, but the exit is locked behind another route. The player must use an earlier echo to keep the keycard retrieval mechanism active while the current Greg opens the second route.

Important implementation detail: the current echo recorder captures movement and interaction events. It does not yet provide a general-purpose persistent inventory history. Therefore, start with a fixed key socket or key-release mechanism that can be activated through the existing interaction recording system. Only implement recorded item carrying if time allows.

Story moment: The original message reveals that the warning was altered. The districts were never instructed to isolate one another.

NULL's signal signature appears in the archive.
Level 04 — Signal Tower: The False Broadcast
Story: NULL realizes that Greg has recovered the original information. It begins actively sabotaging the communication network.

Environment: A tall communication tower with vertical traversal, moving platforms, timed barriers, and multiple signal routing chambers.

Puzzle sequence:
1. Enter the lower tower.
2. Restore the first relay.
3. Coordinate echoes to keep multiple signal channels active.
4. Avoid temporary signal disruptions.
5. Align the broadcast antenna.
6. Reach the upper control chamber.
7. Send the verified message to the city.
Mechanics introduced:
- Timed signals.
- Moving platforms.
- Temporary barriers.
- Multi-channel routing.
- Echo-sensitive sensors.
- Precision timing.
Custom puzzle: A relay only accepts signals during a narrow time window. One echo triggers the first channel, another echo activates the second, and the current Greg must reach the final control before the synchronization window expires.

Story moment: NULL interrupts the broadcast and reveals that it has been manipulating the network to isolate districts and seize control of the Heartline.
Level 05 — Heartline: The Last Signal
Story: The city is close to a cascading system failure. Greg must reconnect the Heartline and distribute the verified information before the central system shuts down.

Environment: A large central network facility with multiple interconnected chambers, branching pathways, and a final central core.

This should be the longest and most complex level.

Puzzle sequence:
1. Restore the outer power connection.
2. Open the left-side routing chamber.
3. Use echoes to keep multiple mechanisms active.
4. Restore the archive connection.
5. Activate the central synchronization system.
6. Route the verified broadcast through three separate channels.
7. Reach the Heartline control.
8. Complete the final synchronization puzzle.
Mechanics: All previously introduced mechanics are combined.

Final puzzle: Three channels must be maintained simultaneously. Earlier Gregs operate the power and verification mechanisms, while the current Greg activates the central broadcast.

The city reconnects.
Ending cutscene
The Heartline lights up.

The false broadcast disappears from the network.

BOLT reconnects the power station. Mira restores the original archive records. Pip delivers the verified information to the remaining districts.

The residents begin communicating again.

Greg watches as the city's lights gradually return.

A final message appears:
"A signal can travel faster than the truth. But the truth can still reach everyone."

Yuuki: Huh. That's a decent ending. Short, not melodramatic, and we don't need a 20-minute cinematic.

3. Reusable mechanics — what we actually build
Yuuko: Here's where we need to be careful. We already have working components. We should extend them, not invent a completely new engine.

I'd divide the mechanics into three categories.
A. Existing mechanics to retain
Component	Reuse
Greg movement	Keep
Echo recording and playback	Keep
Loop timer and reset	Keep
Echo limit	Keep
Interactable interface	Keep
Doors and switches	Keep, but extend
Collision and room geometry	Keep the working foundation
Narrative terminal	Keep
HUD	Keep as a base
Main menu and pause menu	Keep

B. Mechanics worth adding
These are the mechanics I'd prioritize for the new campaign.
Mechanic	Gameplay purpose	Priority
Pressure plate	Hold a mechanism open with an echo	Essential
Dual switch	Two locations must be activated together	Essential
Rotary junction	Rotate energy or signal paths	High
Charge station	Maintain activation for a duration	High
Movable block	Position an object to create a route	Medium
Key/socket	Unlock another section of a level	High
Timing slider	Stop a moving marker in a target zone	High
Timed gate	Pass through during a limited window	High
Signal relay	Connect communication nodes	Essential
Verification terminal	Change unverified information into verified information	Essential
Moving platform	Create a traversal challenge	Medium
Multi-channel synchronizer	Require several echoes to cooperate	High

C. One additional mechanic I strongly recommend: Echo Memory
Yuura: WAIT. What if an echo can do something that changes the next section of the level?

For example:
- Greg activates a release mechanism.
- The echo repeats that activation in a later loop.
- The mechanism releases a key or opens a passage.
- The current Greg uses the newly available route.
This gives us a sense that past actions have consequences beyond simply holding a switch.

Yuuko: There is an implementation distinction here. The current system records positions and interaction events. It doesn't automatically preserve every arbitrary world-state change across resets. So we should implement this as a controlled puzzle-module behavior, not unrestricted object persistence.

For the jam build, I'd prioritize the existing echo-compatible interactions first.

4. Level architecture — stop building rectangular rooms
Yuuka: The containerized approach is still good. But we should change what a container represents.

A level should be a connected environment composed of puzzle chambers, not one large rectangle.

A level can contain:
- Entry area.
- Main traversal route.
- Side chambers.
- Locked shortcuts.
- Puzzle chambers.
- NPC encounter points.
- Story terminals.
- A final objective room.
The individual chambers can still be reusable scenes.
Recommended Godot structure
Scenes/
├── Main.tscn
├── Characters/
│   ├── Greg.tscn
│   ├── Echo.tscn
│   ├── Pip.tscn
│   ├── Mira.tscn
│   └── Bolt.tscn
│
├── Levels/
│   ├── Sector01_BrokenCrossing.tscn
│   ├── Sector02_Powerworks.tscn
│   ├── Sector03_Archive.tscn
│   ├── Sector04_SignalTower.tscn
│   └── Sector05_Heartline.tscn
│
├── Chambers/
│   ├── Entry/
│   ├── Traversal/
│   ├── Puzzle/
│   ├── Mechanism/
│   └── Exit/
│
├── PuzzleModules/
│   ├── PressurePlate.tscn
│   ├── RotaryJunction.tscn
│   ├── ChargeStation.tscn
│   ├── TimedGate.tscn
│   ├── SignalRelay.tscn
│   ├── VerificationTerminal.tscn
│   └── Synchronizer.tscn
│
└── UI/
    ├── MainMenu.tscn
    ├── BasicHud.tscn
    ├── PauseModal.tscn
    ├── LevelSelect.tscn
    ├── DialogueOverlay.tscn
    ├── CutsceneOverlay.tscn
    └── EndingScreen.tscn


This is the target organization, not a demand to create every folder immediately.

Yuuna: And please, no giant generic procedural-generation system right now. We can assemble five handcrafted levels from reusable chambers. That's enough.

5. Cutscenes and UI
We already have some UI infrastructure. We need to add narrative presentation without interrupting gameplay constantly.
Screen / sequence	Purpose
Opening cinematic	Introduce NEXUS, Greg, and the false broadcast
Sector introduction	Establish the location and immediate objective
NPC dialogue	Show character reactions and deliver evidence
Story discovery	Reveal new information through terminals
Sector completion	Show the local environment recovering
Final cinematic	Restore the Heartline
Ending screen	Credits, team, restart, exit

Cutscene approach
Keep them inexpensive.

Use:
- Camera framing.
- Character movement.
- Dialogue boxes.
- Background changes.
- Short animations.
- Sound effects.
- Simple transitions.
No complex animation sequences or voice acting are necessary.

The NPC dialogue should be short. A typical encounter should take around 10–20 seconds, not several minutes.
One UI change I recommend
The current HUD displays a technical-looking sector label, timer, and echo count. Keep that visual language, but add a small story objective panel.

For example:

SECTOR 03 — THE ARCHIVE

Recover the original broadcast record.

CURRENT OBJECTIVE: Authenticate the archive terminal.

And when an NPC encounter occurs, temporarily show a dialogue panel rather than forcing the player to read a giant terminal overlay.
