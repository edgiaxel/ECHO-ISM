using Godot;

namespace ECHOISM;

public partial class RoomGeometry : Node2D
{
    [Export]
    public Rect2 RoomBounds { get; set; } = new(240, 166, 800, 400);

    [Export]
    public Vector2 StartingPosition { get; set; } = new(400, 360);

    private static readonly Color RoomColor = new("#262D3B");
    private static readonly Color EdgeColor = new("#AAB6C4");
    private static readonly Color SignalColor = new("#478CBF");
    private static readonly Color StartColor = new("#FFFFFF");

    public override void _Draw()
    {
        DrawRect(RoomBounds, RoomColor);
        DrawRect(RoomBounds, EdgeColor, false, 6f);

        var traceY = RoomBounds.Position.Y + RoomBounds.Size.Y * 0.72f;
        DrawLine(new Vector2(RoomBounds.Position.X + 72, traceY), new Vector2(RoomBounds.Position.X + 228, traceY), SignalColor, 4f);
        DrawLine(new Vector2(RoomBounds.End.X - 228, traceY), new Vector2(RoomBounds.End.X - 72, traceY), SignalColor, 4f);
        DrawCircle(new Vector2(RoomBounds.Position.X + 72, traceY), 8f, SignalColor);
        DrawCircle(new Vector2(RoomBounds.End.X - 72, traceY), 8f, SignalColor);

        DrawCircle(StartingPosition, 15f, new Color(StartColor, 0.18f));
        DrawCircle(StartingPosition, 5f, StartColor);
    }

    public void SetRoomLayout(Rect2 bounds, Vector2 startPosition)
    {
        RoomBounds = bounds;
        StartingPosition = startPosition;
        QueueRedraw();
    }
}
