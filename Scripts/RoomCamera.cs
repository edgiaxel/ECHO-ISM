using Godot;

namespace ECHOISM;

public partial class RoomCamera : Camera2D
{
    private float _shakeIntensity;
    private float _shakeDuration;
    private float _shakeTimer;
    private Vector2 _originalOffset = Vector2.Zero;

    public override void _Ready()
    {
        _originalOffset = Offset;
    }

    public override void _Process(double delta)
    {
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= (float)delta;
            var progress = _shakeTimer / _shakeDuration;
            var currentIntensity = _shakeIntensity * progress;
            Offset = _originalOffset + new Vector2(
                (float)GD.RandRange(-currentIntensity, currentIntensity),
                (float)GD.RandRange(-currentIntensity, currentIntensity)
            );

            if (_shakeTimer <= 0f)
            {
                Offset = _originalOffset;
            }
        }
    }

    public void Shake(float intensity = 3.5f, float duration = 0.2f)
    {
        _shakeIntensity = intensity;
        _shakeDuration = duration;
        _shakeTimer = duration;
    }
}
