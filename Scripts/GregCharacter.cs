using System;
using Godot;

namespace ECHOISM;

public partial class GregCharacter : CharacterBody2D
{
    [Export]
    public float MovementSpeed { get; set; } = 190f;

    [Export]
    public bool IsEcho { get; set; }

    [Export]
    public float InteractionRadius { get; set; } = 52f;

    public event Action<string> PromptChanged;
    public event Action<string> InteractionPerformed;

    private Vector2 _facing = Vector2.Down;
    private IInteractable _nearbyInteractable;
    private string _currentPrompt = string.Empty;
    private Area2D _interactionArea = null!;

    private static readonly Color GregFill = new("#F2F4F7");
    private static readonly Color GregOutline = new("#17202C");
    private static readonly Color EchoFill = new(0.28f, 0.58f, 0.78f, 0.52f);
    private static readonly Color EchoOutline = new("#9CD8F0");
    private static readonly Color EyeColor = new("#17202C");

    public override void _Ready()
    {
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.Monitoring = !IsEcho;
        ConfigureMovementInput();
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsEcho)
        {
            return;
        }

        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = direction * MovementSpeed;
        if (!direction.IsZeroApprox())
        {
            _facing = direction;
            QueueRedraw();
        }

        MoveAndSlide();
        UpdateNearbyInteractable();

        if (Input.IsActionJustPressed("interact"))
        {
            Interact();
        }
    }

    public override void _Draw()
    {
        var fill = IsEcho ? EchoFill : GregFill;
        var outline = IsEcho ? EchoOutline : GregOutline;
        var center = Vector2.Zero;

        if (IsEcho)
        {
            DrawArc(center, 17f, 0f, Mathf.Tau, 32, outline, 2f, true);
        }

        DrawCircle(center, 13f, outline);
        DrawCircle(center, 10.5f, fill);

        var forward = _facing.Normalized();
        var side = new Vector2(-forward.Y, forward.X);
        var eyeAnchor = forward * 1.4f;
        DrawCircle(eyeAnchor + side * 3.3f, 2.1f, EyeColor);
        DrawCircle(eyeAnchor - side * 3.3f, 2.1f, EyeColor);
    }

    private void UpdateNearbyInteractable()
    {
        IInteractable nearest = null;
        var nearestDistance = float.MaxValue;

        foreach (var area in _interactionArea.GetOverlappingAreas())
        {
            if (area is not IInteractable candidate)
            {
                continue;
            }

            var distance = GlobalPosition.DistanceSquaredTo(area.GlobalPosition);
            if (distance <= InteractionRadius * InteractionRadius && distance < nearestDistance)
            {
                nearest = candidate;
                nearestDistance = distance;
            }
        }

        _nearbyInteractable = nearest;
        var prompt = nearest is null ? string.Empty : nearest.GetPrompt(this);
        if (prompt != _currentPrompt)
        {
            _currentPrompt = prompt;
            PromptChanged?.Invoke(prompt);
        }
    }

    private void Interact()
    {
        if (_nearbyInteractable is not { } target || !target.TryInteract(this))
        {
            return;
        }

        InteractionPerformed?.Invoke(target.StableId);
        UpdateNearbyInteractable();
    }

    private static void ConfigureMovementInput()
    {
        AddKeyAction("move_up", Key.W, Key.Up);
        AddKeyAction("move_down", Key.S, Key.Down);
        AddKeyAction("move_left", Key.A, Key.Left);
        AddKeyAction("move_right", Key.D, Key.Right);
        AddKeyAction("interact", Key.E);
    }

    private static void AddKeyAction(string action, params Key[] keys)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        foreach (var key in keys)
        {
            var input = new InputEventKey { PhysicalKeycode = key };
            if (!InputMap.ActionHasEvent(action, input))
            {
                InputMap.ActionAddEvent(action, input);
            }
        }
    }
}
