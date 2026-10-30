using System;
using Godot;

namespace ECHOISM;

public partial class EchoActor : GregCharacter
{
    public int EchoIndex { get; private set; }
    public string EchoLabel { get; private set; } = "I";
    public RecordedAttempt Track { get; private set; }
    public bool IsPlaybackFinished { get; private set; }

    private int _nextInteractionIndex;
    private Label _romanTag;
    private CollisionShape2D _bodyCollider;

    public override void _Ready()
    {
        base._Ready();
        IsEcho = true;
        CollisionLayer = 1;
        CollisionMask = 1;

        _bodyCollider = GetNodeOrNull<CollisionShape2D>("BodyCollision");
        if (_bodyCollider is not null)
        {
            _bodyCollider.Disabled = false;
        }

        _romanTag = GetNodeOrNull<Label>("RomanTag");
        if (_romanTag is not null)
        {
            _romanTag.Text = EchoLabel;
        }
    }

    public void SetTrack(RecordedAttempt track, int echoIndex, string romanLabel)
    {
        Track = track;
        EchoIndex = echoIndex;
        EchoLabel = romanLabel;
        IsEcho = true;
        CollisionLayer = 1;
        CollisionMask = 1;

        if (_bodyCollider is not null)
        {
            _bodyCollider.Disabled = false;
        }

        if (_romanTag is not null)
        {
            _romanTag.Text = EchoLabel;
        }

        ResetPlayback();
    }

    public void ResetPlayback()
    {
        _nextInteractionIndex = 0;
        IsPlaybackFinished = false;

        if (Track is not null && Track.Frames.Count > 0)
        {
            GlobalPosition = Track.Frames[0].Position;
            _facing = Track.Frames[0].Facing;
        }

        QueueRedraw();
    }

    public void UpdatePlayback(float currentTime, Func<string, IInteractable> interactableResolver)
    {
        if (Track is null || Track.Frames.Count == 0)
        {
            return;
        }

        if (currentTime >= Track.Duration)
        {
            IsPlaybackFinished = true;
            var last = Track.Frames[^1];
            GlobalPosition = last.Position;
            _facing = last.Facing;
        }
        else
        {
            IsPlaybackFinished = false;
            var frame = SampleFrameAtTime(currentTime);
            GlobalPosition = frame.Position;
            _facing = frame.Facing;
        }

        ReplayPendingInteractions(currentTime, interactableResolver);
        QueueRedraw();
    }

    private RecordedFrame SampleFrameAtTime(float time)
    {
        var frames = Track.Frames;
        int low = 0;
        int high = frames.Count - 1;
        int best = 0;

        while (low <= high)
        {
            int mid = (low + high) / 2;
            if (frames[mid].Timestamp <= time)
            {
                best = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return frames[best];
    }

    private void ReplayPendingInteractions(float currentTime, Func<string, IInteractable> interactableResolver)
    {
        if (Track.Interactions is null)
        {
            return;
        }

        while (_nextInteractionIndex < Track.Interactions.Count && currentTime >= Track.Interactions[_nextInteractionIndex].Timestamp)
        {
            var interaction = Track.Interactions[_nextInteractionIndex++];
            var target = interactableResolver?.Invoke(interaction.TargetId);

            if (target is null)
            {
                GD.Print($"[Echo {EchoLabel}] Target '{interaction.TargetId}' missing or disabled. Safely skipped.");
                continue;
            }

            bool success = target.TryInteract(this);
            GD.Print($"[Echo {EchoLabel}] Replayed interaction on '{interaction.TargetId}' at t={currentTime:0.00}s (Success: {success})");
        }
    }
}
