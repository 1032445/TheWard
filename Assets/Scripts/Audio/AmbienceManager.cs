using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    [Serializable]
    private class AmbienceTrack
    {
        public AmbienceState state;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.35f;
    }

    public static AmbienceManager Instance { get; private set; }

    [SerializeField] private AmbienceState defaultState = AmbienceState.Default;
    [SerializeField] private AmbienceTrack[] tracks;
    [SerializeField] private float fadeSeconds = 2f;
    [SerializeField] private bool playDefaultOnStart = true;

    private readonly List<AmbienceZone> activeZones = new List<AmbienceZone>();
    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private Coroutine fadeRoutine;
    private AmbienceState currentState;
    private bool hasState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("multiple ambience managers found");
            return;
        }

        Instance = this;
        CreateAudioSources();
    }

    private void Start()
    {
        if (playDefaultOnStart)
        {
            SetState(defaultState, instant: true);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetState(AmbienceState state, bool instant = false)
    {
        if (hasState && currentState == state)
        {
            return;
        }

        AmbienceTrack track = FindTrack(state);
        currentState = state;
        hasState = true;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(CrossfadeTo(track, instant));
    }

    public void RegisterZone(AmbienceZone zone)
    {
        if (zone == null || activeZones.Contains(zone))
        {
            return;
        }

        activeZones.Add(zone);
        ApplyHighestPriorityZone();
    }

    public void UnregisterZone(AmbienceZone zone)
    {
        if (zone == null)
        {
            return;
        }

        activeZones.Remove(zone);
        ApplyHighestPriorityZone();
    }

    private void ApplyHighestPriorityZone()
    {
        AmbienceZone bestZone = null;

        foreach (AmbienceZone zone in activeZones)
        {
            if (zone == null)
            {
                continue;
            }

            if (bestZone == null || zone.Priority > bestZone.Priority)
            {
                bestZone = zone;
            }
        }

        SetState(bestZone != null ? bestZone.State : defaultState);
    }

    private void CreateAudioSources()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        ConfigureSource(sourceA);
        ConfigureSource(sourceB);

        activeSource = sourceA;
        inactiveSource = sourceB;
    }

    private void ConfigureSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0f;
    }

    private IEnumerator CrossfadeTo(AmbienceTrack track, bool instant)
    {
        AudioClip nextClip = track != null ? track.clip : null;
        float targetVolume = track != null ? track.volume : 0f;

        if (nextClip == null)
        {
            yield return FadeOutActive(instant);
            fadeRoutine = null;
            yield break;
        }

        if (activeSource.clip == nextClip)
        {
            yield return FadeSource(activeSource, activeSource.volume, targetVolume, instant ? 0f : fadeSeconds);
            fadeRoutine = null;
            yield break;
        }

        inactiveSource.clip = nextClip;
        inactiveSource.volume = instant ? targetVolume : 0f;
        inactiveSource.Play();

        if (instant)
        {
            activeSource.Stop();
            activeSource.volume = 0f;
            SwapSources();
            fadeRoutine = null;
            yield break;
        }

        float duration = Mathf.Max(0.01f, fadeSeconds);
        float elapsed = 0f;
        float startActiveVolume = activeSource.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            activeSource.volume = Mathf.Lerp(startActiveVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f;
        inactiveSource.volume = targetVolume;
        SwapSources();
        fadeRoutine = null;
    }

    private IEnumerator FadeOutActive(bool instant)
    {
        if (instant)
        {
            activeSource.Stop();
            activeSource.volume = 0f;
            yield break;
        }

        yield return FadeSource(activeSource, activeSource.volume, 0f, fadeSeconds);
        activeSource.Stop();
    }

    private IEnumerator FadeSource(AudioSource source, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            source.volume = to;
            yield break;
        }

        float elapsed = 0f;
        float safeDuration = Mathf.Max(0.01f, duration);

        while (elapsed < safeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / safeDuration));
            yield return null;
        }

        source.volume = to;
    }

    private void SwapSources()
    {
        AudioSource previousActive = activeSource;
        activeSource = inactiveSource;
        inactiveSource = previousActive;
    }

    private AmbienceTrack FindTrack(AmbienceState state)
    {
        if (tracks == null)
        {
            return null;
        }

        foreach (AmbienceTrack track in tracks)
        {
            if (track != null && track.state == state)
            {
                return track;
            }
        }

        return null;
    }
}
