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

    public void UpdateBattleIntensity(float destructionPercent)
    {
        if (destructionPercent > 0.25f && musicSource.clip != chaosMusic)
        {
            StartCoroutine(CrossfadeMusic(chaosMusic, 2f));
            crowdSource.clip = crowdScream;
            crowdSource.Play();
        }
    }

    public void PlayVictoryMusic()
    {
        StartCoroutine(CrossfadeMusic(victoryMusic, 2f));
        crowdSource.Stop();
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
