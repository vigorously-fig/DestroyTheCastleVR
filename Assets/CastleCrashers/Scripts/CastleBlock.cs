using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleBlock : MonoBehaviour, IDamageable
{
    [SerializeField] ParticleSystem breakParticles;
    [SerializeField] ParticleSystem damageParticles;
    [SerializeField] ParticleSystem fireParticles;

    [SerializeField] AudioClip breakSound;
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip fireSound;

    [SerializeField] AudioSource audioSource;

    [SerializeField] private int health = 100;

    private int fireDamage = 2;
    private float fireDamageRate = 0.5f;

    ParticleSystem spawnedFireParticles;
    ParticleSystem spawnedDamageParticles;
    AudioSource spawnedSound;

    private void Awake()
    {
        spawnedFireParticles = Instantiate(fireParticles, transform.position, transform.rotation, transform);
        spawnedDamageParticles = Instantiate(damageParticles, transform.position, transform.rotation, transform);

        spawnedSound = Instantiate(audioSource, transform.position, transform.rotation, transform);
        spawnedSound.clip = fireSound;
        spawnedSound.loop = true;
    }

    public void Damage(int damageValue, bool isFireDamage)
    {
        subtractHealth(damageValue);

        if (isFireDamage && !spawnedFireParticles.isPlaying)
        {
            spawnedFireParticles.Play();
            spawnedSound.Play();
            StartCoroutine(FireDamage());
        }
        else if (!isFireDamage)
        {
            spawnedDamageParticles.Play();
            spawnedSound.PlayOneShot(damageSound);
        }

        checkIfDead();
    }

    void subtractHealth(int damageValue)
    {
        health = Mathf.Clamp(health - damageValue, 0, 100);
        Debug.Log("Health: " + health);
    }

    void checkIfDead()
    {
        if (health <= 0)
        {
            ParticleSystem spawnedBreakParticles = Instantiate(breakParticles, transform.position, transform.rotation);
            spawnedBreakParticles.Play();

            SoundFXManager.instance.PlaySoundFXClip(breakSound, transform, 1);
            Destroy(gameObject);
        }
    }

    IEnumerator FireDamage()
    {
        while (true)
        {
            subtractHealth(fireDamage);
            checkIfDead();
            yield return new WaitForSeconds(fireDamageRate);
        }
    }
}
