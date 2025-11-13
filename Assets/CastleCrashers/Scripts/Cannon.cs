using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject cannonballPrefab;

    AudioSource audioSource;
    ParticleSystem particles;

    private float cannonballForce = 1000f;
    private float cooldownDelay = 3f;
    Coroutine cooldownTimer;

    private float explosionSoundPitchMin = 0.5f;
    private float explosionSoundPitchMax = 1.2f;

    private bool shouldFire = true;

    public void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        //particles = gameObject.GetComponent<ParticleSystem>();
    }

    public void Trigger()
    {
        if (shouldFire)
        {
            GameObject spawnedCannonball = Instantiate(cannonballPrefab, transform.position + transform.right*-2 + transform.forward*1, transform.rotation);
            spawnedCannonball.GetComponent<Rigidbody>().AddForce((transform.right*-2 + transform.forward*0.5f) * cannonballForce);

            playFX();

            shouldFire = false;
            cooldownTimer = StartCoroutine("doCooldown");
        }
    }

    public void playFX()
    {
        audioSource.pitch = Random.Range(explosionSoundPitchMin, explosionSoundPitchMax);
        audioSource.Play();
    }

    IEnumerator doCooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);
        shouldFire = true;
    }
}
