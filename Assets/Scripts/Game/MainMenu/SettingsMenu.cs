using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TextSlider musicVolumeSlider, sfxVolumeSlider;
    [SerializeField] private BetterButton resetButton;

    private void Awake()
    {
        print("Music volume: " + DataManger.SettingsData.MusicVolume);
        print("Sfx volume: " + DataManger.SettingsData.SfxVolume);
        musicVolumeSlider.SetProgress(DataManger.SettingsData.MusicVolume);
        musicVolumeSlider.onProgressChanged.AddListener(OnMusicVolumeChanged);

        sfxVolumeSlider.SetProgress(DataManger.SettingsData.SfxVolume);
        sfxVolumeSlider.onProgressChanged.AddListener(OnSfxVolumeChanged);

        resetButton.AddClickListener(ResetSettings);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        DataManger.SettingsData.MusicVolume = Mathf.Clamp01(volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        DataManger.SettingsData.SfxVolume = Mathf.Clamp01(volume);
    }

    private void ResetSettings()
    {
        DataManger.SettingsData.Reset();
        musicVolumeSlider.SetProgress(DataManger.SettingsData.MusicVolume);
        sfxVolumeSlider.SetProgress(DataManger.SettingsData.SfxVolume);
    }
}
