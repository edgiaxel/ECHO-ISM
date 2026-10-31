#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class LevelCompletedModal : Control
{
    public event Action? NextSectorPressed;
    public event Action? LevelSelectPressed;
    public event Action? ReplayPressed;

    private Label _sectorHeader = null!;
    private Label _sectorTitle = null!;
    private Label _gradeBadge = null!;
    private Label _runsValue = null!;
    private Label _timeValue = null!;
    private Label _debriefText = null!;
    private Label _fragmentStatus = null!;
    private Button _nextSectorButton = null!;
    private Button _levelSelectButton = null!;
    private Button _replayButton = null!;

    public override void _Ready()
    {
        _sectorHeader = GetNode<Label>("CenterContainer/Card/VBox/Header");
        _sectorTitle = GetNode<Label>("CenterContainer/Card/VBox/Title");
        _gradeBadge = GetNode<Label>("CenterContainer/Card/VBox/GradeBadge");
        _runsValue = GetNode<Label>("CenterContainer/Card/VBox/StatsBox/HBox/RunsContainer/Value");
        _timeValue = GetNode<Label>("CenterContainer/Card/VBox/StatsBox/HBox/TimeContainer/Value");
        _debriefText = GetNode<Label>("CenterContainer/Card/VBox/DebriefBox/VBox/DebriefText");
        _fragmentStatus = GetNode<Label>("CenterContainer/Card/VBox/DebriefBox/VBox/FragmentStatus");
        _nextSectorButton = GetNode<Button>("CenterContainer/Card/VBox/ButtonGroup/NextSectorButton");
        _levelSelectButton = GetNode<Button>("BottomLeft/LevelSelectButton");
        _replayButton = GetNode<Button>("CenterContainer/Card/VBox/ButtonGroup/ReplayButton");

        _nextSectorButton.Pressed += () => NextSectorPressed?.Invoke();
        _levelSelectButton.Pressed += () => LevelSelectPressed?.Invoke();
        _replayButton.Pressed += () => ReplayPressed?.Invoke();
    }

    public void Display(LevelInfo level, int attempts, float timeSeconds, bool hasNextSector)
    {
        Visible = true;
        _sectorHeader.Text = $"{level.SectorCode} RESTORED";
        _sectorTitle.Text = level.DisplayName.ToUpperInvariant();
        _runsValue.Text = $"{attempts} {(attempts == 1 ? "LOOP" : "LOOPS")}";
        _timeValue.Text = $"{timeSeconds:0.00}s";

        var grade = ProgressionManager.CalculateGrade(level.LevelIndex, attempts);
        _gradeBadge.Text = grade switch
        {
            PerformanceGrade.Perfect => "[PERFECT EXECUTION]",
            PerformanceGrade.Efficient => "[EFFICIENT EXECUTION]",
            _ => "[SECTOR CLEARED]"
        };
        _gradeBadge.AddThemeColorOverride("font_color", grade switch
        {
            PerformanceGrade.Perfect => new Color("#F59E0B"), // Gold
            PerformanceGrade.Efficient => new Color("#10B981"), // Emerald
            _ => new Color("#00D2FF") // Cyan
        });

        bool fragRecovered = !string.IsNullOrEmpty(level.OptionalFragmentId) && ProgressionManager.IsFragmentRecovered(level.OptionalFragmentId);
        _fragmentStatus.Text = fragRecovered 
            ? $"FRAGMENT RECOVERED: {level.OptionalFragmentTitle.ToUpperInvariant()}" 
            : "OPTIONAL FRAGMENT: NOT DISCOVERED";
        _fragmentStatus.AddThemeColorOverride("font_color", fragRecovered ? new Color("#10B981") : new Color("#64748B"));

        _debriefText.Text = !string.IsNullOrWhiteSpace(level.DebriefText)
            ? level.DebriefText
            : "Telemetry and circuit connections restored to stable baseline.";

        _nextSectorButton.Visible = hasNextSector;
        _replayButton.Visible = !hasNextSector;
    }

    public void HideModal()
    {
        Visible = false;
    }
}
