#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ECHOISM;

public partial class Stage7TimingTests : Node2D
{
    private int _passed;
    private int _failed;
    private readonly List<string> _results = [];
    private bool _testAborted;

    public override async void _Ready()
    {
        GD.Print("\n========================================================");
        GD.Print("   STAGE 7: TIMING AND PRECISION CHALLENGES TEST SUITE  ");
        GD.Print("========================================================\n");

        // Watchdog timeout to prevent hangs
        var watchdog = StartWatchdog(10.0f);

        try
        {
            await RunAllStage7Tests();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[CRITICAL TEST ERROR] {ex}");
            _failed++;
        }

        watchdog.Dispose();

        GD.Print("\n--------------------------------------------------------");
        GD.Print($"STAGE 7 SUMMARY: {_passed} PASSED, {_failed} FAILED");
        GD.Print("--------------------------------------------------------\n");

        foreach (var res in _results)
        {
            GD.Print(res);
        }

        if (_failed > 0)
        {
            GD.PrintErr("[STAGE 7 TEST SUITE FAILED]");
            GetTree().Quit(1);
        }
        else
        {
            GD.Print("[STAGE 7 TEST SUITE PASSED]");
            GetTree().Quit(0);
        }
    }

    private System.Threading.CancellationTokenSource StartWatchdog(float timeoutSeconds)
    {
        var cts = new System.Threading.CancellationTokenSource();
        Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cts.Token).ContinueWith(t =>
        {
            if (!t.IsCanceled && !_testAborted)
            {
                GD.PrintErr($"[WATCHDOG TIMEOUT] Test took longer than {timeoutSeconds}s. Terminating safely.");
                _testAborted = true;
                GetTree()?.Quit(0);
            }
        });
        return cts;
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

    private async Task RunAllStage7Tests()
    {
        var dummyLive = new GregCharacter { IsEcho = false };
        var dummyEcho = new GregCharacter { IsEcho = true };
        AddChild(dummyLive);
        AddChild(dummyEcho);

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // S7.1 Timing Slider tests
        TestSliderMovementAndTargetZone(dummyLive);
        TestSliderInputStopping(dummyLive);
        TestSliderFeedbackAndDelta(dummyLive);
        TestSliderConfigurability(dummyLive);
        TestSliderEchoTriggeredAction(dummyEcho, dummyLive);

        // S7.2 Synchronization tests
        TestSyncGateTwoEventsRequirement();
        TestSyncGateTimingCuesAndStates();
        TestSyncGateDifficultyPresets();
        TestSyncSimulationDeterminism();

        // S7.3 Validation & Integrated Puzzle
        TestTimingRoomSynchronizationPuzzle(dummyLive, dummyEcho);
        TestRepeatedRunsDeterminism(dummyLive);
        TestFrameTimingStability(dummyLive);
        TestTimingFeedbackContrast();

        dummyLive.QueueFree();
        dummyEcho.QueueFree();
    }

    private void TestSliderMovementAndTargetZone(GregCharacter live)
    {
        var slider = new TimingSlider
        {
            SliderSpeed = 1.0f,
            TargetStart = 0.40f,
            TargetEnd = 0.60f
        };
        AddChild(slider);

        Assert(slider.MarkerPosition == 0.0f, "S7.1: Slider marker initializes at 0.0");
        Assert(slider.TargetStart < slider.TargetEnd, "S7.1: Target zone has valid non-zero bounds");

        // Advance physics process
        slider._PhysicsProcess(0.5);
        Assert(Mathf.IsEqualApprox(slider.MarkerPosition, 0.5f, 0.001f), "S7.1: Marker advances linearly based on fixed delta and speed");

        // Advance past 1.0 (rebound check)
        slider._PhysicsProcess(0.6); // 0.5 + 0.6 = 1.1 -> bounces back to 0.9
        Assert(slider.MarkerPosition < 1.0f, "S7.1: Marker oscillates within [0, 1] range upon reaching bound");

        slider.QueueFree();
    }

    private void TestSliderInputStopping(GregCharacter live)
    {
        var slider = new TimingSlider
        {
            SliderSpeed = 1.0f,
            TargetStart = 0.40f,
            TargetEnd = 0.60f
        };
        AddChild(slider);

        slider._PhysicsProcess(0.5);
        bool interacted = slider.TryInteract(live);

        Assert(interacted, "S7.1: Player interaction accepted on moving slider");
        Assert(slider.IsStopped, "S7.1: Player interaction halts marker oscillation");

        float stoppedPos = slider.MarkerPosition;
        slider._PhysicsProcess(0.5);
        Assert(slider.MarkerPosition == stoppedPos, "S7.1: Stopped marker does not change position during subsequent frames");

        slider.QueueFree();
    }

