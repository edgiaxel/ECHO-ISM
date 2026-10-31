#nullable enable
using System;
using Godot;

namespace ECHOISM;

public enum DualMechanismMode
{
    Simultaneous,
    Sequential
}

public partial class DualMechanismDoor : StaticBody2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "dual-door-m02";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public DualMechanismMode Mode { get; set; } = DualMechanismMode.Simultaneous;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsOpen => IsCompleted;

    public bool IsInputAActive { get; private set; }
    public bool IsInputBActive { get; private set; }

    public event Action<ModuleState>? StateChanged;
    public event Action<bool>? DoorOpened;

    private CollisionShape2D? _collider;
    private bool _sequenceACompleted;

    private static readonly Color ClosedBodyColor = new("#0E1626");
    private static readonly Color ClosedBorderColor = new("#334155");
    private static readonly Color LockInactive = new("#F59E0B");
    private static readonly Color LockActive = new("#10B981");
    private static readonly Color OpenEnergyColor = new(0.06f, 0.72f, 0.51f, 0.22f);
    private static readonly Color OpenLightColor = new("#10B981");

    public override void _Ready()
    {
        _collider = GetNodeOrNull<CollisionShape2D>("CollisionShape");
        UpdateEvaluation();
    }

    public void SetInputA(bool active)
    {
        if (IsInputAActive == active)
        {
            return;
        }

        IsInputAActive = active;
        if (Mode == DualMechanismMode.Sequential && active && !_sequenceACompleted)
        {
            _sequenceACompleted = true;
        }

        UpdateEvaluation();
    }

    public void SetInputB(bool active)
    {
        if (IsInputBActive == active)
        {
            return;
        }

        IsInputBActive = active;
        UpdateEvaluation();
    }

    private void UpdateEvaluation()
    {
        bool shouldOpen = false;

        if (Mode == DualMechanismMode.Simultaneous)
        {
            shouldOpen = IsInputAActive && IsInputBActive;
        }
        else if (Mode == DualMechanismMode.Sequential)
        {
            shouldOpen = _sequenceACompleted && IsInputBActive;
        }

        var newState = shouldOpen ? ModuleState.Completed : (IsInputAActive || IsInputBActive ? ModuleState.Active : ModuleState.Idle);

        if (CurrentState != newState)
        {
            CurrentState = newState;
            _collider?.SetDeferred(CollisionShape2D.PropertyName.Disabled, shouldOpen);
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            DoorOpened?.Invoke(shouldOpen);
        }
        else
        {
            QueueRedraw();
        }
    }

    public void ResetState()
    {
        IsInputAActive = false;
        IsInputBActive = false;
        _sequenceACompleted = false;
        CurrentState = ModuleState.Idle;
        _collider?.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        DoorOpened?.Invoke(false);
    }

    public override void _Draw()
    {
        var rect = new Rect2(-10f, -48f, 20f, 96f);

        if (!IsOpen)
        {
            DrawRect(rect, ClosedBodyColor);
            DrawRect(rect, ClosedBorderColor, false, 2f);

            var lockAColor = IsInputAActive ? LockActive : LockInactive;
            var lockBColor = IsInputBActive ? LockActive : LockInactive;

            DrawCircle(new Vector2(0f, -18f), 4.5f, lockAColor);
            DrawCircle(new Vector2(0f, 18f), 4.5f, lockBColor);

            DrawLine(new Vector2(-8f, 0f), new Vector2(8f, 0f), ClosedBorderColor, 2f);
        }
        else
        {
            DrawRect(rect, OpenEnergyColor);
            DrawRect(new Rect2(-10f, -48f, 4f, 96f), OpenLightColor);
            DrawRect(new Rect2(6f, -48f, 4f, 96f), OpenLightColor);

            DrawCircle(new Vector2(0f, -18f), 4.5f, LockActive);
            DrawCircle(new Vector2(0f, 18f), 4.5f, LockActive);
        }
    }
}
