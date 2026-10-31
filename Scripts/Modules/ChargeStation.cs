#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class ChargeStation : Area2D, IResettable, IPuzzleModule
{
    [Export]
    public string StableId { get; set; } = "charge-station-01";

    public string ModuleId => StableId;

    [Export]
    public string ModuleName { get; set; } = "Charge Station";

    [Export]
    public AcceptedActors AcceptedActors { get; set; } = AcceptedActors.All;

    [Export]
    public float RequiredChargeDuration { get; set; } = 2.0f;

    [Export]
    public float DischargeRate { get; set; } = 1.0f;

    [Export]
    public bool DecaysWhenVacated { get; set; } = true;

    public float CurrentCharge { get; private set; }
    public bool IsFullyCharged => CurrentCharge >= RequiredChargeDuration;
    public bool IsCompleted => IsFullyCharged;

    public ModuleState CurrentState => IsFullyCharged ? ModuleState.Completed : (CurrentCharge > 0f ? ModuleState.Active : ModuleState.Idle);

    public float ChargeRatio => Mathf.Clamp(CurrentCharge / Mathf.Max(0.1f, RequiredChargeDuration), 0f, 1f);

    public event Action<float>? ChargeUpdated;
    public event Action<bool>? BooleanStateChanged;
    public event Action<ModuleState>? StateChanged;

    private readonly HashSet<Node2D> _occupants = [];
    private bool _wasFullyCharged;
    private CollisionShape2D? _collision;

    private static readonly Color PadBg = new("#0B132B");
    private static readonly Color PadBorder = new("#334155");
    private static readonly Color ChargeGlow = new("#F59E0B");
    private static readonly Color FullGlow = new("#10B981");

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 1;

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        _collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (_collision is null)
        {
            _collision = new CollisionShape2D
            {
                Shape = new CircleShape2D { Radius = 26f }
            };
            AddChild(_collision);
        }

        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        bool hasOccupant = _occupants.Count > 0;

        if (hasOccupant)
        {
            if (CurrentCharge < RequiredChargeDuration)
            {
                CurrentCharge = Mathf.Min(RequiredChargeDuration, CurrentCharge + (float)delta);
                ChargeUpdated?.Invoke(ChargeRatio);
                QueueRedraw();
            }
        }
        else if (DecaysWhenVacated && CurrentCharge > 0f)
        {
            CurrentCharge = Mathf.Max(0f, CurrentCharge - (float)delta * DischargeRate);
            ChargeUpdated?.Invoke(ChargeRatio);
            QueueRedraw();
        }

        bool isFull = IsFullyCharged;
        if (isFull != _wasFullyCharged)
        {
            _wasFullyCharged = isFull;
            BooleanStateChanged?.Invoke(isFull);
            StateChanged?.Invoke(CurrentState);
            if (isFull)
            {
                SoundManager.Instance?.Play(SoundEffect.TimingAccurate);
            }
            QueueRedraw();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        _occupants.Add(body);
        SoundManager.Instance?.Play(SoundEffect.SwitchToggle);
    }

    private void OnBodyExited(Node2D body)
    {
        _occupants.Remove(body);
        if (_occupants.Count == 0)
        {
            SoundManager.Instance?.Play(SoundEffect.TimingTick);
        }
    }

    public void ResetState()
    {
        _occupants.Clear();
        CurrentCharge = 0f;
        _wasFullyCharged = false;
        BooleanStateChanged?.Invoke(false);
        StateChanged?.Invoke(CurrentState);
        ChargeUpdated?.Invoke(0f);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 24f, PadBg);
        DrawCircle(Vector2.Zero, 24f, IsFullyCharged ? FullGlow : PadBorder, false, 2f);

        float ratio = ChargeRatio;
        if (ratio > 0f)
        {
            Color arcColor = IsFullyCharged ? FullGlow : ChargeGlow;
            DrawArc(Vector2.Zero, 18f, -Mathf.Pi / 2.0f, -Mathf.Pi / 2.0f + ratio * Mathf.Tau, 32, arcColor, 4f, true);
        }

        DrawCircle(Vector2.Zero, 8f, IsFullyCharged ? FullGlow : (ratio > 0f ? ChargeGlow : PadBorder));
    }
}
