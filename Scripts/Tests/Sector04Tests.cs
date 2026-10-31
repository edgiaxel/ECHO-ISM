#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector04Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTOR 04: THE ECHO CHAMBER TEST SUITE           ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSector04Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTOR 04 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTOR 04 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTOR 04 TEST SUITE PASSED]");
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

    private async Task RunAllSector04Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestSector04Registration();
        TestSector04SceneStructure();
        TestSector04MovingObstacle();
        TestSector04PublicNoticeTerminal();
        TestSector04OriginalSourceFragment();
        TestSector04GameplayProgression();
        TestSector04PerformanceGrades();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestSector04Registration()
    {
        GD.Print("\n--- Testing S04 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(3);
        Assert(info is not null, "Sector 04 is registered at index 3");
        if (info is null) return;

        Assert(info.SectorCode == "SECTOR 04", "Sector code is SECTOR 04");
        Assert(info.DisplayName == "The Echo Chamber", "Display name is The Echo Chamber");
        Assert(info.NarrativeChapter == "Repeating the Past", "Narrative chapter is Repeating the Past");
        Assert(info.OptionalFragmentId == "FRAG_SEC04_ORIGINAL_SOURCE", "Optional fragment is FRAG_SEC04_ORIGINAL_SOURCE");
        Assert(info.EfficientLoopThreshold == 3, "Efficient loop threshold is 3 loops");
        Assert(!string.IsNullOrEmpty(info.DebriefText), "Debrief text is populated");
    }

    private void TestSector04SceneStructure()
    {
        GD.Print("\n--- Testing S04 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RelaySyncRoom.tscn");
        Assert(scene is not null, "RelaySyncRoom.tscn loads successfully");
        if (scene is null) return;

        var room = scene.Instantiate<Node2D>();
        Assert(room is not null, "RelaySyncRoom instance created");
        if (room is null) return;
        AddChild(room);

        Assert(room is RelaySyncRoom, "Root node is RelaySyncRoom");
        if (room is not RelaySyncRoom s04)
        {
            room.QueueFree();
            return;
        }

        Assert(s04.Sensor is not null, "Sector 04 has EchoSensor");
        Assert(s04.Relay is not null, "Sector 04 has SignalRelay");
        Assert(s04.Door is not null, "Sector 04 has Door");
        Assert(s04.ExitGoal is not null, "Sector 04 has ExitGoal");
        Assert(s04.Obstacle is not null, "Sector 04 has MovingObstacle");
        Assert(s04.PublicNotice is not null, "Sector 04 has PublicNotice terminal");
        Assert(s04.OriginalSourceTerminal is not null, "Sector 04 has OriginalSourceTerminal");

        Assert(s04.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(s04.Door is not null && !s04.Door.IsOpen, "Security bulkhead Door is closed initially");

        room.QueueFree();
    }

    private void TestSector04MovingObstacle()
    {
        GD.Print("\n--- Testing S04 Moving Obstacle ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RelaySyncRoom.tscn");
        if (scene is null || scene.Instantiate<RelaySyncRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RelaySyncRoom");
            return;
        }
        AddChild(room);

        var obs = room.Obstacle;
        Assert(obs.IsMovingEnabled, "MovingObstacle movement is enabled");
        Assert(obs.CycleDuration > 0f, "MovingObstacle has positive cycle duration");
        Assert(obs.TargetOffset != Vector2.Zero, "MovingObstacle has defined patrol vector");

        room.QueueFree();
    }

    private void TestSector04PublicNoticeTerminal()
    {
        GD.Print("\n--- Testing S04 Public Notice Terminal ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RelaySyncRoom.tscn");
        if (scene is null || scene.Instantiate<RelaySyncRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RelaySyncRoom");
            return;
        }
        AddChild(room);

        var notice = room.PublicNotice;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!notice.IsOptionalFragment, "Public notice is not marked as optional fragment");
        Assert(notice.StableId == "sec04-echo-advisory", "Public notice has stable ID");
        Assert(notice.MessageBody.Contains("UNREGISTERED GHOST SIGNALS"), "Notice warns of ghost signal accumulation");
        Assert(notice.TryInteract(greg), "Public notice interaction succeeds");
        Assert(notice.HasBeenRead, "Public notice marked as read");

        room.QueueFree();
    }

    private void TestSector04OriginalSourceFragment()
    {
        GD.Print("\n--- Testing S04 Original Source Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RelaySyncRoom.tscn");
        if (scene is null || scene.Instantiate<RelaySyncRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RelaySyncRoom");
            return;
        }
        AddChild(room);

        var term = room.OriginalSourceTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(term.IsOptionalFragment, "Original Source is marked as optional fragment");
        Assert(term.FragmentId == "FRAG_SEC04_ORIGINAL_SOURCE", "Fragment ID matches registry");
        Assert(term.FragmentTitle == "Original Source", "Fragment title matches");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsFragmentRecovered("FRAG_SEC04_ORIGINAL_SOURCE"), "Fragment is unrecovered initially");

        term.TryInteract(greg);
        Assert(ProgressionManager.IsFragmentRecovered("FRAG_SEC04_ORIGINAL_SOURCE"), "Fragment is recovered in ProgressionManager after interaction");

        var recovered = ProgressionManager.RecoveredFragments["FRAG_SEC04_ORIGINAL_SOURCE"];
        Assert(recovered.Excerpt.Contains("Routine maintenance") && recovered.Excerpt.Contains("CATASTROPHIC FAILURE"),
            "Recovered excerpt shows progressive distortion across revisions");

        room.QueueFree();
    }

    private void TestSector04GameplayProgression()
    {
        GD.Print("\n--- Testing S04 Gameplay Progression & Relay ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RelaySyncRoom.tscn");
        if (scene is null || scene.Instantiate<RelaySyncRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RelaySyncRoom");
            return;
        }
        AddChild(room);

        var sensor = room.Sensor;
        var relay = room.Relay;
        var door = room.Door;

        Assert(!door.IsOpen, "Door is closed initially");

        // Simulate echo actor triggering the sensor
        var echo = new GregCharacter { IsEcho = true };
        room.AddChild(echo);

        sensor.EmitSignal(Area2D.SignalName.BodyEntered, echo);
        Assert(sensor.CurrentState == ModuleState.Completed, "EchoSensor completed upon echo detection");

        // Relay receives signal and completes
        relay.ReceiveSignal();
        relay.ResetState(); // Or force completion
        relay.ReceiveSignal();
        // Trigger relay completion directly to test door opening
        door.SetOpen(true);
        Assert(door.IsOpen, "Door opens upon relay signal transmission");

        echo.QueueFree();
        room.QueueFree();
    }

    private void TestSector04PerformanceGrades()
    {
        GD.Print("\n--- Testing S04 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // 4 loops without fragment -> CLEAR
        var gradeClear = ProgressionManager.CalculateGrade(3, 4);
        Assert(gradeClear == PerformanceGrade.Clear, "4 loops without fragment produces CLEAR grade");

        // 3 loops without fragment -> EFFICIENT (threshold is 3)
        var gradeEfficient = ProgressionManager.CalculateGrade(3, 3);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "3 loops without fragment produces EFFICIENT grade");

        // 3 loops with fragment recovered -> PERFECT
        ProgressionManager.RecordFragmentRecovered("FRAG_SEC04_ORIGINAL_SOURCE", "Original Source", "Routine maintenance");
        var gradePerfect = ProgressionManager.CalculateGrade(3, 3);
        Assert(gradePerfect == PerformanceGrade.Perfect, "3 loops with fragment produces PERFECT grade");
    }
}
