#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class KeySocket : Area2D, IInteractable, IResettable, IPuzzleModule
{
    [Export]
    public string StableId { get; set; } = "key-socket-01";

    public string ModuleId => StableId;

    [Export]
    public string ModuleName { get; set; } = "Data Key Socket";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public string RequiredKeyId { get; set; } = "ARCHIVE_KEY_ALPHA";

    [Export]
    public bool AcceptsEchoes { get; set; } = true;

    public bool IsInserted { get; private set; }
    public bool IsCompleted => IsInserted;

    public ModuleState CurrentState => IsInserted ? ModuleState.Completed : ModuleState.Idle;

    private static string? _activeKeyHeld;

    public static string? ActiveKeyHeld => _activeKeyHeld;

    public static void AcquireKey(string keyId)
    {
        _activeKeyHeld = keyId;
    }

    public static void ClearHeldKeys()
    {
        _activeKeyHeld = null;
    }

    public event Action<bool>? SocketStateChanged;
    public event Action<ModuleState>? StateChanged;

    private CollisionShape2D? _collision;
    private static readonly Color HousingColor = new("#0F172A");
    private static readonly Color BorderEmpty = new("#475569");
    private static readonly Color BorderSlotted = new("#FACC15");
    private static readonly Color CoreEmpty = new("#1E293B");
    private static readonly Color CoreSlotted = new("#FACC15");

    public override void _Ready()
    {
        CollisionLayer = 2; // Interactable
        CollisionMask = 0;

        _collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_collision is null)
        {
            _collision = new CollisionShape2D
            {
                Shape = new CircleShape2D { Radius = 24f }
            };
            AddChild(_collision);
        }

        QueueRedraw();
    }

    public string GetPrompt(GregCharacter actor)
    {
        if (IsInserted)
        {
            return "Data Key Inserted [ACTIVE]";
        }
        if (_activeKeyHeld == RequiredKeyId)
        {
            return $"Insert Data Key [{RequiredKeyId}]";
        }
        return $"Key Socket [Requires: {RequiredKeyId}]";
    }

    public bool TryInteract(GregCharacter actor)
    {
        if (IsInserted) return false;

        if (_activeKeyHeld == RequiredKeyId)
        {
            InsertKey();
            return true;
        }

        SoundManager.Instance?.Play(SoundEffect.TimingMiss);
        return false;
    }

    public void InsertKey()
    {
        IsInserted = true;
        SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
        SocketStateChanged?.Invoke(true);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
    }

    public void ResetState()
    {
        IsInserted = false;
        ClearHeldKeys();
        SocketStateChanged?.Invoke(false);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
    }

    public override void _Draw()
    {
        var pts = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float a = i * Mathf.Pi / 3.0f;
            pts[i] = new Vector2(Mathf.Cos(a) * 18f, Mathf.Sin(a) * 18f);
        }
        DrawColoredPolygon(pts, HousingColor);

        Color border = IsInserted ? BorderSlotted : BorderEmpty;
        for (int i = 0; i < 6; i++)
        {
            DrawLine(pts[i], pts[(i + 1) % 6], border, 2f);
        }

        var slotRect = new Rect2(-5f, -9f, 10f, 18f);
        DrawRect(slotRect, IsInserted ? CoreSlotted : CoreEmpty);
        DrawRect(slotRect, border, false, 1.5f);

        if (IsInserted)
        {
            DrawCircle(Vector2.Zero, 4f, new Color("#FFFFFF"));
        }
    }
}