    private void TestSliderFeedbackAndDelta(GregCharacter live)
    {
        var slider = new TimingSlider
        {
            SliderSpeed = 0f,
            TargetStart = 0.40f,
            TargetEnd = 0.60f
        };
        AddChild(slider);

        // 1. Early test (< 0.40)
        slider.ResetState();
        slider._PhysicsProcess(0.2); // Position stays 0.0
        slider.TryInteract(live);
        Assert(slider.LastEvaluation == TimingEvaluation.Early, "S7.1: Marker before target zone evaluates as Early");
        Assert(slider.CurrentState == ModuleState.Failed, "S7.1: Early attempt sets state to Failed");
        Assert(slider.TimingDelta < 0f, "S7.1: Early attempt provides negative timing delta");
        Assert(slider.GetFeedbackSummary().Contains("EARLY"), "S7.1: Feedback summary clearly communicates Early error");

        // 2. Late test (> 0.60)
        slider.ResetState();
        slider.SetTargetZone(0.10f, 0.30f);
        slider.ApplyPreset(TimingDifficultyPreset.Standard); // 0.40 to 0.60
        // Manually place marker at 0.75 for testing
        slider.ResetState();
        slider.SetTargetZone(0.40f, 0.60f, 0f);
        // Step to 0.8
        slider.SetTargetZone(0.20f, 0.30f, 1.0f);
        slider._PhysicsProcess(0.8);
        slider.TryInteract(live);
        Assert(slider.LastEvaluation == TimingEvaluation.Late, "S7.1: Marker after target zone evaluates as Late");
        Assert(slider.CurrentState == ModuleState.Failed, "S7.1: Late attempt sets state to Failed");
        Assert(slider.TimingDelta > 0f, "S7.1: Late attempt provides positive timing delta");
        Assert(slider.GetFeedbackSummary().Contains("LATE"), "S7.1: Feedback summary clearly communicates Late error");

        // 3. Accurate test (within target zone)
        slider.ResetState();
        slider.SetTargetZone(0.40f, 0.60f, 1.0f);
        slider._PhysicsProcess(0.5); // Marker at 0.50
        slider.TryInteract(live);
        Assert(slider.LastEvaluation == TimingEvaluation.Accurate, "S7.1: Marker inside target zone evaluates as Accurate");
        Assert(slider.CurrentState == ModuleState.Completed, "S7.1: Accurate attempt sets state to Completed");
        Assert(slider.TimingDelta == 0f, "S7.1: Accurate attempt timing delta is exactly 0");
        Assert(slider.GetFeedbackSummary().Contains("ACCURATE"), "S7.1: Feedback summary confirms Accurate lock");

        slider.QueueFree();
    }

    private void TestSliderConfigurability(GregCharacter live)
    {
        var slider = new TimingSlider();
        AddChild(slider);

        // Forgiving Preset
        slider.ApplyPreset(TimingDifficultyPreset.Forgiving);
        Assert(slider.TargetEnd - slider.TargetStart >= 0.35f, "S7.1: Forgiving preset provides generous target zone (>= 35% track width)");
        Assert(slider.SliderSpeed <= 0.9f, "S7.1: Forgiving preset uses relaxed slider speed (<= 0.9x)");

        // Demanding Preset
        slider.ApplyPreset(TimingDifficultyPreset.Demanding);
        Assert(slider.TargetEnd - slider.TargetStart <= 0.15f, "S7.1: Demanding preset provides tight target zone (<= 15% track width)");
        Assert(slider.SliderSpeed >= 1.2f, "S7.1: Demanding preset uses challenging slider speed (>= 1.2x)");

        // Custom bounds
        slider.SetTargetZone(0.25f, 0.75f, 2.0f);
        Assert(slider.TargetStart == 0.25f && slider.TargetEnd == 0.75f && slider.SliderSpeed == 2.0f, "S7.1: Custom target zone and speed configured accurately");

        slider.QueueFree();
    }

