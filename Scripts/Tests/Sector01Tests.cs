#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector01Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTOR 01: BOOT CIRCUIT TEST SUITE               ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSector01Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTOR 01 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTOR 01 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTOR 01 TEST SUITE PASSED]");
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

    private async Task RunAllSector01Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestSector01Registration();
        TestSector01SceneStructure();
        TestPublicNoticeTerminal();
        TestLostTransmissionOptionalFragment();
        TestGameplayProgressionAndDoor();
        TestGradeCalculation();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestSector01Registration()
    {
        GD.Print("\n--- Testing S01 Registration & Metadata ---");

        var levelInfo = LevelRegistry.GetLevel(0);
        Assert(levelInfo is not null, "Sector 01 is registered at index 0");
        Assert(levelInfo?.SectorCode == "SECTOR 01", "Sector code is SECTOR 01");
        Assert(levelInfo?.DisplayName == "Boot Circuit" || levelInfo?.DisplayName == "The Broken Crossing", "Display name is valid");
        Assert(levelInfo?.NarrativeChapter == "The First Distortion" || levelInfo?.NarrativeChapter == "Boot Sequence", "Narrative chapter is valid");
        Assert(levelInfo?.OptionalFragmentId == "FRAG_SEC01_LOST_TRANSMISSION", "Optional fragment is FRAG_SEC01_LOST_TRANSMISSION");
        Assert(levelInfo?.EfficientLoopThreshold >= 1, "Efficient loop threshold is at least 1");
        Assert(!string.IsNullOrEmpty(levelInfo?.DebriefText), "Debrief text is populated");
    }

    private void TestSector01SceneStructure()
    {
        GD.Print("\n--- Testing S01 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TestRoom.tscn");
        Assert(scene is not null, "TestRoom.tscn loads successfully");
        if (scene is null) return;

        var room = scene.Instantiate<Node2D>();
        Assert(room is not null, "TestRoom instance created");
        if (room is null) return;
        AddChild(room);

        Assert(room is Sector01BootCircuit, "Root node is Sector01BootCircuit");
        if (room is not Sector01BootCircuit sec01)
        {
            room.QueueFree();
            return;
        }

        Assert(sec01.PowerSwitch is not null, "Sector 01 has PowerSwitch");
        Assert(sec01.SecurityDoor is not null, "Sector 01 has SecurityDoor");
        Assert(sec01.ExitTerminal is not null, "Sector 01 has ExitTerminal");
        Assert(sec01.PublicNotice is not null, "Sector 01 has PublicNotice terminal");
        Assert(sec01.LostTransmission is not null, "Sector 01 has LostTransmission terminal");

        Assert(sec01.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(sec01.SecurityDoor is not null && !sec01.SecurityDoor.IsOpen, "SecurityDoor is closed initially");

        room.QueueFree();
    }

    private void TestPublicNoticeTerminal()
    {
        GD.Print("\n--- Testing S01 Public Notice Terminal ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TestRoom.tscn");
        if (scene is null || scene.Instantiate<Sector01BootCircuit>() is not { } room)
        {
            Assert(false, "Failed to instantiate TestRoom");
            return;
        }
        AddChild(room);

        var notice = room.PublicNotice;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(!notice.IsOptionalFragment, "PublicNotice is not marked as optional fragment");
        Assert(notice.StableId == "sec01-public-notice", "PublicNotice has stable ID");
        Assert(notice.GetPrompt(greg) == "Inspect public notice", "PublicNotice returns prompt text");
        Assert(notice.MessageBody.Contains("SYSTEM STATUS: NORMAL"), "Notice contains suspicious normal status");
        Assert(notice.MessageBody.Contains("NETWORK INTEGRITY: 64%"), "Notice contains conflicting 64% integrity");

        bool terminalReadFired = false;
        Action<NarrativeTerminal> handler = t =>
        {
            if (t == notice) terminalReadFired = true;
        };
        NarrativeTerminal.TerminalRead += handler;

        bool interactSuccess = notice.TryInteract(greg);
        NarrativeTerminal.TerminalRead -= handler;

        Assert(interactSuccess, "TryInteract succeeds on PublicNotice");
        Assert(terminalReadFired, "TerminalRead event fired on notice interaction");
        Assert(notice.HasBeenRead, "PublicNotice marked as read");

        room.QueueFree();
    }

    private void TestLostTransmissionOptionalFragment()
    {
        GD.Print("\n--- Testing S01 Lost Transmission Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TestRoom.tscn");
        if (scene?.Instantiate<Sector01BootCircuit>() is not { } room)
        {
            Assert(false, "Failed to instantiate TestRoom");
            return;
        }
        AddChild(room);

        var fragmentTerm = room.LostTransmission;
        var greg = room.GetNode<GregCharacter>("Greg");

        Assert(fragmentTerm.IsOptionalFragment, "LostTransmission is marked as optional fragment");
        Assert(fragmentTerm.FragmentId == "FRAG_SEC01_LOST_TRANSMISSION", "Fragment ID matches registry");
        Assert(fragmentTerm.FragmentTitle == "Lost Transmission", "Fragment title matches");
        Assert(fragmentTerm.GetPrompt(greg).Contains("Lost Transmission"), "Prompt displays fragment title");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsFragmentRecovered("FRAG_SEC01_LOST_TRANSMISSION"), "Fragment is unrecovered initially");

        fragmentTerm.TryInteract(greg);
        Assert(ProgressionManager.IsFragmentRecovered("FRAG_SEC01_LOST_TRANSMISSION"), "Fragment is recovered in ProgressionManager after interaction");

        var recovered = ProgressionManager.RecoveredFragments["FRAG_SEC01_LOST_TRANSMISSION"];
        Assert(recovered.Excerpt.Contains("why did the warning system activate"), "Recovered excerpt matches narrative lore");

        room.QueueFree();
    }

    private void TestGameplayProgressionAndDoor()
    {
        GD.Print("\n--- Testing S01 Gameplay Progression & Door ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/TestRoom.tscn");
        if (scene?.Instantiate<Sector01BootCircuit>() is not { } room)
        {
            Assert(false, "Failed to instantiate TestRoom");
            return;
        }
        AddChild(room);

        var greg = room.GetNode<GregCharacter>("Greg");
        var sw = room.PowerSwitch;
        var door = room.SecurityDoor;

        Assert(!door.IsOpen, "Door is closed before switch toggle");
        sw.TryInteract(greg);
        Assert(sw.IsActive, "Switch is active after interaction");
        Assert(door.IsOpen, "Door opened upon switch activation");

        room.QueueFree();
    }

    private void TestGradeCalculation()
    {
        GD.Print("\n--- Testing S01 Performance Grades ---");

        ProgressionManager.ResetProgression();

        var level = LevelRegistry.GetLevel(0);
        int thresh = level?.EfficientLoopThreshold ?? 1;

        // Level completed exceeding threshold without fragment => Clear
        var gradeClear = ProgressionManager.CalculateGrade(0, thresh + 1);
        Assert(gradeClear == PerformanceGrade.Clear, "Exceeding threshold without fragment produces CLEAR grade");

        // Level completed in thresh loops without fragment => Efficient
        var gradeEfficient = ProgressionManager.CalculateGrade(0, thresh);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "Efficient loop threshold produces EFFICIENT grade");

        // Recover fragment, thresh loops => Perfect
        ProgressionManager.RecordFragmentRecovered("FRAG_SEC01_LOST_TRANSMISSION", "Lost Transmission", "Excerpt");
        var gradePerfect = ProgressionManager.CalculateGrade(0, thresh);
        Assert(gradePerfect == PerformanceGrade.Perfect, "Efficient loops with fragment produces PERFECT grade");
    }
}
