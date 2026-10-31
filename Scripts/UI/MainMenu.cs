#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class MainMenu : Control
{
    public event Action? PlayPressed;
    public event Action? ExitPressed;

    private Button _playButton = null!;
    private Button _exitButton = null!;
    private Button _unlockAllButton = null!;

    public override void _Ready()
    {
        _playButton = GetNode<Button>("MenuContainer/VBox/PlayButton");
        _exitButton = GetNode<Button>("MenuContainer/VBox/ExitButton");
        _unlockAllButton = GetNode<Button>("BottomRight/UnlockAllButton");

        _playButton.Pressed += () =>
        {
            PlayPressed?.Invoke();
            // If running standalone (e.g. user ran MainMenu.tscn directly or set it as main scene)
            if (GetParent() == GetTree().Root)
            {
                GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
            }
        };

        _exitButton.Pressed += () =>
        {
            ExitPressed?.Invoke();
            GetTree().Quit();
        };

        _unlockAllButton.Pressed += OnUnlockAllPressed;
        UpdateUnlockAllButtonText();
    }

    private void OnUnlockAllPressed()
    {
        ProgressionManager.ToggleUnlockAll();
        UpdateUnlockAllButtonText();
    }

    public void UpdateUnlockAllButtonText()
    {
        if (_unlockAllButton is not null)
        {
            bool unlocked = ProgressionManager.UnlockAllLevels;
            _unlockAllButton.Text = unlocked ? "[DEBUG] UNLOCK ALL: ON" : "[DEBUG] UNLOCK ALL: OFF";
            _unlockAllButton.Modulate = unlocked ? new Color("#10B981") : new Color("#94A3B8");
        }
    }
}
