using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject cannonballPrefab;

    AudioSource audioSource;
    [SerializeField] ParticleSystem particles;
    Animator cannonAnimator;

    private float cannonballForce = 1000f * 7f;
    private float cooldownDelay = 3f;
    Coroutine cooldownTimer;

    private float explosionSoundPitchMin = 0.5f;
    private float explosionSoundPitchMax = 1.2f;

    private bool shouldFire = true;

    public void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        cannonAnimator = gameObject.GetComponent<Animator>();
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
        particles.Play();
        StartCoroutine("playShoot");
    }

    IEnumerator doCooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);
        shouldFire = true;
    }

    IEnumerator playShoot()
    {
        cannonAnimator.SetBool("PlayShoot", true);
        yield return new WaitForSeconds(0.1f);
        cannonAnimator.SetBool("PlayShoot", false);
    }
}
