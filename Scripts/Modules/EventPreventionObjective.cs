#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class EventPreventionObjective : Node2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "event-prevention-m11";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public float CountdownDuration { get; set; } = 10.0f;

    [Export]
    public int RequiredInterventions { get; set; } = 1;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsFailed => CurrentState == ModuleState.Failed;
    public float RemainingTime => _remainingTime;
    public int CompletedInterventions => _completedInterventions;

    public event Action<ModuleState>? StateChanged;
    public event Action? EventPrevented;
    public event Action? EventTriggered;

    private float _remainingTime;
    private int _completedInterventions;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color StableBorder = new("#10B981");
    private static readonly Color WarningBorder = new("#F59E0B");
    private static readonly Color ThreatBorder = new("#EF4444");

    public override void _Ready()
    {
        ResetState();
    }

    public override void _Process(double delta)
    {
        if (CurrentState != ModuleState.Active)
        {
            return;
        }

        _remainingTime -= (float)delta;
        QueueRedraw();

        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            CurrentState = ModuleState.Failed;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            EventTriggered?.Invoke();
        }
    }

    public void StartCountdown()
    {
        if (CurrentState is ModuleState.Completed or ModuleState.Failed)
        {
            return;
        }

        CurrentState = ModuleState.Active;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public void AddIntervention()
    {
        if (CurrentState is ModuleState.Completed or ModuleState.Failed)
        {
            return;
        }

        _completedInterventions++;

        if (_completedInterventions >= RequiredInterventions)
        {
            CurrentState = ModuleState.Completed;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            EventPrevented?.Invoke();
        }
        else
        {
            QueueRedraw();
        }
    }

    public void ResetState()
    {
        _remainingTime = CountdownDuration;
        _completedInterventions = 0;
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        var baseRect = new Rect2(-24f, -24f, 48f, 48f);
        DrawRect(baseRect, BaseColor);

        Color statusColor = CurrentState switch
        {
            ModuleState.Completed => StableBorder,
            ModuleState.Failed => ThreatBorder,
            ModuleState.Active => _remainingTime < CountdownDuration * 0.35f ? ThreatBorder : WarningBorder,
            _ => WarningBorder
        };

        DrawRect(baseRect, statusColor, false, 2f);

        if (CurrentState == ModuleState.Completed)
        {
            DrawCircle(Vector2.Zero, 8f, StableBorder);
            DrawArc(Vector2.Zero, 14f, 0f, Mathf.Tau, 24, StableBorder, 1.5f, true);
        }
        else if (CurrentState == ModuleState.Failed)
        {
            DrawLine(new Vector2(-10f, -10f), new Vector2(10f, 10f), ThreatBorder, 2.5f);
            DrawLine(new Vector2(-10f, 10f), new Vector2(10f, -10f), ThreatBorder, 2.5f);
        }
        else if (CountdownDuration > 0f)
        {
            float fraction = Mathf.Clamp(_remainingTime / CountdownDuration, 0f, 1f);
            DrawArc(Vector2.Zero, 14f, -Mathf.Pi / 2f, -Mathf.Pi / 2f + Mathf.Tau * fraction, 32, statusColor, 2.5f, true);
            DrawCircle(Vector2.Zero, 4f, statusColor);
        }
    }
}
