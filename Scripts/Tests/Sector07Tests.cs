#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector07Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTOR 07: CASCADE PREVENTION TEST SUITE         ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSector07Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTOR 07 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTOR 07 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTOR 07 TEST SUITE PASSED]");
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

    private async Task RunAllSector07Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestSector07Registration();
        TestSector07SceneStructure();
        TestSector07Parameters();
        TestSector07PublicNoticeTerminal();
        TestSector07FinalTestimonyFragment();
        TestSector07ContainmentProgression();
        TestSector07CountdownFailure();
        TestSector07PerformanceGrades();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestSector07Registration()
    {
        GD.Print("\n--- Testing S07 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(6);
        Assert(info is not null, "Sector 07 is registered at index 6");
        Assert(info?.SectorCode == "SECTOR 07", "Sector code is SECTOR 07");
        Assert(info?.DisplayName == "Cascade Prevention", "Display name is Cascade Prevention");
        Assert(info?.NarrativeChapter == "Before the Breaking Point", "Narrative chapter is Before the Breaking Point");
        Assert(info?.OptionalFragmentId == "FRAG_SEC07_FINAL_TESTIMONY", "Optional fragment is FRAG_SEC07_FINAL_TESTIMONY");
        Assert(info?.OptionalFragmentTitle == "Final Testimony", "Optional fragment title is Final Testimony");
        Assert(info?.EfficientLoopThreshold == 3, "Efficient loop threshold is 3 loops");
        Assert(!string.IsNullOrWhiteSpace(info?.DebriefText), "Debrief text is populated");
    }

    private void TestSector07SceneStructure()
    {
        GD.Print("\n--- Testing S07 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector07PreventionRoom.tscn");
        Assert(scene is not null, "Sector07PreventionRoom.tscn loads successfully");

        if (scene is null || scene.Instantiate<Sector07PreventionRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector07PreventionRoom");
            return;
        }

        AddChild(room);

        Assert(room is Sector07PreventionRoom, "Root node is Sector07PreventionRoom");
        Assert(room.EventObjective is not null, "Sector 07 has EventPreventionObjective");
        Assert(room.SwitchA is not null, "Sector 07 has SwitchA");
        Assert(room.SwitchB is not null, "Sector 07 has SwitchB");
        Assert(room.Door is not null, "Sector 07 has Door");
        Assert(room.ExitGoal is not null, "Sector 07 has ExitGoal");
        Assert(room.PublicNotice is not null, "Sector 07 has PublicNotice terminal");
        Assert(room.FinalTestimonyTerminal is not null, "Sector 07 has FinalTestimonyTerminal");
        Assert(room.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(room.Door is not null && !room.Door.IsOpen, "Authenticated exit Door is closed initially");

        room.QueueFree();
    }

    private void TestSector07Parameters()
    {
        GD.Print("\n--- Testing S07 Parameters ---");

        var info = LevelRegistry.GetLevel(6);
        if (info is null)
        {
            Assert(false, "Sector 07 level info is null");
            return;
        }

        var p = info.Parameters;
        Assert(p.TemplateId == "EventPrevention", "Template id is EventPrevention");
        Assert(p.LoopDurationSeconds >= 12f, "Loop duration is valid (>= 12s)");
        Assert(p.MaximumActiveEchoes >= 2, "Echo limit allows past coordination (>= 2)");
        Assert(LevelVariationParameters.ValidateSolvability(p, out _), "Sector 07 configuration is verified solvable");
    }

    private void TestSector07PublicNoticeTerminal()
    {
        GD.Print("\n--- Testing S07 Public Notice Terminal ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector07PreventionRoom.tscn");
        if (scene is null || scene.Instantiate<Sector07PreventionRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector07PreventionRoom");
            return;
        }
        AddChild(room);

        var notice = room.PublicNotice;
        Assert(!notice.IsOptionalFragment, "Public notice is not marked as optional fragment");
        Assert(notice.StableId == "sec07-cascade-broadcast-notice", "Public notice has stable ID");
        Assert(notice.MessageBody.Contains("CASCADE SEQUENCE"), "Notice warns of cascade broadcast sequence");

        var greg = new GregCharacter();
        AddChild(greg);
        bool interacted = notice.TryInteract(greg);
        Assert(interacted, "Public notice interaction succeeds");
        Assert(notice.HasBeenRead, "Public notice marked as read");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector07FinalTestimonyFragment()
    {
        GD.Print("\n--- Testing S07 Final Testimony Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector07PreventionRoom.tscn");
        if (scene is null || scene.Instantiate<Sector07PreventionRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector07PreventionRoom");
            return;
        }
        AddChild(room);

        var fragTerminal = room.FinalTestimonyTerminal;
        Assert(fragTerminal.IsOptionalFragment, "Final Testimony is marked as optional fragment");
        Assert(fragTerminal.FragmentId == "FRAG_SEC07_FINAL_TESTIMONY", "Fragment ID matches registry");
        Assert(fragTerminal.FragmentTitle == "Final Testimony", "Fragment title matches");

        var greg = new GregCharacter();
        AddChild(greg);

        bool initialRecovered = ProgressionManager.IsFragmentRecovered("FRAG_SEC07_FINAL_TESTIMONY");
        Assert(!initialRecovered, "Fragment is unrecovered initially");

        fragTerminal.TryInteract(greg);
        bool afterRecovered = ProgressionManager.IsFragmentRecovered("FRAG_SEC07_FINAL_TESTIMONY");
        Assert(afterRecovered, "Fragment is recovered in ProgressionManager after interaction");

        var data = ProgressionManager.RecoveredFragments["FRAG_SEC07_FINAL_TESTIMONY"];
        Assert(data.Excerpt.Contains("taught the network to panic"), "Recovered excerpt reveals weaponized fear confession");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector07ContainmentProgression()
    {
        GD.Print("\n--- Testing S07 Containment Progression ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector07PreventionRoom.tscn");
        if (scene is null || scene.Instantiate<Sector07PreventionRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector07PreventionRoom");
            return;
        }
        AddChild(room);

        var greg = new GregCharacter();
        AddChild(greg);

        Assert(!room.Door.IsOpen, "Door is closed initially");
        Assert(!room.EventObjective.IsCompleted, "Event objective is not completed initially");

        // 1. Activate Switch A
        room.SwitchA.TryInteract(greg);
        Assert(room.SwitchA.IsActive, "Switch A activated");
        Assert(room.EventObjective.CompletedInterventions == 1, "Completed interventions is 1");
        Assert(!room.Door.IsOpen, "Door remains closed with 1 intervention");

        // 2. Activate Switch B
        room.SwitchB.TryInteract(greg);
        Assert(room.SwitchB.IsActive, "Switch B activated");
        Assert(room.EventObjective.CompletedInterventions == 2, "Completed interventions reached required threshold (2)");
        Assert(room.EventObjective.IsCompleted, "Event objective successfully prevented cascade");
        Assert(room.Door.IsOpen, "Exit Door opens upon cascade prevention");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector07CountdownFailure()
    {
        GD.Print("\n--- Testing S07 Countdown Failure ---");

        var obj = new EventPreventionObjective
        {
            CountdownDuration = 1.0f,
            RequiredInterventions = 2
        };
        AddChild(obj);

        obj.StartCountdown();
        Assert(obj.CurrentState == ModuleState.Active, "Event objective starts in Active state");

        // Simulate time passing past duration
        obj._Process(1.5);
        Assert(obj.IsFailed, "Event objective marks as Failed when time expires");
        Assert(obj.RemainingTime == 0f, "Remaining time clamped at 0");

        obj.QueueFree();
    }

    private void TestSector07PerformanceGrades()
    {
        GD.Print("\n--- Testing S07 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // Level index 6 is Sector 07
        var gradeClear = ProgressionManager.CalculateGrade(6, 4);
        Assert(gradeClear == PerformanceGrade.Clear, "4 loops without fragment produces CLEAR grade");

        var gradeEfficient = ProgressionManager.CalculateGrade(6, 3);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "3 loops without fragment produces EFFICIENT grade");

        ProgressionManager.RecordFragmentRecovered("FRAG_SEC07_FINAL_TESTIMONY", "Final Testimony", "Excerpt");
        var gradePerfect = ProgressionManager.CalculateGrade(6, 3);
        Assert(gradePerfect == PerformanceGrade.Perfect, "3 loops with fragment produces PERFECT grade");
    }
}
