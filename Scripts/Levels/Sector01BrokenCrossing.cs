#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector01BrokenCrossing : LevelContainer
{
    public PressurePlate GatePlate { get; private set; } = null!;
    public Door SecurityDoor { get; private set; } = null!;
    public TestSwitch BridgeSwitch { get; private set; } = null!;
    public Door BridgeBarrier { get; private set; } = null!;
    public NpcCharacter PipNpc { get; private set; } = null!;
    public NarrativeTerminal BroadcastTerminal { get; private set; } = null!;
    public ExitGoal TransitExit { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        if (GetNodeOrNull<RoomGeometry>("RoomGeometry") is { } geom && geom.PartitionWalls.Count == 0)
        {
            geom.PartitionWalls = new()
            {
                new Rect2(192, 0, 16, 156),
                new Rect2(192, 244, 16, 116),
                new Rect2(332, 0, 16, 156),
                new Rect2(332, 244, 16, 116)
            };
            geom.SetRoomLayout(geom.RoomBounds, geom.StartingPosition, partitions: geom.PartitionWalls);
        }

        GatePlate = GetNode<PressurePlate>("PuzzleModules/GatePlate");
        SecurityDoor = GetNode<Door>("PuzzleModules/SecurityDoor");
        BridgeSwitch = GetNode<TestSwitch>("PuzzleModules/BridgeSwitch");
        BridgeBarrier = GetNode<Door>("PuzzleModules/BridgeBarrier");
        PipNpc = GetNode<NpcCharacter>("PuzzleModules/PipNpc");
        BroadcastTerminal = GetNode<NarrativeTerminal>("PuzzleModules/BroadcastTerminal");
        TransitExit = GetNode<ExitGoal>("PuzzleModules/ExitGoal");

        // Pressure plate holds security door open while depressed
        GatePlate.ActiveChanged += active =>
        {
            SecurityDoor.SetOpen(active);
        };

        // Bridge switch extends light-bridge / opens chasm barrier
        BridgeSwitch.StateChanged += active =>
        {
            BridgeBarrier.SetOpen(active);
            if (active)
            {
                ObjectiveText = "Bridge active. Cross the chasm to speak with Pip.";
            }
        };

        // Pip dialogue hookup
        PipNpc.DialogueLines =
        [
            "Greg! You made it across the broken crossing!",
            "An emergency broadcast claimed other districts are isolating us.",
            "Inspect the broadcast terminal to recover the fragment, then take the transit hub!"
        ];
    }
}
