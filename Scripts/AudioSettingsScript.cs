using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsScript : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider weaponsSlider;

    void Start()
    {
        // Load saved settings or use default (1.0f)
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        weaponsSlider.value = PlayerPrefs.GetFloat("WeaponsVolume", 1f);

        // Apply volumes
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetWeaponsVolume(weaponsSlider.value);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetWeaponsVolume(float value)
    {
        audioMixer.SetFloat("WeaponsVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("WeaponsVolume", value);
    }
}
