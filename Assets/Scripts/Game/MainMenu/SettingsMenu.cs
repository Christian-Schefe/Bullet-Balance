using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TextSlider musicVolumeSlider, sfxVolumeSlider;
    [SerializeField] private BetterButton resetButton;

    private void Awake()
    {
        print("Music volume: " + DataManager.SettingsData.MusicVolume);
        print("Sfx volume: " + DataManager.SettingsData.SfxVolume);
        musicVolumeSlider.SetProgress(DataManager.SettingsData.MusicVolume);
        musicVolumeSlider.onProgressChanged.AddListener(OnMusicVolumeChanged);

        sfxVolumeSlider.SetProgress(DataManager.SettingsData.SfxVolume);
        sfxVolumeSlider.onProgressChanged.AddListener(OnSfxVolumeChanged);

        resetButton.AddClickListener(ResetSettings);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        DataManager.SettingsData.MusicVolume = Mathf.Clamp01(volume);
    }

    private void OnSfxVolumeChanged(float volume)
    {
        DataManager.SettingsData.SfxVolume = Mathf.Clamp01(volume);
    }

    private void ResetSettings()
    {
        DataManager.SettingsData.Reset();
        musicVolumeSlider.SetProgress(DataManager.SettingsData.MusicVolume);
        sfxVolumeSlider.SetProgress(DataManager.SettingsData.SfxVolume);
    }
}
