#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class LevelSelect : Control
{
    public event Action<int>? LevelSelected;
    public event Action? BackToMenuPressed;

    private VBoxContainer _levelsList = null!;
    private Button _backButton = null!;

    public override void _Ready()
    {
        _levelsList = GetNode<VBoxContainer>("ScrollContainer/CenterContainer/VBox/LevelsContainer");
        _backButton = GetNode<Button>("BottomLeft/BackButton");

        _backButton.Pressed += () => BackToMenuPressed?.Invoke();
        ProgressionManager.ProgressionChanged += RefreshLevelList;

        BuildLevelList();
    }

    public void RefreshLevelList()
    {
        BuildLevelList();
    }

    private void BuildLevelList()
    {
        if (_levelsList is null)
        {
            return;
        }

        foreach (var child in _levelsList.GetChildren())
        {
            child.QueueFree();
        }

        var levels = LevelRegistry.AllLevels;
        for (int i = 0; i < levels.Count; i++)
        {
            var level = levels[i];
            int index = i;
            bool isUnlocked = ProgressionManager.IsLevelUnlocked(index);
            bool isCompleted = ProgressionManager.IsLevelCompleted(index);
            var stats = ProgressionManager.GetLevelStats(index);

            var card = CreateLevelCard(level, index, isUnlocked, isCompleted, stats);
            _levelsList.AddChild(card);
        }
    }

    private Control CreateLevelCard(LevelInfo level, int index, bool isUnlocked, bool isCompleted, LevelCompletionStats stats)
    {
        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(580, 72);

        var style = new StyleBoxFlat
        {
            BgColor = isUnlocked ? new Color(0.055f, 0.086f, 0.149f, 0.95f) : new Color(0.04f, 0.06f, 0.10f, 0.7f),
            BorderColor = isCompleted ? new Color("#10B981") : (isUnlocked ? new Color("#00D2FF") : new Color("#334155")),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomRight = 4,
            CornerRadiusBottomLeft = 4,
            ContentMarginLeft = 16,
            ContentMarginRight = 16,
            ContentMarginTop = 10,
            ContentMarginBottom = 10
        };
        panel.AddThemeStyleboxOverride("panel", style);

        var hbox = new HBoxContainer();
        hbox.Alignment = BoxContainer.AlignmentMode.Center;
        hbox.AddThemeConstantOverride("separation", 16);
        panel.AddChild(hbox);

        // Left info
        var vboxInfo = new VBoxContainer();
        vboxInfo.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        vboxInfo.AddThemeConstantOverride("separation", 4);
        hbox.AddChild(vboxInfo);

        var topHbox = new HBoxContainer();
        topHbox.AddThemeConstantOverride("separation", 8);
        vboxInfo.AddChild(topHbox);

        var codeLabel = new Label();
        codeLabel.Text = $"{level.SectorCode} - {level.DisplayName}";
        codeLabel.AddThemeColorOverride("font_color", isUnlocked ? new Color("#F8FAFC") : new Color("#64748B"));
        codeLabel.AddThemeFontSizeOverride("font_size", 14);
        topHbox.AddChild(codeLabel);

        if (isCompleted)
        {
            var grade = ProgressionManager.CalculateGrade(index, stats.AttemptsTaken);
            var gradeText = grade switch
            {
                PerformanceGrade.Perfect => "[PERFECT]",
                PerformanceGrade.Efficient => "[EFFICIENT]",
                _ => "[CLEARED]"
            };
            var gradeColor = grade switch
            {
                PerformanceGrade.Perfect => new Color("#F59E0B"),
                PerformanceGrade.Efficient => new Color("#10B981"),
                _ => new Color("#10B981")
            };

            var badge = new Label();
            badge.Text = gradeText;
            badge.AddThemeColorOverride("font_color", gradeColor);
            badge.AddThemeFontSizeOverride("font_size", 11);
            topHbox.AddChild(badge);

            bool hasFrag = !string.IsNullOrEmpty(level.OptionalFragmentId) && ProgressionManager.IsFragmentRecovered(level.OptionalFragmentId);
            if (hasFrag)
            {
                var fragBadge = new Label();
                fragBadge.Text = "[DATA RECOVERED]";
                fragBadge.AddThemeColorOverride("font_color", new Color("#F59E0B"));
                fragBadge.AddThemeFontSizeOverride("font_size", 10);
                topHbox.AddChild(fragBadge);
            }
        }
        else if (!isUnlocked)
        {
            var badge = new Label();
            badge.Text = "[LOCKED]";
            badge.AddThemeColorOverride("font_color", new Color("#64748B"));
            badge.AddThemeFontSizeOverride("font_size", 11);
            topHbox.AddChild(badge);
        }

        var descLabel = new Label();
        descLabel.Text = isUnlocked ? level.MechanicDescription : "Complete preceding sector to decrypt mission parameters.";
        descLabel.AddThemeColorOverride("font_color", isUnlocked ? new Color("#94A3B8") : new Color("#475569"));
        descLabel.AddThemeFontSizeOverride("font_size", 12);
        vboxInfo.AddChild(descLabel);

        if (isCompleted)
        {
            var statsLabel = new Label();
            statsLabel.Text = $"Record: {stats.AttemptsTaken} loops - {stats.TimeTakenSeconds:0.00}s";
            statsLabel.AddThemeColorOverride("font_color", new Color("#38BDF8"));
            statsLabel.AddThemeFontSizeOverride("font_size", 11);
            vboxInfo.AddChild(statsLabel);
        }

        // Action button
        var playBtn = new Button();
        playBtn.CustomMinimumSize = new Vector2(100, 40);
        playBtn.Text = isCompleted ? "REPLAY" : (isUnlocked ? "DEPLOY" : "LOCKED");
        playBtn.Disabled = !isUnlocked;

        var btnStyle = new StyleBoxFlat
        {
            BgColor = isUnlocked ? new Color(0.08f, 0.14f, 0.24f, 0.95f) : new Color(0.04f, 0.05f, 0.08f, 0.5f),
            BorderColor = isUnlocked ? new Color("#00D2FF") : new Color("#334155"),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            CornerRadiusTopLeft = 3,
            CornerRadiusTopRight = 3,
            CornerRadiusBottomRight = 3,
            CornerRadiusBottomLeft = 3,
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 6,
            ContentMarginBottom = 6
        };
        playBtn.AddThemeStyleboxOverride("normal", btnStyle);
        playBtn.AddThemeColorOverride("font_color", isUnlocked ? new Color("#F8FAFC") : new Color("#475569"));
        playBtn.AddThemeFontSizeOverride("font_size", 13);

        if (isUnlocked)
        {
            playBtn.Pressed += () => LevelSelected?.Invoke(index);
            panel.MouseDefaultCursorShape = CursorShape.PointingHand;
            panel.GuiInput += @event =>
            {
                if (@event is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                {
                    LevelSelected?.Invoke(index);
                }
            };
        }

        hbox.AddChild(playBtn);
        return panel;
    }
}
