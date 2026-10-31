#nullable enable
using Godot;

namespace ECHOISM;

public partial class TimingRoom : LevelContainer
{
    public TimingSlider Slider { get; private set; } = null!;
    public PressurePlate Plate { get; private set; } = null!;
    public SynchronizationGate SyncGate { get; private set; } = null!;
    public TimedDoor Door { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public NarrativeTerminal ChannelATerminal { get; private set; } = null!;
    public NarrativeTerminal ChannelBTerminal { get; private set; } = null!;
    public NarrativeTerminal SignalAnalystTerminal { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-02-timing-room";
        DisplayName = "Sector 02 - False Synchrony";
        ObjectiveText = "Synchronize Channel A and Channel B within the tolerance window to unlock the timed security aperture.";
        CompletionCondition = "Synchronize Gate M05 to unlock Timed Door and reach Exit Terminal.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 15f;
        MaximumActiveEchoes = 3;

        base._Ready();

        Slider = GetNode<TimingSlider>("PuzzleModules/TimingSlider");
        Plate = GetNode<PressurePlate>("PuzzleModules/PressurePlate");
        SyncGate = GetNode<SynchronizationGate>("PuzzleModules/SynchronizationGate");
        Door = GetNode<TimedDoor>("PuzzleModules/TimedDoor");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        ChannelATerminal = GetNode<NarrativeTerminal>("PuzzleModules/ChannelATerminal");
        ChannelBTerminal = GetNode<NarrativeTerminal>("PuzzleModules/ChannelBTerminal");
        SignalAnalystTerminal = GetNode<NarrativeTerminal>("PuzzleModules/SignalAnalystTerminal");

        Slider.Evaluated += eval =>
        {
            if (eval == TimingEvaluation.Accurate)
            {
                SyncGate.ReceiveSignalA();
            }
        };

        Plate.ActiveChanged += active =>
        {
            if (active)
            {
                SyncGate.ReceiveSignalB();
            }
        };

        SyncGate.GateSynchronized += () =>
        {
            Door.TriggerOpen();
            var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
            camera?.Shake(3.0f, 0.2f);
        };
    }
}
