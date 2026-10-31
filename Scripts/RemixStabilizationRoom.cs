#nullable enable
using Godot;

namespace ECHOISM;

public partial class RemixStabilizationRoom : LevelContainer
{
    public TimingSlider Slider { get; private set; } = null!;
    public PressurePlate Plate { get; private set; } = null!;
    public DualMechanismDoor Door { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public NarrativeTerminal PublicNotice { get; private set; } = null!;
    public NarrativeTerminal NetworkArchaeologyTerminal { get; private set; } = null!;

    [Export]
    public int RemixSeed { get; set; } = 42;

    public LevelVariationParameters AppliedParameters { get; private set; }

    public override void _Ready()
    {
        LevelId = "sector-05-remix-stabilization";
        DisplayName = "Sector 05 - Fractured Network";
        ObjectiveText = "Synthesize multi-loop coordination: calibrate timing slider while locking pressure plate to bridge the fractured grid.";
        CompletionCondition = "Stabilize parameters and reach final exit gateway.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);

        AppliedParameters = LevelRemixer.GenerateRemix("RemixStabilization", RemixSeed);
        LoopDurationSeconds = AppliedParameters.LoopDurationSeconds > 0 ? AppliedParameters.LoopDurationSeconds : 16f;
        MaximumActiveEchoes = AppliedParameters.MaximumActiveEchoes > 0 ? AppliedParameters.MaximumActiveEchoes : 3;

        base._Ready();

        Slider = GetNode<TimingSlider>("PuzzleModules/TimingSlider");
        Plate = GetNode<PressurePlate>("PuzzleModules/PressurePlate");
        Door = GetNode<DualMechanismDoor>("PuzzleModules/DualMechanismDoor");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        PublicNotice = GetNode<NarrativeTerminal>("PuzzleModules/PublicNoticeTerminal");
        NetworkArchaeologyTerminal = GetNode<NarrativeTerminal>("PuzzleModules/NetworkArchaeologyTerminal");

        Slider.ApplyPreset(AppliedParameters.TimingPreset);

        Plate.ActiveChanged += active =>
        {
            Door.SetInputA(active);
        };

        Slider.StateChanged += state =>
        {
            Door.SetInputB(state == ModuleState.Completed);
        };

        Door.DoorOpened += open =>
        {
            if (open)
            {
                var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
                camera?.Shake(3.0f, 0.2f);
            }
        };
    }
}
