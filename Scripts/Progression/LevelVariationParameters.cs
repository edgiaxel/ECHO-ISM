#nullable enable
using System;
using Godot;

namespace ECHOISM;

public struct LevelVariationParameters
{
    public int Seed { get; set; }
    public string TemplateId { get; set; }
    public float LoopDurationSeconds { get; set; }
    public int MaximumActiveEchoes { get; set; }
    public TimingDifficultyPreset TimingPreset { get; set; }
    public float SyncToleranceSeconds { get; set; }
    public bool SolidEchoes { get; set; }
    public Vector2 MechanismOffset { get; set; }

    public static LevelVariationParameters CreateHandcrafted(string templateId, float loopDuration = 15f, int echoLimit = 3)
    {
        return new LevelVariationParameters
        {
            Seed = 0,
            TemplateId = templateId,
            LoopDurationSeconds = loopDuration,
            MaximumActiveEchoes = echoLimit,
            TimingPreset = TimingDifficultyPreset.Standard,
            SyncToleranceSeconds = 1.5f,
            SolidEchoes = true,
            MechanismOffset = Vector2.Zero
        };
    }

    public static bool ValidateSolvability(LevelVariationParameters p, out string reason)
    {
        if (p.LoopDurationSeconds < 8.0f)
        {
            reason = "Loop duration too short (< 8s) to allow human route traversal.";
            return false;
        }

        if (p.MaximumActiveEchoes < 1)
        {
            reason = "Echo limit must be at least 1 to allow past-attempt cooperation.";
            return false;
        }

        if (p.SyncToleranceSeconds < 0.25f)
        {
            reason = "Synchronization tolerance too strict (< 0.25s).";
            return false;
        }

        if (Mathf.Abs(p.MechanismOffset.X) > 150f || Mathf.Abs(p.MechanismOffset.Y) > 100f)
        {
            reason = "Mechanism offset places interactable outside solvable bounds.";
            return false;
        }

        reason = "Solvability parameters verified.";
        return true;
    }
}
