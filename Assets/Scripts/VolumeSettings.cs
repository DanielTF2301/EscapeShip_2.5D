using UnityEngine;
using UnityEngine.UI;

// Put it on the options panel (main menu and pause menu). Three sliders: Master, Music and SFX.
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Awake()
    {
        Setup(masterSlider, VolumeManager.MasterParam);
        Setup(musicSlider, VolumeManager.MusicParam);
        Setup(sfxSlider, VolumeManager.SfxParam);
    }

    // Show the saved values every time the panel opens.
    void OnEnable()
    {
        Refresh(masterSlider, VolumeManager.MasterParam);
        Refresh(musicSlider, VolumeManager.MusicParam);
        Refresh(sfxSlider, VolumeManager.SfxParam);
    }

    // Write the volumes to disk when the panel closes.
    void OnDisable()
    {
        PlayerPrefs.Save();
    }

    void Setup(Slider slider, string param)
    {
        if (slider == null) return;

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.onValueChanged.AddListener(value => VolumeManager.SetVolume(param, value));
    }

    void Refresh(Slider slider, string param)
    {
        if (slider == null) return;
        slider.SetValueWithoutNotify(VolumeManager.GetVolume(param));
    }
}