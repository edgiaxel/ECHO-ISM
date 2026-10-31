#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class VerificationTerminal : Area2D, IInteractable, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "verification-terminal-m07";

    public string StableId => ModuleId;

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    public bool AcceptsEchoes => (AcceptedActors & AcceptedActors.Echo) != 0;

    [Export]
    public string DataPayload { get; set; } = "NODE_PACKET_VERIFIED";

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsVerified => IsCompleted;

    public event Action<ModuleState>? StateChanged;
    public event Action<string>? Verified;

    private static readonly Color PedestalBody = new("#0E1626");
    private static readonly Color BorderIdle = new("#334155");
    private static readonly Color ScreenUnverified = new("#F59E0B");
    private static readonly Color ScreenVerified = new("#10B981");

    public string GetPrompt(GregCharacter actor)
    {
        return IsVerified ? "Data integrity verified" : "Verify information packet";
    }

    public bool TryInteract(GregCharacter actor)
    {
        if (!ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            return false;
        }

        if (IsVerified)
        {
            return false;
        }

        CurrentState = ModuleState.Completed;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        Verified?.Invoke(DataPayload);
        return true;
    }

    public void ResetState()
    {
        CurrentState = ModuleState.Idle;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
    }

    public override void _Draw()
    {
        var baseRect = new Rect2(-18f, -18f, 36f, 36f);
        DrawRect(baseRect, PedestalBody);
        DrawRect(baseRect, IsVerified ? ScreenVerified : BorderIdle, false, 2f);

        var screenRect = new Rect2(-11f, -11f, 22f, 22f);
        Color screenColor = IsVerified ? ScreenVerified : ScreenUnverified;
        DrawRect(screenRect, new Color(screenColor, 0.25f));
        DrawRect(screenRect, screenColor, false, 1.5f);

        if (IsVerified)
        {
            DrawLine(new Vector2(-6f, 0f), new Vector2(-1f, 5f), ScreenVerified, 2.5f);
            DrawLine(new Vector2(-1f, 5f), new Vector2(6f, -5f), ScreenVerified, 2.5f);
        }
        else
        {
            DrawLine(new Vector2(-5f, -5f), new Vector2(5f, 5f), ScreenUnverified, 2f);
            DrawLine(new Vector2(-5f, 5f), new Vector2(5f, -5f), ScreenUnverified, 2f);
        }
    }
}
