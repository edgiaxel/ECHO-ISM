using Godot;

namespace ECHOISM;

public partial class GameManager : Node2D
{
    [Export]
    public PackedScene InitialLevel { get; set; } = null!;

    public override void _Ready()
    {
        var level = InitialLevel.Instantiate<Node2D>();
        GetNode<Node2D>("LevelHost").AddChild(level);

        if (level is LevelContainer definition)
        {
            var roomBadge = GetNodeOrNull<Label>("BasicHud/HudRoot/TopBar/HBox/RoomInfo/RoomBadge");
            if (roomBadge is not null && !string.IsNullOrEmpty(definition.DisplayName))
            {
                roomBadge.Text = definition.DisplayName.ToUpperInvariant();
            }

            var chronoValue = GetNodeOrNull<Label>("BasicHud/HudRoot/TopBar/HBox/ChronoContainer/ChronoValue");
            if (chronoValue is not null)
            {
                chronoValue.Text = $"{definition.LoopDurationSeconds:0.00}s";
            }

            var objective = GetNode<Label>("BasicHud/HudRoot/ObjectiveCard/VBox/Objective");
            objective.Text = definition.ObjectiveText;

            var testSwitch = level.GetNode<TestSwitch>("PuzzleModules/TestSwitch");
            testSwitch.StateChanged += active =>
            {
                objective.Text = active ? "Switch M01 active. Circuit restored." : definition.ObjectiveText;
            };

            var greg = level.GetNode<GregCharacter>("Greg");
            var promptPanel = GetNode<Control>("BasicHud/HudRoot/PromptPanel");
            var prompt = GetNode<Label>("BasicHud/HudRoot/PromptPanel/HBox/InteractionPrompt");
            greg.PromptChanged += text =>
            {
                prompt.Text = text;
                promptPanel.Visible = !string.IsNullOrEmpty(text);
            };
        }
    }
}
