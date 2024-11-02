using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxSystem : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (!Globals<SfxSystem>.RegisterOrDestroy(this)) return;
        DataManager.SettingsData.sfxVolumeStore.AddListener(ListenerLifetime.Global, OnSfxVolumeChanged);
    }

    private void OnSfxVolumeChanged(bool isPresent, float value)
    {
        sfxSource.volume = isPresent ? value : 1;
    }

    public static void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        Globals<SfxSystem>.Instance.sfxSource.PlayOneShot(clip);
    }
}
