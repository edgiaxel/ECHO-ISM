#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class PauseModal : Control
{
    public event Action? ResumeRequested;
    public event Action? RetryRequested;
    public event Action? LevelSelectRequested;

    private Button _resumeButton = null!;
    private Button _retryButton = null!;
    private Button _muteButton = null!;
    private Button _levelSelectButton = null!;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _resumeButton = GetNode<Button>("CenterContainer/Card/VBox/ResumeButton");
        _retryButton = GetNode<Button>("CenterContainer/Card/VBox/RetryButton");
        _muteButton = GetNode<Button>("CenterContainer/Card/VBox/MuteButton");
        _levelSelectButton = GetNode<Button>("CenterContainer/Card/VBox/LevelSelectButton");

        _resumeButton.Pressed += OnResumeClicked;
        _retryButton.Pressed += OnRetryClicked;
        _muteButton.Pressed += OnMuteClicked;
        _levelSelectButton.Pressed += OnLevelSelectClicked;

        UpdateMuteButtonText();
    }

    public void ShowModal()
    {
        Visible = true;
        MouseFilter = MouseFilterEnum.Stop;
        UpdateMuteButtonText();
        _resumeButton.GrabFocus();
    }

    public void HideModal()
    {
        Visible = false;
        MouseFilter = MouseFilterEnum.Ignore;
    }

    private void OnResumeClicked()
    {
        SoundManager.Instance?.PlayUiClick();
        ResumeRequested?.Invoke();
    }

    private void OnRetryClicked()
    {
        SoundManager.Instance?.PlayUiClick();
        RetryRequested?.Invoke();
    }

    private void OnMuteClicked()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ToggleAudio();
            UpdateMuteButtonText();
            SoundManager.Instance.PlayUiClick();
        }
    }

    private void OnLevelSelectClicked()
    {
        SoundManager.Instance?.PlayUiClick();
        LevelSelectRequested?.Invoke();
    }

    private void UpdateMuteButtonText()
    {
        if (_muteButton == null) return;
        bool audioOn = SoundManager.Instance?.AudioEnabled ?? true;
        _muteButton.Text = audioOn ? "AUDIO: ENABLED" : "AUDIO: MUTED";
    }
}
