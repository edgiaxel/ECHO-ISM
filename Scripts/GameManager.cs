using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class GameManager : Node2D
{
    [Export]
    public PackedScene InitialLevel { get; set; } = null!;

    private LoopController _loopController = null!;
    private LevelContainer _level = null!;

    private Label _chronoValue = null!;
    private ProgressBar _chronoBar = null!;
    private Label _loopLabel = null!;
    private Label _echoPoolLabel = null!;
    private Label _objective = null!;
    private Control _devInspector = null!;
    private Label _trackSummary = null!;

    public override void _Ready()
    {
        var levelNode = InitialLevel.Instantiate<Node2D>();
        GetNode<Node2D>("LevelHost").AddChild(levelNode);

        if (levelNode is not LevelContainer definition)
        {
            return;
        }

        _level = definition;

        _chronoValue = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
        _chronoBar = GetNode<ProgressBar>("BasicHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoBar");
        _loopLabel = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/LoopInfo/LoopLabel");
        _echoPoolLabel = GetNode<Label>("BasicHud/HudRoot/TopBar/HBox/LoopInfo/EchoPoolLabel");
        _objective = GetNode<Label>("BasicHud/HudRoot/ObjectiveCard/VBox/Objective");
        _devInspector = GetNode<Control>("BasicHud/HudRoot/DevInspector");
        _trackSummary = GetNode<Label>("BasicHud/HudRoot/DevInspector/VBox/TrackSummary");

        var roomBadge = GetNodeOrNull<Label>("BasicHud/HudRoot/TopBar/HBox/RoomInfo/RoomBadge");
        if (roomBadge is not null && !string.IsNullOrEmpty(definition.DisplayName))
        {
            roomBadge.Text = definition.DisplayName.ToUpperInvariant();
        }

        _objective.Text = definition.ObjectiveText;

        var testSwitch = _level.GetNode<TestSwitch>("PuzzleModules/TestSwitch");
        testSwitch.StateChanged += active =>
        {
            _objective.Text = active ? "Switch M01 active. Circuit restored." : definition.ObjectiveText;
        };

        var greg = _level.GetNode<GregCharacter>("Greg");
        var promptPanel = GetNode<Control>("BasicHud/HudRoot/PromptPanel");
        var prompt = GetNode<Label>("BasicHud/HudRoot/PromptPanel/HBox/InteractionPrompt");
        greg.PromptChanged += text =>
        {
            prompt.Text = text;
            promptPanel.Visible = !string.IsNullOrEmpty(text);
        };

        _loopController = new LoopController { Name = "LoopController" };
        AddChild(_loopController);

        _loopController.TimerTick += OnTimerTick;
        _loopController.AttemptCommitted += OnAttemptCommitted;
        _loopController.LoopReset += OnLoopReset;

        _loopController.Initialize(_level);

        _chronoBar.MaxValue = _loopController.LoopDuration;
        _chronoBar.Value = _loopController.LoopDuration;
        UpdateLoopHeader();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey { Pressed: true, PhysicalKeycode: Key.F1 })
        {
            _devInspector.Visible = !_devInspector.Visible;
            if (_devInspector.Visible && _loopController is not null)
            {
                _trackSummary.Text = _loopController.GetRecordedTracksSummary();
            }
            GetViewport().SetInputAsHandled();
        }
    }

    private void OnTimerTick(float currentTime, float loopDuration)
    {
        var remaining = Mathf.Max(0f, loopDuration - currentTime);
        _chronoValue.Text = $"{remaining:0.00}s";
        _chronoBar.Value = remaining;

        if (_devInspector.Visible)
        {
            _trackSummary.Text = _loopController.GetRecordedTracksSummary();
        }
    }

    private void OnAttemptCommitted(RecordedAttempt attempt)
    {
        UpdateLoopHeader();
        if (_devInspector.Visible)
        {
            _trackSummary.Text = _loopController.GetRecordedTracksSummary();
        }
    }

    private void OnLoopReset()
    {
        _objective.Text = _level.ObjectiveText;
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
