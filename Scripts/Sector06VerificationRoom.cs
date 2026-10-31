#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector06VerificationRoom : LevelContainer
{
    public VerificationTerminal Terminal { get; private set; } = null!;
    public ConnectionRepair Repair { get; private set; } = null!;
    public PressurePlate Plate { get; private set; } = null!;
    public Door Door { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public NarrativeTerminal PublicNotice { get; private set; } = null!;
    public NarrativeTerminal MissingWitnessTerminal { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-06-truth-protocol";
        DisplayName = "Sector 06 - The Truth Protocol";
        ObjectiveText = "Coordinate with Echo: authenticate telemetry at verification terminal, bridge severed grid, and hold evidence plate to unlock the authenticated bulkhead.";
        CompletionCondition = "Verified data packet authorized, bridge circuit restored, and exit node reached.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 16.0f;
        MaximumActiveEchoes = 3;

        base._Ready();

        Terminal = GetNode<VerificationTerminal>("PuzzleModules/VerificationTerminal");
        Repair = GetNode<ConnectionRepair>("PuzzleModules/ConnectionRepair");
        Plate = GetNode<PressurePlate>("PuzzleModules/PressurePlate");
        Door = GetNode<Door>("PuzzleModules/Door");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        PublicNotice = GetNode<NarrativeTerminal>("PuzzleModules/PublicNoticeTerminal");
        MissingWitnessTerminal = GetNode<NarrativeTerminal>("PuzzleModules/MissingWitnessTerminal");

        Terminal.Verified += payload =>
        {
            SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
            CheckSectorRestoration();
        };

        Repair.ConnectionRestored += () =>
        {
            SoundManager.Instance?.Play(SoundEffect.Interact);
            CheckSectorRestoration();
        };

        Plate.ActiveChanged += active =>
        {
            CheckSectorRestoration();
        };
    }

    public void CheckSectorRestoration()
    {
        bool restored = Terminal.IsVerified && Repair.IsRepaired && Plate.IsCompleted;
        if (Door.IsOpen != restored)
        {
            Door.SetOpen(restored);
            if (restored)
            {
                var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
                camera?.Shake(3.0f, 0.2f);
                SoundManager.Instance?.Play(SoundEffect.DoorOpen);
            }
        }
    }
}
