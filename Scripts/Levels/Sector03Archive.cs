#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector03Archive : LevelContainer
{
    public DataKeyDispenser KeyDispenser { get; private set; } = null!;
    public KeySocket ArchiveSocket { get; private set; } = null!;
    public Door VaultDoor { get; private set; } = null!;
    public VerificationTerminal TerminalAlpha { get; private set; } = null!;
    public VerificationTerminal TerminalBeta { get; private set; } = null!;
    public Door DeepArchiveGate { get; private set; } = null!;
    public NpcCharacter MiraNpc { get; private set; } = null!;
    public NarrativeTerminal OriginalBroadcast { get; private set; } = null!;
    public ExitGoal TransitExit { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        if (GetNodeOrNull<RoomGeometry>("RoomGeometry") is { } geom && geom.PartitionWalls.Count == 0)
        {
            geom.PartitionWalls = new()
            {
                new Rect2(232, 0, 16, 156),
                new Rect2(232, 244, 16, 156),
                new Rect2(412, 0, 16, 156),
                new Rect2(412, 244, 16, 156)
            };
            geom.SetRoomLayout(geom.RoomBounds, geom.StartingPosition, partitions: geom.PartitionWalls);
        }

        KeyDispenser = GetNode<DataKeyDispenser>("PuzzleModules/KeyDispenser");
        ArchiveSocket = GetNode<KeySocket>("PuzzleModules/ArchiveSocket");
        VaultDoor = GetNode<Door>("PuzzleModules/VaultDoor");
        TerminalAlpha = GetNode<VerificationTerminal>("PuzzleModules/TerminalAlpha");
        TerminalBeta = GetNode<VerificationTerminal>("PuzzleModules/TerminalBeta");
        DeepArchiveGate = GetNode<Door>("PuzzleModules/DeepArchiveGate");
        MiraNpc = GetNode<NpcCharacter>("PuzzleModules/MiraNpc");
        OriginalBroadcast = GetNode<NarrativeTerminal>("PuzzleModules/OriginalBroadcast");
        TransitExit = GetNode<ExitGoal>("PuzzleModules/ExitGoal");

        // Key socket opens outer vault door
        ArchiveSocket.SocketStateChanged += inserted =>
        {
            VaultDoor.SetOpen(inserted);
        };

        // Dual terminal authentication opens deep archive gate
        TerminalAlpha.StateChanged += _ => CheckDeepArchiveAccess();
        TerminalBeta.StateChanged += _ => CheckDeepArchiveAccess();

        // Mira dialogue
        MiraNpc.DialogueLines =
        [
            "Greg! Thank goodness you authenticated the archive vaults!",
            "I've cross-referenced the original transmission logs. The emergency notice was completely fabricated!",
            "The malicious entity calls itself NULL. It's attempting to seize the Signal Tower right now!"
        ];
    }

    private void CheckDeepArchiveAccess()
    {
        bool bothVerified = TerminalAlpha.IsCompleted && TerminalBeta.IsCompleted;
        DeepArchiveGate.SetOpen(bothVerified);
        if (bothVerified)
        {
            ObjectiveText = "Deep archive unsealed! Consult with Mira and recover the uncorrupted broadcast.";
        }
    }
}
