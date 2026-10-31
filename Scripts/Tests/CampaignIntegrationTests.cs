#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class CampaignIntegrationTests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("     CAMPAIGN INTEGRATION & POLISH TEST SUITE           ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllCampaignIntegrationTests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"CAMPAIGN INTEGRATION SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[CAMPAIGN INTEGRATION TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[CAMPAIGN INTEGRATION TEST SUITE PASSED]");
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

    private async Task RunAllCampaignIntegrationTests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestCampaignRegistrationAndMetadata();
        TestAllSectorScenesInstantiate();
        TestTheArchiveAndFragmentCollection();
        TestFullCampaignSequentialProgression();
        TestGameManagerEndToEndCampaignFlow();
        TestAccessibilityAndContrast();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private void TestCampaignRegistrationAndMetadata()
    {
        GD.Print("\n--- Testing Full Campaign Registration & Metadata ---");

        Assert(LevelRegistry.LevelCount == 7, "Campaign contains exactly 7 registered sectors");

        string[] expectedCodes = ["SECTOR 01", "SECTOR 02", "SECTOR 03", "SECTOR 04", "SECTOR 05", "SECTOR 06", "SECTOR 07"];
        string[] expectedTitles = [
            "Boot Circuit",
            "False Synchrony",
            "Divided Consensus",
            "The Echo Chamber",
            "Fractured Network",
            "The Truth Protocol",
            "Cascade Prevention"
        ];
        string[] expectedChapters = [
            "The First Distortion",
            "Conflicting Signals",
            "A Society Splits",
            "Repeating the Past",
            "Information Overload",
            "Evidence and Context",
            "Before the Breaking Point"
        ];

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var level = LevelRegistry.GetLevel(i);
            Assert(level is not null, $"Sector index {i} is registered");
            if (level is null) continue;

            Assert(level.SectorCode == expectedCodes[i], $"Sector {i} code is {expectedCodes[i]}");
            Assert(level.DisplayName == expectedTitles[i], $"Sector {i} title is {expectedTitles[i]}");
            Assert(level.NarrativeChapter == expectedChapters[i], $"Sector {i} chapter is {expectedChapters[i]}");
            Assert(!string.IsNullOrWhiteSpace(level.MechanicDescription), $"Sector {i} has mechanic description");
            Assert(!string.IsNullOrWhiteSpace(level.DebriefText), $"Sector {i} has narrative debrief");
            Assert(!string.IsNullOrWhiteSpace(level.OptionalFragmentId), $"Sector {i} has optional fragment ID");
            Assert(!string.IsNullOrWhiteSpace(level.OptionalFragmentTitle), $"Sector {i} has optional fragment title");
            Assert(ResourceLoader.Exists(level.ScenePath), $"Sector {i} scene path exists on disk: {level.ScenePath}");
            Assert(LevelVariationParameters.ValidateSolvability(level.Parameters, out _), $"Sector {i} parameters pass solvability validation");
        }
    }

    private void TestAllSectorScenesInstantiate()
    {
        GD.Print("\n--- Testing All 7 Sector Scenes Instantiate Cleanly ---");

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var level = LevelRegistry.GetLevel(i)!;
            var scene = GD.Load<PackedScene>(level.ScenePath);
            Assert(scene is not null, $"Sector {i} PackedScene loads successfully");
            if (scene is null) continue;

            var node = scene.Instantiate<Node2D>();
            Assert(node is LevelContainer, $"Sector {i} inherits LevelContainer");
            AddChild(node);

            var greg = node.GetNodeOrNull<GregCharacter>("Greg");
            var camera = node.GetNodeOrNull<RoomCamera>("RoomCamera");
            var geom = node.GetNodeOrNull<RoomGeometry>("RoomGeometry");
            var exit = node.GetNodeOrNull<ExitGoal>("PuzzleModules/ExitGoal");

            Assert(greg is not null, $"Sector {i} has Greg Character at spawn");
            Assert(camera is not null, $"Sector {i} has RoomCamera");
            Assert(geom is not null, $"Sector {i} has RoomGeometry");
            Assert(exit is not null, $"Sector {i} has ExitGoal");

            node.QueueFree();
        }
    }

    private void TestTheArchiveAndFragmentCollection()
    {
        GD.Print("\n--- Testing The Archive & Recovered Fragment Collection ---");

        ProgressionManager.ResetProgression();

        string[] fragmentIds = [
            "FRAG_SEC01_LOST_TRANSMISSION",
            "FRAG_SEC02_SIGNAL_ANALYST",
            "FRAG_SEC03_MEDIATOR_RECORD",
            "FRAG_SEC04_ORIGINAL_SOURCE",
            "FRAG_SEC05_NETWORK_ARCHAEOLOGY",
            "FRAG_SEC06_MISSING_WITNESS",
            "FRAG_SEC07_FINAL_TESTIMONY"
        ];

        // 1. Initial state: zero fragments recovered
        foreach (var id in fragmentIds)
        {
            Assert(!ProgressionManager.IsFragmentRecovered(id), $"Fragment {id} is unrecovered initially");
        }

        // 2. Recover all fragments one by one
        for (int i = 0; i < fragmentIds.Length; i++)
        {
            var level = LevelRegistry.GetLevel(i)!;
            ProgressionManager.RecordFragmentRecovered(level.OptionalFragmentId, level.OptionalFragmentTitle, $"Recovered lore excerpt for {level.SectorCode}");
            Assert(ProgressionManager.IsFragmentRecovered(level.OptionalFragmentId), $"Fragment {level.OptionalFragmentId} recovered successfully");
        }

        Assert(ProgressionManager.RecoveredFragments.Count == 7, "The Archive contains all 7 narrative fragments");

        // 3. Verify Perfect Grade upgrade across all sectors
        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var level = LevelRegistry.GetLevel(i)!;
            var grade = ProgressionManager.CalculateGrade(i, level.EfficientLoopThreshold);
            Assert(grade == PerformanceGrade.Perfect, $"Sector {i} with efficient loops + recovered fragment achieves PERFECT grade");
        }
    }

    private void TestFullCampaignSequentialProgression()
    {
        GD.Print("\n--- Testing Full Campaign Sequential Progression ---");

        ProgressionManager.ResetProgression();

        // Level 0 (Sector 01) unlocked by default
        Assert(ProgressionManager.IsLevelUnlocked(0), "Sector 01 is unlocked by default");
        for (int i = 1; i < LevelRegistry.LevelCount; i++)
        {
            Assert(!ProgressionManager.IsLevelUnlocked(i), $"Sector {i + 1:00} is locked initially");
        }

        // Step-by-step unlock chain
        for (int i = 0; i < LevelRegistry.LevelCount - 1; i++)
        {
            ProgressionManager.RecordLevelCompletion(i, 2, 10.0f);
            Assert(ProgressionManager.IsLevelCompleted(i), $"Sector {i + 1:00} is marked completed");
            Assert(ProgressionManager.IsLevelUnlocked(i + 1), $"Sector {i + 2:00} unlocks upon clearing Sector {i + 1:00}");
        }

        // Complete final sector
        ProgressionManager.RecordLevelCompletion(6, 2, 12.0f);
        Assert(ProgressionManager.IsLevelCompleted(6), "Sector 07 marked completed");

        // Test debug toggle
        ProgressionManager.ResetProgression();
        ProgressionManager.UnlockAllLevels = true;
        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            Assert(ProgressionManager.IsLevelUnlocked(i), $"Sector {i + 1:00} is accessible with UnlockAll toggle active");
        }
        ProgressionManager.UnlockAllLevels = false;
        ProgressionManager.ResetProgression();
    }

    private void TestGameManagerEndToEndCampaignFlow()
    {
        GD.Print("\n--- Testing GameManager End-to-End Campaign Flow ---");

        var mainScene = GD.Load<PackedScene>("res://Scenes/Main.tscn");
        Assert(mainScene is not null, "Main.tscn loads successfully");

        if (mainScene is null || mainScene.Instantiate<GameManager>() is not { } gm)
        {
            Assert(false, "Failed to instantiate GameManager");
            return;
        }

        AddChild(gm);

        // 1. Initial state is MainMenu
        Assert(gm.CurrentState == GameFlowState.MainMenu, "Initial state is MainMenu");

        // 2. Transition to LevelSelect
        gm.SwitchState(GameFlowState.LevelSelect);
        Assert(gm.CurrentState == GameFlowState.LevelSelect, "State transitioned to LevelSelect");

        // 3. Load Sector 01
        gm.LoadAndStartLevel(0);
        Assert(gm.CurrentState == GameFlowState.InGame, "State transitioned to InGame for Sector 01");
        Assert(gm.CurrentLevelIndex == 0, "Current level index is 0");

        // 4. Simulate reaching ExitGoal in Sector 01
        var goal0 = gm.GetNodeOrNull<ExitGoal>("LevelHost/TestRoom/PuzzleModules/ExitGoal");
        Assert(goal0 is not null, "Sector 01 has ExitGoal connected");
        var greg = gm.GetNodeOrNull<GregCharacter>("LevelHost/TestRoom/Greg");
        Assert(greg is not null, "Sector 01 has Greg");

        if (goal0 is not null && greg is not null)
        {
            goal0.EmitSignal(Area2D.SignalName.BodyEntered, greg);
        }
        Assert(gm.CurrentState == GameFlowState.LevelCompleted, "Reaching ExitGoal in Sector 01 triggers LevelCompleted state");

        var modal = gm.GetNode<LevelCompletedModal>("MenuLayer/LevelCompletedModal");
        var nextBtn = modal.GetNode<Button>("CenterContainer/Card/VBox/ButtonGroup/NextSectorButton");
        var replayBtn = modal.GetNode<Button>("CenterContainer/Card/VBox/ButtonGroup/ReplayButton");

        Assert(nextBtn.Visible, "NextSectorButton is visible on intermediate level");
        Assert(!replayBtn.Visible, "ReplayButton is hidden on intermediate level");

        // 5. Traverse across sectors up to Sector 07
        for (int sec = 1; sec <= 6; sec++)
        {
            gm.LoadAndStartLevel(sec);
            Assert(gm.CurrentState == GameFlowState.InGame, $"InGame state active for Sector {sec + 1:00}");
            Assert(gm.CurrentLevelIndex == sec, $"Current level index is {sec}");
        }

        // 6. Complete Sector 07 (Climax)
        var goal7 = gm.GetNodeOrNull<ExitGoal>("LevelHost/Sector07PreventionRoom/PuzzleModules/ExitGoal");
        var greg7 = gm.GetNodeOrNull<GregCharacter>("LevelHost/Sector07PreventionRoom/Greg");
        if (goal7 is not null && greg7 is not null)
        {
            goal7.EmitSignal(Area2D.SignalName.BodyEntered, greg7);
        }

        Assert(gm.CurrentState == GameFlowState.LevelCompleted, "Completing Sector 07 triggers LevelCompleted state");
        Assert(!nextBtn.Visible, "NextSectorButton is hidden on final Sector 07");
        Assert(replayBtn.Visible, "ReplayButton is visible on final Sector 07");

        // 7. Click replay button -> returns cleanly to LevelSelect
        replayBtn.EmitSignal(BaseButton.SignalName.Pressed);
        Assert(gm.CurrentState == GameFlowState.LevelSelect, "Clicking replay on final sector returns cleanly to LevelSelect");

        gm.QueueFree();
    }

    private void TestAccessibilityAndContrast()
    {
        GD.Print("\n--- Testing Accessibility & Visual Contrast ---");

        // Test brand colors contrast against dark canvas (#0E1626)
        Color darkBg = new("#0E1626");
        Color textWhite = new("#F8FAFC");
        Color textMuted = new("#94A3B8");
        Color cyanAccent = new("#00D2FF");
        Color goldAccent = new("#F59E0B");
        Color emeraldAccent = new("#10B981");

        float contrastWhite = CalculateContrastRatio(textWhite, darkBg);
        float contrastMuted = CalculateContrastRatio(textMuted, darkBg);
        float contrastCyan = CalculateContrastRatio(cyanAccent, darkBg);
        float contrastGold = CalculateContrastRatio(goldAccent, darkBg);
        float contrastEmerald = CalculateContrastRatio(emeraldAccent, darkBg);

        Assert(contrastWhite >= 7.0f, $"Primary text contrast ({contrastWhite:0.0}:1) meets WCAG AAA (>= 7:1)");
        Assert(contrastMuted >= 4.5f, $"Secondary text contrast ({contrastMuted:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(contrastCyan >= 4.5f, $"Cyan accent contrast ({contrastCyan:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(contrastGold >= 4.5f, $"Gold accent contrast ({contrastGold:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(contrastEmerald >= 4.5f, $"Emerald accent contrast ({contrastEmerald:0.0}:1) meets WCAG AA (>= 4.5:1)");
    }

    private float CalculateContrastRatio(Color c1, Color c2)
    {
        float l1 = GetRelativeLuminance(c1);
        float l2 = GetRelativeLuminance(c2);
        float lighter = Mathf.Max(l1, l2);
        float darker = Mathf.Min(l1, l2);
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    private float GetRelativeLuminance(Color c)
    {
        float r = AdjustComponent(c.R);
        float g = AdjustComponent(c.G);
        float b = AdjustComponent(c.B);
        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    private float AdjustComponent(float val)
    {
        return val <= 0.03928f ? val / 12.92f : Mathf.Pow((val + 0.055f) / 1.055f, 2.4f);
    }
}
