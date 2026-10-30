using System;
using Godot;

namespace ECHOISM;

public partial class ExitGoal : Area2D, IResettable
{
    [Signal]
    public delegate void CompletedEventHandler();

    [Export]
    public string GoalId { get; set; } = "exit-terminal-m06";

    public bool IsReached { get; private set; }

    private CpuParticles2D _particles;

    private static readonly Color PedestalColor = new("#0E1626");
    private static readonly Color BorderColor = new("#00D2FF");
    private static readonly Color VerifiedColor = new("#10B981");
    private static readonly Color InactiveColor = new("#38BDF8");

    public override void _Ready()
    {
        _particles = GetNodeOrNull<CpuParticles2D>("SuccessParticles");
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is GregCharacter actor && !actor.IsEcho && !IsReached)
        {
            IsReached = true;
            EmitSignal(SignalName.Completed);

            if (_particles is not null)
            {
                _particles.Restart();
                _particles.Emitting = true;
            }

            QueueRedraw();
            GD.Print($"[ExitGoal] Goal reached by Greg! Level complete.");
        }
    }

    public void ResetState()
    {
        IsReached = false;
        QueueRedraw();
    }

    public override void _Draw()
    {
        var activeColor = IsReached ? VerifiedColor : InactiveColor;

        var baseRect = new Rect2(-18f, -18f, 36f, 36f);
        DrawRect(baseRect, PedestalColor);
        DrawRect(baseRect, activeColor, false, 2.5f);

        DrawCircle(Vector2.Zero, 8f, new Color(activeColor, 0.25f));
        DrawCircle(Vector2.Zero, 4.5f, activeColor);

        var glowRing = new Rect2(-24f, -24f, 48f, 48f);
        DrawArc(Vector2.Zero, 24f, 0f, Mathf.Tau, 32, new Color(activeColor, 0.4f), 1.5f, true);
    }
}
