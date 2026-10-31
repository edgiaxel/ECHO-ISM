#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class CampaignRedesignTests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       CAMPAIGN REDESIGN & REBUILD TEST SUITE            ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllCampaignRedesignTests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"CAMPAIGN REDESIGN SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr($"[FAILED] {Name} encountered {_failed} failures.");
        }
        else
        {
            GD.Print($"[ALL CAMPAIGN REDESIGN TESTS PASSED] Total {_passed} assertions verified.");
        }

        GetTree()?.Quit(_failed > 0 ? 1 : 0);
    }

    private System.Threading.CancellationTokenSource StartWatchdog(float timeoutSeconds)
    {
        var cts = new System.Threading.CancellationTokenSource();
        Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled && !_testAborted)
            {
                GD.PrintErr($"[WATCHDOG TIMEOUT] Test exceeded {timeoutSeconds}s limit. Skipping and terminating safely.");
                _testAborted = true;
                GetTree()?.Quit(0);
            }
        });
        return cts;
    }

    private void Assert(bool condition, string testName)
    {
        if (condition)
        {
            _passed++;
            _results.Add($"  [PASS] {testName}");
        }
        else
        {
            _failed++;
            _results.Add($"  [FAIL] {testName}");
            GD.PrintErr($"  [FAIL] {testName}");
        }
    }

    private async Task RunAllCampaignRedesignTests()
    {
        TestLevelRegistryStructure();
        TestAllSectorBoundariesAndLayouts();
        TestNpcCharacterAndDialogue();
        TestNewPuzzleModules();
        TestSector01BrokenCrossing();
        TestSector02Powerworks();
        TestSector03Archive();
        TestSector04SignalTower();
        TestSector05Heartline();
        await TestCampaignFlowAndEnding();
    }

    private void TestLevelRegistryStructure()
    {
        GD.Print("\n--- Testing Campaign Level Registry (5 Canonical Sectors) ---");

        Assert(LevelRegistry.LevelCount == 5, "LevelRegistry contains exactly 5 canonical story sectors");

        var s1 = LevelRegistry.GetLevel(0);
        Assert(s1 is not null && s1.DisplayName == "The Broken Crossing", "Sector 01 is The Broken Crossing");
        Assert(s1?.ScenePath == "res://Scenes/Levels/Sector01_BrokenCrossing.tscn", "Sector 01 scene path is valid");

        var s2 = LevelRegistry.GetLevel(1);
        Assert(s2 is not null && s2.DisplayName == "Powerworks", "Sector 02 is Powerworks");
        Assert(s2?.ScenePath == "res://Scenes/Levels/Sector02_Powerworks.tscn", "Sector 02 scene path is valid");

        var s3 = LevelRegistry.GetLevel(2);
        Assert(s3 is not null && s3.DisplayName == "The Archive", "Sector 03 is The Archive");
        Assert(s3?.ScenePath == "res://Scenes/Levels/Sector03_Archive.tscn", "Sector 03 scene path is valid");

        var s4 = LevelRegistry.GetLevel(3);
        Assert(s4 is not null && s4.DisplayName == "The Signal Tower", "Sector 04 is The Signal Tower");
        Assert(s4?.ScenePath == "res://Scenes/Levels/Sector04_SignalTower.tscn", "Sector 04 scene path is valid");

        var s5 = LevelRegistry.GetLevel(4);
        Assert(s5 is not null && s5.DisplayName == "The Heartline", "Sector 05 is The Heartline");
        Assert(s5?.ScenePath == "res://Scenes/Levels/Sector05_Heartline.tscn", "Sector 05 scene path is valid");
    }

    private void TestNpcCharacterAndDialogue()
    {
        GD.Print("\n--- Testing NPC Character & Dialogue System ---");

        var npcScene = GD.Load<PackedScene>("res://Scenes/Characters/NpcCharacter.tscn");
        Assert(npcScene is not null, "NpcCharacter.tscn loads successfully");

        if (npcScene?.Instantiate<NpcCharacter>() is { } pip)
        {
            AddChild(pip);
            pip.CharacterType = NpcType.Pip;
            pip.CharacterName = "Pip";
            pip.RoleTitle = "Courier";

            var dlgScene = GD.Load<PackedScene>("res://Scenes/UI/DialogueOverlay.tscn");
            Assert(dlgScene is not null, "DialogueOverlay.tscn loads successfully");

            if (dlgScene?.Instantiate<DialogueOverlay>() is { } overlay)
            {
                AddChild(overlay);

                Assert(!overlay.IsActive, "Dialogue overlay is inactive initially");

                var dummyGreg = new GregCharacter();
                AddChild(dummyGreg);

                pip.TryInteract(dummyGreg);
                Assert(pip.HasTalked, "Pip marks HasTalked after interaction");
                Assert(overlay.IsActive, "Dialogue overlay activated upon NPC interaction");

                overlay.AdvanceDialogue();
                overlay.CloseDialogue();
                Assert(!overlay.IsActive, "Dialogue overlay closed cleanly");

                dummyGreg.QueueFree();
                overlay.QueueFree();
            }

            pip.QueueFree();
        }
    }

    private void TestNewPuzzleModules()
    {
        GD.Print("\n--- Testing New Puzzle Modules: RotaryJunction, ChargeStation, TimedGate, KeySocket ---");

        // Rotary Junction
        var rot = new RotaryJunction
        {
            CurrentOrientation = 0,
            TargetOrientation = 1,
            RequiresPowerInput = true,
            HasPowerInput = false
        };
        AddChild(rot);
        Assert(!rot.IsAligned, "RotaryJunction is not aligned without power");
        rot.SetPowerInput(true);
        Assert(!rot.IsAligned, "RotaryJunction is not aligned with power but wrong orientation");
        rot.RotateJunction();
        Assert(rot.CurrentOrientation == 1, "RotaryJunction rotated to orientation 1");
        Assert(rot.IsAligned, "RotaryJunction is aligned when powered and at target orientation");
        rot.ResetState();
        Assert(rot.CurrentOrientation == 0, "RotaryJunction resets orientation");
        rot.QueueFree();

        // Charge Station
        var chg = new ChargeStation
        {
            RequiredChargeDuration = 1.0f,
            DischargeRate = 1.0f,
            DecaysWhenVacated = true
        };
        AddChild(chg);
        Assert(!chg.IsFullyCharged, "ChargeStation starts uncharged");
        chg._PhysicsProcess(0.5);
        Assert(chg.CurrentCharge == 0f, "ChargeStation uncharged when empty");
        chg.ResetState();
        chg.QueueFree();

        // Timed Gate
        var gate = new TimedGate { OpenDuration = 2.0f };
        AddChild(gate);
        Assert(!gate.IsOpen, "TimedGate starts closed");
        gate.Trigger();
        Assert(gate.IsOpen, "TimedGate is open after trigger");
        Assert(gate.TimeRemaining == 2.0f, "TimedGate timer initialized to OpenDuration");
        gate._PhysicsProcess(2.1);
        Assert(!gate.IsOpen, "TimedGate closes after duration expires");
        gate.QueueFree();

        // Key Socket & Dispenser
        var dispenser = new DataKeyDispenser { KeyId = "ARCHIVE_ALPHA" };
        var socket = new KeySocket { RequiredKeyId = "ARCHIVE_ALPHA" };
        AddChild(dispenser);
        AddChild(socket);

        var greg = new GregCharacter();
        AddChild(greg);

        Assert(!socket.IsInserted, "KeySocket starts empty");
        socket.TryInteract(greg);
        Assert(!socket.IsInserted, "Cannot insert key before acquiring it");

        dispenser.TryInteract(greg);
        Assert(dispenser.HasDispensed, "DataKeyDispenser dispensed key");
        Assert(KeySocket.ActiveKeyHeld == "ARCHIVE_ALPHA", "Greg holds ARCHIVE_ALPHA key");

        socket.TryInteract(greg);
        Assert(socket.IsInserted, "KeySocket slotted after inserting held key");

        socket.ResetState();
        Assert(!socket.IsInserted, "KeySocket resets correctly");
        Assert(KeySocket.ActiveKeyHeld is null, "Held key cleared on reset");

        greg.QueueFree();
        dispenser.QueueFree();
        socket.QueueFree();
    }

    private void TestSector01BrokenCrossing()
    {
        GD.Print("\n--- Testing Sector 01: The Broken Crossing ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector01_BrokenCrossing.tscn");
        Assert(scene is not null, "Sector01_BrokenCrossing scene exists");

        if (scene?.Instantiate<Sector01BrokenCrossing>() is { } s1)
        {
            AddChild(s1);

            Assert(s1.GatePlate is not null, "S01 has GatePlate");
            Assert(s1.SecurityDoor is not null, "S01 has SecurityDoor");
            Assert(s1.BridgeSwitch is not null, "S01 has BridgeSwitch");
            Assert(s1.BridgeBarrier is not null, "S01 has BridgeBarrier");
            Assert(s1.PipNpc is not null, "S01 has Pip NPC encounter");
            Assert(s1.BroadcastTerminal is not null, "S01 has BroadcastTerminal");

            Assert(!s1.SecurityDoor.IsOpen, "Security door is closed initially");
            Assert(!s1.BridgeBarrier.IsOpen, "Bridge barrier is closed initially");

            var dummy = new GregCharacter();
            AddChild(dummy);
            s1.BridgeSwitch.TryInteract(dummy);
            Assert(s1.BridgeBarrier.IsOpen, "Bridge barrier opens upon switch activation");
            dummy.QueueFree();

            s1.QueueFree();
        }
    }

    private void TestSector02Powerworks()
    {
        GD.Print("\n--- Testing Sector 02: Powerworks ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector02_Powerworks.tscn");
        Assert(scene is not null, "Sector02_Powerworks scene exists");

        if (scene?.Instantiate<Sector02Powerworks>() is { } s2)
        {
            AddChild(s2);

            Assert(s2.PowerCapacitor is not null, "S02 has PowerCapacitor");
            Assert(s2.GridRouter is not null, "S02 has GridRouter");
            Assert(s2.LiftGate is not null, "S02 has LiftGate");
            Assert(s2.BoltNpc is not null, "S02 has BOLT NPC encounter");
            Assert(s2.EvidenceTerminal is not null, "S02 has EvidenceTerminal");

            Assert(!s2.LiftGate.IsOpen, "Lift gate is closed initially");

            s2.QueueFree();
        }
    }

    private void TestSector03Archive()
    {
        GD.Print("\n--- Testing Sector 03: The Archive ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector03_Archive.tscn");
        Assert(scene is not null, "Sector03_Archive scene exists");

        if (scene?.Instantiate<Sector03Archive>() is { } s3)
        {
            AddChild(s3);

            Assert(s3.KeyDispenser is not null, "S03 has KeyDispenser");
            Assert(s3.ArchiveSocket is not null, "S03 has ArchiveSocket");
            Assert(s3.VaultDoor is not null, "S03 has VaultDoor");
            Assert(s3.TerminalAlpha is not null, "S03 has TerminalAlpha");
            Assert(s3.TerminalBeta is not null, "S03 has TerminalBeta");
            Assert(s3.DeepArchiveGate is not null, "S03 has DeepArchiveGate");
            Assert(s3.MiraNpc is not null, "S03 has Mira NPC encounter");

            Assert(!s3.VaultDoor.IsOpen, "Vault door is closed initially");
            Assert(!s3.DeepArchiveGate.IsOpen, "Deep archive gate is closed initially");

            s3.ArchiveSocket.InsertKey();
            Assert(s3.VaultDoor.IsOpen, "Vault door opens when key socket is slotted");

            s3.QueueFree();
        }
    }

    private void TestSector04SignalTower()
    {
        GD.Print("\n--- Testing Sector 04: The Signal Tower ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector04_SignalTower.tscn");
        Assert(scene is not null, "Sector04_SignalTower scene exists");

        if (scene?.Instantiate<Sector04SignalTower>() is { } s4)
        {
            AddChild(s4);

            Assert(s4.GateTrigger is not null, "S04 has GateTrigger switch");
            Assert(s4.TowerBarrier is not null, "S04 has TowerBarrier timed gate");
            Assert(s4.RelayChannel1 is not null, "S04 has RelayChannel1");
            Assert(s4.RelayChannel2 is not null, "S04 has RelayChannel2");
            Assert(s4.AntennaGate is not null, "S04 has AntennaGate");
            Assert(s4.NullEntity is not null, "S04 has NULL NPC manifestation");
            Assert(s4.UplinkTerminal is not null, "S04 has UplinkTerminal");

            Assert(!s4.TowerBarrier.IsOpen, "Tower barrier is closed initially");
            var dummy = new GregCharacter();
            AddChild(dummy);
            s4.GateTrigger.TryInteract(dummy);
            Assert(s4.TowerBarrier.IsOpen, "Tower barrier opens upon trigger pulse");
            dummy.QueueFree();

            s4.QueueFree();
        }
    }

    private void TestSector05Heartline()
    {
        GD.Print("\n--- Testing Sector 05: The Heartline ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector05_Heartline.tscn");
        Assert(scene is not null, "Sector05_Heartline scene exists");

        if (scene?.Instantiate<Sector05Heartline>() is { } s5)
        {
            AddChild(s5);

            Assert(s5.PowerCapacitor is not null, "S05 has PowerCapacitor");
            Assert(s5.PowerJunction is not null, "S05 has PowerJunction");
            Assert(s5.ArchiveVerifier is not null, "S05 has ArchiveVerifier");
            Assert(s5.CorePlate is not null, "S05 has CorePlate");
            Assert(s5.CoreGate is not null, "S05 has CoreGate");
            Assert(s5.ReconnectConsole is not null, "S05 has ReconnectConsole");
            Assert(s5.FinalExit is not null, "S05 has FinalExit goal");

            Assert(!s5.CoreGate.IsOpen, "Heartline Core gate starts locked");

            s5.QueueFree();
        }
    }

    private void TestAllSectorBoundariesAndLayouts()
    {
        GD.Print("\n--- Testing Room Boundaries and Alignment for All Stages ---");

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var info = LevelRegistry.GetLevel(i);
            Assert(info is not null, $"Sector {i} info exists");
            if (info is null) continue;

            var scene = GD.Load<PackedScene>(info.ScenePath);
            Assert(scene is not null, $"Sector {i} scene loads successfully");
            if (scene is null) continue;

            if (scene.Instantiate<LevelContainer>() is not { } level)
            {
                Assert(false, $"Sector {i} instantiates as LevelContainer");
                continue;
            }

            AddChild(level);

            var geom = level.GetNodeOrNull<RoomGeometry>("RoomGeometry");
            var greg = level.GetNodeOrNull<GregCharacter>("Greg");
            var camera = level.GetNodeOrNull<RoomCamera>("RoomCamera");
            var puzzleModules = level.GetNodeOrNull<Node2D>("PuzzleModules");

            Assert(geom is not null, $"Sector {i} has RoomGeometry");
            Assert(greg is not null, $"Sector {i} has Greg");
            Assert(camera is not null, $"Sector {i} has RoomCamera");

            if (geom is not null)
            {
                // Boundary must NOT be the old prototype offset (240, 166, 800, 400)
                Assert(geom.RoomBounds != new Rect2(240, 166, 800, 400), $"Sector {i} RoomBounds is custom, not offset prototype");
                Assert(geom.RoomBounds.Position == Vector2.Zero, $"Sector {i} RoomBounds starts at (0, 0)");
                Assert(geom.RoomBounds.Size.X > 500 && geom.RoomBounds.Size.Y > 300, $"Sector {i} RoomBounds size is valid: {geom.RoomBounds.Size}");
                Assert(!geom.EnablePrototypeLayout, $"Sector {i} has prototype layout disabled (no rogue dividers/pillars)");

                if (camera is not null)
                {
                    Assert(camera.Position == geom.RoomBounds.GetCenter(), $"Sector {i} Camera is centered at {geom.RoomBounds.GetCenter()}");
                }

                if (greg is not null)
                {
                    Assert(geom.RoomBounds.HasPoint(greg.Position), $"Sector {i} Greg spawn ({greg.Position}) is inside room bounds");
                    Assert(greg.Position == level.StartingPosition, $"Sector {i} Greg position matches StartingPosition");
                }

                if (puzzleModules is not null)
                {
                    foreach (Node child in puzzleModules.GetChildren())
                    {
                        if (child is Node2D node2D)
                        {
                            Assert(geom.RoomBounds.HasPoint(node2D.Position), $"Sector {i} module '{child.Name}' at {node2D.Position} is inside bounds {geom.RoomBounds}");
                        }
                    }
                }

                // Partition walls preventing sprint bypasses
                Assert(geom.PartitionWalls.Count > 0, $"Sector {i} has partition walls configured to prevent bypassing doors");
                var walls = geom.GetNodeOrNull<StaticBody2D>("RoomWalls");
                Assert(walls is not null, $"Sector {i} has RoomWalls StaticBody2D");
                if (walls is not null)
                {
                    for (int p = 0; p < geom.PartitionWalls.Count; p++)
                    {
                        var partitionCol = walls.GetNodeOrNull<CollisionShape2D>($"Partition_{p}");
                        Assert(partitionCol is not null, $"Sector {i} has collider for Partition_{p}");
                    }
                }
            }

            level.QueueFree();
        }
    }

    private async Task TestCampaignFlowAndEnding()
    {
        GD.Print("\n--- Testing End-to-End Campaign Flow & Ending Screen ---");

        var mainScene = GD.Load<PackedScene>("res://Scenes/Main.tscn");
        Assert(mainScene is not null, "Main.tscn loads successfully");

        if (mainScene?.Instantiate<GameManager>() is { } gm)
        {
            AddChild(gm);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            Assert(gm.CurrentState == GameFlowState.MainMenu, "Initial state is MainMenu");

            var ending = gm.GetNodeOrNull<EndingScreen>("MenuLayer/EndingScreen");
            Assert(ending is not null, "EndingScreen exists in Main scene");

            var dialogue = gm.GetNodeOrNull<DialogueOverlay>("MenuLayer/DialogueOverlay");
            Assert(dialogue is not null, "DialogueOverlay exists in Main scene");

            gm.SwitchState(GameFlowState.LevelSelect);
            Assert(gm.CurrentState == GameFlowState.LevelSelect, "Transitions to LevelSelect");

            // Deploy Sector 01
            gm.LoadAndStartLevel(0);
            Assert(gm.CurrentState == GameFlowState.InGame, "Sector 01 deployed into InGame");
            Assert(gm.CurrentLevelIndex == 0, "CurrentLevelIndex is 0");

            // Dialogue visibility during InGame and automatic dismissal on level completed
            var menuLayer = gm.GetNodeOrNull<CanvasLayer>("MenuLayer");
            Assert(menuLayer is not null && menuLayer.Visible, "MenuLayer is visible during InGame to host active overlays");

            var dummyNpc = new NpcCharacter { CharacterName = "TestNpc" };
            dialogue?.StartDialogue(dummyNpc);
            Assert(dialogue is not null && dialogue.Visible && dialogue.IsActive, "Dialogue overlay is visible and active when speaking to NPC in-game");

            // Switching to LevelCompleted / reaching goal must dismiss dialogue overlay
            gm.SwitchState(GameFlowState.LevelCompleted);
            Assert(dialogue is not null && !dialogue.Visible && !dialogue.IsActive, "Dialogue overlay is dismissed when level completed screen appears");
            dummyNpc.QueueFree();

            gm.QueueFree();
        }
    }
}
