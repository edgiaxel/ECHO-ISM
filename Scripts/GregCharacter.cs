#nullable enable
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

    public event Action<string>? PromptChanged;
    public event Action<string>? InteractionPerformed;

    public Vector2 Facing => _facing;

    protected Vector2 _facing = Vector2.Down;
    private IInteractable? _nearbyInteractable;
    private string _currentPrompt = string.Empty;
    private Area2D _interactionArea = null!;

    // Expression & wobble state
    private float _walkCycle;
    private float _blinkTimer = 3.5f;
    private bool _isBlinking;
    private float _blinkDuration;
    private bool _isInteracting;
    private float _interactionAnimTimer;

    private static readonly Color GregFill = new("#00D2FF");
    private static readonly Color GregOutline = new("#081B33");
    private static readonly Color EchoFill = new(0.505f, 0.549f, 0.973f, 0.55f);
    private static readonly Color EchoOutline = new("#38BDF8");
    private static readonly Color EyeColor = new("#081B33");
    private static readonly Color EyeSpecular = new("#FFFFFF");
    private static readonly Color DigitalTrailColor = new(0.0f, 0.82f, 1.0f, 0.25f);

    public override void _Ready()
    {
        _interactionArea = GetNodeOrNull<Area2D>("InteractionArea");
        if (_interactionArea is not null)
        {
            _interactionArea.Monitoring = !IsEcho;
        }
        ConfigureMovementInput();
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Blink logic
        _blinkTimer -= (float)delta;
        if (_blinkTimer <= 0f)
        {
            _isBlinking = true;
            _blinkDuration = 0.12f;
            _blinkTimer = (float)GD.RandRange(2.5, 5.0);
        }

        if (_isBlinking)
        {
            _blinkDuration -= (float)delta;
            if (_blinkDuration <= 0f)
            {
                _isBlinking = false;
            }
        }

        // Interaction reaction timer
        if (_isInteracting)
        {
            _interactionAnimTimer -= (float)delta;
            if (_interactionAnimTimer <= 0f)
            {
                _isInteracting = false;
            }
        }

        if (IsEcho)
        {
            QueueRedraw();
            return;
        }

        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = direction * MovementSpeed;
        if (!direction.IsZeroApprox())
        {
            _facing = direction;
            _walkCycle += (float)delta * 12f;
        }
        else
        {
            _walkCycle = 0f;
        }

        QueueRedraw();
        MoveAndSlide();
        UpdateNearbyInteractable();

        if (Input.IsActionJustPressed("interact"))
        {
            Interact();
        }
    }

    public void ResetTo(Vector2 startPosition, Vector2 facing)
    {
        GlobalPosition = startPosition;
        Velocity = Vector2.Zero;
        _facing = facing;
        _nearbyInteractable = null;
        _currentPrompt = string.Empty;
        _walkCycle = 0f;
        _isBlinking = false;
        _isInteracting = false;
        PromptChanged?.Invoke(string.Empty);
        QueueRedraw();
    }

    public override void _Draw()
    {
        var fill = IsEcho ? EchoFill : GregFill;
        var outline = IsEcho ? EchoOutline : GregOutline;
        var center = Vector2.Zero;

        // Faint digital movement trail
        if (!Velocity.IsZeroApprox())
        {
            var trailOffset = -_facing.Normalized() * 8f;
            DrawCircle(trailOffset, 6.5f, DigitalTrailColor);
        }

        // Squash and stretch wobble based on walk cycle or interaction
        float scaleX = 1.0f;
        float scaleY = 1.0f;

        if (_isInteracting)
        {
            scaleX = 1.15f;
            scaleY = 0.85f;
        }
        else if (!Velocity.IsZeroApprox())
        {
            float wobble = Mathf.Sin(_walkCycle) * 0.08f;
            scaleX += wobble;
            scaleY -= wobble;
        }

        // Echo aura ring
        if (IsEcho)
        {
            DrawArc(center, 18f, 0f, Mathf.Tau, 32, outline, 1.5f, true);
        }

        // Scaled body blob
        DrawEllipse(center, 13f * scaleX, 13f * scaleY, outline);
        DrawEllipse(center, 10.5f * scaleX, 10.5f * scaleY, fill);

        // Directional eye expressions
        var forward = _facing.Normalized();
        var side = new Vector2(-forward.Y, forward.X);
        var eyeAnchor = forward * 1.8f;
        var leftEye = eyeAnchor + side * 3.4f;
        var rightEye = eyeAnchor - side * 3.4f;

        if (_isBlinking)
        {
            // Closed eye slit
            DrawLine(leftEye - side * 2f, leftEye + side * 2f, EyeColor, 1.5f);
            DrawLine(rightEye - side * 2f, rightEye + side * 2f, EyeColor, 1.5f);
        }
        else if (_isInteracting)
        {
            // Excited happy eye arches ^ ^
            DrawArc(leftEye + new Vector2(0f, 1f), 2.2f, Mathf.Pi, Mathf.Tau, 12, EyeColor, 1.5f);
            DrawArc(rightEye + new Vector2(0f, 1f), 2.2f, Mathf.Pi, Mathf.Tau, 12, EyeColor, 1.5f);
        }
        else
        {
            // Expressive round eyes with specular highlights
            DrawCircle(leftEye, 2.3f, EyeColor);
            DrawCircle(rightEye, 2.3f, EyeColor);

            DrawCircle(leftEye + new Vector2(-0.6f, -0.6f), 0.7f, EyeSpecular);
            DrawCircle(rightEye + new Vector2(-0.6f, -0.6f), 0.7f, EyeSpecular);
        }
    }

    private void DrawEllipse(Vector2 center, float radiusX, float radiusY, Color color)
    {
        var points = new Vector2[32];
        for (int i = 0; i < 32; i++)
        {
            float angle = (float)i / 32 * Mathf.Tau;
            points[i] = center + new Vector2(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY);
        }
        DrawColoredPolygon(points, color);
    }

    private void UpdateNearbyInteractable()
    {
        if (_interactionArea is null)
        {
            return;
        }

        IInteractable? nearest = null;
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

        _isInteracting = true;
        _interactionAnimTimer = 0.25f;
        SoundManager.Instance?.Play(SoundEffect.Interact);

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
