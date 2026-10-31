#nullable enable
using Godot;

namespace ECHOISM;

public partial class RelaySyncRoom : LevelContainer
{
    public EchoSensor Sensor { get; private set; } = null!;
    public SignalRelay Relay { get; private set; } = null!;
    public Door Door { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public MovingObstacle Obstacle { get; private set; } = null!;
    public NarrativeTerminal PublicNotice { get; private set; } = null!;
    public NarrativeTerminal OriginalSourceTerminal { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-04-relay-sync";
        DisplayName = "Sector 04 - The Echo Chamber";
        ObjectiveText = "Coordinate an Echo on the resonance sensor to route a delayed pulse through the relay and bypass the hazard.";
        CompletionCondition = "Signal linked and sensor satisfied; Exit Terminal reached.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 18f;
        MaximumActiveEchoes = 3;

        base._Ready();

        Sensor = GetNode<EchoSensor>("PuzzleModules/EchoSensor");
        Relay = GetNode<SignalRelay>("PuzzleModules/SignalRelay");
        Door = GetNode<Door>("PuzzleModules/Door");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        Obstacle = GetNode<MovingObstacle>("PuzzleModules/MovingObstacle");
        PublicNotice = GetNode<NarrativeTerminal>("PuzzleModules/PublicNoticeTerminal");
        OriginalSourceTerminal = GetNode<NarrativeTerminal>("PuzzleModules/OriginalSourceTerminal");

        Sensor.ActiveChanged += active =>
        {
            if (active)
            {
                Relay.ReceiveSignal();
            }
            CheckCompletion();
        };

        Relay.SignalTransmitted += () =>
        {
            Door.SetOpen(true);
            var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
            camera?.Shake(3.0f, 0.2f);
            CheckCompletion();
        };
    }

    public void CheckCompletion()
    {
        if (Sensor.IsCompleted && Relay.IsCompleted)
        {
            Door.SetOpen(true);
        }
    }
}
