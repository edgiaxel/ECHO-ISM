#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class DataKeyDispenser : Area2D, IInteractable, IResettable, IPuzzleModule
{
    [Export]
    public string StableId { get; set; } = "data-key-dispenser-01";

    public string ModuleId => StableId;

    [Export]
    public string ModuleName { get; set; } = "Data Key Dispenser";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public string KeyId { get; set; } = "ARCHIVE_KEY_ALPHA";

    [Export]
    public bool AcceptsEchoes { get; set; } = true;

    public bool HasDispensed { get; private set; }
    public bool IsCompleted => HasDispensed;

    public ModuleState CurrentState => HasDispensed ? ModuleState.Completed : ModuleState.Idle;

    public event Action<bool>? DispensedStateChanged;
    public event Action<ModuleState>? StateChanged;

    private CollisionShape2D? _collision;
    private static readonly Color ConsoleBg = new("#0B132B");
    private static readonly Color ConsoleBorder = new("#334155");
    private static readonly Color KeyColor = new("#FACC15");

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
        return HasDispensed ? $"Key Retrieved [{KeyId}]" : $"Collect Data Key [{KeyId}]";
    }

    public bool TryInteract(GregCharacter actor)
    {
        HasDispensed = true;
        KeySocket.AcquireKey(KeyId);
        SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
        DispensedStateChanged?.Invoke(true);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
        return true;
    }

    public void ResetState()
    {
        HasDispensed = false;
        DispensedStateChanged?.Invoke(false);
        StateChanged?.Invoke(CurrentState);
        QueueRedraw();
    }

    public override void _Draw()
    {
        var rect = new Rect2(-14f, -14f, 28f, 28f);
        DrawRect(rect, ConsoleBg);
        DrawRect(rect, HasDispensed ? ConsoleBorder : KeyColor, false, 2f);

        if (!HasDispensed)
        {
            var keyRect = new Rect2(-6f, -9f, 12f, 18f);
            DrawRect(keyRect, KeyColor);
            DrawCircle(new Vector2(0f, -4f), 2.5f, ConsoleBg);
        }
        else
        {
            DrawRect(new Rect2(-6f, -9f, 12f, 18f), ConsoleBorder, false, 1.5f);
        }
    }
}
