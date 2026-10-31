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
            DisplayName = "Boot Circuit",
            NarrativeChapter = "The First Distortion",
            MechanicDescription = "Switches, doors, and facility navigation",
            DebriefText = "Communication link restored. Public notice warns citizens to disregard previous warnings, indicating early network distortion.",
            EfficientLoopThreshold = 1,
            OptionalFragmentId = "FRAG_SEC01_LOST_TRANSMISSION",
            OptionalFragmentTitle = "Lost Transmission",
            ScenePath = "res://Scenes/Levels/TestRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("DualWing", 15f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 1,
            SectorCode = "SECTOR 02",
            DisplayName = "False Synchrony",
            NarrativeChapter = "Conflicting Signals",
            MechanicDescription = "Timing slider calibration and pressure plate synchronization",
            DebriefText = "Both channels synchronized. Discrepant timestamps confirm that simultaneous reports were fabricated from different events.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC02_SIGNAL_ANALYST",
            OptionalFragmentTitle = "Signal Analyst",
            ScenePath = "res://Scenes/Levels/TimingRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("TimingHub", 15f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 2,
            SectorCode = "SECTOR 03",
            DisplayName = "Divided Consensus",
            NarrativeChapter = "A Society Splits",
            MechanicDescription = "Cooperation across divided districts with past Echoes",
            DebriefText = "Inter-district transit unlocked. Records reveal both districts originally requested communication, not separation.",
            EfficientLoopThreshold = 2,
            OptionalFragmentId = "FRAG_SEC03_MEDIATOR_RECORD",
            OptionalFragmentTitle = "The Mediator's Record",
            ScenePath = "res://Scenes/Levels/DualProtocolRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("DualChamber", 16f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 3,
            SectorCode = "SECTOR 04",
            DisplayName = "The Echo Chamber",
            NarrativeChapter = "Repeating the Past",
            MechanicDescription = "Echo resonance sensor and delayed signal relay routing",
            DebriefText = "Relay facility synchronized. Corrupted archives reveal how duplicated messages systematically lost their qualifying context.",
            EfficientLoopThreshold = 3,
            OptionalFragmentId = "FRAG_SEC04_ORIGINAL_SOURCE",
            OptionalFragmentTitle = "Original Source",
            ScenePath = "res://Scenes/Levels/RelaySyncRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("RelayGrid", 18f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 4,
            SectorCode = "SECTOR 05",
            DisplayName = "Fractured Network",
            NarrativeChapter = "Information Overload",
            MechanicDescription = "Multi-chamber stabilization combining multiple mechanics",
            DebriefText = "Core infrastructure stabilized. Recovered archive logs demonstrate that the network failure was deliberately orchestrated.",
            EfficientLoopThreshold = 3,
            OptionalFragmentId = "FRAG_SEC05_NETWORK_ARCHAEOLOGY",
            OptionalFragmentTitle = "Network Archaeology",
            ScenePath = "res://Scenes/Levels/RemixStabilizationRoom.tscn",
            Parameters = LevelRemixer.GenerateRemix("RemixStabilization", 42)
        },
        new LevelInfo
        {
            LevelIndex = 5,
            SectorCode = "SECTOR 06",
            DisplayName = "The Truth Protocol",
            NarrativeChapter = "Evidence and Context",
            MechanicDescription = "Verification terminal authentication and severed line repair",
            DebriefText = "Archive integrity verified. Reconstructed evidence shows reports were taken from disparate events and presented as one crisis.",
            EfficientLoopThreshold = 3,
            OptionalFragmentId = "FRAG_SEC06_MISSING_WITNESS",
            OptionalFragmentTitle = "The Missing Witness",
            ScenePath = "res://Scenes/Levels/Sector06VerificationRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("IntegrityBridge", 16f, 3)
        },
        new LevelInfo
        {
            LevelIndex = 6,
            SectorCode = "SECTOR 07",
            DisplayName = "Cascade Prevention",
            NarrativeChapter = "Before the Breaking Point",
            MechanicDescription = "Countdown containment and synchronized multi-station stabilization",
            DebriefText = "Emergency broadcast cancelled. Original reports restored. Information cannot replace trust, but without information, trust cannot be rebuilt.",
            EfficientLoopThreshold = 3,
            OptionalFragmentId = "FRAG_SEC07_FINAL_TESTIMONY",
            OptionalFragmentTitle = "Final Testimony",
            ScenePath = "res://Scenes/Levels/Sector07PreventionRoom.tscn",
            Parameters = LevelVariationParameters.CreateHandcrafted("EventPrevention", 15f, 3)
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
