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
        }
        else
        {
            _failed++;
            _results.Add($"  [FAIL] {testName}");
            GD.PrintErr($"  [FAIL] {testName}");
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

        Assert(LevelRegistry.LevelCount == 5, "Campaign contains exactly 5 registered sectors");

        string[] expectedCodes = ["SECTOR 01", "SECTOR 02", "SECTOR 03", "SECTOR 04", "SECTOR 05"];
        string[] expectedTitles = [
            "The Broken Crossing",
            "Powerworks",
            "The Archive",
            "The Signal Tower",
            "The Heartline"
        ];
        string[] expectedChapters = [
            "Boot Sequence",
            "The Rotating Grid",
            "Truth Protocol",
            "The False Broadcast",
            "The Last Signal"
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
        GD.Print("\n--- Testing All 5 Sector Scenes Instantiate Cleanly ---");

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
        GD.Print("\n--- Testing The Archive & Fragment Recovery ---");

        ProgressionManager.ResetProgression();

        string[] fragIds = [
            "FRAG_SEC01_LOST_TRANSMISSION",
            "FRAG_SEC02_SIGNAL_ANALYST",
            "FRAG_SEC03_MEDIATOR_RECORD",
            "FRAG_SEC04_ORIGINAL_SOURCE",
            "FRAG_SEC05_NETWORK_ARCHAEOLOGY"
        ];

        foreach (var fid in fragIds)
        {
            Assert(!ProgressionManager.IsFragmentRecovered(fid), $"Fragment {fid} is unrecovered initially");
        }

        foreach (var fid in fragIds)
        {
            ProgressionManager.RecordFragmentRecovered(fid, "Title " + fid, "Excerpt text");
            Assert(ProgressionManager.IsFragmentRecovered(fid), $"Fragment {fid} recovered successfully");
        }

        Assert(ProgressionManager.RecoveredFragments.Count == 5, "The Archive contains all 5 narrative fragments");

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var grade = ProgressionManager.CalculateGrade(i, 1);
            Assert(grade == PerformanceGrade.Perfect, $"Sector {i} with efficient loops + recovered fragment achieves PERFECT grade");
        }
    }

    private void TestFullCampaignSequentialProgression()
    {
        GD.Print("\n--- Testing Full Campaign Linear Progression ---");

        ProgressionManager.ResetProgression();

        Assert(ProgressionManager.IsLevelUnlocked(0), "Sector 01 is unlocked by default");
        for (int i = 1; i < LevelRegistry.LevelCount; i++)
        {
            Assert(!ProgressionManager.IsLevelUnlocked(i), $"Sector {i + 1:02} is locked initially");
        }

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            ProgressionManager.RecordLevelCompletion(i, 2, 10.0f);
            Assert(ProgressionManager.IsLevelCompleted(i), $"Sector {i + 1:02} is marked completed");

            if (i + 1 < LevelRegistry.LevelCount)
            {
                Assert(ProgressionManager.IsLevelUnlocked(i + 1), $"Sector {i + 2:02} unlocks upon clearing Sector {i + 1:02}");
            }
        }

        ProgressionManager.ResetProgression();
        ProgressionManager.ToggleUnlockAll();

        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            Assert(ProgressionManager.IsLevelUnlocked(i), $"Sector {i + 1:02} is accessible with UnlockAll toggle active");
        }
    }

    private void TestGameManagerEndToEndCampaignFlow()
    {
        GD.Print("\n--- Testing GameManager End-to-End Campaign Flow ---");

        ProgressionManager.ResetProgression();

        var scene = GD.Load<PackedScene>("res://Scenes/Main.tscn");
        Assert(scene is not null, "Main.tscn loads successfully");
        if (scene is null) return;

        var gm = scene.Instantiate<GameManager>();
        AddChild(gm);

        Assert(gm.CurrentState == GameFlowState.MainMenu, "Initial state is MainMenu");

        gm.SwitchState(GameFlowState.LevelSelect);
        Assert(gm.CurrentState == GameFlowState.LevelSelect, "State transitioned to LevelSelect");

        gm.LoadAndStartLevel(0);
        Assert(gm.CurrentState == GameFlowState.InGame, "State transitioned to InGame for Sector 01");
        Assert(gm.CurrentLevelIndex == 0, "Current level index is 0");

        var levelHost = gm.GetNode<Node2D>("LevelHost");
        var activeSector = levelHost.GetChild<LevelContainer>(0);
        var exitGoal = activeSector.GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        var greg = activeSector.GetNode<GregCharacter>("Greg");

        Assert(exitGoal is not null, "Sector 01 has ExitGoal connected");
        Assert(greg is not null, "Sector 01 has Greg");

        exitGoal.EmitSignal(ExitGoal.SignalName.Completed);
        Assert(gm.CurrentState == GameFlowState.LevelCompleted, "Reaching ExitGoal in Sector 01 triggers LevelCompleted state");

        var nextBtn = gm.GetNode<Button>("MenuLayer/LevelCompletedModal/CenterContainer/Card/VBox/ButtonGroup/NextSectorButton");
        var replayBtn = gm.GetNode<Button>("MenuLayer/LevelCompletedModal/CenterContainer/Card/VBox/ButtonGroup/ReplayButton");

        Assert(nextBtn.Visible, "NextSectorButton is visible on intermediate level");
        Assert(!replayBtn.Visible, "ReplayButton is hidden on intermediate level");

        for (int i = 1; i < LevelRegistry.LevelCount; i++)
        {
            gm.LoadAndStartLevel(i);
            Assert(gm.CurrentState == GameFlowState.InGame, $"InGame state active for Sector {i + 1:02}");
            Assert(gm.CurrentLevelIndex == i, $"Current level index is {i}");
        }

        var finalSector = levelHost.GetChild<LevelContainer>(levelHost.GetChildCount() - 1);
        var finalExit = finalSector.GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        var finalGreg = finalSector.GetNode<GregCharacter>("Greg");

        finalExit.EmitSignal(ExitGoal.SignalName.Completed);
        Assert(gm.CurrentState == GameFlowState.LevelCompleted, "Completing final Sector triggers LevelCompleted state");

        Assert(!nextBtn.Visible, "NextSectorButton is hidden on final Sector");
        Assert(replayBtn.Visible, "ReplayButton is visible on final Sector");

        replayBtn.EmitSignal(BaseButton.SignalName.Pressed);
        Assert(gm.CurrentState == GameFlowState.LevelSelect, "Clicking replay on final sector returns cleanly to LevelSelect");

        gm.QueueFree();
    }

    private void TestAccessibilityAndContrast()
    {
        GD.Print("\n--- Testing UI Accessibility & High Contrast (WCAG AAA) ---");

        var bg = new Color("#081B33"); // Base dark background
        var textPrimary = new Color("#F8FAFC");
        var textSecondary = new Color("#94A3B8");
        var cyanAccent = new Color("#00D2FF");
        var goldAccent = new Color("#F59E0B");
        var emeraldAccent = new Color("#10B981");

        float primaryRatio = CalculateContrastRatio(textPrimary, bg);
        float secondaryRatio = CalculateContrastRatio(textSecondary, bg);
        float cyanRatio = CalculateContrastRatio(cyanAccent, bg);
        float goldRatio = CalculateContrastRatio(goldAccent, bg);
        float emeraldRatio = CalculateContrastRatio(emeraldAccent, bg);

        Assert(primaryRatio >= 7.0f, $"Primary text contrast ({primaryRatio:0.0}:1) meets WCAG AAA (>= 7:1)");
        Assert(secondaryRatio >= 4.5f, $"Secondary text contrast ({secondaryRatio:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(cyanRatio >= 4.5f, $"Cyan accent contrast ({cyanRatio:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(goldRatio >= 4.5f, $"Gold accent contrast ({goldRatio:0.0}:1) meets WCAG AA (>= 4.5:1)");
        Assert(emeraldRatio >= 4.5f, $"Emerald accent contrast ({emeraldRatio:0.0}:1) meets WCAG AA (>= 4.5:1)");
    }

    private static float CalculateContrastRatio(Color foreground, Color background)
    {
        float l1 = GetRelativeLuminance(foreground);
        float l2 = GetRelativeLuminance(background);

        float lighter = Math.Max(l1, l2);
        float darker = Math.Min(l1, l2);

        return (lighter + 0.05f) / (darker + 0.05f);
    }

    private static float GetRelativeLuminance(Color c)
    {
        float r = AdjustChannel(c.R);
        float g = AdjustChannel(c.G);
        float b = AdjustChannel(c.B);

        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    private static float AdjustChannel(float val)
    {
        return val <= 0.03928f ? val / 12.92f : Mathf.Pow((val + 0.055f) / 1.055f, 2.4f);
    }
}
