#nullable enable
using System;
using System.Collections.Generic;

namespace ECHOISM;

public struct LevelCompletionStats
{
    public int AttemptsTaken;
    public float TimeTakenSeconds;
    public bool Completed;
    public bool FragmentRecovered;
}

public enum PerformanceGrade
{
    None,
    Clear,
    Efficient,
    Perfect
}

public struct FragmentData
{
    public string FragmentId;
    public string Title;
    public string Excerpt;
    public bool Recovered;
}

public static class ProgressionManager
{
    public static int MaxUnlockedLevel { get; private set; } = 1;
    public static bool UnlockAllLevels { get; set; } = false;

    private static readonly Dictionary<int, LevelCompletionStats> _stats = [];
    private static readonly Dictionary<string, FragmentData> _fragments = [];

    public static event Action? ProgressionChanged;

    public static bool IsLevelUnlocked(int levelIndex)
    {
        if (UnlockAllLevels)
        {
            return true;
        }

        return levelIndex < MaxUnlockedLevel;
    }

    public static bool IsLevelCompleted(int levelIndex)
    {
        return _stats.TryGetValue(levelIndex, out var stat) && stat.Completed;
    }

    public static LevelCompletionStats GetLevelStats(int levelIndex)
    {
        if (_stats.TryGetValue(levelIndex, out var stat))
        {
            return stat;
        }

        return new LevelCompletionStats { AttemptsTaken = 0, TimeTakenSeconds = 0f, Completed = false, FragmentRecovered = false };
    }

    public static void RecordLevelCompletion(int levelIndex, int attempts, float timeSeconds)
    {
        var current = GetLevelStats(levelIndex);
        var info = LevelRegistry.GetLevel(levelIndex);
        bool hasFragment = info is not null && !string.IsNullOrEmpty(info.OptionalFragmentId) && IsFragmentRecovered(info.OptionalFragmentId);

        var updated = new LevelCompletionStats
        {
            Completed = true,
            AttemptsTaken = current.Completed ? Math.Min(current.AttemptsTaken, attempts) : attempts,
            TimeTakenSeconds = current.Completed ? Math.Min(current.TimeTakenSeconds, timeSeconds) : timeSeconds,
            FragmentRecovered = current.FragmentRecovered || hasFragment
        };

        _stats[levelIndex] = updated;

        if (levelIndex + 2 > MaxUnlockedLevel)
        {
            MaxUnlockedLevel = levelIndex + 2;
        }

        ProgressionChanged?.Invoke();
    }

    public static void RecordFragmentRecovered(string fragmentId, string title, string excerpt)
    {
        _fragments[fragmentId] = new FragmentData
        {
            FragmentId = fragmentId,
            Title = title,
            Excerpt = excerpt,
            Recovered = true
        };

        // Mark on current level stat if matching
        for (int i = 0; i < LevelRegistry.LevelCount; i++)
        {
            var lvl = LevelRegistry.GetLevel(i);
            if (lvl?.OptionalFragmentId == fragmentId)
            {
                var stat = GetLevelStats(i);
                stat.FragmentRecovered = true;
                _stats[i] = stat;
            }
        }

        ProgressionChanged?.Invoke();
    }

    public static bool IsFragmentRecovered(string fragmentId)
    {
        return _fragments.TryGetValue(fragmentId, out var data) && data.Recovered;
    }

    public static IReadOnlyDictionary<string, FragmentData> RecoveredFragments => _fragments;

    public static PerformanceGrade CalculateGrade(int levelIndex, int attempts)
    {
        var info = LevelRegistry.GetLevel(levelIndex);
        if (info is null) return PerformanceGrade.Clear;

        bool hasFragment = !string.IsNullOrEmpty(info.OptionalFragmentId) && IsFragmentRecovered(info.OptionalFragmentId);
        bool efficient = attempts <= info.EfficientLoopThreshold;

        if (efficient && hasFragment)
        {
            return PerformanceGrade.Perfect;
        }
        if (efficient)
        {
            return PerformanceGrade.Efficient;
        }
        return PerformanceGrade.Clear;
    }

    public static bool ToggleUnlockAll()
    {
        UnlockAllLevels = !UnlockAllLevels;
        ProgressionChanged?.Invoke();
        return UnlockAllLevels;
    }

    public static void ResetProgression()
    {
        MaxUnlockedLevel = 1;
        UnlockAllLevels = false;
        _stats.Clear();
        _fragments.Clear();
        ProgressionChanged?.Invoke();
    }
}
