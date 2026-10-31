#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class RotaryJunction : Area2D, IInteractable, IResettable, IPuzzleModule
{
    [Export]
    public string StableId { get; set; } = "rotary-junction-01";

    public string ModuleId => StableId;

    [Export]
    public string ModuleName { get; set; } = "Rotary Junction";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public int CurrentOrientation { get; set; } = 0; // 0=Up, 1=Right, 2=Down, 3=Left

    [Export]
    public int TargetOrientation { get; set; } = 1;

    [Export]
    public bool AcceptsEchoes { get; set; } = true;

    [Export]
    public bool RequiresPowerInput { get; set; } = false;

    public bool HasPowerInput { get; set; } = true;

    public bool IsAligned => CurrentOrientation % 4 == TargetOrientation % 4 && (!RequiresPowerInput || HasPowerInput);

    public bool IsCompleted => IsAligned;

    public ModuleState CurrentState => IsAligned ? ModuleState.Completed : (RequiresPowerInput && !HasPowerInput ? ModuleState.Idle : ModuleState.Active);

    public event Action<bool>? AlignmentChanged;
    public event Action<ModuleState>? StateChanged;

    private int _initialOrientation;
    private CollisionShape2D? _collision;

    private static readonly Color HousingColor = new("#0F172A");
    private static readonly Color BorderDefault = new("#334155");
    private static readonly Color BorderActive = new("#00D2FF");
    private static readonly Color ConduitOff = new("#1E293B");
    private static readonly Color ConduitOn = new("#10B981");

    public override void _Ready()
    {
        CollisionLayer = 2; // Interactable layer
        CollisionMask = 0;

        _initialOrientation = CurrentOrientation;

        _collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_collision is null)
        {
            _collision = new CollisionShape2D
            {
                Shape = new CircleShape2D { Radius = 24f }
            };
            AddChild(_collision);
        }

        QueueRedraw();
    }

    public string GetPrompt(GregCharacter actor)
    {
        return IsAligned ? "Rotary Junction [ALIGNED - E to Rotate]" : "Rotate Junction 90° [E]";
    }

    public bool TryInteract(GregCharacter actor)
    {
        RotateJunction();
        return true;
    }

    public void RotateJunction()
    {
        CurrentOrientation = (CurrentOrientation + 1) % 4;
        SoundManager.Instance?.Play(SoundEffect.SwitchToggle);

        bool aligned = IsAligned;
        AlignmentChanged?.Invoke(aligned);
        StateChanged?.Invoke(CurrentState);

        if (aligned)
        {
            SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
        }

        QueueRedraw();
    }

    public void SetPowerInput(bool hasPower)
    {
        if (HasPowerInput != hasPower)
        {
            HasPowerInput = hasPower;
            bool aligned = IsAligned;
            AlignmentChanged?.Invoke(aligned);
            StateChanged?.Invoke(CurrentState);
            QueueRedraw();
        }
    }

    public void ResetState()
    {
        CurrentOrientation = _initialOrientation;
        if (RequiresPowerInput)
        {
            HasPowerInput = false;
        }
        AlignmentChanged?.Invoke(IsAligned);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 20f, HousingColor);
        DrawCircle(Vector2.Zero, 20f, IsAligned ? BorderActive : BorderDefault, false, 2f);

        float angle = CurrentOrientation * Mathf.Pi / 2.0f;
        var dir = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle));
        var start = -dir * 16f;
        var end = dir * 16f;

        Color conduitColor = IsAligned ? ConduitOn : ConduitOff;
        DrawLine(start, end, conduitColor, 5f);

        DrawCircle(end, 4f, conduitColor);
        DrawCircle(Vector2.Zero, 6f, HousingColor);
        DrawCircle(Vector2.Zero, 4f, IsAligned ? ConduitOn : BorderDefault);
    }
}
