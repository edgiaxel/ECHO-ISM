#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector05Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTOR 05: FRACTURED NETWORK TEST SUITE          ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSector05Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTOR 05 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTOR 05 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTOR 05 TEST SUITE PASSED]");
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

    private async Task RunAllSector05Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestSector05Registration();
        TestSector05SceneStructure();
        TestSector05SeededParameters();
        TestSector05PublicNoticeTerminal();
        TestSector05NetworkArchaeologyFragment();
        TestSector05DualMechanismProgression();
        TestSector05PerformanceGrades();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestSector05Registration()
    {
        GD.Print("\n--- Testing S05 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(4);
        Assert(info is not null, "Sector 05 is registered at index 4");
        if (info is null) return;

        Assert(info.SectorCode == "SECTOR 05", "Sector code is SECTOR 05");
        Assert(info.DisplayName == "Fractured Network", "Display name is Fractured Network");
        Assert(info.NarrativeChapter == "Information Overload", "Narrative chapter is Information Overload");
        Assert(info.OptionalFragmentId == "FRAG_SEC05_NETWORK_ARCHAEOLOGY", "Optional fragment is FRAG_SEC05_NETWORK_ARCHAEOLOGY");
        Assert(info.EfficientLoopThreshold == 3, "Efficient loop threshold is 3 loops");
        Assert(!string.IsNullOrEmpty(info.DebriefText), "Debrief text is populated");
    }

    private void TestSector05SceneStructure()
    {
        GD.Print("\n--- Testing S05 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RemixStabilizationRoom.tscn");
        Assert(scene is not null, "RemixStabilizationRoom.tscn loads successfully");
        if (scene is null) return;

        var room = scene.Instantiate<Node2D>();
        Assert(room is not null, "RemixStabilizationRoom instance created");
        if (room is null) return;
        AddChild(room);

        Assert(room is RemixStabilizationRoom, "Root node is RemixStabilizationRoom");
        if (room is not RemixStabilizationRoom s05)
        {
            room.QueueFree();
            return;
        }

        Assert(s05.Slider is not null, "Sector 05 has TimingSlider");
        Assert(s05.Plate is not null, "Sector 05 has PressurePlate");
        Assert(s05.Door is not null, "Sector 05 has DualMechanismDoor");
        Assert(s05.ExitGoal is not null, "Sector 05 has ExitGoal");
        Assert(s05.PublicNotice is not null, "Sector 05 has PublicNotice terminal");
        Assert(s05.NetworkArchaeologyTerminal is not null, "Sector 05 has NetworkArchaeologyTerminal");

        Assert(s05.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(s05.Door is not null && !s05.Door.IsOpen, "DualMechanismDoor is closed initially");

        room.QueueFree();
    }

    private void TestSector05SeededParameters()
    {
        GD.Print("\n--- Testing S05 Seeded Parameters ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RemixStabilizationRoom.tscn");
        if (scene is null || scene.Instantiate<RemixStabilizationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RemixStabilizationRoom");
            return;
        }
        AddChild(room);

        var p = room.AppliedParameters;
        Assert(p.Seed == 42, "Seed matches default configuration (42)");
        Assert(p.TemplateId == "RemixStabilization", "Template id is RemixStabilization");
        Assert(p.LoopDurationSeconds >= 8f, "Loop duration is valid");
        Assert(p.MaximumActiveEchoes >= 1, "Echo limit is valid");
        Assert(LevelVariationParameters.ValidateSolvability(p, out _), "Seeded configuration is verified solvable");

        room.QueueFree();
    }

    private void TestSector05PublicNoticeTerminal()
    {
        GD.Print("\n--- Testing S05 Public Notice Terminal ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RemixStabilizationRoom.tscn");
        if (scene is null || scene.Instantiate<RemixStabilizationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RemixStabilizationRoom");
            return;
        }
        AddChild(room);

        var notice = room.PublicNotice;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!notice.IsOptionalFragment, "Public notice is not marked as optional fragment");
        Assert(notice.StableId == "sec05-fractured-bulletin", "Public notice has stable ID");
        Assert(notice.MessageBody.Contains("CROSS-SECTOR COMM-LINES SEVERED"), "Notice details cross-sector severing");
        Assert(notice.TryInteract(greg), "Public notice interaction succeeds");
        Assert(notice.HasBeenRead, "Public notice marked as read");

        room.QueueFree();
    }

    private void TestSector05NetworkArchaeologyFragment()
    {
        GD.Print("\n--- Testing S05 Network Archaeology Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RemixStabilizationRoom.tscn");
        if (scene is null || scene.Instantiate<RemixStabilizationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RemixStabilizationRoom");
            return;
        }
        AddChild(room);

        var term = room.NetworkArchaeologyTerminal;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(term.IsOptionalFragment, "Network Archaeology is marked as optional fragment");
        Assert(term.FragmentId == "FRAG_SEC05_NETWORK_ARCHAEOLOGY", "Fragment ID matches registry");
        Assert(term.FragmentTitle == "Network Archaeology", "Fragment title matches");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsFragmentRecovered("FRAG_SEC05_NETWORK_ARCHAEOLOGY"), "Fragment is unrecovered initially");

        term.TryInteract(greg);
        Assert(ProgressionManager.IsFragmentRecovered("FRAG_SEC05_NETWORK_ARCHAEOLOGY"), "Fragment is recovered in ProgressionManager after interaction");

        var recovered = ProgressionManager.RecoveredFragments["FRAG_SEC05_NETWORK_ARCHAEOLOGY"];
        Assert(recovered.Excerpt.Contains("TIMELINE RECONSTRUCTION") && recovered.Excerpt.Contains("phased countdown to isolate us"),
            "Recovered excerpt reconstructs orchestrated countdown timeline");

        room.QueueFree();
    }

    private void TestSector05DualMechanismProgression()
    {
        GD.Print("\n--- Testing S05 Dual Mechanism Progression ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/RemixStabilizationRoom.tscn");
        if (scene is null || scene.Instantiate<RemixStabilizationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate RemixStabilizationRoom");
            return;
        }
        AddChild(room);

        var door = room.Door;
        var plate = room.Plate;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!door.IsOpen, "DualDoor is closed initially");

        // Input A only
        plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(!door.IsOpen, "Door remains closed with Input A (plate) only");

        // Input B only (set through door SetInputB)
        plate.EmitSignal(Area2D.SignalName.BodyExited, greg);
        door.SetInputB(true);
        Assert(!door.IsOpen, "Door remains closed with Input B (slider) only");

        // Both active simultaneously
        plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(door.IsOpen, "DualDoor opens when Input A and Input B are active simultaneously");

        room.QueueFree();
    }

    private void TestSector05PerformanceGrades()
    {
        GD.Print("\n--- Testing S05 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // 4 loops without fragment -> CLEAR
        var gradeClear = ProgressionManager.CalculateGrade(4, 4);
        Assert(gradeClear == PerformanceGrade.Clear, "4 loops without fragment produces CLEAR grade");

        // 3 loops without fragment -> EFFICIENT (threshold is 3)
        var gradeEfficient = ProgressionManager.CalculateGrade(4, 3);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "3 loops without fragment produces EFFICIENT grade");

        // 3 loops with fragment recovered -> PERFECT
        ProgressionManager.RecordFragmentRecovered("FRAG_SEC05_NETWORK_ARCHAEOLOGY", "Network Archaeology", "TIMELINE RECONSTRUCTION");
        var gradePerfect = ProgressionManager.CalculateGrade(4, 3);
        Assert(gradePerfect == PerformanceGrade.Perfect, "3 loops with fragment produces PERFECT grade");
    }
}
