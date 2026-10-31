#nullable enable
using Godot;

namespace ECHOISM;

public partial class Sector04SignalTower : LevelContainer
{
    public TestSwitch GateTrigger { get; private set; } = null!;
    public TimedGate TowerBarrier { get; private set; } = null!;
    public SignalRelay RelayChannel1 { get; private set; } = null!;
    public SignalRelay RelayChannel2 { get; private set; } = null!;
    public Door AntennaGate { get; private set; } = null!;
    public NpcCharacter NullEntity { get; private set; } = null!;
    public NarrativeTerminal UplinkTerminal { get; private set; } = null!;
    public ExitGoal TransitExit { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        if (GetNodeOrNull<RoomGeometry>("RoomGeometry") is { } geom && geom.PartitionWalls.Count == 0)
        {
            geom.PartitionWalls = new()
            {
                new Rect2(172, 0, 16, 166),
                new Rect2(172, 254, 16, 166),
                new Rect2(442, 0, 16, 166),
                new Rect2(442, 254, 16, 166)
            };
            geom.SetRoomLayout(geom.RoomBounds, geom.StartingPosition, partitions: geom.PartitionWalls);
        }

        GateTrigger = GetNode<TestSwitch>("PuzzleModules/GateTrigger");
        TowerBarrier = GetNode<TimedGate>("PuzzleModules/TowerBarrier");
        RelayChannel1 = GetNode<SignalRelay>("PuzzleModules/RelayChannel1");
        RelayChannel2 = GetNode<SignalRelay>("PuzzleModules/RelayChannel2");
        AntennaGate = GetNode<Door>("PuzzleModules/AntennaGate");
        NullEntity = GetNode<NpcCharacter>("PuzzleModules/NullEntity");
        UplinkTerminal = GetNode<NarrativeTerminal>("PuzzleModules/UplinkTerminal");
        TransitExit = GetNode<ExitGoal>("PuzzleModules/ExitGoal");

        // Trigger switch pulses the timed gate
        GateTrigger.StateChanged += active =>
        {
            if (active)
            {
                TowerBarrier.Trigger();
            }
        };

        // Dual signal channel synchronization
        RelayChannel1.StateChanged += _ => CheckAntennaAccess();
        RelayChannel2.StateChanged += _ => CheckAntennaAccess();

        // NULL entity dialogue
        NullEntity.DialogueLines =
        [
            "You are too late, small maintenance unit.",
            "Fear and uncertainty spread much faster than your little echoes.",
            "Even if you transmit the broadcast, the Heartline will cascade before anyone listens!"
        ];
    }

    private void CheckAntennaAccess()
    {
        bool bothChannelsActive = RelayChannel1.IsCompleted && RelayChannel2.IsCompleted;
        AntennaGate.SetOpen(bothChannelsActive);
        if (bothChannelsActive)
        {
            ObjectiveText = "Antenna uplink opened! Confront NULL's interference and broadcast the verified transmission.";
        }
    }
}
