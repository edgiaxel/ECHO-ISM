#nullable enable
using Godot;

namespace ECHOISM;

public enum SoundEffect
{
    Interact,
    SwitchToggle,
    DoorOpen,
    TimingTick,
    TimingAccurate,
    TimingMiss,
    LoopReset,
    LevelComplete,
    UiClick
}

public partial class SoundManager : Node
{
    public static SoundManager Instance { get; private set; } = null!;

    [Export]
    public bool AudioEnabled { get; set; } = true;

    [Export(PropertyHint.Range, "0,1,0.05")]
    public float MasterVolume { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0,1,0.05")]
    public float MusicVolume { get; set; } = 0.5f;

    private AudioStreamPlayer _sfxPlayer = null!;
    private AudioStreamPlayer _ambientPlayer = null!;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;

        _sfxPlayer = new AudioStreamPlayer { Name = "SfxPlayer" };
        AddChild(_sfxPlayer);

        _ambientPlayer = new AudioStreamPlayer { Name = "AmbientPlayer" };
        AddChild(_ambientPlayer);

        StartAmbientHum();
    }

    public void Play(SoundEffect effect)
    {
        if (!AudioEnabled)
        {
            return;
        }

        var stream = GenerateProceduralSound(effect);
        if (stream is not null)
        {
            var tempPlayer = new AudioStreamPlayer
            {
                Stream = stream,
                VolumeDb = Mathf.LinearToDb(MasterVolume)
            };
            AddChild(tempPlayer);
            tempPlayer.Finished += () => tempPlayer.QueueFree();
            tempPlayer.Play();
        }
    }

    public void ToggleAudio()
    {
        AudioEnabled = !AudioEnabled;
        if (AudioEnabled)
        {
            StartAmbientHum();
        }
        else
        {
            _ambientPlayer.Stop();
        }
    }

    public void PlayUiClick() => Play(SoundEffect.UiClick);
    public void PlayInteract() => Play(SoundEffect.Interact);
    public void PlaySwitchToggle() => Play(SoundEffect.SwitchToggle);
    public void PlayDoorOpen() => Play(SoundEffect.DoorOpen);
    public void PlayTimingTick() => Play(SoundEffect.TimingTick);
    public void PlayTimingAccurate() => Play(SoundEffect.TimingAccurate);
    public void PlayTimingMiss() => Play(SoundEffect.TimingMiss);
    public void PlayLoopReset() => Play(SoundEffect.LoopReset);
    public void PlayLevelComplete() => Play(SoundEffect.LevelComplete);

    private void StartAmbientHum()
    {
        if (!AudioEnabled)
        {
            return;
        }

        // Generate subtle ambient synth drone for the digital city atmosphere
        const int sampleRate = 22050;
        const float duration = 4.0f;
        int totalSamples = (int)(sampleRate * duration);
        var bytes = new byte[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / sampleRate;
            // Warm sub-bass fundamental (55Hz / A1) plus gentle 110Hz harmonic
            float val = Mathf.Sin(t * Mathf.Tau * 55f) * 0.12f + Mathf.Sin(t * Mathf.Tau * 110f) * 0.04f;
            // 8-bit PCM unsigned [0..255]
            bytes[i] = (byte)Mathf.Clamp((val + 1f) * 127.5f, 0f, 255f);
        }

        var sample = new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format8Bits,
            MixRate = sampleRate,
            Data = bytes,
            LoopMode = AudioStreamWav.LoopModeEnum.Forward,
            LoopBegin = 0,
            LoopEnd = totalSamples
        };

        _ambientPlayer.Stream = sample;
        _ambientPlayer.VolumeDb = Mathf.LinearToDb(MusicVolume * 0.4f);
        _ambientPlayer.Play();
    }

    private static AudioStreamWav GenerateProceduralSound(SoundEffect effect)
    {
        const int sampleRate = 22050;
        float duration;
        float freqStart;
        float freqEnd;
        float amp = 0.35f;

        switch (effect)
        {
            case SoundEffect.UiClick:
                duration = 0.04f;
                freqStart = 880f;
                freqEnd = 1200f;
                amp = 0.25f;
                break;
            case SoundEffect.Interact:
                duration = 0.08f;
                freqStart = 520f;
                freqEnd = 780f;
                amp = 0.3f;
                break;
            case SoundEffect.SwitchToggle:
                duration = 0.1f;
                freqStart = 340f;
                freqEnd = 580f;
                amp = 0.35f;
                break;
            case SoundEffect.DoorOpen:
                duration = 0.35f;
                freqStart = 180f;
                freqEnd = 440f;
                amp = 0.4f;
                break;
            case SoundEffect.TimingTick:
                duration = 0.03f;
                freqStart = 660f;
                freqEnd = 660f;
                amp = 0.2f;
                break;
            case SoundEffect.TimingAccurate:
                duration = 0.22f;
                freqStart = 523.25f; // C5
                freqEnd = 1046.5f;  // C6
                amp = 0.45f;
                break;
            case SoundEffect.TimingMiss:
                duration = 0.18f;
                freqStart = 320f;
                freqEnd = 160f;
                amp = 0.3f;
                break;
            case SoundEffect.LoopReset:
                duration = 0.3f;
                freqStart = 440f;
                freqEnd = 220f;
                amp = 0.4f;
                break;
            case SoundEffect.LevelComplete:
                duration = 0.6f;
                freqStart = 440f;
                freqEnd = 880f;
                amp = 0.5f;
                break;
            default:
                duration = 0.05f;
                freqStart = 440f;
                freqEnd = 440f;
                break;
        }

        int totalSamples = (int)(sampleRate * duration);
        var bytes = new byte[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / totalSamples;
            float timeSec = (float)i / sampleRate;
            float currentFreq = Mathf.Lerp(freqStart, freqEnd, t);
            // Linear decay envelope
            float envelope = (1.0f - t);
            float val = Mathf.Sin(timeSec * Mathf.Tau * currentFreq) * amp * envelope;

            bytes[i] = (byte)Mathf.Clamp((val + 1f) * 127.5f, 0f, 255f);
        }

        return new AudioStreamWav
        {
            Format = AudioStreamWav.FormatEnum.Format8Bits,
            MixRate = sampleRate,
            Data = bytes
        };
    }
}
