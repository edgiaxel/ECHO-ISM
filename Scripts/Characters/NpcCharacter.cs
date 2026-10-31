#nullable enable
using System;
using Godot;

namespace ECHOISM;

public enum NpcType
{
    Pip,
    Bolt,
    Mira,
    Null
}

public partial class NpcCharacter : Area2D, IInteractable, IResettable
{
    [Export]
    public string StableId { get; set; } = "npc-pip";

    [Export]
    public NpcType CharacterType { get; set; } = NpcType.Pip;

    [Export]
    public string CharacterName { get; set; } = "Pip";

    [Export]
    public string RoleTitle { get; set; } = "Courier Blob";

    [Export(PropertyHint.MultilineText)]
    public string[] DialogueLines { get; set; } =
    [
        "Greg! You made it across the broken crossing!",
        "Someone broadcast a message saying the districts are cutting each other off...",
        "I tried to deliver the mail, but the relays were shut down. We have to find the real source!"
    ];

    [Export]
    public bool AcceptsEchoes { get; set; } = false;

    public bool HasTalked { get; private set; }

    public static event Action<NpcCharacter>? DialogueStarted;

    // Palette per character (WCAG AAA compliant and vibrant)
    private static readonly Color PipFill = new("#10B981");
    private static readonly Color PipOutline = new("#064E3B");
    private static readonly Color PipAccent = new("#34D399");

    private static readonly Color BoltFill = new("#F97316");
    private static readonly Color BoltOutline = new("#7C2D12");
    private static readonly Color BoltAccent = new("#FDBA74");

    private static readonly Color MiraFill = new("#FACC15");
    private static readonly Color MiraOutline = new("#713F12");
    private static readonly Color MiraAccent = new("#FEF08A");

    private static readonly Color NullFill = new("#7C3AED");
    private static readonly Color NullOutline = new("#2E1065");
    private static readonly Color NullAccent = new("#C4B5FD");

    private static readonly Color EyeColor = new("#0B132B");
    private static readonly Color EyeSpecular = new("#FFFFFF");

    private float _bobTimer;
    private CollisionShape2D? _collision;

    public override void _Ready()
    {
        CollisionLayer = 2; // Interactable layer for Greg
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

    public override void _Process(double delta)
    {
        _bobTimer += (float)delta * 2.5f;
        QueueRedraw();
    }

    public string GetPrompt(GregCharacter actor)
    {
        return HasTalked ? $"Speak with {CharacterName} again" : $"Talk to {CharacterName}";
    }

    public bool TryInteract(GregCharacter actor)
    {
        HasTalked = true;
        SoundManager.Instance?.Play(SoundEffect.Interact);
        DialogueStarted?.Invoke(this);
        return true;
    }

    public void ResetState()
    {
        // Dialogue read state is preserved across loop resets within sector
        QueueRedraw();
    }

    public Color GetPrimaryColor() => CharacterType switch
    {
        NpcType.Pip => PipFill,
        NpcType.Bolt => BoltFill,
        NpcType.Mira => MiraFill,
        NpcType.Null => NullFill,
        _ => PipFill
    };

    public override void _Draw()
    {
        var bob = Mathf.Sin(_bobTimer) * 1.5f;
        var center = new Vector2(0f, bob);

        switch (CharacterType)
        {
            case NpcType.Pip:
                DrawPip(center);
                break;
            case NpcType.Bolt:
                DrawBolt(center);
                break;
            case NpcType.Mira:
                DrawMira(center);
                break;
            case NpcType.Null:
                DrawNull(center);
                break;
        }
    }

    private void DrawPip(Vector2 center)
    {
        // Pip: Friendly small pear-shaped courier blob
        DrawEllipse(center, 13f, 12f, PipOutline);
        DrawEllipse(center, 11f, 10f, PipFill);

        // Courier cap / antenna feather
        var capBase = center + new Vector2(0f, -11f);
        DrawCircle(capBase, 3.5f, PipAccent);

        // Friendly eyes
        DrawCircle(center + new Vector2(-4f, -2f), 2f, EyeColor);
        DrawCircle(center + new Vector2(4f, -2f), 2f, EyeColor);
        DrawCircle(center + new Vector2(-4.6f, -2.6f), 0.7f, EyeSpecular);
        DrawCircle(center + new Vector2(3.4f, -2.6f), 0.7f, EyeSpecular);

        // Little smile
        DrawArc(center + new Vector2(0f, 2f), 2.5f, 0.2f, Mathf.Pi - 0.2f, 8, EyeColor, 1.2f);
    }

    private void DrawBolt(Vector2 center)
    {
        // BOLT: Sturdy hexagonal industrial operator
        var points = new Vector2[6];
        var innerPoints = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float a = i * Mathf.Pi / 3.0f + Mathf.Pi / 6.0f;
            points[i] = center + new Vector2(Mathf.Cos(a) * 15f, Mathf.Sin(a) * 15f);
            innerPoints[i] = center + new Vector2(Mathf.Cos(a) * 12.5f, Mathf.Sin(a) * 12.5f);
        }
        DrawColoredPolygon(points, BoltOutline);
        DrawColoredPolygon(innerPoints, BoltFill);

        // Industrial visor line
        DrawLine(center + new Vector2(-7f, -2f), center + new Vector2(7f, -2f), BoltOutline, 3f);
        DrawLine(center + new Vector2(-6f, -2f), center + new Vector2(6f, -2f), BoltAccent, 2f);

        // Bolt chest emblem
        var boltPts = new Vector2[]
        {
            center + new Vector2(1f, 1f),
            center + new Vector2(-3f, 5f),
            center + new Vector2(0f, 5f),
            center + new Vector2(-1f, 8f),
            center + new Vector2(3f, 4f),
            center + new Vector2(0f, 4f)
        };
        DrawColoredPolygon(boltPts, BoltAccent);
    }

