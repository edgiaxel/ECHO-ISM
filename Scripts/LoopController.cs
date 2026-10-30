using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public enum LoopState
{
    Ready,
    Active,
    Committing,
    Resetting,
    Completed
}

public partial class LoopController : Node
{
    public LoopState CurrentState { get; private set; } = LoopState.Ready;

    public float LoopDuration { get; private set; } = 15f;
    public float CurrentTime { get; private set; }
    public int CurrentAttempt { get; private set; } = 1;
    public int MaxEchoes { get; private set; } = 3;

    public IReadOnlyList<RecordedAttempt> CommittedRecordings => _committedRecordings;

    public event Action<float, float> TimerTick;
    public event Action<LoopState> StateChanged;
    public event Action<RecordedAttempt> AttemptCommitted;
    public event Action LoopReset;

    private readonly List<RecordedAttempt> _committedRecordings = new();
    private RecordedAttempt _currentAttempt;

    private LevelContainer _level;
    private GregCharacter _greg;
    private Vector2 _startPosition;

    public void Initialize(LevelContainer level)
    {
        _level = level;
        LoopDuration = level.LoopDurationSeconds > 0 ? level.LoopDurationSeconds : 15f;
        MaxEchoes = level.MaximumActiveEchoes > 0 ? level.MaximumActiveEchoes : 3;
        _startPosition = level.StartingPosition;

        _greg = level.GetNode<GregCharacter>("Greg");
        _greg.InteractionPerformed += OnInteractionPerformed;

        ConfigureResetInput();
        StartNewAttempt();
        SetState(LoopState.Active);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState != LoopState.Active || _greg is null)
        {
            return;
        }

        if (Input.IsActionJustPressed("reset_loop"))
        {
            CommitAndReset("Manual Reset");
            return;
        }

        RecordCurrentFrame();

        CurrentTime += (float)delta;
        TimerTick?.Invoke(CurrentTime, LoopDuration);

        if (CurrentTime >= LoopDuration)
        {
            CommitAndReset("Timer Expired");
        }
    }

    public void CommitAndReset(string reason)
    {
        if (CurrentState != LoopState.Active)
        {
            return;
        }

        SetState(LoopState.Committing);

        _currentAttempt.Duration = CurrentTime;

        if (_committedRecordings.Count >= MaxEchoes)
        {
            _committedRecordings.RemoveAt(0);
        }
        _committedRecordings.Add(_currentAttempt);
        AttemptCommitted?.Invoke(_currentAttempt);

        GD.Print($"[LoopController] {_currentAttempt.GetSummary()} committed ({reason}). Total pool: {_committedRecordings.Count}/{MaxEchoes}");

        SetState(LoopState.Resetting);

        _greg.ResetTo(_startPosition, Vector2.Down);
        ResetLevelMechanisms();

        CurrentAttempt++;
        CurrentTime = 0f;
        StartNewAttempt();

        LoopReset?.Invoke();
        SetState(LoopState.Active);
    }

    public void CompleteLevel()
    {
        if (CurrentState == LoopState.Completed)
        {
            return;
        }

        SetState(LoopState.Completed);
        GD.Print($"[LoopController] Level completed on attempt {CurrentAttempt}!");
    }

    public void FastForwardTimeForTesting(float time)
    {
        CurrentTime = time;
    }

    public string GetRecordedTracksSummary()
    {
        var lines = new List<string>
        {
            $"Loop State: {CurrentState} | Attempt: {CurrentAttempt} | Time: {CurrentTime:0.00}/{LoopDuration:0.00}s",
            $"Committed Echo Pool ({_committedRecordings.Count}/{MaxEchoes}):"
        };

        for (int i = 0; i < _committedRecordings.Count; i++)
        {
            lines.Add($"  Echo {i + 1}: {_committedRecordings[i].GetSummary()}");
        }

        lines.Add($"Current Live Attempt: {_currentAttempt.Frames.Count} frames, {_currentAttempt.Interactions.Count} interactions");
        return string.Join("\n", lines);
    }

    private void StartNewAttempt()
    {
        _currentAttempt = new RecordedAttempt
        {
            AttemptIndex = CurrentAttempt,
            Duration = 0f
        };
    }

    private void RecordCurrentFrame()
    {
        _currentAttempt.Frames.Add(new RecordedFrame
        {
            Timestamp = CurrentTime,
            Position = _greg.GlobalPosition,
            Facing = _greg.Facing
        });
    }

    private void OnInteractionPerformed(string targetId)
    {
        if (CurrentState != LoopState.Active)
        {
            return;
        }

        _currentAttempt.Interactions.Add(new RecordedInteraction
        {
            Timestamp = CurrentTime,
            TargetId = targetId
        });
        GD.Print($"[LoopController] Recorded interaction with '{targetId}' at t={CurrentTime:0.00}s");
    }

    private void ResetLevelMechanisms()
    {
        if (_level is null)
        {
            return;
        }

        ResetResettablesRecursive(_level);
    }

    private static void ResetResettablesRecursive(Node node)
    {
        if (node is IResettable resettable)
        {
            resettable.ResetState();
        }

        foreach (Node child in node.GetChildren())
        {
            ResetResettablesRecursive(child);
        }
    }

    private void SetState(LoopState state)
    {
        if (CurrentState == state)
        {
            return;
        }

        CurrentState = state;
        StateChanged?.Invoke(state);
    }

    private static void ConfigureResetInput()
    {
        const string action = "reset_loop";
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        var input = new InputEventKey { PhysicalKeycode = Key.R };
        if (!InputMap.ActionHasEvent(action, input))
        {
            InputMap.ActionAddEvent(action, input);
        }
    }
}
