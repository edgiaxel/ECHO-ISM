#nullable enable
using System;
using Godot;

namespace ECHOISM;

public partial class ConnectionRepair : Area2D, IInteractable, IPuzzleModule
{
    [Export]
    public string ModuleId { get; set; } = "connection-repair-m10";

    public string StableId => ModuleId;

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    public bool AcceptsEchoes => (AcceptedActors & AcceptedActors.Echo) != 0;

    [Export]
    public Vector2 TargetOffset { get; set; } = new(60f, 0f);

    public ModuleState CurrentState { get; private set; } = ModuleState.Idle;
    public bool IsCompleted => CurrentState == ModuleState.Completed;
    public bool IsRepaired => IsCompleted;

    public event Action<ModuleState>? StateChanged;
    public event Action? ConnectionRestored;

    private static readonly Color BaseColor = new("#0E1626");
    private static readonly Color TraceBroken = new("#F59E0B");
    private static readonly Color TraceRepaired = new("#00D2FF");
    private static readonly Color NodeIdle = new("#334155");
    private static readonly Color NodeRepaired = new("#10B981");

    public string GetPrompt(GregCharacter actor)
    {
        return IsRepaired ? "Circuit connection intact" : "Repair broken connection";
    }

    public bool TryInteract(GregCharacter actor)
    {
        if (!ModuleActorCheck.AcceptsActor(AcceptedActors, actor))
        {
            return false;
        }

        if (IsRepaired)
        {
            return false;
        }

        CurrentState = ModuleState.Completed;
        QueueRedraw();
        StateChanged?.Invoke(CurrentState);
        ConnectionRestored?.Invoke();
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
        var terminalA = Vector2.Zero;
        var terminalB = TargetOffset;

        DrawCircle(terminalA, 10f, BaseColor);
        DrawCircle(terminalA, 10f, IsRepaired ? NodeRepaired : NodeIdle, false, 2f);

        DrawCircle(terminalB, 8f, BaseColor);
        DrawCircle(terminalB, 8f, IsRepaired ? NodeRepaired : NodeIdle, false, 2f);

        if (IsRepaired)
        {
            DrawLine(terminalA, terminalB, TraceRepaired, 3.5f);
            DrawCircle((terminalA + terminalB) / 2f, 4f, TraceRepaired);
        }
        else
        {
            var mid = (terminalA + terminalB) / 2f;
            DrawLine(terminalA, terminalA + (mid - terminalA) * 0.7f, TraceBroken, 2f);
            DrawLine(mid + (terminalB - mid) * 0.3f, terminalB, TraceBroken, 2f);
            DrawCircle(mid, 3f, TraceBroken, false, 1.5f);
        }
    }
}
