#nullable enable
using System;
using Godot;

namespace ECHOISM;

public enum TimingEvaluation
{
    None,
    Early,
    Accurate,
    Late
}

public enum TimingDifficultyPreset
{
    Forgiving,
    Standard,
    Demanding
}

public partial class TimingSlider : Area2D, IInteractable, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "timing-slider-m04";

    public string StableId => ModuleId;

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    public bool AcceptsEchoes => (AcceptedActors & AcceptedActors.Echo) != 0;

    [Export]
    public float SliderSpeed { get; set; } = 1.0f;

    [Export]
    public float TargetStart { get; set; } = 0.40f;

    [Export]
    public float TargetEnd { get; set; } = 0.60f;

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;

    public float MarkerPosition { get; private set; } = 0.0f;
    public TimingEvaluation LastEvaluation { get; private set; } = TimingEvaluation.None;
    public float TimingDelta { get; private set; } = 0.0f;
    public bool IsStopped => _isStopped;

    public event Action<ModuleState>? StateChanged;
    public event Action<TimingEvaluation>? Evaluated;
    public event Action<float>? TimingDeltaReported;

    private float _direction = 1.0f;
    private bool _isStopped;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color BorderColor = new("#334155");
    private static readonly Color TargetZoneColor = new(0.06f, 0.72f, 0.51f, 0.4f);
    private static readonly Color TargetBorder = new("#10B981");
    private static readonly Color MarkerIdle = new("#00D2FF");
    private static readonly Color EarlyColor = new("#F59E0B");
    private static readonly Color LateColor = new("#EF4444");
    private static readonly Color AccurateColor = new("#10B981");
    private static readonly Color TextColor = new("#F8FAFC");

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
        if (_isStopped)
        {
            return;
        }

        MarkerPosition += delta * SliderSpeed * _direction;
        if (MarkerPosition >= 1.0f)
        {
            float overshoot = MarkerPosition - 1.0f;
            MarkerPosition = Mathf.Clamp(1.0f - overshoot, 0.0f, 1.0f);
            _direction = -1.0f;
        }
        else if (MarkerPosition <= 0.0f)
        {
            float undershoot = -MarkerPosition;
            MarkerPosition = Mathf.Clamp(undershoot, 0.0f, 1.0f);
            _direction = 1.0f;
        }

        QueueRedraw();
    }

    public void ApplyPreset(TimingDifficultyPreset preset)
    {
        switch (preset)
        {
            case TimingDifficultyPreset.Forgiving:
                TargetStart = 0.30f;
                TargetEnd = 0.70f;
                SliderSpeed = 0.8f;
                break;
            case TimingDifficultyPreset.Demanding:
                TargetStart = 0.45f;
                TargetEnd = 0.55f;
                SliderSpeed = 1.4f;
                break;
            case TimingDifficultyPreset.Standard:
            default:
                TargetStart = 0.40f;
                TargetEnd = 0.60f;
                SliderSpeed = 1.0f;
                break;
        }
        QueueRedraw();
    }

    public void SetTargetZone(float start, float end, float speed = 1.0f)
    {
        TargetStart = Mathf.Clamp(start, 0f, 1f);
        TargetEnd = Mathf.Clamp(end, 0f, 1f);
        SliderSpeed = Mathf.Max(0f, speed);
        QueueRedraw();
    }

    public string GetPrompt(GregCharacter actor)
    {
        if (_isStopped)
        {
            return LastEvaluation switch
            {
                TimingEvaluation.Accurate => "Timing locked (Accurate)",
                TimingEvaluation.Early => $"Early by {Mathf.Abs(TimingDelta) * 100f:0}% - press to retry",
                TimingEvaluation.Late => $"Late by {Mathf.Abs(TimingDelta) * 100f:0}% - press to retry",
                _ => "Press to restart slider"
            };
        }
        return "Calibrate timing";
    }

    public string GetFeedbackSummary()
    {
        return LastEvaluation switch
        {
            TimingEvaluation.Accurate => "ACCURATE: Target calibrated and locked",
            TimingEvaluation.Early => $"EARLY: Stopped {Mathf.Abs(TimingDelta) * 100f:0.0}% before target zone",
            TimingEvaluation.Late => $"LATE: Stopped {Mathf.Abs(TimingDelta) * 100f:0.0}% after target zone",
            _ => "IDLE: Marker oscillating"
        };
    }

    public bool TryInteract(GregCharacter actor)
    {
        if (!ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            return false;
        }

        if (_isStopped && CurrentState != ModuleState.Completed)
        {
            _isStopped = false;
            LastEvaluation = TimingEvaluation.None;
            TimingDelta = 0.0f;
            CurrentState = ModuleState.Idle;
            QueueRedraw();
            StateChanged?.Invoke(CurrentState);
            return true;
        }

        if (_isStopped)
        {
            return false;
        }

        _isStopped = true;

        if (MarkerPosition < TargetStart)
        {
            LastEvaluation = TimingEvaluation.Early;
            TimingDelta = MarkerPosition - TargetStart;
            CurrentState = ModuleState.Failed;
        }
        else if (MarkerPosition > TargetEnd)
        {
            LastEvaluation = TimingEvaluation.Late;
            TimingDelta = MarkerPosition - TargetEnd;
            CurrentState = ModuleState.Failed;
        }
        else
        {
            LastEvaluation = TimingEvaluation.Accurate;
            TimingDelta = 0.0f;
            CurrentState = ModuleState.Completed;
        }

        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        Evaluated?.Invoke(LastEvaluation);
        TimingDeltaReported?.Invoke(TimingDelta);
        return true;
    }

    public void ResetState()
    {
        MarkerPosition = 0.0f;
        _direction = 1.0f;
        _isStopped = false;
        LastEvaluation = TimingEvaluation.None;
        TimingDelta = 0.0f;
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        const float width = 96f;
        const float height = 20f;
        var trackRect = new Rect2(-width / 2f, -height / 2f, width, height);

        DrawRect(trackRect, BaseColor);
        DrawRect(trackRect, BorderColor, false, 2f);

        float targetLeft = -width / 2f + TargetStart * width;
        float targetWidth = Mathf.Max(2f, (TargetEnd - TargetStart) * width);
        var targetRect = new Rect2(targetLeft, -height / 2f + 2f, targetWidth, height - 4f);
        DrawRect(targetRect, TargetZoneColor);
        DrawRect(targetRect, TargetBorder, false, 1.5f);

        float markerX = -width / 2f + MarkerPosition * width;
        Color markerColor = LastEvaluation switch
        {
            TimingEvaluation.Early => EarlyColor,
            TimingEvaluation.Late => LateColor,
            TimingEvaluation.Accurate => AccurateColor,
            _ => MarkerIdle
        };

        DrawLine(new Vector2(markerX, -height / 2f - 4f), new Vector2(markerX, height / 2f + 4f), markerColor, 3f);
        DrawCircle(new Vector2(markerX, -height / 2f - 5f), 3.5f, markerColor);

        // Feedback label indicator drawn below the track
        if (_isStopped)
        {
            string feedbackText = LastEvaluation switch
            {
                TimingEvaluation.Accurate => "ACCURATE",
                TimingEvaluation.Early => $"EARLY ({-TimingDelta * 100f:0}%)",
                TimingEvaluation.Late => $"LATE (+{TimingDelta * 100f:0}%)",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(feedbackText))
            {
                var font = ThemeDB.FallbackFont;
                int fontSize = 11;
                var textSize = font.GetStringSize(feedbackText, HorizontalAlignment.Center, -1, fontSize);
                DrawString(font, new Vector2(-textSize.X / 2f, height / 2f + 16f), feedbackText, HorizontalAlignment.Center, -1, fontSize, markerColor);
            }
        }
    }
}
