#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class DialogueOverlay : Control
{
    public static event Action? DialogueClosed;
    public static DialogueOverlay? Instance { get; private set; }
    public static bool IsDialogueActive => Instance?._isActive ?? false;

    private PanelContainer _cardPanel = null!;
    private ColorRect _portraitBadge = null!;
    private Label _speakerNameLabel = null!;
    private Label _speakerRoleLabel = null!;
    private Label _bodyLabel = null!;
    private Label _advancePrompt = null!;

    private NpcCharacter? _currentNpc;
    private int _lineIndex;
    private bool _isActive;

    public bool IsActive => _isActive;

    public override void _Ready()
    {
        Instance = this;
        _cardPanel = GetNode<PanelContainer>("CardPanel");
        _portraitBadge = GetNode<ColorRect>("CardPanel/Margin/HBox/PortraitBadge");
        _speakerNameLabel = GetNode<Label>("CardPanel/Margin/HBox/VBox/Header/SpeakerName");
        _speakerRoleLabel = GetNode<Label>("CardPanel/Margin/HBox/VBox/Header/SpeakerRole");
        _bodyLabel = GetNode<Label>("CardPanel/Margin/HBox/VBox/BodyLabel");
        _advancePrompt = GetNode<Label>("CardPanel/Margin/HBox/VBox/AdvancePrompt");

        Visible = false;
        ProcessMode = ProcessModeEnum.Always;

        NpcCharacter.DialogueStarted += OnNpcDialogueStarted;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        NpcCharacter.DialogueStarted -= OnNpcDialogueStarted;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (!_isActive) return;

        if (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Left)
        {
            AdvanceDialogue();
            AcceptEvent();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isActive) return;

        if (@event.IsActionPressed("interact") || 
            (@event is InputEventKey key && key.Pressed && (key.Keycode == Key.Space || key.Keycode == Key.Enter)) ||
            (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Left))
        {
            AdvanceDialogue();
            GetViewport()?.SetInputAsHandled();
        }
    }

    public void StartDialogue(NpcCharacter npc)
    {
        _currentNpc = npc;
        _lineIndex = 0;
        _isActive = true;
        Visible = true;

        _speakerNameLabel.Text = npc.CharacterName.ToUpperInvariant();
        _speakerRoleLabel.Text = $"· {npc.RoleTitle.ToUpperInvariant()}";
        _portraitBadge.Color = npc.GetPrimaryColor();

        DisplayCurrentLine();
    }

    private void OnNpcDialogueStarted(NpcCharacter npc)
    {
        StartDialogue(npc);
    }

    private void DisplayCurrentLine()
    {
        if (_currentNpc is null || _lineIndex >= _currentNpc.DialogueLines.Length)
        {
            CloseDialogue();
            return;
        }

        _bodyLabel.Text = _currentNpc.DialogueLines[_lineIndex];

        bool isLast = _lineIndex == _currentNpc.DialogueLines.Length - 1;
        _advancePrompt.Text = isLast ? "[E] DISMISS" : "[E] CONTINUE ▶";
        SoundManager.Instance?.Play(SoundEffect.Interact);
    }

    public void AdvanceDialogue()
    {
        if (!_isActive || _currentNpc is null) return;

        _lineIndex++;
        if (_lineIndex < _currentNpc.DialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        _isActive = false;
        Visible = false;
        _currentNpc = null;
        DialogueClosed?.Invoke();
    }
}
