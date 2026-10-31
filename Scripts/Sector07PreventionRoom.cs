#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector07PreventionRoom : LevelContainer
{
    public EventPreventionObjective EventObjective { get; private set; } = null!;
    public TestSwitch SwitchA { get; private set; } = null!;
    public TestSwitch SwitchB { get; private set; } = null!;
    public Door Door { get; private set; } = null!;
    public ExitGoal ExitGoal { get; private set; } = null!;
    public NarrativeTerminal PublicNotice { get; private set; } = null!;
    public NarrativeTerminal FinalTestimonyTerminal { get; private set; } = null!;

    public override void _Ready()
    {
        LevelId = "sector-07-cascade-prevention";
        DisplayName = "Sector 07 - Cascade Prevention";
        ObjectiveText = "Coordinate with Echoes: stabilize dual stations to halt cascade broadcast and restore the network.";
        CompletionCondition = "Escalation event prevented before cascade cutoff; exit gateway unlocked.";
        RoomBounds = new Rect2(240, 166, 800, 400);
        StartingPosition = new Vector2(360, 366);
        LoopDurationSeconds = 15.0f;
        MaximumActiveEchoes = 3;

        base._Ready();

        EventObjective = GetNode<EventPreventionObjective>("PuzzleModules/EventPreventionObjective");
        SwitchA = GetNode<TestSwitch>("PuzzleModules/SwitchA");
        SwitchB = GetNode<TestSwitch>("PuzzleModules/SwitchB");
        Door = GetNode<Door>("PuzzleModules/Door");
        ExitGoal = GetNode<ExitGoal>("PuzzleModules/ExitGoal");
        PublicNotice = GetNode<NarrativeTerminal>("PuzzleModules/PublicNoticeTerminal");
        FinalTestimonyTerminal = GetNode<NarrativeTerminal>("PuzzleModules/FinalTestimonyTerminal");

        // Start escalation countdown when room initializes
        EventObjective.StartCountdown();

        SwitchA.StateChanged += active =>
        {
            if (active)
            {
                EventObjective.AddIntervention();
                SoundManager.Instance?.Play(SoundEffect.SwitchToggle);
            }
        };

        SwitchB.StateChanged += active =>
        {
            if (active)
            {
                EventObjective.AddIntervention();
                SoundManager.Instance?.Play(SoundEffect.SwitchToggle);
            }
        };

        EventObjective.EventPrevented += () =>
        {
            Door.SetOpen(true);
            var camera = GetNodeOrNull<RoomCamera>("RoomCamera");
            camera?.Shake(3.0f, 0.25f);
            SoundManager.Instance?.Play(SoundEffect.DoorOpen);
        };
    }
}
