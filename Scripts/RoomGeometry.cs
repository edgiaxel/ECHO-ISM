using Godot;

namespace ECHOISM;

public partial class RoomGeometry : Node2D
{
    [Export]
    public Rect2 RoomBounds { get; set; } = new(240, 166, 800, 400);

    [Export]
    public Vector2 StartingPosition { get; set; } = new(360, 366);

    private static readonly Color RoomColor = new("#090E1A");
    private static readonly Color EdgeColor = new("#1E293B");
    private static readonly Color EdgeInnerColor = new("#334155");
    private static readonly Color ObstacleBody = new("#0E1626");
    private static readonly Color ObstacleBorder = new("#334155");
    private static readonly Color SignalColor = new("#00D2FF");
    private static readonly Color StartColor = new("#38BDF8");

    public override void _Draw()
    {
        DrawRect(RoomBounds, RoomColor);
        DrawRect(RoomBounds, EdgeColor, false, 4f);
        DrawRect(RoomBounds.Grow(-3f), EdgeInnerColor, false, 1f);

        // Circuit Traces: Switch (440, 470) -> Door (640, 366) -> Exit (920, 366)
        DrawLine(new Vector2(440, 470), new Vector2(580, 470), SignalColor, 2.5f);
        DrawLine(new Vector2(580, 470), new Vector2(580, 366), SignalColor, 2.5f);
        DrawLine(new Vector2(580, 366), new Vector2(632, 366), SignalColor, 2.5f);
        DrawCircle(new Vector2(580, 470), 4f, SignalColor);
        DrawCircle(new Vector2(580, 366), 4f, SignalColor);

        DrawLine(new Vector2(648, 366), new Vector2(920, 366), SignalColor, 2.5f);
        DrawCircle(new Vector2(740, 366), 4f, SignalColor);

        // Center Dividing Wall (Upper & Lower)
        var upperDivider = new Rect2(632, 166, 16, 154);
        var lowerDivider = new Rect2(632, 412, 16, 154);
        DrawRect(upperDivider, ObstacleBody);
        DrawRect(upperDivider, ObstacleBorder, false, 2f);
        DrawRect(lowerDivider, ObstacleBody);
        DrawRect(lowerDivider, ObstacleBorder, false, 2f);

        // Obstacle Pillars
        DrawPillar(new Vector2(480, 270));
        DrawPillar(new Vector2(800, 270));
        DrawPillar(new Vector2(800, 460));

        // Spawn Marker
        DrawCircle(StartingPosition, 16f, new Color(StartColor, 0.15f));
        DrawCircle(StartingPosition, 4f, StartColor);
    }

    private void DrawPillar(Vector2 center)
    {
        var rect = new Rect2(center.X - 16f, center.Y - 16f, 32f, 32f);
        DrawRect(rect, ObstacleBody);
        DrawRect(rect, ObstacleBorder, false, 2f);
        DrawCircle(center, 3f, SignalColor);
    }

    public override void _Ready()
    {
        BuildRoomWalls();
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
            walls = new StaticBody2D { Name = "RoomWalls", CollisionLayer = 1, CollisionMask = 1 };
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

        // Upper & Lower Dividing Walls
        AddWall(walls, "DividerNorth", new Vector2(16, 154), new Vector2(640, 243));
        AddWall(walls, "DividerSouth", new Vector2(16, 154), new Vector2(640, 489));

        // Obstacle Pillars
        AddWall(walls, "PillarWest", new Vector2(32, 32), new Vector2(480, 270));
        AddWall(walls, "PillarEastNorth", new Vector2(32, 32), new Vector2(800, 270));
        AddWall(walls, "PillarEastSouth", new Vector2(32, 32), new Vector2(800, 460));
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
