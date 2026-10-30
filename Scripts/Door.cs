using System;
using Godot;

namespace ECHOISM;

public partial class Door : StaticBody2D, IResettable
{
    [Export]
    public string DoorId { get; set; } = "door-m02";

    public bool IsOpen { get; private set; }

    public event Action<bool> StateChanged;

    private CollisionShape2D _collider;
    private CpuParticles2D _particles;

    private static readonly Color ClosedBodyColor = new("#0E1626");
    private static readonly Color ClosedBorderColor = new("#334155");
    private static readonly Color ClosedLockColor = new("#F59E0B");
    private static readonly Color OpenEnergyColor = new(0.06f, 0.72f, 0.51f, 0.22f);
    private static readonly Color OpenLightColor = new("#10B981");

    public override void _Ready()
    {
        _collider = GetNode<CollisionShape2D>("CollisionShape");
        _particles = GetNodeOrNull<CpuParticles2D>("OpenParticles");
        SetOpen(false);
    }

    public void SetOpen(bool open)
    {
        if (IsOpen == open)
        {
            return;
        }

        IsOpen = open;
        _collider?.SetDeferred(CollisionShape2D.PropertyName.Disabled, open);

        if (open && _particles is not null)
        {
            _particles.Restart();
            _particles.Emitting = true;
        }

        QueueRedraw();
        StateChanged?.Invoke(open);
    }

    public void ResetState()
    {
        SetOpen(false);
    }

    public override void _Draw()
    {
        var rect = new Rect2(-8f, -44f, 16f, 88f);

        if (!IsOpen)
        {
            DrawRect(rect, ClosedBodyColor);
            DrawRect(rect, ClosedBorderColor, false, 2f);

            DrawLine(new Vector2(-6f, -20f), new Vector2(6f, -20f), ClosedBorderColor, 1.5f);
            DrawLine(new Vector2(-6f, 0f), new Vector2(6f, 0f), ClosedBorderColor, 1.5f);
            DrawLine(new Vector2(-6f, 20f), new Vector2(6f, 20f), ClosedBorderColor, 1.5f);

            DrawCircle(new Vector2(0f, -6f), 3.5f, ClosedLockColor);
            DrawCircle(new Vector2(0f, 6f), 3.5f, ClosedLockColor);
        }
        else
        {
            DrawRect(rect, OpenEnergyColor);
            DrawRect(new Rect2(-8f, -44f, 4f, 88f), OpenLightColor);
            DrawRect(new Rect2(4f, -44f, 4f, 88f), OpenLightColor);
        }
    }
}
