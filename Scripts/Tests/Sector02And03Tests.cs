#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector02And03Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTORS 02 & 03 AUTOMATED TEST SUITE             ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSectorTests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTORS 02 & 03 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTORS 02 & 03 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTORS 02 & 03 TEST SUITE PASSED]");
            GetTree().Quit(0);
        }
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
            GD.Print($"[PASS] {testName}");
        }
        else
        {
            _failed++;
            _results.Add($"  [FAIL] {testName}");
            GD.PrintErr($"[FAIL] {testName}");
        }
    }

    private async Task RunAllSectorTests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // --- Sector 02 Tests ---
        TestSector02Registration();
        TestSector02SceneStructure();
        TestSector02ChannelTerminals();
        TestSector02SignalAnalystFragment();
        TestSector02GameplaySync();
        TestSector02PerformanceGrades();

        // --- Sector 03 Tests ---
        TestSector03Registration();
        TestSector03SceneStructure();
        TestSector03DistrictTerminals();
        TestSector03MediatorFragment();
        TestSector03DualMechanismProgression();
        TestSector03PerformanceGrades();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    // =========================================================================
    // SECTOR 02: FALSE SYNCHRONY
    // =========================================================================

    private void TestSector02Registration()
    {
        GD.Print("\n--- Testing S02 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(1);
        Assert(info is not null, "Sector 02 is registered at index 1");
        if (info is null) return;

        Assert(info.SectorCode == "SECTOR 02", "Sector code is SECTOR 02");
        Assert(info.DisplayName == "False Synchrony", "Display name is False Synchrony");
        Assert(info.NarrativeChapter == "Conflicting Signals", "Narrative chapter is Conflicting Signals");
        Assert(info.OptionalFragmentId == "FRAG_SEC02_SIGNAL_ANALYST", "Optional fragment is FRAG_SEC02_SIGNAL_ANALYST");
        Assert(info.EfficientLoopThreshold == 2, "Efficient loop threshold is 2 loops");
        Assert(!string.IsNullOrEmpty(info.DebriefText), "Debrief text is populated");
    }

    private void TestSector02SceneStructure()
    {
        GD.Print("\n--- Testing S02 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        Assert(scene is not null, "TimingRoom.tscn loads successfully");
        if (scene is null) return;

        var room = scene.Instantiate<Node2D>();
        Assert(room is not null, "TimingRoom instance created");
        if (room is null) return;
        AddChild(room);

        Assert(room is TimingRoom, "Root node is TimingRoom");
        if (room is not TimingRoom s02)
        {
            room.QueueFree();
            return;
        }

        Assert(s02.Slider is not null, "Sector 02 has TimingSlider");
        Assert(s02.Plate is not null, "Sector 02 has PressurePlate");
        Assert(s02.SyncGate is not null, "Sector 02 has SynchronizationGate");
        Assert(s02.Door is not null, "Sector 02 has TimedDoor");
        Assert(s02.ExitGoal is not null, "Sector 02 has ExitGoal");
        Assert(s02.ChannelATerminal is not null, "Sector 02 has ChannelATerminal");
        Assert(s02.ChannelBTerminal is not null, "Sector 02 has ChannelBTerminal");
        Assert(s02.SignalAnalystTerminal is not null, "Sector 02 has SignalAnalystTerminal");

        Assert(s02.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(s02.Door is not null && !s02.Door.IsOpen, "TimedDoor is closed initially");

        room.QueueFree();
    }

    private void TestSector02ChannelTerminals()
    {
        GD.Print("\n--- Testing S02 Channel Terminals ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        if (scene is null || scene.Instantiate<TimingRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate TimingRoom");
            return;
        }
        AddChild(room);

        var chA = room.ChannelATerminal;
        var chB = room.ChannelBTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!chA.IsOptionalFragment, "Channel A is not marked as optional fragment");
        Assert(chA.StableId == "sec02-channel-a", "Channel A has stable ID");
        Assert(chA.MessageBody.Contains("REQUESTING EVACUATION"), "Channel A broadcasts evacuation request");
        Assert(chA.TryInteract(greg), "Channel A interaction succeeds");
        Assert(chA.HasBeenRead, "Channel A marked as read");

        Assert(!chB.IsOptionalFragment, "Channel B is not marked as optional fragment");
        Assert(chB.StableId == "sec02-channel-b", "Channel B has stable ID");
        Assert(chB.MessageBody.Contains("NO EVACUATION REQUIRED"), "Channel B broadcasts contradictory no-evacuation dispatch");
        Assert(chB.TryInteract(greg), "Channel B interaction succeeds");
        Assert(chB.HasBeenRead, "Channel B marked as read");

        room.QueueFree();
    }

    private void TestSector02SignalAnalystFragment()
    {
        GD.Print("\n--- Testing S02 Signal Analyst Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        if (scene is null || scene.Instantiate<TimingRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate TimingRoom");
            return;
        }
        AddChild(room);

        var term = room.SignalAnalystTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(term.IsOptionalFragment, "Signal Analyst is marked as optional fragment");
        Assert(term.FragmentId == "FRAG_SEC02_SIGNAL_ANALYST", "Fragment ID matches registry");
        Assert(term.FragmentTitle == "Signal Analyst", "Fragment title matches");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsFragmentRecovered("FRAG_SEC02_SIGNAL_ANALYST"), "Fragment is unrecovered initially");

        term.TryInteract(greg);
        Assert(ProgressionManager.IsFragmentRecovered("FRAG_SEC02_SIGNAL_ANALYST"), "Fragment is recovered in ProgressionManager after interaction");

        var recovered = ProgressionManager.RecoveredFragments["FRAG_SEC02_SIGNAL_ANALYST"];
        Assert(recovered.Excerpt.Contains("timestamps") && recovered.Excerpt.Contains("four hours apart"),
            "Recovered excerpt reveals fabricated simultaneity");

        room.QueueFree();
    }

    private void TestSector02GameplaySync()
    {
        GD.Print("\n--- Testing S02 Gameplay Synchronization ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        if (scene is null || scene.Instantiate<TimingRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate TimingRoom");
            return;
        }
        AddChild(room);

        var gate = room.SyncGate;
        var door = room.Door;

        Assert(!door.IsOpen, "TimedDoor is closed before synchronization");

        // Simulate simultaneous Signal A and Signal B arrival within tolerance
        gate.ReceiveSignalA();
        gate.ReceiveSignalB();

        Assert(door.IsOpen, "TimedDoor opens upon successful gate synchronization");

        room.QueueFree();
    }

    private void TestSector02PerformanceGrades()
    {
        GD.Print("\n--- Testing S02 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // 3 loops without fragment -> CLEAR
        var gradeClear = ProgressionManager.CalculateGrade(1, 3);
        Assert(gradeClear == PerformanceGrade.Clear, "3 loops without fragment produces CLEAR grade");

        // 2 loops without fragment -> EFFICIENT (threshold is 2)
        var gradeEfficient = ProgressionManager.CalculateGrade(1, 2);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "2 loops without fragment produces EFFICIENT grade");

        // 2 loops with fragment recovered -> PERFECT
        ProgressionManager.RecordFragmentRecovered("FRAG_SEC02_SIGNAL_ANALYST", "Signal Analyst", "timestamps");
        var gradePerfect = ProgressionManager.CalculateGrade(1, 2);
        Assert(gradePerfect == PerformanceGrade.Perfect, "2 loops with fragment produces PERFECT grade");
    }

    // =========================================================================
    // SECTOR 03: DIVIDED CONSENSUS
    // =========================================================================

    private void TestSector03Registration()
    {
        GD.Print("\n--- Testing S03 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(2);
        Assert(info is not null, "Sector 03 is registered at index 2");
        if (info is null) return;

        Assert(info.SectorCode == "SECTOR 03", "Sector code is SECTOR 03");
        Assert(info.DisplayName == "Divided Consensus", "Display name is Divided Consensus");
        Assert(info.NarrativeChapter == "A Society Splits", "Narrative chapter is A Society Splits");
        Assert(info.OptionalFragmentId == "FRAG_SEC03_MEDIATOR_RECORD", "Optional fragment is FRAG_SEC03_MEDIATOR_RECORD");
        Assert(info.EfficientLoopThreshold == 2, "Efficient loop threshold is 2 loops");
        Assert(!string.IsNullOrEmpty(info.DebriefText), "Debrief text is populated");
    }

    private void TestSector03SceneStructure()
    {
        GD.Print("\n--- Testing S03 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/DualProtocolRoom.tscn");
        Assert(scene is not null, "DualProtocolRoom.tscn loads successfully");
        if (scene is null) return;

        var room = scene.Instantiate<Node2D>();
        Assert(room is not null, "DualProtocolRoom instance created");
        if (room is null) return;
        AddChild(room);

        Assert(room is DualProtocolRoom, "Root node is DualProtocolRoom");
        if (room is not DualProtocolRoom s03)
        {
            room.QueueFree();
            return;
        }

        Assert(s03.Plate is not null, "Sector 03 has PressurePlate");
        Assert(s03.Switch is not null, "Sector 03 has TestSwitch");
        Assert(s03.DualDoor is not null, "Sector 03 has DualMechanismDoor");
        Assert(s03.ExitGoal is not null, "Sector 03 has ExitGoal");
        Assert(s03.WestTerminal is not null, "Sector 03 has WestTerminal");
        Assert(s03.EastTerminal is not null, "Sector 03 has EastTerminal");
        Assert(s03.MediatorTerminal is not null, "Sector 03 has MediatorTerminal");

        Assert(s03.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(s03.DualDoor is not null && !s03.DualDoor.IsOpen, "DualMechanismDoor is closed initially");

        room.QueueFree();
    }

    private void TestSector03DistrictTerminals()
    {
        GD.Print("\n--- Testing S03 District Terminals ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/DualProtocolRoom.tscn");
        if (scene is null || scene.Instantiate<DualProtocolRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate DualProtocolRoom");
            return;
        }
        AddChild(room);

        var west = room.WestTerminal;
        var east = room.EastTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!west.IsOptionalFragment, "West terminal is not marked as optional fragment");
        Assert(west.StableId == "sec03-west-notice", "West terminal has stable ID");
        Assert(west.MessageBody.Contains("EAST DISTRICT REFUSES"), "West announcement claims East refuses dialogue");
        Assert(west.TryInteract(greg), "West terminal interaction succeeds");
        Assert(west.HasBeenRead, "West terminal marked as read");

        Assert(!east.IsOptionalFragment, "East terminal is not marked as optional fragment");
        Assert(east.StableId == "sec03-east-notice", "East terminal has stable ID");
        Assert(east.MessageBody.Contains("WEST DISTRICT HAS SEVERED"), "East announcement claims West severed ties");
        Assert(east.TryInteract(greg), "East terminal interaction succeeds");
        Assert(east.HasBeenRead, "East terminal marked as read");

        room.QueueFree();
    }

    private void TestSector03MediatorFragment()
    {
        GD.Print("\n--- Testing S03 Mediator Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/DualProtocolRoom.tscn");
        if (scene is null || scene.Instantiate<DualProtocolRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate DualProtocolRoom");
            return;
        }
        AddChild(room);

        var term = room.MediatorTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(term.IsOptionalFragment, "Mediator terminal is marked as optional fragment");
        Assert(term.FragmentId == "FRAG_SEC03_MEDIATOR_RECORD", "Fragment ID matches registry");
        Assert(term.FragmentTitle == "The Mediator's Record", "Fragment title matches");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsFragmentRecovered("FRAG_SEC03_MEDIATOR_RECORD"), "Fragment is unrecovered initially");

        term.TryInteract(greg);
        Assert(ProgressionManager.IsFragmentRecovered("FRAG_SEC03_MEDIATOR_RECORD"), "Fragment is recovered in ProgressionManager after interaction");

        var recovered = ProgressionManager.RecoveredFragments["FRAG_SEC03_MEDIATOR_RECORD"];
        Assert(recovered.Excerpt.Contains("identical requests") && recovered.Excerpt.Contains("Who changed the message"),
            "Recovered excerpt reveals suppression of joint petition");

        room.QueueFree();
    }

    private void TestSector03DualMechanismProgression()
    {
        GD.Print("\n--- Testing S03 Dual Mechanism Progression ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/DualProtocolRoom.tscn");
        if (scene is null || scene.Instantiate<DualProtocolRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate DualProtocolRoom");
            return;
        }
        AddChild(room);

        var door = room.DualDoor;
        var plate = room.Plate;
        var sw = room.Switch;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!door.IsOpen, "DualDoor is closed initially");

        // Input A only
        plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(!door.IsOpen, "Door remains closed with Input A only");

        // Input B only
        plate.EmitSignal(Area2D.SignalName.BodyExited, greg);
        sw.TryInteract(greg);
        Assert(!door.IsOpen, "Door remains closed with Input B only");

        // Both Input A and Input B active simultaneously
        plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(door.IsOpen, "DualDoor opens when Input A and Input B are active simultaneously");

        room.QueueFree();
    }

    private void TestSector03PerformanceGrades()
    {
        GD.Print("\n--- Testing S03 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // 3 loops without fragment -> CLEAR
        var gradeClear = ProgressionManager.CalculateGrade(2, 3);
        Assert(gradeClear == PerformanceGrade.Clear, "3 loops without fragment produces CLEAR grade");

        // 2 loops without fragment -> EFFICIENT (threshold is 2)
        var gradeEfficient = ProgressionManager.CalculateGrade(2, 2);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "2 loops without fragment produces EFFICIENT grade");

        // 2 loops with fragment recovered -> PERFECT
        ProgressionManager.RecordFragmentRecovered("FRAG_SEC03_MEDIATOR_RECORD", "The Mediator's Record", "identical requests");
        var gradePerfect = ProgressionManager.CalculateGrade(2, 2);
        Assert(gradePerfect == PerformanceGrade.Perfect, "2 loops with fragment produces PERFECT grade");
    }
}
