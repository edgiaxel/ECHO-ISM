#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class SignalRelay : Area2D, IInteractable, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "signal-relay-m08";

    public string StableId => ModuleId;

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    public bool AcceptsEchoes => (AcceptedActors & AcceptedActors.Echo) != 0;

    [Export]
    public float TransmissionDelaySeconds { get; set; } = 0.2f;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsTransmitting => CurrentState is ModuleState.Active or ModuleState.Completed;

    public event Action<ModuleState>? StateChanged;
    public event Action? SignalTransmitted;

    private float _delayTimer;
    private bool _pendingTransmission;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color BorderIdle = new("#334155");
    private static readonly Color BeamInactive = new("#334155");
    private static readonly Color BeamActive = new("#00D2FF");
    private static readonly Color CoreActive = new("#10B981");

    public override void _Process(double delta)
    {
        if (_pendingTransmission)
        {
            _delayTimer -= (float)delta;
            if (_delayTimer <= 0f)
            {
                _pendingTransmission = false;
                CurrentState = ModuleState.Completed;
                QueueRedraw();
                StateChanged?.Invoke(CurrentState);
                SignalTransmitted?.Invoke();
            }
        }
    }

    public string GetPrompt(GregCharacter actor)
    {
        return IsTransmitting ? "Relay transmitting" : "Activate signal relay";
    }

    public bool TryInteract(GregCharacter actor)
    {
        if (!ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            return false;
        }

        ReceiveSignal();
        return true;
    }

    public void ReceiveSignal()
    {
        if (IsTransmitting)
        {
            return;
        }

        if (TransmissionDelaySeconds > 0f)
        {
            _pendingTransmission = true;
            _delayTimer = TransmissionDelaySeconds;
            CurrentState = ModuleState.Active;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
        }
        else
        {
            CurrentState = ModuleState.Completed;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            SignalTransmitted?.Invoke();
        }
    }

    public void ResetState()
    {
        _pendingTransmission = false;
        _delayTimer = 0f;
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        bool active = IsTransmitting;
        var borderColor = active ? BeamActive : BorderIdle;
        var coreColor = active ? CoreActive : BeamInactive;

        var baseRect = new Rect2(-16f, -16f, 32f, 32f);
        DrawRect(baseRect, BaseColor);
        DrawRect(baseRect, borderColor, false, 2f);

        DrawCircle(Vector2.Zero, active ? 6f : 4f, coreColor);
        DrawArc(Vector2.Zero, 12f, 0f, Mathf.Tau, 16, borderColor, 1.5f, true);
        if (active)
        {
            DrawArc(Vector2.Zero, 16f, 0f, Mathf.Tau, 16, new Color(borderColor, 0.4f), 1f, true);
        }
    }
}
