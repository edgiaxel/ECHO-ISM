#nullable enable
using System.Collections.Generic;
using Godot;

namespace ECHOISM;

public partial class BackgroundBlobRunner : Node2D
{
    private readonly List<Vector2> _waypoints =
    [
        new Vector2(200f, 220f),
        new Vector2(500f, 220f),
        new Vector2(500f, 440f),
        new Vector2(850f, 440f),
        new Vector2(850f, 220f),
        new Vector2(1080f, 220f),
        new Vector2(1080f, 520f),
        new Vector2(200f, 520f)
    ];

    private float _primaryProgress;
    private float _echoProgress;
    private const float Speed = 160f;
    private float _totalPathLength;

    private static readonly Color GridColor = new(0.08f, 0.12f, 0.18f, 0.5f);
    private static readonly Color WallColor = new(0.12f, 0.16f, 0.24f, 0.8f);
    private static readonly Color DimOverlay = new(0.04f, 0.06f, 0.09f, 0.75f);
    private static readonly Color PadColor = new(0.08f, 0.22f, 0.32f, 0.4f);
    private static readonly Color GregFill = new(0.0f, 0.82f, 1.0f, 0.75f);
    private static readonly Color GregOutline = new("#081B33");
    private static readonly Color EchoFill = new(0.505f, 0.549f, 0.973f, 0.45f);
    private static readonly Color EchoOutline = new(0.22f, 0.74f, 0.97f, 0.5f);

    public override void _Ready()
    {
        _totalPathLength = CalculateTotalLength();
        _primaryProgress = 0f;
        _echoProgress = Mathf.Max(0f, _totalPathLength - 200f);
    }

    public override void _Process(double delta)
    {
        float step = Speed * (float)delta;
        _primaryProgress = (_primaryProgress + step) % _totalPathLength;
        _echoProgress = (_echoProgress + step) % _totalPathLength;
        QueueRedraw();
    }

    private float CalculateTotalLength()
    {
        float length = 0f;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            var next = _waypoints[(i + 1) % _waypoints.Count];
            length += _waypoints[i].DistanceTo(next);
        }
        return length;
    }

    private (Vector2 position, Vector2 facing) SamplePath(float distance)
    {
        float current = 0f;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            var a = _waypoints[i];
            var b = _waypoints[(i + 1) % _waypoints.Count];
            float seg = a.DistanceTo(b);

            if (current + seg >= distance || i == _waypoints.Count - 1)
            {
                float t = seg > 0.001f ? (distance - current) / seg : 0f;
                var pos = a.Lerp(b, Mathf.Clamp(t, 0f, 1f));
                var dir = (b - a).Normalized();
                return (pos, dir);
            }

            current += seg;
        }

        return (_waypoints[0], Vector2.Right);
    }

    public override void _Draw()
    {
        // 1. Subtle grid lines
        for (float x = 0; x < 1280; x += 64)
        {
            DrawLine(new Vector2(x, 0), new Vector2(x, 720), GridColor, 1f);
        }
        for (float y = 0; y < 720; y += 64)
        {
            DrawLine(new Vector2(0, y), new Vector2(1280, y), GridColor, 1f);
        }

        // 2. Chamber boundary outline
        DrawRect(new Rect2(160, 140, 960, 440), WallColor, false, 2f);

        // 3. Mock interactive pads
        DrawRect(new Rect2(480, 200, 40, 40), PadColor);
        DrawRect(new Rect2(480, 200, 40, 40), WallColor, false, 1.5f);
        DrawRect(new Rect2(830, 420, 40, 40), PadColor);
        DrawRect(new Rect2(830, 420, 40, 40), WallColor, false, 1.5f);

        // 4. Draw Echo blob
        var (echoPos, echoFacing) = SamplePath(_echoProgress);
        DrawBlob(echoPos, echoFacing, EchoFill, EchoOutline, true);

        // 5. Draw Primary Greg blob
        var (gregPos, gregFacing) = SamplePath(_primaryProgress);
        DrawBlob(gregPos, gregFacing, GregFill, GregOutline, false);

        // 6. Dim overlay to make menu text pop
        DrawRect(new Rect2(0, 0, 1280, 720), DimOverlay);
    }

    private void DrawBlob(Vector2 position, Vector2 facing, Color fill, Color outline, bool isEcho)
    {
        if (isEcho)
        {
            DrawArc(position, 17f, 0f, Mathf.Tau, 24, outline, 1.5f, true);
        }

        DrawCircle(position, 12f, outline);
        DrawCircle(position, 9.5f, fill);

        var forward = facing.Normalized();
        var side = new Vector2(-forward.Y, forward.X);
        var eyeAnchor = position + forward * 1.5f;
        var leftEye = eyeAnchor + side * 3.2f;
        var rightEye = eyeAnchor - side * 3.2f;

        DrawCircle(leftEye, 2.0f, new Color("#081B33"));
        DrawCircle(rightEye, 2.0f, new Color("#081B33"));
        DrawCircle(leftEye + new Vector2(-0.5f, -0.5f), 0.6f, Colors.White);
        DrawCircle(rightEye + new Vector2(-0.5f, -0.5f), 0.6f, Colors.White);
    }
}
