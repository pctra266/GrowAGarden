using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    [Header("Gán Audio Mixer vào đây")]
    public AudioMixer mainMixer;

    [Header("Gán các Slider vào đây")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        LoadSettings();
    }

    public void OnSettingsOpen()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;


        mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterVol) * 20);
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(musicVol) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }



    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}