#nullable enable
using System;
using Godot;

namespace ECHOISM;

public enum ObstacleMovementType
{
    PingPong,
    Loop,
    OneWay
}

public partial class MovingObstacle : AnimatableBody2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "moving-obstacle-m06";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public Vector2 TargetOffset { get; set; } = new(100f, 0f);

    [Export]
    public float CycleDuration { get; set; } = 3.0f;

    [Export]
    public ObstacleMovementType MovementType { get; set; } = ObstacleMovementType.PingPong;

    [Export]
    public bool IsMovingEnabled { get; set; } = true;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;

    public event Action<ModuleState>? StateChanged;

    private Vector2 _initialPosition;
    private float _timeAccumulator;

    private static readonly Color ObstacleBody = new("#0E1626");
    private static readonly Color ObstacleBorder = new("#EF4444");
    private static readonly Color HazardStripe = new("#F59E0B");

    public override void _Ready()
    {
        SyncToPhysics = false;
        _initialPosition = Position;
        SyncState();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsMovingEnabled || CycleDuration <= 0f)
        {
            return;
        }

        _timeAccumulator += (float)delta;
        float progress = (_timeAccumulator % CycleDuration) / CycleDuration;

        float t = MovementType switch
        {
            ObstacleMovementType.PingPong => 0.5f * (1f - Mathf.Cos(progress * Mathf.Tau)),
            ObstacleMovementType.OneWay => Mathf.Clamp(_timeAccumulator / CycleDuration, 0f, 1f),
            _ => progress
        };

        Position = _initialPosition + TargetOffset * t;
        SyncState();
    }

    public void SetMovingEnabled(bool enabled)
    {
        if (IsMovingEnabled == enabled)
        {
            return;
        }

        IsMovingEnabled = enabled;
        SyncState();
    }

    private void SyncState()
    {
        var newState = IsMovingEnabled ? ModuleState.Active : ModuleState.Idle;
        if (CurrentState != newState)
        {
            CurrentState = newState;
            StateChanged?.Invoke(CurrentState);
        }
    }

    public void ResetState()
    {
        _timeAccumulator = 0f;
        Position = _initialPosition;
        CurrentState = ModuleState.Idle;
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        var rect = new Rect2(-20f, -20f, 40f, 40f);
        DrawRect(rect, ObstacleBody);
        DrawRect(rect, ObstacleBorder, false, 2.5f);

        DrawLine(new Vector2(-15f, -10f), new Vector2(-5f, 10f), HazardStripe, 2.5f);
        DrawLine(new Vector2(-5f, -10f), new Vector2(5f, 10f), HazardStripe, 2.5f);
        DrawLine(new Vector2(5f, -10f), new Vector2(15f, 10f), HazardStripe, 2.5f);
    }
}
