using UnityEngine;
using UnityEngine.Audio;

// Keeps the three volumes (Master, Music, SFX) saved and applied to the AudioMixer.
// It creates itself when the game starts, so the saved volumes are applied in every scene.
// The mixer must be at Assets/Resources/MainMixer with these exposed parameters:
// MasterVolume, MusicVolume and SFXVolume. Its groups must be Master/Music and Master/SFX.
public class VolumeManager : MonoBehaviour
{
    public const string MasterParam = "MasterVolume";
    public const string MusicParam = "MusicVolume";
    public const string SfxParam = "SFXVolume";

    const string MixerResourceName = "MainMixer";
    const float MinVolume = 0.0001f;

    static AudioMixer mixer;
    static bool mixerSearched;

    static AudioMixer Mixer
    {
        get
        {
            if (mixer == null && !mixerSearched)
            {
                mixerSearched = true;
                mixer = Resources.Load<AudioMixer>(MixerResourceName);

                if (mixer == null)
                {
                    Debug.LogWarning("VolumeManager: no AudioMixer called '" + MixerResourceName + "' in a Resources folder.");
                }
            }

            return mixer;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        GameObject go = new GameObject("VolumeManager");
        DontDestroyOnLoad(go);
        go.AddComponent<VolumeManager>();
    }

    // Start, not Awake: setting mixer parameters in Awake can be ignored.
    void Start()
    {
        Apply(MasterParam, GetVolume(MasterParam));
        Apply(MusicParam, GetVolume(MusicParam));
        Apply(SfxParam, GetVolume(SfxParam));
    }

    /// <summary>
    /// Saved volume from 0.0001 to 1 (1 by default).
    /// </summary>
    public static float GetVolume(string param)
    {
        return PlayerPrefs.GetFloat(param, 1f);
    }

    /// <summary>
    /// Applies the volume to the mixer and stores it. Call PlayerPrefs.Save() when the player is done.
    /// </summary>
    public static void SetVolume(string param, float value)
    {
        value = Mathf.Clamp(value, MinVolume, 1f);
        PlayerPrefs.SetFloat(param, value);
        Apply(param, value);
    }

    static void Apply(string param, float value)
    {
        AudioMixer m = Mixer;
        if (m == null) return;

        // Sliders are linear, the mixer works in decibels.
        m.SetFloat(param, Mathf.Log10(Mathf.Clamp(value, MinVolume, 1f)) * 20f);
    }

    public static AudioMixerGroup SfxGroup => FindGroup("Master/SFX");
    public static AudioMixerGroup MusicGroup => FindGroup("Master/Music");

    static AudioMixerGroup FindGroup(string path)
    {
        AudioMixer m = Mixer;
        if (m == null) return null;

        AudioMixerGroup[] groups = m.FindMatchingGroups(path);
        return groups.Length > 0 ? groups[0] : null;
    }

    /// <summary>
    /// Plays a clip through the SFX group (AudioSource.PlayClipAtPoint ignores the mixer).
    /// </summary>
    public static void PlaySfxAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        GameObject go = new GameObject("SfxOneShot");
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = SfxGroup;
        source.Play();

        Destroy(go, clip.length + 0.1f);
    }
}