#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public enum GameFlowState
{
    MainMenu,
    LevelSelect,
    InGame,
    Paused,
    LevelCompleted
}

public partial class GameManager : Node2D
{
    [Export]
    public PackedScene? InitialLevel { get; set; }

    [Export]
    public bool AutoStartSector01ForTests { get; set; }

    public GameFlowState CurrentState { get; private set; } = GameFlowState.MainMenu;
    public int CurrentLevelIndex { get; private set; } = 0;
    public float LevelElapsedTime { get; private set; }

    private Node2D _levelHost = null!;
    private LoopController? _loopController;
    private LevelContainer? _level;

    // HUD controls
    private CanvasLayer _basicHud = null!;
    private Label _chronoValue = null!;
    private ProgressBar _chronoBar = null!;
    private Label _loopLabel = null!;
    private Label _echoPoolLabel = null!;
    private Label _objective = null!;
    private Label _roomBadge = null!;
    private Label _sectorLabel = null!;
    private Control _promptPanel = null!;
    private Label _interactionPrompt = null!;
    private Control _devInspector = null!;
    private Label _trackSummary = null!;
    private Control _terminalOverlay = null!;
    private Label _terminalCategory = null!;
    private Label _terminalTitle = null!;
    private Label _terminalMessage = null!;

    // UI Menu Layer and Screens
    private CanvasLayer _menuLayer = null!;
    private MainMenu _mainMenu = null!;
    private LevelSelect _levelSelect = null!;
    private LevelCompletedModal _levelCompletedModal = null!;
    private PauseModal _pauseModal = null!;

    private RoomCamera? _camera;
    private Door? _door;
    private ExitGoal? _exitGoal;

    public override void _Ready()
    {
        _levelHost = GetNode<Node2D>("LevelHost");
        _basicHud = GetNode<CanvasLayer>("BasicHud");
        _basicHud.Layer = 5;

        _chronoValue = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
        _chronoBar = GetNode<ProgressBar>("BasicHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoBar");
        _loopLabel = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/LoopInfo/LoopLabel");
        _echoPoolLabel = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/LoopInfo/EchoPoolLabel");
        _objective = GetNode<Label>("BasicHud/HudRoot/ObjectiveCard/VBox/Objective");
        _roomBadge = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/RoomInfo/RoomBadge");
        _sectorLabel = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/RoomInfo/SectorLabel");
        _promptPanel = GetNode<Control>("BasicHud/HudRoot/PromptPanel");
        _interactionPrompt = GetNode<Label>("BasicHud/HudRoot/PromptPanel/HBox/InteractionPrompt");
        _devInspector = GetNode<Control>("BasicHud/HudRoot/DevInspector");
        _trackSummary = GetNode<Label>("BasicHud/HudRoot/DevInspector/VBox/TrackSummary");
        _terminalOverlay = GetNode<Control>("BasicHud/HudRoot/TerminalOverlay");
        _terminalCategory = GetNode<Label>("BasicHud/HudRoot/TerminalOverlay/VBox/CategoryLabel");
        _terminalTitle = GetNode<Label>("BasicHud/HudRoot/TerminalOverlay/VBox/TitleLabel");
        _terminalMessage = GetNode<Label>("BasicHud/HudRoot/TerminalOverlay/VBox/MessageLabel");

        NarrativeTerminal.TerminalRead += OnNarrativeTerminalRead;

        _menuLayer = GetNode<CanvasLayer>("MenuLayer");
        _menuLayer.Layer = 10;

        _mainMenu = GetNode<MainMenu>("MenuLayer/MainMenu");
        _levelSelect = GetNode<LevelSelect>("MenuLayer/LevelSelect");
        _levelCompletedModal = GetNode<LevelCompletedModal>("MenuLayer/LevelCompletedModal");
        _pauseModal = GetNode<PauseModal>("MenuLayer/PauseModal");

        // Event bindings
        _mainMenu.PlayPressed += () => SwitchState(GameFlowState.LevelSelect);
        _mainMenu.ExitPressed += () => GetTree().Quit();

        _levelSelect.LevelSelected += LoadAndStartLevel;
        _levelSelect.BackToMenuPressed += () => SwitchState(GameFlowState.MainMenu);

        _levelCompletedModal.NextSectorPressed += OnNextSector;
        _levelCompletedModal.LevelSelectPressed += () => SwitchState(GameFlowState.LevelSelect);
        _levelCompletedModal.ReplayPressed += () => SwitchState(GameFlowState.LevelSelect);

        _pauseModal.ResumeRequested += () => SwitchState(GameFlowState.InGame);
        _pauseModal.RetryRequested += () => LoadAndStartLevel(CurrentLevelIndex);
        _pauseModal.LevelSelectRequested += () => SwitchState(GameFlowState.LevelSelect);

        if (AutoStartSector01ForTests)
        {
            LoadAndStartLevel(0);
        }
        else
        {
            SwitchState(GameFlowState.MainMenu);
        }
    }

