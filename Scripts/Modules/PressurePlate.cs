#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public enum PressurePlateMode
{
    Momentary,
    Toggle,
    TimedRelease
}

public partial class PressurePlate : Area2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "pressure-plate-m01";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public PressurePlateMode Mode { get; set; } = PressurePlateMode.Momentary;

    [Export]
    public float ReleaseDelaySeconds { get; set; } = 1.5f;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState is ModuleState.Active or ModuleState.Completed;

    public event Action<ModuleState>? StateChanged;
    public event Action<bool>? ActiveChanged;

    private readonly HashSet<GregCharacter> _occupants = [];
    private float _releaseTimer;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color BorderIdle = new("#334155");
    private static readonly Color BorderActive = new("#00D2FF");
    private static readonly Color CoreIdle = new("#F59E0B");
    private static readonly Color CoreActive = new("#10B981");

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (Mode == PressurePlateMode.TimedRelease && _releaseTimer > 0f)
        {
            _releaseTimer -= (float)delta;
            if (_releaseTimer <= 0f && _occupants.Count == 0)
            {
                SetState(ModuleState.Idle);
            }
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is GregCharacter actor && ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            _occupants.Add(actor);
            _releaseTimer = 0f;

            if (Mode == PressurePlateMode.Toggle)
            {
                SetState(CurrentState == ModuleState.Idle ? ModuleState.Active : ModuleState.Idle);
            }
            else
            {
                SetState(ModuleState.Active);
            }
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is GregCharacter actor)
        {
            _occupants.Remove(actor);

            if (_occupants.Count == 0)
            {
                if (Mode == PressurePlateMode.Momentary)
                {
                    SetState(ModuleState.Idle);
                }
                else if (Mode == PressurePlateMode.TimedRelease)
                {
                    _releaseTimer = ReleaseDelaySeconds;
                }
            }
        }
    }

    private void SetState(ModuleState state)
    {
        if (CurrentState == state)
        {
            return;
        }

        CurrentState = state;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        ActiveChanged?.Invoke(IsCompleted);
    }

    public void ResetState()
    {
        _occupants.Clear();
        _releaseTimer = 0f;
        SetState(ModuleState.Idle);
    }

    public override void _Draw()
    {
        bool active = IsCompleted;
        var borderColor = active ? BorderActive : BorderIdle;
        var coreColor = active ? CoreActive : CoreIdle;

        var outerRect = new Rect2(-20f, -20f, 40f, 40f);
        DrawRect(outerRect, BaseColor);
        DrawRect(outerRect, borderColor, false, 2f);

        var innerRect = active ? new Rect2(-13f, -13f, 26f, 26f) : new Rect2(-15f, -15f, 30f, 30f);
        DrawRect(innerRect, BaseColor);
        DrawRect(innerRect, borderColor, false, 1.5f);

        DrawCircle(Vector2.Zero, active ? 6f : 4f, coreColor);
        DrawArc(Vector2.Zero, 10f, 0f, Mathf.Tau, 16, new Color(borderColor, 0.4f), 1f, true);
    }
}
