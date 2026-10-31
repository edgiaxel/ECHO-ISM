#nullable enable
using Godot;

namespace ECHOISM;

public partial class DualProtocolRoom : LevelContainer
{
    public PressurePlate Plate { get; private set; } = null!;
    public TestSwitch Switch { get; private set; } = null!;
    public DualMechanismDoor DualDoor { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public NarrativeTerminal WestTerminal { get; private set; } = null!;
    public NarrativeTerminal EastTerminal { get; private set; } = null!;
    public NarrativeTerminal MediatorTerminal { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-03-dual-protocol";
        DisplayName = "Sector 03 - Divided Consensus";
        ObjectiveText = "Coordinate with an Echo to hold the West pressure plate while activating the East district switch.";
        CompletionCondition = "Dual Door M02 unlocked; Exit Goal reached.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 16f;
        MaximumActiveEchoes = 3;

        base._Ready();

        Plate = GetNode<PressurePlate>("PuzzleModules/PressurePlate");
        Switch = GetNode<TestSwitch>("PuzzleModules/TestSwitch");
        DualDoor = GetNode<DualMechanismDoor>("PuzzleModules/DualMechanismDoor");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        WestTerminal = GetNode<NarrativeTerminal>("PuzzleModules/WestTerminal");
        EastTerminal = GetNode<NarrativeTerminal>("PuzzleModules/EastTerminal");
        MediatorTerminal = GetNode<NarrativeTerminal>("PuzzleModules/MediatorTerminal");

        Plate.ActiveChanged += active =>
        {
            DualDoor.SetInputA(active);
        };

        Switch.StateChanged += active =>
        {
            DualDoor.SetInputB(active);
        };

        DualDoor.DoorOpened += open =>
        {
            if (open)
            {
                var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
                camera?.Shake(3.0f, 0.2f);
            }
        };
    }
}
