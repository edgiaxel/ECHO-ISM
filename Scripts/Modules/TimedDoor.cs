#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class TimedDoor : StaticBody2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "timed-door-m03";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public float WindowDurationSeconds { get; set; } = 3.0f;

    [Export]
    public bool IsPeriodic { get; set; }

    [Export]
    public float PeriodSeconds { get; set; } = 6.0f;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsOpen => IsCompleted;
    public float RemainingWindowTime => _remainingTime;

    public event Action<ModuleState>? StateChanged;
    public event Action<bool>? DoorOpened;

    private CollisionShape2D? _collider;
    private float _remainingTime;
    private float _periodicTimer;

    private static readonly Color ClosedBodyColor = new("#0E1626");
    private static readonly Color ClosedBorderColor = new("#334155");
    private static readonly Color TimerColor = new("#00D2FF");
    private static readonly Color OpenEnergyColor = new(0.06f, 0.72f, 0.51f, 0.22f);
    private static readonly Color OpenLightColor = new("#10B981");

    public override void _Ready()
    {
        _collider = GetNodeOrNull<CollisionShape2D>("CollisionShape");
        SetOpen(false);
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (IsOpen)
        {
            _remainingTime -= dt;
            QueueRedraw();

            if (_remainingTime <= 0f)
            {
                SetOpen(false);
            }
        }
        else if (IsPeriodic)
        {
            _periodicTimer += dt;
            if (_periodicTimer >= PeriodSeconds)
            {
                _periodicTimer = 0f;
                TriggerOpen();
            }
        }
    }

    public void TriggerOpen()
    {
        _remainingTime = WindowDurationSeconds;
        SetOpen(true);
    }

    public void SetOpen(bool open)
    {
        var newState = open ? ModuleState.Completed : ModuleState.Idle;
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        _collider?.SetDeferred(CollisionShape2D.PropertyName.Disabled, open);
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        DoorOpened?.Invoke(open);
    }

    public void ResetState()
    {
        _remainingTime = 0f;
        _periodicTimer = 0f;
        SetOpen(false);
    }

    public override void _Draw()
    {
        var rect = new Rect2(-10f, -44f, 20f, 88f);

        if (!IsOpen)
        {
            DrawRect(rect, ClosedBodyColor);
            DrawRect(rect, ClosedBorderColor, false, 2f);
            DrawLine(new Vector2(-8f, 0f), new Vector2(8f, 0f), ClosedBorderColor, 2f);
        }
        else
        {
            DrawRect(rect, OpenEnergyColor);
            DrawRect(new Rect2(-10f, -44f, 4f, 88f), OpenLightColor);
            DrawRect(new Rect2(6f, -44f, 4f, 88f), OpenLightColor);

            if (WindowDurationSeconds > 0f)
            {
                float fraction = Mathf.Clamp(_remainingTime / WindowDurationSeconds, 0f, 1f);
                float timerHeight = 84f * fraction;
                var timerRect = new Rect2(-2f, 42f - timerHeight, 4f, timerHeight);
                DrawRect(timerRect, TimerColor);
            }
        }
    }
}
