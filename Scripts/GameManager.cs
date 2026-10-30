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
            var objective = GetNode<Label>("BasicHud/HudRoot/ObjectivePanel/Objective");
            objective.Text = definition.ObjectiveText;
            var testSwitch = level.GetNode<TestSwitch>("PuzzleModules/TestSwitch");
            testSwitch.StateChanged += active => objective.Text = active ? "Switch active." : definition.ObjectiveText;

            var greg = level.GetNode<GregCharacter>("Greg");
            var promptPanel = GetNode<Control>("BasicHud/HudRoot/PromptPanel");
            var prompt = GetNode<Label>("BasicHud/HudRoot/PromptPanel/InteractionPrompt");
            greg.PromptChanged += text =>
            {
                prompt.Text = text;
                promptPanel.Visible = !string.IsNullOrEmpty(text);
            };
        }
    }
}
