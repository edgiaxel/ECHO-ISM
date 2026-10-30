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
            GetNode<Label>("BasicHud/HudRoot/InfoPanel/Info/RoomName").Text = definition.DisplayName;
            GetNode<Label>("BasicHud/HudRoot/InfoPanel/Info/Objective").Text = definition.ObjectiveText;
        }
    }
}