    public override void _Process(double delta)
    {
        if (CurrentState == GameFlowState.InGame && _loopController?.CurrentState == LoopState.Active)
        {
            LevelElapsedTime += (float)delta;
        }
    }

    public void SwitchState(GameFlowState newState)
    {
        CurrentState = newState;

        bool showMainMenu = (newState == GameFlowState.MainMenu);
        bool showLevelSelect = (newState == GameFlowState.LevelSelect);
        bool showCompleted = (newState == GameFlowState.LevelCompleted);
        bool showPaused = (newState == GameFlowState.Paused);
        bool inGame = (newState == GameFlowState.InGame);

        // Explicitly set Visibility and ProcessMode so non-active screens never capture input
        _mainMenu.Visible = showMainMenu;
        _mainMenu.MouseFilter = showMainMenu ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
        _mainMenu.ProcessMode = showMainMenu ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;

        _levelSelect.Visible = showLevelSelect;
        _levelSelect.MouseFilter = showLevelSelect ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
        _levelSelect.ProcessMode = showLevelSelect ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;

        _levelCompletedModal.Visible = showCompleted;
        _levelCompletedModal.MouseFilter = showCompleted ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;
        _levelCompletedModal.ProcessMode = showCompleted ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;

        if (showPaused)
        {
            _pauseModal.ShowModal();
            _levelHost.ProcessMode = ProcessModeEnum.Disabled;
            if (_loopController is not null)
            {
                _loopController.ProcessMode = ProcessModeEnum.Disabled;
            }
        }
        else
        {
            _pauseModal.HideModal();
            _levelHost.ProcessMode = ProcessModeEnum.Inherit;
            if (_loopController is not null)
            {
                _loopController.ProcessMode = ProcessModeEnum.Inherit;
            }
        }

        _menuLayer.Visible = (showMainMenu || showLevelSelect || showCompleted || showPaused);
        _basicHud.Visible = (inGame || showPaused || showCompleted);

        if (showMainMenu)
        {
            UnloadCurrentLevel();
            _mainMenu.UpdateUnlockAllButtonText();
        }
        else if (showLevelSelect)
        {
            UnloadCurrentLevel();
            _levelSelect.RefreshLevelList();
        }
    }

