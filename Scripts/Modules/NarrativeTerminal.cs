#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class NarrativeTerminal : Area2D, IInteractable, IResettable
{
    [Export]
    public string StableId { get; set; } = "narrative-terminal-01";

    [Export]
    public string TerminalTitle { get; set; } = "PUBLIC INFORMATION TERMINAL";

    [Export]
    public string MessageHeader { get; set; } = "SYSTEM NOTICE";

    [Export(PropertyHint.MultilineText)]
    public string MessageBody { get; set; } = "No unusual activity has been reported.\nPlease disregard all previous warnings.";

    [Export]
    public bool IsOptionalFragment { get; set; } = false;

    [Export]
    public string FragmentId { get; set; } = "FRAG_SEC01_LOST_TRANSMISSION";

    [Export]
    public string FragmentTitle { get; set; } = "Lost Transmission";

    [Export]
    public string PromptText { get; set; } = "Inspect terminal";

    [Export]
    public bool AcceptsEchoes { get; set; } = false;

    public bool HasBeenRead { get; private set; }

    public static event Action<NarrativeTerminal>? TerminalRead;

    private static readonly Color CasingColor = new("#0B132B");
    private static readonly Color BorderDefault = new("#334155");
    private static readonly Color BorderRead = new("#00D2FF");
    private static readonly Color BorderFragment = new("#F59E0B");
    private static readonly Color ScreenActive = new("#10B981");
    private static readonly Color ScreenNotice = new("#00D2FF");
    private static readonly Color ScreenFragment = new("#F59E0B");

    private CollisionShape2D? _collision;

    public override void _Ready()
    {
        _collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_collision is null)
        {
            _collision = new CollisionShape2D
            {
                Shape = new CircleShape2D { Radius = 32f }
            };
            AddChild(_collision);
        }
        QueueRedraw();
    }

    public string GetPrompt(GregCharacter actor)
    {
        if (IsOptionalFragment)
        {
            return HasBeenRead ? "Re-read recovered fragment" : $"Recover fragment: {FragmentTitle}";
        }
        return HasBeenRead ? "Re-read notice" : PromptText;
    }

    public bool TryInteract(GregCharacter actor)
    {
        HasBeenRead = true;
        QueueRedraw();

        if (IsOptionalFragment)
        {
            ProgressionManager.RecordFragmentRecovered(FragmentId, FragmentTitle, MessageBody);
            SoundManager.Instance?.PlayTimingAccurate();
        }
        else
        {
            SoundManager.Instance?.PlayInteract();
        }

        TerminalRead?.Invoke(this);
        return true;
    }

    public void ResetState()
    {
        // Notice read states persist across loops within a session, but can be redrawn
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Outer housing pedestal
        var outerRect = new Rect2(-18f, -18f, 36f, 36f);
        DrawRect(outerRect, CasingColor);

        Color borderColor = IsOptionalFragment 
            ? (HasBeenRead ? ScreenActive : BorderFragment)
            : (HasBeenRead ? BorderRead : BorderDefault);

        DrawRect(outerRect, borderColor, false, 2f);

        // Screen bezel
        var screenRect = new Rect2(-12f, -12f, 24f, 24f);
        Color screenColor = IsOptionalFragment
            ? (HasBeenRead ? ScreenActive : ScreenFragment)
            : (HasBeenRead ? ScreenActive : ScreenNotice);

        DrawRect(screenRect, new Color(screenColor, 0.2f));
        DrawRect(screenRect, screenColor, false, 1.5f);

        // Icon inside screen
        if (IsOptionalFragment)
        {
            // Fragment symbol (diamond diamond / data gem)
            var points = new Vector2[]
            {
                new(0f, -6f),
                new(6f, 0f),
                new(0f, 6f),
                new(-6f, 0f)
            };
            DrawColoredPolygon(points, screenColor);
        }
        else
        {
            // Public notice icon (horizontal scanlines)
            DrawLine(new Vector2(-6f, -4f), new Vector2(6f, -4f), screenColor, 1.5f);
            DrawLine(new Vector2(-6f, 0f), new Vector2(4f, 0f), screenColor, 1.5f);
            DrawLine(new Vector2(-6f, 4f), new Vector2(6f, 4f), screenColor, 1.5f);
        }
    }
}
