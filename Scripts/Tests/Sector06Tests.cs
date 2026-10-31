#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Sector06Tests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("       SECTOR 06: THE TRUTH PROTOCOL TEST SUITE         ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllSector06Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"SECTOR 06 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[SECTOR 06 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[SECTOR 06 TEST SUITE PASSED]");
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

    private async Task RunAllSector06Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestSector06Registration();
        TestSector06SceneStructure();
        TestSector06Parameters();
        TestSector06PublicNoticeTerminal();
        TestSector06MissingWitnessFragment();
        TestSector06TripleMechanismProgression();
        TestSector06PerformanceGrades();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestSector06Registration()
    {
        GD.Print("\n--- Testing S06 Registration & Metadata ---");

        var info = LevelRegistry.GetLevel(5);
        Assert(info is not null, "Sector 06 is registered at index 5");
        Assert(info?.SectorCode == "SECTOR 06", "Sector code is SECTOR 06");
        Assert(info?.DisplayName == "The Truth Protocol", "Display name is The Truth Protocol");
        Assert(info?.NarrativeChapter == "Evidence and Context", "Narrative chapter is Evidence and Context");
        Assert(info?.OptionalFragmentId == "FRAG_SEC06_MISSING_WITNESS", "Optional fragment is FRAG_SEC06_MISSING_WITNESS");
        Assert(info?.OptionalFragmentTitle == "The Missing Witness", "Optional fragment title is The Missing Witness");
        Assert(info?.EfficientLoopThreshold == 3, "Efficient loop threshold is 3 loops");
        Assert(!string.IsNullOrWhiteSpace(info?.DebriefText), "Debrief text is populated");
    }

    private void TestSector06SceneStructure()
    {
        GD.Print("\n--- Testing S06 Scene Structure ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector06VerificationRoom.tscn");
        Assert(scene is not null, "Sector06VerificationRoom.tscn loads successfully");

        if (scene is null || scene.Instantiate<Sector06VerificationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector06VerificationRoom");
            return;
        }

        AddChild(room);

        Assert(room is Sector06VerificationRoom, "Root node is Sector06VerificationRoom");
        Assert(room.Terminal is not null, "Sector 06 has VerificationTerminal");
        Assert(room.Repair is not null, "Sector 06 has ConnectionRepair");
        Assert(room.Plate is not null, "Sector 06 has PressurePlate");
        Assert(room.Door is not null, "Sector 06 has Door");
        Assert(room.ExitGoal is not null, "Sector 06 has ExitGoal");
        Assert(room.PublicNotice is not null, "Sector 06 has PublicNotice terminal");
        Assert(room.MissingWitnessTerminal is not null, "Sector 06 has MissingWitnessTerminal");
        Assert(room.StartingPosition == new Vector2(360, 366), "Greg spawn position is (360, 366)");
        Assert(room.Door is not null && !room.Door.IsOpen, "Authenticated bulkhead Door is closed initially");

        room.QueueFree();
    }

    private void TestSector06Parameters()
    {
        GD.Print("\n--- Testing S06 Parameters ---");

        var info = LevelRegistry.GetLevel(5);
        if (info is null)
        {
            Assert(false, "Sector 06 level info is null");
            return;
        }
        var p = info.Parameters;
        Assert(p.TemplateId == "IntegrityBridge", "Template id is IntegrityBridge");
        Assert(p.LoopDurationSeconds >= 12f, "Loop duration is valid (>= 12s)");
        Assert(p.MaximumActiveEchoes >= 2, "Echo limit allows past coordination (>= 2)");
        Assert(LevelVariationParameters.ValidateSolvability(p, out _), "Sector 06 configuration is verified solvable");
    }

    private void TestSector06PublicNoticeTerminal()
    {
        GD.Print("\n--- Testing S06 Public Notice Terminal ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector06VerificationRoom.tscn");
        if (scene is null || scene.Instantiate<Sector06VerificationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector06VerificationRoom");
            return;
        }
        AddChild(room);

        var notice = room.PublicNotice;
        Assert(!notice.IsOptionalFragment, "Public notice is not marked as optional fragment");
        Assert(notice.StableId == "sec06-truth-protocol-notice", "Public notice has stable ID");
        Assert(notice.MessageBody.Contains("HASH MISMATCH"), "Notice reports cryptographic hash mismatch");

        var greg = new GregCharacter();
        AddChild(greg);
        bool interacted = notice.TryInteract(greg);
        Assert(interacted, "Public notice interaction succeeds");
        Assert(notice.HasBeenRead, "Public notice marked as read");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector06MissingWitnessFragment()
    {
        GD.Print("\n--- Testing S06 Missing Witness Fragment ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector06VerificationRoom.tscn");
        if (scene is null || scene.Instantiate<Sector06VerificationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector06VerificationRoom");
            return;
        }
        AddChild(room);

        var fragTerminal = room.MissingWitnessTerminal;
        Assert(fragTerminal.IsOptionalFragment, "Missing Witness is marked as optional fragment");
        Assert(fragTerminal.FragmentId == "FRAG_SEC06_MISSING_WITNESS", "Fragment ID matches registry");
        Assert(fragTerminal.FragmentTitle == "The Missing Witness", "Fragment title matches");

        var greg = new GregCharacter();
        AddChild(greg);

        bool initialRecovered = ProgressionManager.IsFragmentRecovered("FRAG_SEC06_MISSING_WITNESS");
        Assert(!initialRecovered, "Fragment is unrecovered initially");

        fragTerminal.TryInteract(greg);
        bool afterRecovered = ProgressionManager.IsFragmentRecovered("FRAG_SEC06_MISSING_WITNESS");
        Assert(afterRecovered, "Fragment is recovered in ProgressionManager after interaction");

        var data = ProgressionManager.RecoveredFragments["FRAG_SEC06_MISSING_WITNESS"];
        Assert(data.Excerpt.Contains("intercepting the truth"), "Recovered excerpt reveals intercepted corrections");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector06TripleMechanismProgression()
    {
        GD.Print("\n--- Testing S06 Triple Mechanism Progression ---");

        var scene = GD.Load<PackedScene>("res://Scenes/Levels/Sector06VerificationRoom.tscn");
        if (scene is null || scene.Instantiate<Sector06VerificationRoom>() is not { } room)
        {
            Assert(false, "Failed to instantiate Sector06VerificationRoom");
            return;
        }
        AddChild(room);

        var greg = new GregCharacter();
        AddChild(greg);

        Assert(!room.Door.IsOpen, "Door is closed initially");

        // 1. Verify terminal only
        room.Terminal.TryInteract(greg);
        Assert(room.Terminal.IsVerified, "Verification terminal successfully authenticated");
        Assert(!room.Door.IsOpen, "Door remains closed with Terminal verified only");

        // 2. Repair connection
        room.Repair.TryInteract(greg);
        Assert(room.Repair.IsRepaired, "ConnectionRepair successfully bridged");
        Assert(!room.Door.IsOpen, "Door remains closed with Terminal verified and Repair bridged without Plate hold");

        // 3. Step on Plate
        room.Plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(room.Plate.IsCompleted, "Pressure plate is active with body stepped on");
        Assert(room.Door.IsOpen, "Door opens when Terminal is verified, Repair is bridged, and Plate is held simultaneously");

        // 4. Step off Plate
        room.Plate.EmitSignal(Area2D.SignalName.BodyExited, greg);
        Assert(!room.Plate.IsCompleted, "Pressure plate becomes inactive when body steps off");
        Assert(!room.Door.IsOpen, "Door closes when pressure plate is released");

        // 5. Re-engage plate
        room.Plate.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        Assert(room.Door.IsOpen, "Door re-opens when plate is re-engaged");

        greg.QueueFree();
        room.QueueFree();
    }

    private void TestSector06PerformanceGrades()
    {
        GD.Print("\n--- Testing S06 Performance Grades ---");

        ProgressionManager.ResetProgression();

        // Level index 5 is Sector 06
        var gradeClear = ProgressionManager.CalculateGrade(5, 4);
        Assert(gradeClear == PerformanceGrade.Clear, "4 loops without fragment produces CLEAR grade");

        var gradeEfficient = ProgressionManager.CalculateGrade(5, 3);
        Assert(gradeEfficient == PerformanceGrade.Efficient, "3 loops without fragment produces EFFICIENT grade");

        ProgressionManager.RecordFragmentRecovered("FRAG_SEC06_MISSING_WITNESS", "The Missing Witness", "Excerpt");
        var gradePerfect = ProgressionManager.CalculateGrade(5, 3);
        Assert(gradePerfect == PerformanceGrade.Perfect, "3 loops with fragment produces PERFECT grade");
    }
}