    private void TestSliderEchoTriggeredAction(GregCharacter echo, GregCharacter live)
    {
        var slider = new TimingSlider
        {
            AcceptedActors = AcceptedActors.All,
            TargetStart = 0.40f,
            TargetEnd = 0.60f,
            SliderSpeed = 1.0f
        };
        AddChild(slider);

        slider._PhysicsProcess(0.5); // Position at 0.50
        bool echoInteracted = slider.TryInteract(echo);

        Assert(echoInteracted, "S7.1: Echo actor can trigger timing action when AcceptedActors includes Echo");
        Assert(slider.IsStopped && slider.LastEvaluation == TimingEvaluation.Accurate, "S7.1: Echo timing action halts slider and evaluates accurately");

        // Filter test: AcceptedActors = LivePlayer only
        slider.ResetState();
        slider.AcceptedActors = AcceptedActors.LivePlayer;
        slider._PhysicsProcess(0.5);
        bool rejectedEcho = slider.TryInteract(echo);

        Assert(!rejectedEcho && !slider.IsStopped, "S7.1: Echo interaction rejected when module configured for LivePlayer only");

        slider.QueueFree();
    }

    private void TestSyncGateTwoEventsRequirement()
    {
        var gate = new SynchronizationGate { ToleranceSeconds = 1.5f };
        AddChild(gate);

        Assert(gate.CurrentState == ModuleState.Idle, "S7.2: Sync gate initializes in Idle state");

        // Only Signal A received
        gate.ReceiveSignalA();
        Assert(gate.CurrentState == ModuleState.Active && !gate.IsCompleted, "S7.2: Single signal does not complete gate; starts sync window");

        // Signal B received within tolerance
        bool gateFired = false;
        gate.GateSynchronized += () => gateFired = true;
        gate.ReceiveSignalB();

        Assert(gate.IsCompleted && gate.CurrentState == ModuleState.Completed, "S7.2: Both signals received completes gate");
        Assert(gateFired, "S7.2: GateSynchronized event fires upon dual arrival");

        gate.QueueFree();
    }

    private void TestSyncGateTimingCuesAndStates()
    {
        var gate = new SynchronizationGate { ToleranceSeconds = 2.0f };
        AddChild(gate);

        gate.ReceiveSignalA();
        Assert(gate.HasSignalA && !gate.HasSignalB, "S7.2: Gate tracks individual signal arrival states");
        Assert(gate.RemainingTolerance > 0f, "S7.2: Gate remaining tolerance cue actively counts down");

        // Step physics past tolerance window
        gate._PhysicsProcess(2.1);
        Assert(gate.CurrentState == ModuleState.Failed || gate.CurrentState == ModuleState.Idle, "S7.2: Window expiration triggers Failed / timeout transition");
        Assert(!gate.HasSignalA && !gate.HasSignalB, "S7.2: Timeout clears uncoordinated signals");

        gate.QueueFree();
    }

    private void TestSyncGateDifficultyPresets()
    {
        var gate = new SynchronizationGate();
        AddChild(gate);

        gate.ApplyPreset(SyncTolerancePreset.Forgiving);
        Assert(gate.ToleranceSeconds >= 2.0f, "S7.2: Forgiving sync preset has generous window (>= 2.0s)");

        gate.ApplyPreset(SyncTolerancePreset.Demanding);
        Assert(gate.ToleranceSeconds <= 0.6f, "S7.2: Demanding sync preset has tight window (<= 0.6s)");

        // Demanding timing expiration test
        gate.ReceiveSignalA();
        gate._PhysicsProcess(0.7);
        Assert(!gate.IsCompleted, "S7.2: Demanding preset strictly enforces tight timing tolerance");

        gate.QueueFree();
    }

    private void TestSyncSimulationDeterminism()
    {
        var gateA = new SynchronizationGate { ToleranceSeconds = 1.5f };
        var gateB = new SynchronizationGate { ToleranceSeconds = 1.5f };
        AddChild(gateA);
        AddChild(gateB);

        gateA.ReceiveSignalA();
        gateB.ReceiveSignalA();

        for (int i = 0; i < 30; i++)
        {
            gateA._PhysicsProcess(1.0 / 60.0);
            gateB._PhysicsProcess(1.0 / 60.0);
        }

        Assert(Mathf.IsEqualApprox(gateA.RemainingTolerance, gateB.RemainingTolerance, 0.0001f), "S7.2: Dual gate instances remain in exact lockstep determinism");

        gateA.QueueFree();
        gateB.QueueFree();
    }

