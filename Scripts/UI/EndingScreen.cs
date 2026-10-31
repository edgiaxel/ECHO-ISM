#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class EndingScreen : Control
{
    public static event Action? ReplayCampaignRequested;
    public static event Action? MainMenuRequested;

    private Label _epilogueText = null!;
    private Label _finalMessage = null!;
    private Button _replayButton = null!;
    private Button _mainMenuButton = null!;

    public override void _Ready()
    {
        _epilogueText = GetNode<Label>("Margin/VBox/EpilogueText");
        _finalMessage = GetNode<Label>("Margin/VBox/FinalQuoteCard/FinalMessage");
        _replayButton = GetNode<Button>("Margin/VBox/ButtonRow/ReplayButton");
        _mainMenuButton = GetNode<Button>("Margin/VBox/ButtonRow/MainMenuButton");

        _replayButton.Pressed += () =>
        {
            SoundManager.Instance?.Play(SoundEffect.UiClick);
            ReplayCampaignRequested?.Invoke();
        };

        _mainMenuButton.Pressed += () =>
        {
            SoundManager.Instance?.Play(SoundEffect.UiClick);
            MainMenuRequested?.Invoke();
        };
    }

    public void ShowEnding(int totalLoops, float totalTimeSeconds)
    {
        Visible = true;
        _epilogueText.Text = 
            "The Heartline pulses with restored resonance. Across NEXUS, districts reconnect.\n\n" +
            "BOLT has stabilized the power routing conduits. Mira has restored the original broadcast archives. " +
            "Pip speeds across the restored crossings, delivering authenticated packets to waiting residents.\n\n" +
            $"Campaign Synchronized in {totalLoops} total chrono loops across {totalTimeSeconds:F1}s.\n" +
            "The corrupted entity NULL has dispersed from the network channels.";

        _finalMessage.Text = "“A signal can travel faster than the truth.\nBut the truth can still reach everyone.”";
    }
}