    private void DrawMira(Vector2 center)
    {
        // Mira: Intellectual archive keeper with oval silhouette & visor
        DrawEllipse(center, 12f, 14f, MiraOutline);
        DrawEllipse(center, 10f, 12f, MiraFill);

        // Archive data visor / spectacles
        var visorRect = new Rect2(center.X - 9f, center.Y - 4f, 18f, 5f);
        DrawRect(visorRect, MiraOutline);
        DrawRect(new Rect2(center.X - 8f, center.Y - 3f, 16f, 3f), MiraAccent);

        // Data crystal ornament
        var crystal = new Vector2[]
        {
            center + new Vector2(0f, -14f),
            center + new Vector2(3f, -11f),
            center + new Vector2(0f, -8f),
            center + new Vector2(-3f, -11f)
        };
        DrawColoredPolygon(crystal, MiraAccent);
    }

    private void DrawNull(Vector2 center)
    {
        // NULL: Glitching fragmented signal entity
        var pulse = (Mathf.Sin(_bobTimer * 4f) + 1f) * 0.5f;
        var r = 14f + pulse * 2.5f;

        DrawCircle(center, r + 2f, NullOutline);
        DrawCircle(center, r, NullFill);

        // Glitch lines / signal distortion bars
        for (int i = -2; i <= 2; i++)
        {
            float y = center.Y + i * 4f;
            float glitchOffset = Mathf.Sin(_bobTimer * 6f + i) * 4f;
            DrawLine(new Vector2(center.X - 10f + glitchOffset, y),
                     new Vector2(center.X + 10f + glitchOffset, y),
                     NullAccent, 1.5f);
        }

        // Menacing cold central eye
        DrawCircle(center, 3.5f, EyeColor);
        DrawCircle(center, 1.8f, NullAccent);
    }

    private void DrawEllipse(Vector2 center, float radiusX, float radiusY, Color color)
    {
        var points = new Vector2[24];
        for (int i = 0; i < 24; i++)
        {
            float angle = (float)i / 24 * Mathf.Tau;
            points[i] = center + new Vector2(Mathf.Cos(angle) * radiusX, Mathf.Sin(angle) * radiusY);
        }
        DrawColoredPolygon(points, color);
    }
}
