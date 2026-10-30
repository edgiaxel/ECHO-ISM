using Godot;
using System;

namespace ECHOISM;

public partial class TestSwitch : Area2D, IInteractable
{
    [Export]
    public string StableId { get; set; } = "test-switch";

    public bool IsActive { get; private set; }

    public event Action<bool> StateChanged;

    private static readonly Color BodyColor = new("#303B4B");
    private static readonly Color EdgeColor = new("#AAB6C4");
    private static readonly Color SignalColor = new("#478CBF");
    private static readonly Color ActiveColor = new("#F2F4F7");

    public string GetPrompt(GregCharacter actor) => IsActive ? "E   Deactivate switch" : "E   Activate switch";

    public bool TryInteract(GregCharacter actor)
    {
        IsActive = !IsActive;
        QueueRedraw();
        StateChanged?.Invoke(IsActive);
        return true;
    }

    public override void _Draw()
    {
        var rect = new Rect2(-18f, -15f, 36f, 30f);
        DrawRect(rect, BodyColor);
        DrawRect(rect, EdgeColor, false, 2f);
        DrawLine(new Vector2(-29f, 0f), new Vector2(-18f, 0f), SignalColor, 3f);
        DrawLine(new Vector2(18f, 0f), new Vector2(29f, 0f), SignalColor, 3f);
        var track = new Rect2(-10f, -5f, 20f, 10f);
        DrawRect(track, BodyColor);
        DrawRect(track, EdgeColor, false, 1.5f);
        DrawCircle(new Vector2(IsActive ? 5f : -5f, 0f), 3.2f, ActiveColor);
    }
}
