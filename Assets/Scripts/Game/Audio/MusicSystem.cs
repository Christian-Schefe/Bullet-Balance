using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private List<AudioClip> musicClipList;

    private int currentMusicIndex;
    private bool startedPlaying = false;

    private void Awake()
    {
        if (!Globals<MusicSystem>.RegisterOrDestroy(this)) return;

        DataManger.SettingsData.musicVolumeStore.AddListener(ListenerLifetime.Global, OnMusicVolumeChanged);

        currentMusicIndex = Random.Range(0, musicClipList.Count);

        if (!startedPlaying)
        {
            PlayNextMusic();
            startedPlaying = true;
        }

    }

    private void Update()
    {
        if (musicSource.clip == null) return;

        if (!musicSource.isPlaying)
        {
            PlayNextMusic();
        }
    }

    private void PlayNextMusic()
    {
        musicSource.clip = musicClipList[currentMusicIndex];
        musicSource.Play();

        print("Playing music: " + musicSource.clip.name);
        currentMusicIndex = (currentMusicIndex + 1) % musicClipList.Count;
    }

    private void OnMusicVolumeChanged(bool isPresent, float value)
    {
        musicSource.volume = isPresent ? value : 1;
    }
}
