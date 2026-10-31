#nullable enable
using System;
using Godot;

namespace ECHOISM;

public static class LevelRemixer
{
    public static LevelVariationParameters GenerateRemix(string templateId, int seed)
    {
        var rng = new Random(seed);

        float loopDuration = 12.0f + (float)rng.NextDouble() * 6.0f; // 12.0s to 18.0s
        int echoLimit = rng.Next(2, 4); // 2 or 3 echoes
        var timingPreset = (TimingDifficultyPreset)rng.Next(0, 3);
        float syncTolerance = 1.0f + (float)rng.NextDouble() * 1.5f; // 1.0s to 2.5s
        float offsetX = ((float)rng.NextDouble() * 2f - 1f) * 40f;
        float offsetY = ((float)rng.NextDouble() * 2f - 1f) * 30f;

        var parameters = new LevelVariationParameters
        {
            Seed = seed,
            TemplateId = templateId,
            LoopDurationSeconds = (float)Math.Round(loopDuration, 1),
            MaximumActiveEchoes = echoLimit,
            TimingPreset = timingPreset,
            SyncToleranceSeconds = (float)Math.Round(syncTolerance, 2),
            SolidEchoes = true,
            MechanismOffset = new Vector2(offsetX, offsetY)
        };

        if (!LevelVariationParameters.ValidateSolvability(parameters, out _))
        {
            // Safe fallback
            return LevelVariationParameters.CreateHandcrafted(templateId);
        }

        return parameters;
    }
}
