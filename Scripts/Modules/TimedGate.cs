#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class TimedGate : StaticBody2D, IResettable, IPuzzleModule
{
    [Export]
    public string StableId { get; set; } = "timed-gate-01";

    public string ModuleId => StableId;

    [Export]
    public string ModuleName { get; set; } = "Timed Gate";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public float OpenDuration { get; set; } = 4.0f;

    [Export]
    public Vector2 GateSize { get; set; } = new(48f, 16f);

    public bool IsOpen { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsCompleted => IsOpen;

    public ModuleState CurrentState => IsOpen ? ModuleState.Completed : ModuleState.Idle;

    public event Action<bool>? GateStateChanged;
    public event Action<ModuleState>? StateChanged;

    private CollisionShape2D? _collision;
    private static readonly Color FrameColor = new("#1E293B");
    private static readonly Color BarrierClosed = new("#EF4444");
    private static readonly Color BarrierWarning = new("#F59E0B");
    private static readonly Color BarrierOpen = new("#10B981");

    public override void _Ready()
    {
        CollisionLayer = 1; // Solid barrier
        CollisionMask = 0;

        _collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_collision is null)
        {
            _collision = new CollisionShape2D
            {
                Shape = new RectangleShape2D { Size = GateSize }
            };
            AddChild(_collision);
        }
        else if (_collision.Shape is RectangleShape2D rectShape)
        {
            rectShape.Size = GateSize;
        }

        SetOpen(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsOpen)
        {
            TimeRemaining -= (float)delta;
            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                SetOpen(false);
                SoundManager.Instance?.Play(SoundEffect.TimingMiss);
            }
            QueueRedraw();
        }
    }

    public void Trigger()
    {
        TimeRemaining = OpenDuration;
        SetOpen(true);
        SoundManager.Instance?.Play(SoundEffect.DoorOpen);
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;
        if (_collision is not null)
        {
            _collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, open);
        }

        GateStateChanged?.Invoke(open);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
    }

    public void ResetState()
    {
        TimeRemaining = 0f;
        SetOpen(false);
    }

    public override void _Draw()
    {
        var half = GateSize * 0.5f;

        if (GateSize.Y >= GateSize.X)
        {
            // Vertical aperture barrier (e.g. 16x88)
            DrawRect(new Rect2(-half.X, -half.Y, GateSize.X, 4f), FrameColor);
            DrawRect(new Rect2(-half.X, half.Y - 4f, GateSize.X, 4f), FrameColor);
            DrawRect(new Rect2(-half.X, -half.Y, 4f, GateSize.Y), FrameColor);
            DrawRect(new Rect2(half.X - 4f, -half.Y, 4f, GateSize.Y), FrameColor);

            if (!IsOpen)
            {
                DrawRect(new Rect2(-half.X + 2f, -half.Y + 4f, GateSize.X - 4f, GateSize.Y - 8f), new Color(BarrierClosed, 0.35f));
                for (float y = -half.Y + 12f; y < half.Y - 6f; y += 12f)
                {
                    DrawLine(new Vector2(-half.X + 2f, y), new Vector2(half.X - 2f, y), BarrierClosed, 2f);
                }
            }
            else
            {
                Color activeColor = TimeRemaining < 1.5f ? BarrierWarning : BarrierOpen;
                DrawRect(new Rect2(-half.X + 2f, -half.Y + 4f, GateSize.X - 4f, GateSize.Y - 8f), new Color(activeColor, 0.15f));

                int dots = Mathf.CeilToInt(TimeRemaining);
                for (int i = 0; i < dots; i++)
                {
                    float y = -half.Y + 14f + i * 16f;
                    if (y < half.Y - 8f)
                    {
                        DrawCircle(new Vector2(0f, y), 3f, activeColor);
                    }
                }
            }
        }
        else
        {
            // Horizontal barrier (e.g. 48x16)
            DrawRect(new Rect2(-half.X, -half.Y, 6f, GateSize.Y), FrameColor);
            DrawRect(new Rect2(half.X - 6f, -half.Y, 6f, GateSize.Y), FrameColor);

            if (!IsOpen)
            {
                DrawRect(new Rect2(-half.X + 6f, -half.Y + 2f, GateSize.X - 12f, GateSize.Y - 4f), new Color(BarrierClosed, 0.3f));
                for (float x = -half.X + 10f; x < half.X - 6f; x += 8f)
                {
                    DrawLine(new Vector2(x, -half.Y + 2f), new Vector2(x, half.Y - 2f), BarrierClosed, 2f);
                }
            }
            else
            {
                Color activeColor = TimeRemaining < 1.5f ? BarrierWarning : BarrierOpen;
                DrawRect(new Rect2(-half.X + 6f, -half.Y + 4f, GateSize.X - 12f, GateSize.Y - 8f), new Color(activeColor, 0.15f));

                int dots = Mathf.CeilToInt(TimeRemaining);
                for (int i = 0; i < dots; i++)
                {
                    float x = -half.X + 12f + i * 8f;
                    DrawCircle(new Vector2(x, 0f), 2.5f, activeColor);
                }
            }
        }
    }
}
