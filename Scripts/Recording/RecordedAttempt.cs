using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public struct RecordedFrame
{
    public float Timestamp;
    public Vector2 Position;
    public Vector2 Facing;
}

public struct RecordedInteraction
{
    public float Timestamp;
    public string TargetId;
}

public class RecordedAttempt
{
    public int AttemptIndex { get; set; }
    public float Duration { get; set; }
    public List<RecordedFrame> Frames { get; set; } = new();
    public List<RecordedInteraction> Interactions { get; set; } = new();

    public string GetSummary()
    {
        return $"Attempt #{AttemptIndex}: {Duration:0.00}s ({Frames.Count} frames, {Interactions.Count} interactions)";
    }
}