    public void LoadAndStartLevel(int levelIndex)
    {
        var info = LevelRegistry.GetLevel(levelIndex);
        if (info is null)
        {
            GD.PrintErr($"[GameManager] Level index {levelIndex} not found in registry.");
            return;
        }

        UnloadCurrentLevel();

        CurrentLevelIndex = levelIndex;
        LevelElapsedTime = 0f;

        var scene = GD.Load<PackedScene>(info.ScenePath);
        var levelNode = scene.Instantiate<Node2D>();
        _levelHost.AddChild(levelNode);

        if (levelNode is not LevelContainer definition)
        {
            GD.PrintErr($"[GameManager] Scene {info.ScenePath} is not a LevelContainer.");
            return;
        }

        _level = definition;

        // Configure HUD
        _roomBadge.Text = info.SectorCode;
        _sectorLabel.Text = info.DisplayName.ToUpperInvariant();
        _objective.Text = definition.ObjectiveText;

        _camera = _level.GetNodeOrNull<RoomCamera>("RoomCamera");
        _door = _level.GetNodeOrNull<Door>("PuzzleModules/Door");
        _exitGoal = _level.GetNodeOrNull<ExitGoal>("PuzzleModules/ExitGoal");

        // Generic switch-door hookup for Sector 01
        var testSwitch = _level.GetNodeOrNull<TestSwitch>("PuzzleModules/TestSwitch");
        if (testSwitch is not null && _door is not null)
        {
            testSwitch.StateChanged += active =>
            {
                _door.SetOpen(active);
                _camera?.Shake(active ? 3.0f : 1.5f, 0.15f);
                _objective.Text = active ? "Door opened. Reach the exit terminal." : definition.ObjectiveText;
            };
        }

        // Completion hookup
        if (_exitGoal is not null)
        {
            _exitGoal.Completed += OnLevelGoalReached;
        }

        // Greg interaction prompt hookup
        var greg = _level.GetNode<GregCharacter>("Greg");
        greg.PromptChanged += text =>
        {
            _interactionPrompt.Text = text;
            _promptPanel.Visible = !string.IsNullOrEmpty(text);
        };

        // Initialize LoopController
        _loopController = new LoopController { Name = "LoopController" };
        AddChild(_loopController);

        _loopController.TimerTick += OnTimerTick;
        _loopController.AttemptCommitted += OnAttemptCommitted;
        _loopController.LoopReset += OnLoopReset;

        _loopController.Initialize(_level);

        _chronoBar.MaxValue = _loopController.LoopDuration;
        _chronoBar.Value = _loopController.LoopDuration;
        _chronoValue.Text = $"{_loopController.LoopDuration:0.00}s";
        UpdateLoopHeader();

        SwitchState(GameFlowState.InGame);
    }

    private void OnLevelGoalReached()
    {
        if (CurrentState != GameFlowState.InGame || _loopController is null)
        {
            return;
        }

        _loopController.CompleteLevel();
        _camera?.Shake(5.0f, 0.4f);

        int attempts = _loopController.CurrentAttempt;
        float timeTaken = LevelElapsedTime;

        ProgressionManager.RecordLevelCompletion(CurrentLevelIndex, attempts, timeTaken);

        var info = LevelRegistry.GetLevel(CurrentLevelIndex);
        if (info is not null)
        {
            bool hasNextSector = (CurrentLevelIndex + 1) < LevelRegistry.LevelCount;
            _levelCompletedModal.Display(info, attempts, timeTaken, hasNextSector);
        }

        SwitchState(GameFlowState.LevelCompleted);
    }

    private void OnNextSector()
    {
        int nextIndex = CurrentLevelIndex + 1;
        if (nextIndex < LevelRegistry.LevelCount)
        {
            LoadAndStartLevel(nextIndex);
        }
        else
        {
            SwitchState(GameFlowState.LevelSelect);
        }
    }

