using Godot;
using System;

namespace ECHOISM;

public partial class TestSwitch : Area2D, IInteractable, IResettable
{
    [Export]
    public string StableId { get; set; } = "test-switch";

    [Export]
    public bool AcceptsEchoes { get; set; } = true;

    public bool IsActive { get; private set; }

    public event Action<bool> StateChanged;

    private CpuParticles2D _particles;

    private static readonly Color BodyColor = new("#0E1626");
    private static readonly Color EdgeColorInactive = new("#334155");
    private static readonly Color EdgeColorActive = new("#00D2FF");
    private static readonly Color SignalInactive = new("#334155");
    private static readonly Color SignalActive = new("#10B981");
    private static readonly Color CoreInactive = new("#F59E0B");
    private static readonly Color CoreActive = new("#10B981");

    public override void _Ready()
    {
        _particles = GetNodeOrNull<CpuParticles2D>("ActiveParticles");
    }

    public string GetPrompt(GregCharacter actor) => IsActive ? "Deactivate switch" : "Activate switch";

    public bool TryInteract(GregCharacter actor)
    {
        if (actor.IsEcho && !AcceptsEchoes)
        {
            return false;
        }

        IsActive = !IsActive;

        if (IsActive && _particles is not null)
        {
            _particles.Restart();
            _particles.Emitting = true;
        }

        QueueRedraw();
        StateChanged?.Invoke(IsActive);
        return true;
    }

    public void ResetState()
    {
        if (IsActive)
        {
            IsActive = false;
            QueueRedraw();
            StateChanged?.Invoke(IsActive);
        }
    }

    public override void _Draw()
    {
        var edgeColor = IsActive ? EdgeColorActive : EdgeColorInactive;
        var signalColor = IsActive ? SignalActive : SignalInactive;
        var coreColor = IsActive ? CoreActive : CoreInactive;

        var rect = new Rect2(-18f, -15f, 36f, 30f);
        DrawRect(rect, BodyColor);
        DrawRect(rect, edgeColor, false, 2f);

        DrawLine(new Vector2(-29f, 0f), new Vector2(-18f, 0f), signalColor, 3f);
        DrawLine(new Vector2(18f, 0f), new Vector2(29f, 0f), signalColor, 3f);

        var track = new Rect2(-10f, -5f, 20f, 10f);
        DrawRect(track, BodyColor);
        DrawRect(track, edgeColor, false, 1.5f);

        DrawCircle(new Vector2(IsActive ? 5f : -5f, 0f), 3.5f, coreColor);
    }
}
