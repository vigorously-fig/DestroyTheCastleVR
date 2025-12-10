using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Music / Ambience")]
    public AudioSource musicSource;
    public AudioClip paradeMusic;
    public AudioClip chaosMusic;
    public AudioClip victoryMusic;

    [Header("Crowd Sounds")]
    public AudioSource crowdSource;
    public AudioClip crowdCheer;
    public AudioClip crowdScream;

    [Header("Voice Lines")]
    public List<AudioClip> voiceLines;
    public AudioSource voiceSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayParadeAmbience()
    {
        musicSource.clip = paradeMusic;
        musicSource.loop = true;
        musicSource.Play();

        crowdSource.clip = crowdCheer;
        crowdSource.loop = true;
        crowdSource.Play();
    }

    bool chaosStarted = false;

    public void TriggerChaosMode()
    {
        if (chaosStarted) return;
        chaosStarted = true;

        StartCoroutine(CrossfadeMusic(chaosMusic, 2f));

        crowdSource.clip = crowdScream;
        crowdSource.loop = true;
        crowdSource.Play();
    }


    public void PlayVictoryMusic()
    {
        chaosStarted = true; // lock chaos out during win sequence

        // Stop parade / chaos crowd sounds
        crowdSource.Stop();

        // Immediately stop current music so victory track is clean
        musicSource.Stop();
        musicSource.volume = 1f; // reset volume before starting fade

        // Start victory music
        StartCoroutine(CrossfadeMusic(victoryMusic, 2f));
    }


    public void PlayRandomVoiceLine()
    {
        if (voiceLines.Count == 0) return;
        voiceSource.PlayOneShot(voiceLines[Random.Range(0, voiceLines.Count)]);
    }

   /* public void PlaySoundFXClip(AudioClip sound , Transform transform, int num)
    {
        AudioSource audioSource.Instantiate();
        audioSource.clip = sound;
        audioSource.volume = num;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }*/

    IEnumerator CrossfadeMusic(AudioClip newClip, float fadeTime)
    {
        float startVol = musicSource.volume;
        while (musicSource.volume > 0)
        {
            musicSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        while (musicSource.volume < startVol)
        {
            musicSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
