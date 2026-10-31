#nullable enable
using System;
using Godot;

namespace ECHOISM;

[Flags]
public enum AcceptedActors
{
    None = 0,
    LivePlayer = 1 << 0,
    Echo = 1 << 1,
    All = LivePlayer | Echo
}

public enum ModuleState
{
    Idle,
    Active,
    Failed,
    Completed
}

public interface IPuzzleModule : IResettable
{
    string ModuleId { get; }
    AcceptedActors AcceptedActors { get; }
    ModuleState CurrentState { get; }
    bool IsCompleted { get; }
    event Action<ModuleState> StateChanged;
}

public static class ModuleActorCheck
{
    public static bool AcceptsActor(AcceptedActors accepted, GregCharacter? actor)
    {
        if (actor is null)
        {
            return false;
        }

        if (actor.IsEcho)
        {
            return (accepted & AcceptedActors.Echo) != 0;
        }

        return (accepted & AcceptedActors.LivePlayer) != 0;
    }
}
