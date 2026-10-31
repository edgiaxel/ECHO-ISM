#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector02Powerworks : LevelContainer
{
    public ChargeStation PowerCapacitor { get; private set; } = null!;
    public RotaryJunction GridRouter { get; private set; } = null!;
    public Door LiftGate { get; private set; } = null!;
    public NarrativeTerminal EvidenceTerminal { get; private set; } = null!;
    public NpcCharacter BoltNpc { get; private set; } = null!;
    public ExitGoal TransitExit { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        if (GetNodeOrNull<RoomGeometry>("RoomGeometry") is { } geom && geom.PartitionWalls.Count == 0)
        {
            geom.PartitionWalls = new()
            {
                new Rect2(352, 0, 16, 156),
                new Rect2(352, 244, 16, 136)
            };
            geom.SetRoomLayout(geom.RoomBounds, geom.StartingPosition, partitions: geom.PartitionWalls);
        }

        PowerCapacitor = GetNode<ChargeStation>("PuzzleModules/PowerCapacitor");
        GridRouter = GetNode<RotaryJunction>("PuzzleModules/GridRouter");
        LiftGate = GetNode<Door>("PuzzleModules/LiftGate");
        EvidenceTerminal = GetNode<NarrativeTerminal>("PuzzleModules/EvidenceTerminal");
        BoltNpc = GetNode<NpcCharacter>("PuzzleModules/BoltNpc");
        TransitExit = GetNode<ExitGoal>("PuzzleModules/ExitGoal");

        // Power capacitor supplies energy to the grid router
        PowerCapacitor.BooleanStateChanged += charged =>
        {
            GridRouter.SetPowerInput(charged);
            CheckLiftCondition();
        };

        // Grid router alignment checks lift condition
        GridRouter.AlignmentChanged += _ =>
        {
            CheckLiftCondition();
        };

        // BOLT dialogue lines
        BoltNpc.DialogueLines =
        [
            "Hold it right there! District 3 sabotaged our routing grid!",
            "What? That terminal log proves the shutdown command was spoofed from an outside source?",
            "Then the other districts didn't isolate us... I'll re-engage the power grid. Head to the Archive!"
        ];
    }

    private void CheckLiftCondition()
    {
        bool canOpenLift = PowerCapacitor.IsFullyCharged && GridRouter.IsAligned;
        LiftGate.SetOpen(canOpenLift);
        if (canOpenLift)
        {
            ObjectiveText = "Lift energized! Ascend to the upper control deck and speak with BOLT.";
        }
    }
}
