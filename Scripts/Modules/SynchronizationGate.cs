#nullable enable
using System;
using Godot;

namespace ECHOISM;

public enum SyncTolerancePreset
{
    Forgiving,
    Standard,
    Demanding
}

public partial class SynchronizationGate : Node2D, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "sync-gate-m05";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public float ToleranceSeconds { get; set; } = 1.5f;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;

    public bool HasSignalA { get; private set; }
    public bool HasSignalB { get; private set; }
    public float RemainingTolerance => _remainingTolerance;

    public event Action<ModuleState>? StateChanged;
    public event Action? GateSynchronized;
    public event Action? SyncFailed;

    private float _remainingTolerance;
    private float _flashTimer;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color BorderIdle = new("#334155");
    private static readonly Color ReceiverAColor = new("#00D2FF");
    private static readonly Color ReceiverBColor = new("#818CF8");
    private static readonly Color SyncActiveColor = new("#10B981");
    private static readonly Color TimeoutColor = new("#EF4444");

    public override void _Ready()
    {
        SetProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        StepSimulation((float)delta);
    }

    public override void _Process(double delta)
    {
        StepSimulation((float)delta);
    }

    public void StepSimulation(float delta)
    {
        if (CurrentState == ModuleState.Active && _remainingTolerance > 0f)
        {
            _remainingTolerance -= delta;
            QueueRedraw();

            if (_remainingTolerance <= 0f)
            {
                _remainingTolerance = 0f;
                HasSignalA = false;
                HasSignalB = false;
                CurrentState = ModuleState.Idle;
                QueueRedraw();
                StateChanged?.Invoke(CurrentState);
                SyncFailed?.Invoke();
            }
        }
    }

    public void ApplyPreset(SyncTolerancePreset preset)
    {
        ToleranceSeconds = preset switch
        {
            SyncTolerancePreset.Forgiving => 2.5f,
            SyncTolerancePreset.Demanding => 0.5f,
            SyncTolerancePreset.Standard => 1.5f,
            _ => 1.5f
        };
        QueueRedraw();
    }

    public void ReceiveSignalA()
    {
        if (IsCompleted)
        {
            return;
        }

        HasSignalA = true;
        EvaluateSignals();
    }

    public void ReceiveSignalB()
    {
        if (IsCompleted)
        {
            return;
        }

        HasSignalB = true;
        EvaluateSignals();
    }

    private void EvaluateSignals()
    {
        if (HasSignalA && HasSignalB)
        {
            CurrentState = ModuleState.Completed;
            _remainingTolerance = 0f;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            GateSynchronized?.Invoke();
            return;
        }

        if (CurrentState != ModuleState.Active)
        {
            CurrentState = ModuleState.Active;
            _remainingTolerance = ToleranceSeconds;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
        }
    }

    public void ResetState()
    {
        HasSignalA = false;
        HasSignalB = false;
        _remainingTolerance = 0f;
        _flashTimer = 0f;
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        var coreRect = new Rect2(-24f, -24f, 48f, 48f);
        DrawRect(coreRect, BaseColor);

        Color coreBorder = CurrentState switch
        {
            ModuleState.Completed => SyncActiveColor,
            ModuleState.Failed => TimeoutColor,
            ModuleState.Active => ReceiverAColor,
            _ => BorderIdle
        };
        DrawRect(coreRect, coreBorder, false, 2f);

        // Receiver A (left node)
        var colorA = HasSignalA ? ReceiverAColor : BorderIdle;
        DrawCircle(new Vector2(-36f, 0f), 7f, BaseColor);
        DrawCircle(new Vector2(-36f, 0f), 7f, colorA);
        DrawLine(new Vector2(-29f, 0f), new Vector2(-24f, 0f), colorA, 2f);

        // Receiver B (right node)
        var colorB = HasSignalB ? ReceiverBColor : BorderIdle;
        DrawCircle(new Vector2(36f, 0f), 7f, BaseColor);
        DrawCircle(new Vector2(36f, 0f), 7f, colorB);
        DrawLine(new Vector2(24f, 0f), new Vector2(29f, 0f), colorB, 2f);

        // Center sync status and timing countdown cue
        if (IsCompleted)
        {
            DrawCircle(Vector2.Zero, 10f, SyncActiveColor);
            DrawArc(Vector2.Zero, 15f, 0f, Mathf.Tau, 24, SyncActiveColor, 1.5f, true);
        }
        else if (CurrentState == ModuleState.Active && ToleranceSeconds > 0f)
        {
            float fraction = Mathf.Clamp(_remainingTolerance / ToleranceSeconds, 0f, 1f);
            Color arcColor = _remainingTolerance < 0.5f ? TimeoutColor : ReceiverAColor;
            DrawArc(Vector2.Zero, 13f, -Mathf.Pi / 2f, -Mathf.Pi / 2f + Mathf.Tau * fraction, 32, arcColor, 2.5f, true);

            var font = ThemeDB.FallbackFont;
            int fontSize = 10;
            string cueText = $"{_remainingTolerance:0.0}s";
            var textSize = font.GetStringSize(cueText, HorizontalAlignment.Center, -1, fontSize);
            DrawString(font, new Vector2(-textSize.X / 2f, 22f), cueText, HorizontalAlignment.Center, -1, fontSize, arcColor);
        }
        else if (CurrentState == ModuleState.Failed)
        {
            DrawLine(new Vector2(-8f, -8f), new Vector2(8f, 8f), TimeoutColor, 2f);
            DrawLine(new Vector2(-8f, 8f), new Vector2(8f, -8f), TimeoutColor, 2f);
        }
        else
        {
            DrawCircle(Vector2.Zero, 5f, BorderIdle);
        }
    }
}
