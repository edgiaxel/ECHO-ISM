#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector05Heartline : LevelContainer
{
    public ChargeStation PowerCapacitor { get; private set; } = null!;
    public RotaryJunction PowerJunction { get; private set; } = null!;
    public VerificationTerminal ArchiveVerifier { get; private set; } = null!;
    public PressurePlate CorePlate { get; private set; } = null!;
    public Door CoreGate { get; private set; } = null!;
    public NarrativeTerminal ReconnectConsole { get; private set; } = null!;
    public ExitGoal FinalExit { get; private set; } = null!;

    public bool IsPowerSynchronized => PowerCapacitor.IsFullyCharged && PowerJunction.IsAligned;
    public bool IsArchiveSynchronized => ArchiveVerifier.IsCompleted;
    public bool IsPlateStabilized => CorePlate.IsCompleted;

    public bool AreAllChannelsSynchronized => IsPowerSynchronized && IsArchiveSynchronized && IsPlateStabilized;

    public override void _Ready()
    {
        base._Ready();

        if (GetNodeOrNull<RoomGeometry>("RoomGeometry") is { } geom && geom.PartitionWalls.Count == 0)
        {
            geom.PartitionWalls = new()
            {
                new Rect2(432, 0, 16, 166),
                new Rect2(432, 254, 16, 186)
            };
            geom.SetRoomLayout(geom.RoomBounds, geom.StartingPosition, partitions: geom.PartitionWalls);
        }

        PowerCapacitor = GetNode<ChargeStation>("PuzzleModules/PowerCapacitor");
        PowerJunction = GetNode<RotaryJunction>("PuzzleModules/PowerJunction");
        ArchiveVerifier = GetNode<VerificationTerminal>("PuzzleModules/ArchiveVerifier");
        CorePlate = GetNode<PressurePlate>("PuzzleModules/CorePlate");
        CoreGate = GetNode<Door>("PuzzleModules/CoreGate");
        ReconnectConsole = GetNode<NarrativeTerminal>("PuzzleModules/ReconnectConsole");
        FinalExit = GetNode<ExitGoal>("PuzzleModules/ExitGoal");

        // Power capacitor supplies energy to junction
        PowerCapacitor.BooleanStateChanged += charged =>
        {
            PowerJunction.SetPowerInput(charged);
            EvaluateHeartlineStatus();
        };

        PowerJunction.AlignmentChanged += _ => EvaluateHeartlineStatus();
        ArchiveVerifier.StateChanged += _ => EvaluateHeartlineStatus();
        CorePlate.ActiveChanged += _ => EvaluateHeartlineStatus();
    }

    private void EvaluateHeartlineStatus()
    {
        bool synchronized = AreAllChannelsSynchronized;
        CoreGate.SetOpen(synchronized);

        if (synchronized)
        {
            ObjectiveText = "All 3 channels synchronized! Enter the Heartline Core to restore NEXUS!";
            SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
        }
    }
}
