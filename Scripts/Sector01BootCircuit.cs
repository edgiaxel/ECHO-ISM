#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector01BootCircuit : LevelContainer
{
    public TestSwitch PowerSwitch { get; private set; } = null!;
    public Door SecurityDoor { get; private set; } = null!;
    public ExitGoal ExitTerminal { get; private set; } = null!;
    public NarrativeTerminal PublicNotice { get; private set; } = null!;
    public NarrativeTerminal LostTransmission { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-01-boot-circuit";
        DisplayName = "Sector 01 - Boot Circuit";
        ObjectiveText = "Energize the power switch to open the bulkhead and reach the exit terminal.";
        CompletionCondition = "Traverse the communication facility and reach the exit terminal.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 15f;
        MaximumActiveEchoes = 3;

        base._Ready();

        PowerSwitch = GetNode<TestSwitch>("PuzzleModules/TestSwitch");
        SecurityDoor = GetNode<Door>("PuzzleModules/Door");
        ExitTerminal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        PublicNotice = GetNode<NarrativeTerminal>("PuzzleModules/PublicNoticeTerminal");
        LostTransmission = GetNode<NarrativeTerminal>("PuzzleModules/LostTransmissionTerminal");

        PowerSwitch.StateChanged += active =>
        {
            SecurityDoor.SetOpen(active);
            var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
            camera?.Shake(active ? 3.0f : 1.5f, 0.15f);
        };
    }
}
