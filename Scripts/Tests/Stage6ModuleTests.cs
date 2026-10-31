#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class Stage6ModuleTests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];

    public override async void _Ready()
    {
        GD.Print("\n==================================================");
        GD.Print("     STAGE 6: REUSABLE PUZZLE MODULE TEST SUITE   ");
        GD.Print("==================================================\n");

        try
        {
            await RunAllTests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        GD.Print("\n--------------------------------------------------");
        GD.Print($"SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[TEST SUITE PASSED]");
            GetTree().Quit(0);
        }
    }

    private async System.Threading.Tasks.Task RunAllTests()
    {
        var dummyLive = new GregCharacter { IsEcho = false };
        var dummyEcho = new GregCharacter { IsEcho = true };
        AddChild(dummyLive);
        AddChild(dummyEcho);

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        TestPressurePlate(dummyLive, dummyEcho);
        TestDualMechanismDoor();
        TestTimedDoor();
        TestTimingSlider(dummyLive, dummyEcho);
        TestSynchronizationGate();
        TestMovingObstacle();
        TestVerificationTerminal(dummyLive, dummyEcho);
        TestSignalRelay(dummyLive);
        TestEchoSensor(dummyLive, dummyEcho);
        TestConnectionRepair(dummyLive);
        TestEventPreventionObjective();
        TestModuleActorCheckMatrix(dummyLive, dummyEcho);
        TestInvalidAndEdgeConfigurations();
        TestVisualContrast();
    }

    private void Assert(bool condition, string testName)
    {
        if (condition)
        {
            _passed++;
            _results.Add($"  [PASS] {testName}");
            GD.Print($"[PASS] {testName}");
        }
        else
        {
            _failed++;
            _results.Add($"  [FAIL] {testName}");
            GD.PrintErr($"[FAIL] {testName}");
        }
    }

    private void TestPressurePlate(GregCharacter live, GregCharacter echo)
    {
        var plate = new PressurePlate { Mode = PressurePlateMode.Momentary, AcceptedActors = AcceptedActors.All };
        AddChild(plate);

        Assert(plate.CurrentState == ModuleState.Idle, "PressurePlate: Initial state is Idle");

        // Test Live player entry
        plate.EmitSignal(Area2D.SignalName.BodyEntered, live);
        Assert(plate.CurrentState == ModuleState.Active && plate.IsCompleted, "PressurePlate: Active on live player entry");

        // Test Live player exit
        plate.EmitSignal(Area2D.SignalName.BodyExited, live);
        Assert(plate.CurrentState == ModuleState.Idle, "PressurePlate: Returns to Idle on player exit (Momentary)");

        // Test Echo compatibility
        plate.EmitSignal(Area2D.SignalName.BodyEntered, echo);
        Assert(plate.CurrentState == ModuleState.Active, "PressurePlate: Echo activates plate when AcceptedActors includes Echo");

        // Test Reset
        plate.ResetState();
        Assert(plate.CurrentState == ModuleState.Idle, "PressurePlate: ResetState restores Idle");

        // Test actor exclusion filter
        plate.AcceptedActors = AcceptedActors.Echo;
        plate.EmitSignal(Area2D.SignalName.BodyEntered, live);
        Assert(plate.CurrentState == ModuleState.Idle, "PressurePlate: Live player ignored when AcceptedActors is Echo-only");

        plate.QueueFree();
    }

    private void TestDualMechanismDoor()
    {
        var door = new DualMechanismDoor { Mode = DualMechanismMode.Simultaneous };
        AddChild(door);

        Assert(!door.IsOpen && door.CurrentState == ModuleState.Idle, "DualMechanismDoor: Initial state is closed and Idle");

        door.SetInputA(true);
        Assert(!door.IsOpen && door.CurrentState == ModuleState.Active, "DualMechanismDoor: One active input keeps door closed (Active state)");

        door.SetInputB(true);
        Assert(door.IsOpen && door.CurrentState == ModuleState.Completed, "DualMechanismDoor: Both inputs active opens door (Completed state)");

        door.SetInputA(false);
        Assert(!door.IsOpen && door.CurrentState == ModuleState.Active, "DualMechanismDoor: Removing Input A closes simultaneous door");

        // Test sequential mode
        var seqDoor = new DualMechanismDoor { Mode = DualMechanismMode.Sequential };
        AddChild(seqDoor);
        seqDoor.SetInputB(true);
        Assert(!seqDoor.IsOpen, "DualMechanismDoor: Sequential mode rejects Input B before Input A");
        seqDoor.SetInputA(true);
        Assert(seqDoor.IsOpen, "DualMechanismDoor: Sequential mode opens once sequence is satisfied");

        seqDoor.ResetState();
        Assert(!seqDoor.IsOpen && seqDoor.CurrentState == ModuleState.Idle, "DualMechanismDoor: ResetState restores closed Idle");

        door.QueueFree();
        seqDoor.QueueFree();
    }

    private void TestTimedDoor()
    {
        var door = new TimedDoor { WindowDurationSeconds = 1.0f };
        AddChild(door);

        Assert(!door.IsOpen, "TimedDoor: Initial state is closed");

        door.TriggerOpen();
        Assert(door.IsOpen && door.CurrentState == ModuleState.Completed, "TimedDoor: TriggerOpen opens door");

        // Advance time past duration
        door._Process(1.2);
        Assert(!door.IsOpen && door.CurrentState == ModuleState.Idle, "TimedDoor: Window expiration closes door");

        door.TriggerOpen();
        door.ResetState();
        Assert(!door.IsOpen && door.RemainingWindowTime == 0f, "TimedDoor: ResetState closes door and resets timer");

        door.QueueFree();
    }

    private void TestTimingSlider(GregCharacter live, GregCharacter echo)
    {
        var slider = new TimingSlider
        {
            SliderSpeed = 0f,
            TargetStart = 0.40f,
            TargetEnd = 0.60f,
            AcceptedActors = AcceptedActors.All
        };
        AddChild(slider);

        Assert(slider.CurrentState == ModuleState.Idle, "TimingSlider: Initial state is Idle");

        // Early attempt (< 0.40)
        slider.TryInteract(live);
        Assert(slider.LastEvaluation == TimingEvaluation.Early && slider.CurrentState == ModuleState.Failed, "TimingSlider: Position < TargetStart evaluates Early");

        slider.ResetState();
        Assert(slider.CurrentState == ModuleState.Idle, "TimingSlider: ResetState resets evaluation and Idle state");

        // Echo compatibility rejection test
        slider.AcceptedActors = AcceptedActors.LivePlayer;
        bool echoAllowed = slider.TryInteract(echo);
        Assert(!echoAllowed, "TimingSlider: Rejects Echo when AcceptedActors is LivePlayer-only");

        slider.QueueFree();
    }

    private void TestSynchronizationGate()
    {
        var gate = new SynchronizationGate { ToleranceSeconds = 1.0f };
        AddChild(gate);

        Assert(gate.CurrentState == ModuleState.Idle, "SynchronizationGate: Initial state is Idle");

        gate.ReceiveSignalA();
        Assert(gate.CurrentState == ModuleState.Active && gate.HasSignalA, "SynchronizationGate: Signal A starts sync window (Active state)");

        gate.ReceiveSignalB();
        Assert(gate.CurrentState == ModuleState.Completed && gate.IsCompleted, "SynchronizationGate: Signal B within tolerance completes gate");

        // Timeout test
        gate.ResetState();
        gate.ReceiveSignalA();
        gate._Process(1.5);
        Assert(gate.CurrentState == ModuleState.Idle && !gate.HasSignalA, "SynchronizationGate: Timeout without Signal B resets to Idle");

        gate.QueueFree();
    }

    private void TestMovingObstacle()
    {
        var obstacle = new MovingObstacle
        {
            Position = Vector2.Zero,
            TargetOffset = new Vector2(100f, 0f),
            CycleDuration = 2.0f,
            MovementType = ObstacleMovementType.OneWay
        };
        AddChild(obstacle);

        Assert(obstacle.CurrentState == ModuleState.Active, "MovingObstacle: Initial state is Active when moving is enabled");

        obstacle._PhysicsProcess(1.0);
        Assert(obstacle.Position.X > 0f, "MovingObstacle: PhysicsProcess advances obstacle position");

        obstacle.SetMovingEnabled(false);
        Assert(obstacle.CurrentState == ModuleState.Idle, "MovingObstacle: Disabling movement sets state to Idle");

        obstacle.ResetState();
        Assert(obstacle.Position == Vector2.Zero, "MovingObstacle: ResetState returns to initial position");

        obstacle.QueueFree();
    }

    private void TestVerificationTerminal(GregCharacter live, GregCharacter echo)
    {
        var terminal = new VerificationTerminal
        {
            ModuleId = "test-term",
            AcceptedActors = AcceptedActors.LivePlayer,
            DataPayload = "RESTORED_NODE_HASH"
        };
        AddChild(terminal);

        Assert(!terminal.IsVerified && terminal.CurrentState == ModuleState.Idle, "VerificationTerminal: Initial state is Unverified (Idle)");

        // Echo rejection test
        bool echoResult = terminal.TryInteract(echo);
        Assert(!echoResult && !terminal.IsVerified, "VerificationTerminal: Rejects Echo when configured for LivePlayer-only");

        // Live interaction test
        string receivedPayload = string.Empty;
        terminal.Verified += payload => receivedPayload = payload;
        bool liveResult = terminal.TryInteract(live);

        Assert(liveResult && terminal.IsVerified && terminal.CurrentState == ModuleState.Completed, "VerificationTerminal: Live player verifies terminal");
        Assert(receivedPayload == "RESTORED_NODE_HASH", "VerificationTerminal: Emits verified data payload signal");

        terminal.ResetState();
        Assert(!terminal.IsVerified && terminal.CurrentState == ModuleState.Idle, "VerificationTerminal: ResetState reverts to Unverified");

        terminal.QueueFree();
    }

    private void TestSignalRelay(GregCharacter live)
    {
        var relay = new SignalRelay { TransmissionDelaySeconds = 0f };
        AddChild(relay);

        Assert(relay.CurrentState == ModuleState.Idle, "SignalRelay: Initial state is Idle");

        bool transmitted = false;
        relay.SignalTransmitted += () => transmitted = true;

        relay.TryInteract(live);
        Assert(relay.IsCompleted && transmitted, "SignalRelay: Immediate transmission completes on interaction");

        relay.ResetState();
        Assert(relay.CurrentState == ModuleState.Idle, "SignalRelay: ResetState returns to Idle");

        relay.QueueFree();
    }

    private void TestEchoSensor(GregCharacter live, GregCharacter echo)
    {
        var sensor = new EchoSensor
        {
            AcceptedActors = AcceptedActors.Echo,
            RequiredEchoCount = 1
        };
        AddChild(sensor);

        Assert(sensor.CurrentState == ModuleState.Idle, "EchoSensor: Initial state is Idle");

        // Live player ignored test
        sensor.EmitSignal(Area2D.SignalName.BodyEntered, live);
        Assert(sensor.CurrentState == ModuleState.Idle && sensor.DetectedCount == 0, "EchoSensor: Ignores Live player when configured for Echo-only");

        // Echo detected test
        sensor.EmitSignal(Area2D.SignalName.BodyEntered, echo);
        Assert(sensor.IsCompleted && sensor.DetectedCount == 1, "EchoSensor: Detects Echo and completes state");

        sensor.ResetState();
        Assert(sensor.CurrentState == ModuleState.Idle && sensor.DetectedCount == 0, "EchoSensor: ResetState clears detected count and restores Idle");

        sensor.QueueFree();
    }

    private void TestConnectionRepair(GregCharacter live)
    {
        var repair = new ConnectionRepair();
        AddChild(repair);

        Assert(!repair.IsRepaired && repair.CurrentState == ModuleState.Idle, "ConnectionRepair: Initial state is Broken (Idle)");

        bool restored = false;
        repair.ConnectionRestored += () => restored = true;

        repair.TryInteract(live);
        Assert(repair.IsRepaired && repair.IsCompleted && restored, "ConnectionRepair: Repair completes and emits ConnectionRestored");

        repair.ResetState();
        Assert(!repair.IsRepaired && repair.CurrentState == ModuleState.Idle, "ConnectionRepair: ResetState breaks connection again");

        repair.QueueFree();
    }

    private void TestEventPreventionObjective()
    {
        var objective = new EventPreventionObjective
        {
            CountdownDuration = 2.0f,
            RequiredInterventions = 1
        };
        AddChild(objective);

        Assert(objective.CurrentState == ModuleState.Idle, "EventPreventionObjective: Initial state is Idle");

        objective.StartCountdown();
        Assert(objective.CurrentState == ModuleState.Active, "EventPreventionObjective: StartCountdown sets Active state");

        // Test timeout failure
        objective._Process(2.5);
        Assert(objective.IsFailed && objective.CurrentState == ModuleState.Failed, "EventPreventionObjective: Timer expiration causes event failure");

        // Test prevention success
        objective.ResetState();
        objective.StartCountdown();
        objective.AddIntervention();
        Assert(objective.IsCompleted && objective.CurrentState == ModuleState.Completed, "EventPreventionObjective: Intervention prevents event");

        objective.ResetState();
        Assert(objective.CurrentState == ModuleState.Idle && objective.CompletedInterventions == 0, "EventPreventionObjective: ResetState restores Idle");

        objective.QueueFree();
    }

    private void TestModuleActorCheckMatrix(GregCharacter live, GregCharacter echo)
    {
        Assert(ModuleActorCheck.AcceptsActor(AcceptedActors.All, live), "ModuleActorCheck: All accepts live player");
        Assert(ModuleActorCheck.AcceptsActor(AcceptedActors.All, echo), "ModuleActorCheck: All accepts echo");
        Assert(ModuleActorCheck.AcceptsActor(AcceptedActors.LivePlayer, live), "ModuleActorCheck: LivePlayer accepts live player");
        Assert(!ModuleActorCheck.AcceptsActor(AcceptedActors.LivePlayer, echo), "ModuleActorCheck: LivePlayer rejects echo");
        Assert(!ModuleActorCheck.AcceptsActor(AcceptedActors.Echo, live), "ModuleActorCheck: Echo rejects live player");
        Assert(ModuleActorCheck.AcceptsActor(AcceptedActors.Echo, echo), "ModuleActorCheck: Echo accepts echo");
        Assert(!ModuleActorCheck.AcceptsActor(AcceptedActors.None, live), "ModuleActorCheck: None rejects live player");
        Assert(!ModuleActorCheck.AcceptsActor(AcceptedActors.None, echo), "ModuleActorCheck: None rejects echo");
    }

    private void TestInvalidAndEdgeConfigurations()
    {
        // 1. AcceptedActors.None on interactable
        var plate = new PressurePlate { AcceptedActors = AcceptedActors.None };
        var live = new GregCharacter { IsEcho = false };
        plate.EmitSignal(Area2D.SignalName.BodyEntered, live);
        Assert(plate.CurrentState == ModuleState.Idle, "EdgeConfig: AcceptedActors.None rejects all bodies safely");

        // 2. TimedDoor with zero or negative duration
        var timedDoor = new TimedDoor { WindowDurationSeconds = 0f };
        timedDoor.TriggerOpen();
        timedDoor._Process(0.01);
        Assert(!timedDoor.IsOpen, "EdgeConfig: TimedDoor with 0s window closes immediately and safely");

        // 3. TimingSlider with inverted bounds
        var slider = new TimingSlider { TargetStart = 0.8f, TargetEnd = 0.2f };
        slider.TryInteract(live);
        Assert(slider.LastEvaluation != TimingEvaluation.None, "EdgeConfig: Inverted TimingSlider bounds evaluated safely without exception");

        // 4. SynchronizationGate with zero tolerance
        var gate = new SynchronizationGate { ToleranceSeconds = 0f };
        gate.ReceiveSignalA();
        gate.ReceiveSignalB();
        Assert(gate.IsCompleted, "EdgeConfig: Zero-tolerance sync gate synchronizes with simultaneous signals");

        plate.QueueFree();
        timedDoor.QueueFree();
        slider.QueueFree();
        gate.QueueFree();
        live.QueueFree();
    }

    private void TestVisualContrast()
    {
        // Verify WCAG luminance contrast for core module color palette
        float Contrast(Color c1, Color c2)
        {
            float Lum(Color c)
            {
                float R = c.R <= 0.03928f ? c.R / 12.92f : Mathf.Pow((c.R + 0.055f) / 1.055f, 2.4f);
                float G = c.G <= 0.03928f ? c.G / 12.92f : Mathf.Pow((c.G + 0.055f) / 1.055f, 2.4f);
                float B = c.B <= 0.03928f ? c.B / 12.92f : Mathf.Pow((c.B + 0.055f) / 1.055f, 2.4f);
                return 0.2126f * R + 0.7152f * G + 0.0722f * B;
            }
            float l1 = Lum(c1);
            float l2 = Lum(c2);
            return (Mathf.Max(l1, l2) + 0.05f) / (Mathf.Min(l1, l2) + 0.05f);
        }

        var background = new Color("#0E1626");
        var textWhite = new Color("#F8FAFC");
        var activeAzure = new Color("#00D2FF");
        var verifiedMint = new Color("#10B981");
        var echoSpectral = new Color("#818CF8");
        var warningAmber = new Color("#F59E0B");

        float textRatio = Contrast(textWhite, background);
        float azureRatio = Contrast(activeAzure, background);
        float mintRatio = Contrast(verifiedMint, background);
        float echoRatio = Contrast(echoSpectral, background);
        float amberRatio = Contrast(warningAmber, background);

        Assert(textRatio >= 4.5f, $"Contrast: White text on dark base ({textRatio:0.00}:1 >= 4.5:1)");
        Assert(azureRatio >= 3.0f, $"Contrast: Electric Azure non-text UI ({azureRatio:0.00}:1 >= 3.0:1)");
        Assert(mintRatio >= 3.0f, $"Contrast: Verified Mint UI ({mintRatio:0.00}:1 >= 3.0:1)");
        Assert(echoRatio >= 3.0f, $"Contrast: Echo Spectral UI ({echoRatio:0.00}:1 >= 3.0:1)");
        Assert(amberRatio >= 3.0f, $"Contrast: Warning Amber UI ({amberRatio:0.00}:1 >= 3.0:1)");
    }
}
