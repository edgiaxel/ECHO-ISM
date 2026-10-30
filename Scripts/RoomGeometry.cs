using Godot;

namespace ECHOISM;

public partial class RoomGeometry : Node2D
{
    [Export]
    public Rect2 RoomBounds { get; set; } = new(240, 166, 800, 400);

    [Export]
    public Vector2 StartingPosition { get; set; } = new(400, 360);

    private static readonly Color RoomColor = new("#090E1A");
    private static readonly Color EdgeColor = new("#1E293B");
    private static readonly Color EdgeInnerColor = new("#334155");
    private static readonly Color SignalColor = new("#00D2FF");
    private static readonly Color StartColor = new("#38BDF8");

    public override void _Draw()
    {
        DrawRect(RoomBounds, RoomColor);
        DrawRect(RoomBounds, EdgeColor, false, 4f);
        DrawRect(RoomBounds.Grow(-3f), EdgeInnerColor, false, 1f);

        var traceY = RoomBounds.Position.Y + RoomBounds.Size.Y * 0.72f;
        DrawLine(new Vector2(RoomBounds.Position.X + 72, traceY), new Vector2(RoomBounds.Position.X + 228, traceY), SignalColor, 3f);
        DrawLine(new Vector2(RoomBounds.End.X - 228, traceY), new Vector2(RoomBounds.End.X - 72, traceY), SignalColor, 3f);
        DrawCircle(new Vector2(RoomBounds.Position.X + 72, traceY), 6f, SignalColor);
        DrawCircle(new Vector2(RoomBounds.End.X - 72, traceY), 6f, SignalColor);

        DrawCircle(StartingPosition, 16f, new Color(StartColor, 0.15f));
        DrawCircle(StartingPosition, 4f, StartColor);
    }

    public void SetRoomLayout(Rect2 bounds, Vector2 startPosition)
    {
        RoomBounds = bounds;
        StartingPosition = startPosition;
        BuildRoomWalls();
        QueueRedraw();
    }

    private void BuildRoomWalls()
    {
        var walls = GetNodeOrNull<StaticBody2D>("RoomWalls");
        if (walls is null)
        {
            walls = new StaticBody2D { Name = "RoomWalls", CollisionLayer = 1 };
            AddChild(walls);
        }

        foreach (Node child in walls.GetChildren())
        {
            child.QueueFree();
        }

        const float thickness = 16f;
        AddWall(walls, "North", new Vector2(RoomBounds.Size.X + thickness * 2f, thickness), new Vector2(RoomBounds.GetCenter().X, RoomBounds.Position.Y));
        AddWall(walls, "South", new Vector2(RoomBounds.Size.X + thickness * 2f, thickness), new Vector2(RoomBounds.GetCenter().X, RoomBounds.End.Y));
        AddWall(walls, "West", new Vector2(thickness, RoomBounds.Size.Y), new Vector2(RoomBounds.Position.X, RoomBounds.GetCenter().Y));
        AddWall(walls, "East", new Vector2(thickness, RoomBounds.Size.Y), new Vector2(RoomBounds.End.X, RoomBounds.GetCenter().Y));
    }

    private static void AddWall(StaticBody2D parent, string name, Vector2 size, Vector2 position)
    {
        var shape = new RectangleShape2D { Size = size };
        var collider = new CollisionShape2D
        {
            Name = name,
            Shape = shape,
            Position = position
        };
        parent.AddChild(collider);
    }
}
