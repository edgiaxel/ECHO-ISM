#nullable enable
using System.Collections.Generic;

namespace ECHOISM;

public class LevelInfo
{
    public int LevelIndex { get; set; }
    public string SectorCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string NarrativeChapter { get; set; } = string.Empty;
    public string MechanicDescription { get; set; } = string.Empty;
    public string DebriefText { get; set; } = string.Empty;
    public int EfficientLoopThreshold { get; set; } = 2;
    public string OptionalFragmentId { get; set; } = string.Empty;
    public string OptionalFragmentTitle { get; set; } = string.Empty;
    public string ScenePath { get; set; } = string.Empty;
    public LevelVariationParameters Parameters { get; set; }
}

public static class LevelRegistry
{
    private static readonly List<LevelInfo> _levels =
    [
        new LevelInfo
        {
            LevelIndex = 0,
            SectorCode = "SECTOR 01",
            DisplayName = "The Broken Crossing",
            NarrativeChapter = "Boot Sequence",
            MechanicDescription = "Pressure plate gate hold, bridge power activation, and Pip encounter",
            DebriefText = "Crossing repaired. Pip confirms the emergency broadcast instructed residents to isolate districts, fabricating a crisis.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC01_LOST_TRANSMISSION",
            OptionalFragmentTitle = "Lost Transmission",
            ScenePath = "res://Scenes/Levels/Sector01_BrokenCrossing.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("BrokenCrossing", 18f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 1,
            SectorCode = "SECTOR 02",
            DisplayName = "Powerworks",
            NarrativeChapter = "The Rotating Grid",
            MechanicDescription = "Capacitor charging, rotary junction routing, and BOLT encounter",
            DebriefText = "Power station re-engaged. Telemetry proves the shutdown command was spoofed from an outside source.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC02_SIGNAL_ANALYST",
            OptionalFragmentTitle = "Signal Analyst",
            ScenePath = "res://Scenes/Levels/Sector02_Powerworks.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("PowerworksGrid", 18f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 2,
            SectorCode = "SECTOR 03",
            DisplayName = "The Archive",
            NarrativeChapter = "Truth Protocol",
            MechanicDescription = "Data key authorization, dual terminal verification, and Mira encounter",
            DebriefText = "Deep archive unsealed. The original announcement confirms districts were never ordered to isolate. NULL signature identified.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC03_MEDIATOR_RECORD",
            OptionalFragmentTitle = "The Mediator's Record",
            ScenePath = "res://Scenes/Levels/Sector03_Archive.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("ArchiveTruth", 20f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 3,
            SectorCode = "SECTOR 04",
            DisplayName = "The Signal Tower",
            NarrativeChapter = "The False Broadcast",
            MechanicDescription = "Timed gate sprint, multi-channel relays, and NULL confrontation",
            DebriefText = "Signal Tower online. Verified broadcast beamed across the city. NULL attempts to collapse the Heartline in response.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC04_ORIGINAL_SOURCE",
            OptionalFragmentTitle = "Original Source",
            ScenePath = "res://Scenes/Levels/Sector04_SignalTower.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("SignalTower", 22f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 4,
            SectorCode = "SECTOR 05",
            DisplayName = "The Heartline",
            NarrativeChapter = "The Last Signal",
            MechanicDescription = "3-channel Echo coordination across Power, Archive, and Stabilizer cores",
            DebriefText = "Heartline synchronized across all channels. All districts reconnected. The truth reaches everyone.",
            EfficientLoopThreshold = 3,
            OptionalFragmentId = "FRAG_SEC05_NETWORK_ARCHAEOLOGY",
            OptionalFragmentTitle = "Network Archaeology",
            ScenePath = "res://Scenes/Levels/Sector05_Heartline.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("HeartlineCore", 24f, 3)
        }
    ];

    public static IReadOnlyList<LevelInfo> AllLevels => _levels;

    public static LevelInfo? GetLevel(int index)
    {
        if (index >= 0 && index < _levels.Count)
        {
            return _levels[index];
        }
        return null;
    }

    public static int LevelCount => _levels.Count;
}
