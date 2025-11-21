using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject fireballPrefab;

    //Sound + VFX
    private AudioSource audioSource;

    [SerializeField] private AudioClip wandSoundEffect;   // <-- FIXED
    [SerializeField] private ParticleSystem particles;

    //Wand settings
    [SerializeField] public float wandForce = 500f;
    [SerializeField] public float cooldownDelay = 1.5f;
    private Coroutine cooldownTimer;
    [SerializeField] private Transform shootPoint;

    private float wandSoundPitchMin = 0.8f;
    private float wandSoundPitchMax = 1.5f;

    private bool shouldCast = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Trigger()
    {
        if (!shouldCast) return;

        GameObject spawnedMagic = Instantiate(
            fireballPrefab,
            transform.position + (transform.right * -1) + transform.forward,
            transform.rotation
        );

        Rigidbody rb = spawnedMagic.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * wandForce, ForceMode.Impulse);

        PlayFX();

        shouldCast = false;
        cooldownTimer = StartCoroutine(Cooldown());
    }

    private void PlayFX()
    {
        audioSource.pitch = Random.Range(wandSoundPitchMin, wandSoundPitchMax);
        audioSource.PlayOneShot(wandSoundEffect);   // <-- WILL NOW WORK

        if (particles != null)
            particles.Play();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);
        shouldCast = true;
    }
}

