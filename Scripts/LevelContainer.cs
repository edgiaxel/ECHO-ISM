using Godot;

namespace ECHOISM;

public partial class LevelContainer : Node2D
{
    [Export]
    public string LevelId { get; set; } = "boot-sequence-test-room";

    [Export]
    public string DisplayName { get; set; } = "Boot Sequence Test Room";

    [Export]
    public Rect2 RoomBounds { get; set; } = new(240, 166, 800, 400);

    [Export]
    public Vector2 StartingPosition { get; set; } = new(360, 366);

    [Export(PropertyHint.Range, "1,120,1")]
    public float LoopDurationSeconds { get; set; } = 15;

    [Export(PropertyHint.Range, "0,3,1")]
    public int MaximumActiveEchoes { get; set; } = 3;

    [Export(PropertyHint.MultilineText)]
    public string ObjectiveText { get; set; } = "Coordinate with Echo to open Door M02 and reach Exit Node.";

    [Export(PropertyHint.MultilineText)]
    public string CompletionCondition { get; set; } = "Reach Exit Node while Door M02 is unlocked.";

    public override void _Ready()
    {
        GetNode<RoomGeometry>("RoomGeometry").SetRoomLayout(RoomBounds, StartingPosition);
        GetNode<GregCharacter>("Greg").Position = StartingPosition;
    }
}