    private void UnloadCurrentLevel()
    {
        DismissTerminalOverlay();

        if (_pauseModal is not null)
        {
            _pauseModal.HideModal();
        }

        _levelHost.ProcessMode = ProcessModeEnum.Inherit;

        if (_loopController is not null)
        {
            _loopController.TimerTick -= OnTimerTick;
            _loopController.AttemptCommitted -= OnAttemptCommitted;
            _loopController.LoopReset -= OnLoopReset;
            _loopController.QueueFree();
            _loopController = null;
        }

        if (_exitGoal is not null)
        {
            _exitGoal.Completed -= OnLevelGoalReached;
            _exitGoal = null;
        }

        _level = null;
        _camera = null;
        _door = null;

        foreach (var child in _levelHost.GetChildren())
        {
            child.QueueFree();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_terminalOverlay is not null && _terminalOverlay.Visible && @event is InputEventKey { Pressed: true } termKey)
        {
            if (termKey.PhysicalKeycode == Key.E || termKey.PhysicalKeycode == Key.Escape || termKey.PhysicalKeycode == Key.Space)
            {
                DismissTerminalOverlay();
                GetViewport().SetInputAsHandled();
                return;
            }
        }

        if (@event is InputEventKey { Pressed: true, PhysicalKeycode: Key.F1 })
        {
            _devInspector.Visible = !_devInspector.Visible;
            if (_devInspector.Visible && _loopController is not null)
            {
                _trackSummary.Text = _loopController.GetRecordedTracksSummary();
            }
            GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventKey { Pressed: true, PhysicalKeycode: Key.Escape })
        {
            if (CurrentState == GameFlowState.InGame)
            {
                SwitchState(GameFlowState.Paused);
                GetViewport().SetInputAsHandled();
            }
            else if (CurrentState == GameFlowState.Paused)
            {
                SwitchState(GameFlowState.InGame);
                GetViewport().SetInputAsHandled();
            }
            else if (CurrentState == GameFlowState.LevelCompleted)
            {
                SwitchState(GameFlowState.LevelSelect);
                GetViewport().SetInputAsHandled();
            }
            else if (CurrentState == GameFlowState.LevelSelect)
            {
                SwitchState(GameFlowState.MainMenu);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _ExitTree()
    {
        NarrativeTerminal.TerminalRead -= OnNarrativeTerminalRead;
    }

    private void OnNarrativeTerminalRead(NarrativeTerminal terminal)
    {
        if (CurrentState != GameFlowState.InGame) return;
        _terminalCategory.Text = terminal.IsOptionalFragment 
            ? "[DATA FRAGMENT RECOVERED]" 
            : $"[{terminal.MessageHeader.ToUpperInvariant()}]";
        _terminalTitle.Text = terminal.IsOptionalFragment 
            ? terminal.FragmentTitle.ToUpperInvariant() 
            : terminal.TerminalTitle.ToUpperInvariant();
        _terminalMessage.Text = terminal.MessageBody;
        _terminalOverlay.Visible = true;
    }

    private void DismissTerminalOverlay()
    {
        if (_terminalOverlay is not null)
        {
            _terminalOverlay.Visible = false;
        }
    }

    private void OnTimerTick(float currentTime, float loopDuration)
    {
        var remaining = Mathf.Max(0f, loopDuration - currentTime);
        _chronoValue.Text = $"{remaining:0.00}s";
        _chronoBar.Value = remaining;

        if (_devInspector.Visible && _loopController is not null)
        {
            _trackSummary.Text = _loopController.GetRecordedTracksSummary();
        }
    }

    private void OnAttemptCommitted(RecordedAttempt attempt)
    {
        UpdateLoopHeader();
        if (_devInspector.Visible && _loopController is not null)
        {
            _trackSummary.Text = _loopController.GetRecordedTracksSummary();
        }
    }

    private void OnLoopReset()
    {
        if (_level is not null)
        {
            _objective.Text = _level.ObjectiveText;
        }
        _camera?.Shake(2.0f, 0.12f);
        UpdateLoopHeader();
    }

    private void UpdateLoopHeader()
    {
        if (_loopController is null)
        {
            return;
        }

        _loopLabel.Text = $"LOOP {_loopController.CurrentAttempt:00}/03";

        var roman = new[] { "I", "II", "III" };
        var slots = new List<string>();
        for (int i = 0; i < _loopController.MaxEchoes; i++)
        {
            slots.Add(i < _loopController.CommittedRecordings.Count ? roman[i % roman.Length] : "·");
        }
        _echoPoolLabel.Text = $"ECHOES [{string.Join("] [", slots)}]";
    }
}

