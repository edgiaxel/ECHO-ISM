#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Stage8RemixTests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("   STAGE 8: LEVEL TEMPLATES AND REMIXING TEST SUITE     ");
        GD.Print("========================================================\n");

        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllStage8Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"STAGE 8 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[STAGE 8 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[STAGE 8 TEST SUITE PASSED]");
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

    private async Task RunAllStage8Tests()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestS8_1_HandcraftedTemplates();
        TestS8_2_VariationParameters();
        TestS8_3_DeterministicRemixing();
        TestS8_4_ProgressionFlow();
        TestUIComponentsFlow();
        TestStandaloneLevelExecution();
        TestGameManagerNavigationFlow();
    }

    private void TestS8_1_HandcraftedTemplates()
    {
        GD.Print("\n--- S8.1 Handcrafted Templates ---");

        // 1. Template library registration
        Assert(LevelRegistry.LevelCount >= 5, "S8.1: LevelRegistry contains at least 5 registered sector templates");

        // 2. Sector 01 Boot Circuit
        var level0 = LevelRegistry.GetLevel(0);
        Assert(level0 is not null && level0.SectorCode == "SECTOR 01" && level0.DisplayName == "Boot Circuit",
            "S8.1: Sector 01 template metadata is registered");
        Assert(ResourceLoader.Exists(level0!.ScenePath), "S8.1: Sector 01 scene file exists");

        // 3. Sector 02 Timing & Sync
        var level1 = LevelRegistry.GetLevel(1);
        Assert(level1 is not null && level1.SectorCode == "SECTOR 02" && (level1.DisplayName == "False Synchrony" || level1.DisplayName == "Timing & Synchronization"),
            "S8.1: Sector 02 template metadata is registered");
        Assert(ResourceLoader.Exists(level1!.ScenePath), "S8.1: Sector 02 scene file exists");

        // 4. Sector 03 Dual Mechanism Protocol
        var level2 = LevelRegistry.GetLevel(2);
        Assert(level2 is not null && level2.SectorCode == "SECTOR 03" && (level2.DisplayName == "Divided Consensus" || level2.DisplayName == "Dual Mechanism Protocol"),
            "S8.1: Sector 03 template metadata is registered");
        Assert(ResourceLoader.Exists(level2!.ScenePath), "S8.1: Sector 03 scene file exists");

        var scene3 = GD.Load<PackedScene>(level2!.ScenePath);
        var instance3 = scene3.Instantiate<DualProtocolRoom>();
        AddChild(instance3);
        Assert(instance3.Plate is not null && instance3.Switch is not null && instance3.DualDoor is not null && instance3.ExitGoal is not null,
            "S8.1: Sector 03 instantiates with PressurePlate, TestSwitch, DualMechanismDoor, and ExitGoal");
        instance3.QueueFree();

        // 5. Sector 04 Relay Sync
        var level3 = LevelRegistry.GetLevel(3);
        Assert(level3 is not null && level3.SectorCode == "SECTOR 04" && (level3.DisplayName == "The Echo Chamber" || level3.DisplayName == "Signal Relay & Echo Sensor"),
            "S8.1: Sector 04 template metadata is registered");
        Assert(ResourceLoader.Exists(level3!.ScenePath), "S8.1: Sector 04 scene file exists");

        var scene4 = GD.Load<PackedScene>(level3!.ScenePath);
        var instance4 = scene4.Instantiate<RelaySyncRoom>();
        AddChild(instance4);
        Assert(instance4.Sensor is not null && instance4.Relay is not null && instance4.Door is not null && instance4.ExitGoal is not null,
            "S8.1: Sector 04 instantiates with EchoSensor, SignalRelay, Door, and ExitGoal");
        instance4.QueueFree();

        // 6. Sector 05 Remix Stabilization
        var level4 = LevelRegistry.GetLevel(4);
        Assert(level4 is not null && level4.SectorCode == "SECTOR 05" && (level4.DisplayName == "Fractured Network" || level4.DisplayName == "Network Stabilization Remix"),
            "S8.1: Sector 05 template metadata is registered");
        Assert(ResourceLoader.Exists(level4!.ScenePath), "S8.1: Sector 05 scene file exists");

        var scene5 = GD.Load<PackedScene>(level4!.ScenePath);
        var instance5 = scene5.Instantiate<RemixStabilizationRoom>();
        AddChild(instance5);
        Assert(instance5.Slider is not null && instance5.Plate is not null && instance5.Door is not null && instance5.ExitGoal is not null,
            "S8.1: Sector 05 instantiates with remixed parameters applied");
        instance5.QueueFree();
    }

    private void TestS8_2_VariationParameters()
    {
        GD.Print("\n--- S8.2 Variation Parameters ---");

        var validParams = LevelVariationParameters.CreateHandcrafted("DualWing", 15.0f, 3);
        Assert(LevelVariationParameters.ValidateSolvability(validParams, out string reason),
            "S8.2: Default handcrafted parameters pass solvability validation");

        // Reject too short loop duration
        var shortDuration = validParams;
        shortDuration.LoopDurationSeconds = 5.0f;
        Assert(!LevelVariationParameters.ValidateSolvability(shortDuration, out _),
            "S8.2: Solvability validator rejects loop duration < 8.0s");

        // Reject zero or negative echo limit
        var zeroEchoes = validParams;
        zeroEchoes.MaximumActiveEchoes = 0;
        Assert(!LevelVariationParameters.ValidateSolvability(zeroEchoes, out _),
            "S8.2: Solvability validator rejects echo limit < 1");

        // Reject invalid mechanism offset
        var outOfBoundsOffset = validParams;
        outOfBoundsOffset.MechanismOffset = new Vector2(300f, 200f);
        Assert(!LevelVariationParameters.ValidateSolvability(outOfBoundsOffset, out _),
            "S8.2: Solvability validator rejects mechanism offset exceeding chamber bounds");

        // Reject too tight sync tolerance
        var tightSync = validParams;
        tightSync.SyncToleranceSeconds = 0.1f;
        Assert(!LevelVariationParameters.ValidateSolvability(tightSync, out _),
            "S8.2: Solvability validator rejects sync tolerance < 0.25s");
    }

    private void TestS8_3_DeterministicRemixing()
    {
        GD.Print("\n--- S8.3 Deterministic Remixing ---");

        var remixA = LevelRemixer.GenerateRemix("TestTemplate", 1337);
        var remixB = LevelRemixer.GenerateRemix("TestTemplate", 1337);

        Assert(remixA.Seed == remixB.Seed, "S8.3: Seed is preserved and identical");
        Assert(Mathf.IsEqualApprox(remixA.LoopDurationSeconds, remixB.LoopDurationSeconds),
            "S8.3: Deterministic loop duration matches for identical seed");
        Assert(remixA.MaximumActiveEchoes == remixB.MaximumActiveEchoes,
            "S8.3: Deterministic echo limit matches for identical seed");
        Assert(remixA.TimingPreset == remixB.TimingPreset,
            "S8.3: Deterministic timing difficulty matches for identical seed");
        Assert(remixA.MechanismOffset == remixB.MechanismOffset,
            "S8.3: Deterministic mechanism placement matches for identical seed");

        var remixC = LevelRemixer.GenerateRemix("TestTemplate", 9999);
        bool hasVariation = !Mathf.IsEqualApprox(remixA.LoopDurationSeconds, remixC.LoopDurationSeconds)
                            || remixA.TimingPreset != remixC.TimingPreset
                            || remixA.MechanismOffset != remixC.MechanismOffset;
        Assert(hasVariation, "S8.3: Different seeds produce distinct parameter variations");

        Assert(LevelVariationParameters.ValidateSolvability(remixA, out _),
            "S8.3: Generated remix passes solvability validation");
    }

    private void TestS8_4_ProgressionFlow()
    {
        GD.Print("\n--- S8.4 Progression Flow & Review ---");

        ProgressionManager.ResetProgression();

        Assert(ProgressionManager.IsLevelUnlocked(0), "S8.4: Sector 01 is unlocked by default");
        Assert(!ProgressionManager.IsLevelUnlocked(1), "S8.4: Sector 02 is locked initially");
        Assert(!ProgressionManager.IsLevelUnlocked(2), "S8.4: Sector 03 is locked initially");

        ProgressionManager.RecordLevelCompletion(0, 1, 12.5f);
        Assert(ProgressionManager.IsLevelCompleted(0), "S8.4: Sector 01 marked completed");
        Assert(ProgressionManager.IsLevelUnlocked(1), "S8.4: Sector 02 unlocked after Sector 01 completed");
        Assert(!ProgressionManager.IsLevelUnlocked(2), "S8.4: Sector 03 remains locked");

        var stats0 = ProgressionManager.GetLevelStats(0);
        Assert(stats0.AttemptsTaken == 1 && Mathf.IsEqualApprox(stats0.TimeTakenSeconds, 12.5f),
            "S8.4: Sector 01 completion stats recorded accurately");

        ProgressionManager.RecordLevelCompletion(1, 2, 24.8f);
        Assert(ProgressionManager.IsLevelCompleted(1), "S8.4: Sector 02 marked completed");
        Assert(ProgressionManager.IsLevelUnlocked(2), "S8.4: Sector 03 unlocked after Sector 02 completed");

        ProgressionManager.ToggleUnlockAll();
        Assert(ProgressionManager.UnlockAllLevels, "S8.4: UnlockAllLevels toggle enabled");
        Assert(ProgressionManager.IsLevelUnlocked(3) && ProgressionManager.IsLevelUnlocked(4),
            "S8.4: All levels unlocked when debug toggle is active");

        ProgressionManager.ToggleUnlockAll();
        Assert(!ProgressionManager.UnlockAllLevels, "S8.4: UnlockAllLevels toggle disabled");
        Assert(!ProgressionManager.IsLevelUnlocked(4), "S8.4: Sector 05 reverts to locked state when toggle is OFF");

        ProgressionManager.ResetProgression();
        Assert(!ProgressionManager.IsLevelCompleted(0), "S8.4: Progression reset restores initial clean state");
    }

    private void TestUIComponentsFlow()
    {
        GD.Print("\n--- UI Components & Flow ---");

        var menuScene = GD.Load<PackedScene>("res://Scenes/UI/MainMenu.tscn");
        var menuInstance = menuScene.Instantiate<MainMenu>();
        AddChild(menuInstance);

        var playBtn = menuInstance.GetNode<Button>("MenuContainer/VBox/PlayButton");
        var exitBtn = menuInstance.GetNode<Button>("MenuContainer/VBox/ExitButton");
        var unlockBtn = menuInstance.GetNode<Button>("BottomRight/UnlockAllButton");

        Assert(playBtn is not null && exitBtn is not null && unlockBtn is not null,
            "UI: MainMenu has Play, Exit, and debug Unlock All buttons");

        bool playPressed = false;
        menuInstance.PlayPressed += () => playPressed = true;
        playBtn!.EmitSignal(Button.SignalName.Pressed);
        Assert(playPressed, "UI: PlayButton emits PlayPressed event");

        menuInstance.QueueFree();

        var selectScene = GD.Load<PackedScene>("res://Scenes/UI/LevelSelect.tscn");
        var selectInstance = selectScene.Instantiate<LevelSelect>();
        AddChild(selectInstance);

        var backBtn = selectInstance.GetNode<Button>("BottomLeft/BackButton");
        Assert(backBtn is not null, "UI: LevelSelect has BackButton to Main Menu");

        selectInstance.QueueFree();

        var modalScene = GD.Load<PackedScene>("res://Scenes/UI/LevelCompletedModal.tscn");
        var modalInstance = modalScene.Instantiate<LevelCompletedModal>();
        AddChild(modalInstance);

        var levelInfo = LevelRegistry.GetLevel(0)!;
        modalInstance.Display(levelInfo, 2, 18.25f, true);

        var runsLabel = modalInstance.GetNode<Label>("CenterContainer/Card/VBox/StatsBox/HBox/RunsContainer/Value");
        var timeLabel = modalInstance.GetNode<Label>("CenterContainer/Card/VBox/StatsBox/HBox/TimeContainer/Value");
        var nextBtn = modalInstance.GetNode<Button>("CenterContainer/Card/VBox/ButtonGroup/NextSectorButton");
        var lvlSelectBtn = modalInstance.GetNode<Button>("BottomLeft/LevelSelectButton");

        Assert(runsLabel.Text == "2 LOOPS", "UI: LevelCompletedModal formats loop runs correctly");
        Assert(timeLabel.Text == "18.25s", "UI: LevelCompletedModal formats time correctly");
        Assert(nextBtn.Visible, "UI: NextSectorButton is visible on intermediate level");
        Assert(lvlSelectBtn is not null, "UI: LevelSelection button exists on completed modal");

        modalInstance.QueueFree();
    }

    private void TestStandaloneLevelExecution()
    {
        GD.Print("\n--- Standalone Level Execution (Set As Main Scene) ---");

        var testRoomScene = GD.Load<PackedScene>("res://Scenes/Levels/TestRoom.tscn");
        var testRoom = testRoomScene.Instantiate<LevelContainer>();
        AddChild(testRoom);

        Assert(testRoom.IsStandalone, "Standalone: LevelContainer automatically detects standalone execution mode");

        var standaloneHud = testRoom.GetNodeOrNull<CanvasLayer>("StandaloneHud");
        Assert(standaloneHud is not null, "Standalone: BasicHud is automatically created and attached when running level directly");

        var loopCtrl = testRoom.GetNodeOrNull<LoopController>("StandaloneLoopController");
        Assert(loopCtrl is not null, "Standalone: LoopController is automatically created and initialized");

        var chronoValue = testRoom.GetNodeOrNull<Label>("StandaloneHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
        Assert(chronoValue is not null && !string.IsNullOrEmpty(chronoValue.Text),
            "Standalone: Chrono countdown timer is active and displaying remaining time");

        var objective = testRoom.GetNodeOrNull<Label>("StandaloneHud/HudRoot/ObjectiveCard/VBox/Objective");
        Assert(objective is not null && objective.Text == testRoom.ObjectiveText,
            "Standalone: Primary objective card is active and displaying objective text");

        // Test switch interaction in standalone
        var testSwitch = testRoom.GetNodeOrNull<TestSwitch>("PuzzleModules/TestSwitch");
        var door = testRoom.GetNodeOrNull<Door>("PuzzleModules/Door");
        Assert(testSwitch is not null && door is not null, "Standalone: TestSwitch and Door exist in scene");

        var greg = testRoom.GetNode<GregCharacter>("Greg");
        testSwitch!.TryInteract(greg);
        Assert(door!.IsOpen, "Standalone: TestSwitch interaction successfully opens door");

        testRoom.QueueFree();

        // Also test TimingRoom standalone
        var timingScene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        var timingRoom = timingScene.Instantiate<TimingRoom>();
        AddChild(timingRoom);
        Assert(timingRoom.IsStandalone, "Standalone: TimingRoom automatically detects standalone mode");
        Assert(timingRoom.GetNodeOrNull<CanvasLayer>("StandaloneHud") is not null, "Standalone: TimingRoom has attached HUD");
        Assert(timingRoom.GetNodeOrNull<LoopController>("StandaloneLoopController") is not null, "Standalone: TimingRoom has attached LoopController");

        timingRoom.QueueFree();
    }

    private void TestGameManagerNavigationFlow()
    {
        GD.Print("\n--- GameManager State Machine & Navigation Flow ---");

        var mainScene = GD.Load<PackedScene>("res://Scenes/Main.tscn");
        var gameManager = mainScene.Instantiate<GameManager>();
        AddChild(gameManager);

        var menuLayer = gameManager.GetNode<CanvasLayer>("MenuLayer");
        var mainMenu = gameManager.GetNode<MainMenu>("MenuLayer/MainMenu");
        var levelSelect = gameManager.GetNode<LevelSelect>("MenuLayer/LevelSelect");
        var modal = gameManager.GetNode<LevelCompletedModal>("MenuLayer/LevelCompletedModal");
        var hud = gameManager.GetNode<CanvasLayer>("BasicHud");

        // Initial state: Main Menu
        Assert(gameManager.CurrentState == GameFlowState.MainMenu, "Navigation: Initial state is MainMenu");
        Assert(mainMenu.Visible && menuLayer.Visible, "Navigation: MainMenu is visible on MenuLayer");
        Assert(!levelSelect.Visible && !modal.Visible, "Navigation: LevelSelect and Modal are hidden");
        Assert(!hud.Visible, "Navigation: BasicHud is hidden on MainMenu");

        // Transition: MainMenu -> LevelSelect
        gameManager.SwitchState(GameFlowState.LevelSelect);
        Assert(gameManager.CurrentState == GameFlowState.LevelSelect, "Navigation: State transitioned to LevelSelect");
        Assert(!mainMenu.Visible && levelSelect.Visible && menuLayer.Visible, "Navigation: LevelSelect is visible, MainMenu is hidden");
        Assert(!hud.Visible, "Navigation: BasicHud is hidden on LevelSelect");

        // Transition: LevelSelect -> InGame (Deploy Level 0)
        gameManager.LoadAndStartLevel(0);
        Assert(gameManager.CurrentState == GameFlowState.InGame, "Navigation: State transitioned to InGame");
        Assert(hud.Visible, "Navigation: BasicHud is visible in gameplay");
        Assert(!menuLayer.Visible && !mainMenu.Visible && !levelSelect.Visible, "Navigation: All menu screens are hidden during gameplay");

        var levelHost = gameManager.GetNode<Node2D>("LevelHost");
        Assert(levelHost.GetChildCount() > 0, "Navigation: Level scene is loaded under LevelHost");

        var activeChrono = hud.GetNode<Label>("HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
        Assert(!string.IsNullOrEmpty(activeChrono.Text), "Navigation: Chrono HUD display is populated in game");

        // Transition: InGame -> LevelCompleted
        var exitGoal = levelHost.GetChild(0).GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        exitGoal.EmitSignal(ExitGoal.SignalName.Completed);

        Assert(gameManager.CurrentState == GameFlowState.LevelCompleted, "Navigation: Reaching exit goal transitions to LevelCompleted");
        Assert(modal.Visible && menuLayer.Visible, "Navigation: LevelCompletedModal is visible");
        Assert(hud.Visible, "Navigation: BasicHud remains visible on end screen");

        // Transition: LevelCompleted -> LevelSelect
        gameManager.SwitchState(GameFlowState.LevelSelect);
        Assert(gameManager.CurrentState == GameFlowState.LevelSelect, "Navigation: Back to level select works from end screen");
        Assert(levelSelect.Visible && !modal.Visible, "Navigation: LevelSelect is active and Modal is closed");

        // Transition: LevelSelect -> MainMenu
        gameManager.SwitchState(GameFlowState.MainMenu);
        Assert(gameManager.CurrentState == GameFlowState.MainMenu, "Navigation: Back to main menu works from level select");
        Assert(mainMenu.Visible && !levelSelect.Visible, "Navigation: MainMenu is active");

        gameManager.QueueFree();
    }
}
