using UnityEngine;

[ExecuteAlways]
public class VfxPlaybackSpeedController : MonoBehaviour
{
    [Header("VFX Target")]
    [Tooltip("Prefab or scene object that contains all the child Particle Systems.")]
    [SerializeField] private GameObject vfxRoot;

    [Header("Spawn Settings")]
    [Tooltip("If enabled, the prefab will be instantiated when entering Play Mode.")]
    [SerializeField] private bool instantiateOnPlay = false;

    [Tooltip("Optional parent for the spawned VFX. If empty, this object will be used.")]
    [SerializeField] private Transform spawnParent;

    [Header("Playback")]
    [SerializeField, Min(0f)] private float playbackSpeed = 1f;

    [Header("Controls")]
    [SerializeField] private bool applyInEditMode = true;
    [SerializeField] private bool playOnStart = true;

    private GameObject spawnedVfx;
    private ParticleSystem[] particleSystems;

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        if (instantiateOnPlay)
        {
            SpawnVfx();
        }
        else
        {
            CacheParticleSystems();
            ApplyPlaybackSpeed();
        }

        if (playOnStart)
        {
            Play();
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (!applyInEditMode)
            return;

        CacheParticleSystems();
        ApplyPlaybackSpeed();
    }

    private void OnValidate()
    {
        playbackSpeed = Mathf.Max(0f, playbackSpeed);

        if (!applyInEditMode)
            return;

        CacheParticleSystems();
        ApplyPlaybackSpeed();
    }

    public void SpawnVfx()
    {
        if (vfxRoot == null)
        {
            Debug.LogWarning("VFX Root is missing.");
            return;
        }

        if (spawnedVfx != null)
        {
            if (Application.isPlaying)
                Destroy(spawnedVfx);
            else
                DestroyImmediate(spawnedVfx);
        }

        Transform parent = spawnParent != null ? spawnParent : transform;

        spawnedVfx = Instantiate(vfxRoot, parent);
        spawnedVfx.transform.localPosition = Vector3.zero;
        spawnedVfx.transform.localRotation = Quaternion.identity;
        spawnedVfx.transform.localScale = Vector3.one;

        CacheParticleSystems(spawnedVfx);
        ApplyPlaybackSpeed();
    }

    public void SetPlaybackSpeed(float newSpeed)
    {
        playbackSpeed = Mathf.Max(0f, newSpeed);
        ApplyPlaybackSpeed();
    }

    public void ApplyPlaybackSpeed()
    {
        if (particleSystems == null)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ParticleSystem.MainModule main = ps.main;
            main.simulationSpeed = playbackSpeed;
        }
    }

    public void Play()
    {
        if (particleSystems == null)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ps.Play(true);
        }
    }

    public void Stop()
    {
        if (particleSystems == null)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void Restart()
    {
        if (particleSystems == null)
            return;

        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null)
                continue;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
    }

    private void CacheParticleSystems()
    {
        GameObject root = spawnedVfx != null ? spawnedVfx : vfxRoot;

        if (root == null)
        {
            particleSystems = null;
            return;
        }

        particleSystems = root.GetComponentsInChildren<ParticleSystem>(true);
    }

    private void CacheParticleSystems(GameObject root)
    {
        if (root == null)
        {
            particleSystems = null;
            return;
        }

        particleSystems = root.GetComponentsInChildren<ParticleSystem>(true);
    }
}