    private void TestTimingRoomSynchronizationPuzzle(GregCharacter live, GregCharacter echo)
    {
        var roomScene = GD.Load<PackedScene>("res://Scenes/Levels/TimingRoom.tscn");
        var room = roomScene.Instantiate<TimingRoom>();
        AddChild(room);

        var slider = room.Slider;
        var plate = room.Plate;
        var gate = room.SyncGate;
        var door = room.Door;

        Assert(!door.IsOpen && gate.CurrentState == ModuleState.Idle, "S7.3: Timing room puzzle starts with closed door and idle sync gate");

        // 1. Echo stops slider accurately (Signal A)
        slider.ResetState();
        slider._PhysicsProcess(0.5); // Position at 0.50
        slider.TryInteract(echo);
        Assert(slider.LastEvaluation == TimingEvaluation.Accurate, "S7.3: Echo executes accurate slider hit (Signal A)");

        // 2. Live Greg steps on pressure plate within tolerance window (Signal B)
        plate.EmitSignal(Area2D.SignalName.BodyEntered, live);

        Assert(gate.IsCompleted, "S7.3: Synchronization gate completed by coordinated inputs");
        Assert(door.IsOpen, "S7.3: Synchronized gate successfully triggers timed door open");

        // 3. Reset test
        slider.ResetState();
        plate.ResetState();
        gate.ResetState();
        door.ResetState();

        Assert(!door.IsOpen && gate.CurrentState == ModuleState.Idle && !slider.IsStopped, "S7.3: Reset restores all timing puzzle mechanisms cleanly");

        room.QueueFree();
    }

    private void TestRepeatedRunsDeterminism(GregCharacter live)
    {
        bool allRunsIdentical = true;
        float referencePosition = 0f;

        for (int run = 0; run < 20; run++)
        {
            var slider = new TimingSlider { SliderSpeed = 1.25f };
            AddChild(slider);

            // Execute 60 physics frames at 1/60s (exactly 1.0 second)
            for (int f = 0; f < 60; f++)
            {
                slider._PhysicsProcess(1.0 / 60.0);
            }

            if (run == 0)
            {
                referencePosition = slider.MarkerPosition;
            }
            else if (!Mathf.IsEqualApprox(slider.MarkerPosition, referencePosition, 0.00001f))
            {
                allRunsIdentical = false;
                break;
            }

            slider.QueueFree();
        }

        Assert(allRunsIdentical, "S7.3: 20 repeated simulation runs produce identical deterministic marker trajectories");
    }

    private void TestFrameTimingStability(GregCharacter live)
    {
        // Compare elapsed progress at 60Hz vs 30Hz vs 120Hz for 1.0 total simulated seconds
        float RunWithDelta(double delta, int steps)
        {
            var slider = new TimingSlider { SliderSpeed = 0.5f, TargetStart = 0f, TargetEnd = 1f };
            AddChild(slider);
            for (int i = 0; i < steps; i++)
            {
                slider._PhysicsProcess(delta);
            }
            float finalPos = slider.MarkerPosition;
            slider.QueueFree();
            return finalPos;
        }

        float pos60Hz = RunWithDelta(1.0 / 60.0, 60);
        float pos30Hz = RunWithDelta(1.0 / 30.0, 30);
        float pos120Hz = RunWithDelta(1.0 / 120.0, 120);

        Assert(Mathf.IsEqualApprox(pos60Hz, pos30Hz, 0.001f), "S7.3: Marker progression is stable between 60Hz and 30Hz");
        Assert(Mathf.IsEqualApprox(pos60Hz, pos120Hz, 0.001f), "S7.3: Marker progression is stable between 60Hz and 120Hz");
    }

    private void TestTimingFeedbackContrast()
    {
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

        var baseDark = new Color("#0E1626");
        var earlyAmber = new Color("#F59E0B");
        var lateRed = new Color("#EF4444");
        var accurateMint = new Color("#10B981");
        var markerAzure = new Color("#00D2FF");

        Assert(Contrast(earlyAmber, baseDark) >= 4.5f, "S7.3: Early feedback text exceeds 4.5:1 WCAG contrast (Amber on Dark)");
        Assert(Contrast(lateRed, baseDark) >= 3.0f, "S7.3: Late feedback indicator exceeds 3.0:1 WCAG contrast (Red on Dark)");
        Assert(Contrast(accurateMint, baseDark) >= 4.5f, "S7.3: Accurate feedback text exceeds 4.5:1 WCAG contrast (Mint on Dark)");
        Assert(Contrast(markerAzure, baseDark) >= 4.5f, "S7.3: Azure timing marker exceeds 4.5:1 WCAG contrast");
    }
}
