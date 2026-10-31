#nullable enable
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class LevelContainer : Node2D
{
    [Export]
    public string LevelId { get; set; } = "boot-sequence-test-room";

    [Export]
    public string DisplayName { get; set; } = "Boot Sequence Test Room";

    [Export]
    public Rect2 RoomBounds { get; set; } = new(240, 166, 800, 400);

    [Export]
    public Vector2 StartingPosition { get; set; } = new(360, 366);

    [Export(PropertyHint.Range, "1,120,1")]
    public float LoopDurationSeconds { get; set; } = 15;

    [Export(PropertyHint.Range, "0,3,1")]
    public int MaximumActiveEchoes { get; set; } = 3;

    [Export(PropertyHint.MultilineText)]
    public string ObjectiveText { get; set; } = "Coordinate with Echo to open Door M02 and reach Exit Node.";

    [Export(PropertyHint.MultilineText)]
    public string CompletionCondition { get; set; } = "Reach Exit Node while Door M02 is unlocked.";

    public bool IsStandalone { get; private set; }

    private LoopController? _standaloneLoopController;
    private CanvasLayer? _standaloneHud;
    private Label? _standaloneObjective;
    private Label? _standaloneChronoValue;
    private ProgressBar? _standaloneChronoBar;
    private Label? _standaloneLoopLabel;
    private Label? _standaloneEchoPoolLabel;
    private Control? _standaloneDevInspector;
    private Label? _standaloneTrackSummary;
    private RoomCamera? _camera;

    public override void _Ready()
    {
        GetNode<RoomGeometry>("RoomGeometry").SetRoomLayout(RoomBounds, StartingPosition);
        GetNode<GregCharacter>("Greg").Position = StartingPosition;
        _camera = GetNodeOrNull<RoomCamera>("RoomCamera");

        // Detect if level is run standalone (e.g. "Run Current Scene" / set as main scene in Godot editor)
        bool isManagedByGameManager = GetParent() is Node2D parent && parent.Name == "LevelHost";
        if (!isManagedByGameManager)
        {
            SetupStandaloneEnvironment();
        }
    }

    private void SetupStandaloneEnvironment()
    {
        IsStandalone = true;

        // 1. Instantiate and attach BasicHud
        var hudScene = GD.Load<PackedScene>("res://Scenes/UI/BasicHud.tscn");
        _standaloneHud = hudScene.Instantiate<CanvasLayer>();
        _standaloneHud.Name = "StandaloneHud";
        AddChild(_standaloneHud);

        _standaloneChronoValue = _standaloneHud.GetNode<Label>("HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
        _standaloneChronoBar = _standaloneHud.GetNode<ProgressBar>("HudRoot/TopBar/HBox/ChronoContainer/ChronoBar");
        _standaloneLoopLabel = _standaloneHud.GetNode<Label>("HudRoot/TopBar/HBox/LoopInfo/LoopLabel");
        _standaloneEchoPoolLabel = _standaloneHud.GetNode<Label>("HudRoot/TopBar/HBox/LoopInfo/EchoPoolLabel");
        _standaloneObjective = _standaloneHud.GetNode<Label>("HudRoot/ObjectiveCard/VBox/Objective");
        _standaloneDevInspector = _standaloneHud.GetNode<Control>("HudRoot/DevInspector");
        _standaloneTrackSummary = _standaloneHud.GetNode<Label>("HudRoot/DevInspector/VBox/TrackSummary");

        var roomBadge = _standaloneHud.GetNode<Label>("HudRoot/TopBar/HBox/RoomInfo/RoomBadge");
        var sectorLabel = _standaloneHud.GetNode<Label>("HudRoot/TopBar/HBox/RoomInfo/SectorLabel");
        var promptPanel = _standaloneHud.GetNode<Control>("HudRoot/PromptPanel");
        var promptLabel = _standaloneHud.GetNode<Label>("HudRoot/PromptPanel/HBox/InteractionPrompt");

        roomBadge.Text = DisplayName.ToUpperInvariant();
        sectorLabel.Text = LevelId.ToUpperInvariant();
        _standaloneObjective.Text = ObjectiveText;

        // 2. Greg interaction prompt
        var greg = GetNode<GregCharacter>("Greg");
        greg.PromptChanged += text =>
        {
            promptLabel.Text = text;
            promptPanel.Visible = !string.IsNullOrEmpty(text);
        };

        // 3. Connect generic switch and door if present in TestRoom
        var testSwitch = GetNodeOrNull<TestSwitch>("PuzzleModules/TestSwitch");
        var door = GetNodeOrNull<Door>("PuzzleModules/Door");
        if (testSwitch is not null && door is not null)
        {
            testSwitch.StateChanged += active =>
            {
                door.SetOpen(active);
                _camera?.Shake(active ? 3.0f : 1.5f, 0.15f);
                _standaloneObjective.Text = active ? "Door opened. Reach the exit terminal." : ObjectiveText;
            };
        }

        // 4. Connect ExitGoal
        var exitGoal = GetNodeOrNull<ExitGoal>("PuzzleModules/ExitGoal");
        if (exitGoal is not null)
        {
            exitGoal.Completed += () =>
            {
                _standaloneLoopController?.CompleteLevel();
                _camera?.Shake(5.0f, 0.4f);
                if (_standaloneObjective is not null)
                {
                    _standaloneObjective.Text = "SECTOR RESTORED: Exit terminal reached!";
                }
            };
        }

        // 5. Connect NarrativeTerminal overlay
        var terminalOverlay = _standaloneHud.GetNodeOrNull<Control>("HudRoot/TerminalOverlay");
        if (terminalOverlay is not null)
        {
            var cat = terminalOverlay.GetNode<Label>("VBox/CategoryLabel");
            var tit = terminalOverlay.GetNode<Label>("VBox/TitleLabel");
            var msg = terminalOverlay.GetNode<Label>("VBox/MessageLabel");

            NarrativeTerminal.TerminalRead += term =>
            {
                if (!IsStandalone) return;
                cat.Text = term.IsOptionalFragment ? "[DATA FRAGMENT RECOVERED]" : $"[{term.MessageHeader.ToUpperInvariant()}]";
                tit.Text = term.IsOptionalFragment ? term.FragmentTitle.ToUpperInvariant() : term.TerminalTitle.ToUpperInvariant();
                msg.Text = term.MessageBody;
                terminalOverlay.Visible = true;
            };
        }

        // 6. Initialize Standalone LoopController
        _standaloneLoopController = new LoopController { Name = "StandaloneLoopController" };
        AddChild(_standaloneLoopController);

        _standaloneLoopController.TimerTick += OnStandaloneTimerTick;
        _standaloneLoopController.AttemptCommitted += OnStandaloneAttemptCommitted;
        _standaloneLoopController.LoopReset += OnStandaloneLoopReset;

        _standaloneLoopController.Initialize(this);

        _standaloneChronoBar.MaxValue = _standaloneLoopController.LoopDuration;
        _standaloneChronoBar.Value = _standaloneLoopController.LoopDuration;
        UpdateStandaloneLoopHeader();
    }

    private void OnStandaloneTimerTick(float currentTime, float loopDuration)
    {
        if (_standaloneChronoValue is null || _standaloneChronoBar is null)
        {
            return;
        }

        var remaining = Mathf.Max(0f, loopDuration - currentTime);
        _standaloneChronoValue.Text = $"{remaining:0.00}s";
        _standaloneChronoBar.Value = remaining;

        if (_standaloneDevInspector is { Visible: true } && _standaloneTrackSummary is not null && _standaloneLoopController is not null)
        {
            _standaloneTrackSummary.Text = _standaloneLoopController.GetRecordedTracksSummary();
        }
    }

    private void OnStandaloneAttemptCommitted(RecordedAttempt attempt)
    {
        UpdateStandaloneLoopHeader();
        if (_standaloneDevInspector is { Visible: true } && _standaloneTrackSummary is not null && _standaloneLoopController is not null)
        {
            _standaloneTrackSummary.Text = _standaloneLoopController.GetRecordedTracksSummary();
        }
    }

    private void OnStandaloneLoopReset()
    {
        if (_standaloneObjective is not null)
        {
            _standaloneObjective.Text = ObjectiveText;
        }
        _camera?.Shake(2.0f, 0.12f);
        UpdateStandaloneLoopHeader();
    }

    private void UpdateStandaloneLoopHeader()
    {
        if (_standaloneLoopController is null || _standaloneLoopLabel is null || _standaloneEchoPoolLabel is null)
        {
            return;
        }

        _standaloneLoopLabel.Text = $"LOOP {_standaloneLoopController.CurrentAttempt:00}/03";

        var roman = new[] { "I", "II", "III" };
        var slots = new List<string>();
        for (int i = 0; i < _standaloneLoopController.MaxEchoes; i++)
        {
            slots.Add(i < _standaloneLoopController.CommittedRecordings.Count ? roman[i % roman.Length] : "·");
        }
        _standaloneEchoPoolLabel.Text = $"ECHOES [{string.Join("] [", slots)}]";
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!IsStandalone)
        {
            return;
        }

        var terminalOverlay = _standaloneHud?.GetNodeOrNull<Control>("HudRoot/TerminalOverlay");
        if (terminalOverlay is not null && terminalOverlay.Visible && @event is InputEventKey { Pressed: true } termKey)
        {
            if (termKey.PhysicalKeycode == Key.E || termKey.PhysicalKeycode == Key.Escape || termKey.PhysicalKeycode == Key.Space)
            {
                terminalOverlay.Visible = false;
                GetViewport().SetInputAsHandled();
                return;
            }
        }

        if (@event is InputEventKey { Pressed: true, PhysicalKeycode: Key.F1 } && _standaloneDevInspector is not null)
        {
            _standaloneDevInspector.Visible = !_standaloneDevInspector.Visible;
            if (_standaloneDevInspector.Visible && _standaloneTrackSummary is not null && _standaloneLoopController is not null)
            {
                _standaloneTrackSummary.Text = _standaloneLoopController.GetRecordedTracksSummary();
            }
            GetViewport().SetInputAsHandled();
        }
    }
}
