#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class EchoSensor : Area2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "echo-sensor-m09";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.Echo;

    [Export]
    public int RequiredEchoCount { get; set; } = 1;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public int DetectedCount => _occupants.Count;

    public event Action<ModuleState>? StateChanged;
    public event Action<bool>? ActiveChanged;

    private readonly HashSet<GregCharacter> _occupants = [];

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color RingIdle = new("#334155");
    private static readonly Color RingActive = new("#818CF8");
    private static readonly Color CoreActive = new("#38BDF8");

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is GregCharacter actor && ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            _occupants.Add(actor);
            EvaluateDetection();
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is GregCharacter actor)
        {
            _occupants.Remove(actor);
            EvaluateDetection();
        }
    }

    private void EvaluateDetection()
    {
        bool detected = _occupants.Count >= RequiredEchoCount;
        var newState = detected ? ModuleState.Completed : (_occupants.Count > 0 ? ModuleState.Active : ModuleState.Idle);

        if (CurrentState != newState)
        {
            CurrentState = newState;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            ActiveChanged?.Invoke(detected);
        }
        else
        {
            QueueRedraw();
        }
    }

    public void ResetState()
    {
        _occupants.Clear();
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        ActiveChanged?.Invoke(false);
    }

    public override void _Draw()
    {
        bool complete = IsCompleted;
        var ringColor = complete ? RingActive : RingIdle;
        var coreColor = complete ? CoreActive : RingIdle;

        DrawCircle(Vector2.Zero, 24f, BaseColor);
        DrawArc(Vector2.Zero, 24f, 0f, Mathf.Tau, 32, ringColor, 2f, true);
        DrawArc(Vector2.Zero, 16f, 0f, Mathf.Tau, 24, new Color(ringColor, 0.6f), 1.5f, true);
        DrawArc(Vector2.Zero, 8f, 0f, Mathf.Tau, 16, coreColor, 2f, true);
        DrawCircle(Vector2.Zero, 4f, coreColor);
    }
}
