using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleBlock : MonoBehaviour, IDamageable
{
    //public GameObject damageText;

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
    private bool isOnFire = false;
    private bool isDead = false;

    ParticleSystem spawnedFireParticles;
    ParticleSystem spawnedDamageParticles;
    AudioSource spawnedSound;

    private float damageSoundVolumeMin = 0.25f;
    private float damageSoundVolumeMax = 0.75f;
    private float damageSoundPitchMin = 0.5f;
    private float damageSoundPitchMax = 2.0f;
    private int breakVolume = 2;

    private void Awake()
    {
        spawnedFireParticles = Instantiate(fireParticles, transform.position, transform.rotation, transform);
        spawnedDamageParticles = Instantiate(damageParticles, transform.position, transform.rotation, transform);

        spawnedSound = Instantiate(audioSource, transform.position, transform.rotation, transform);
        spawnedSound.clip = fireSound;
        spawnedSound.loop = true;

    }

    public void Start()
    {
        // Register block with GameManager
        GameManager.Instance.RegisterCastleBlock(this);
    }

    public void Damage(int damageValue, bool isFireDamage)
    {
        if (isDead) return;

        // Add score for this damage
        GameManager.Instance.AddScore(damageValue);

        subtractHealth(damageValue);

        if (isFireDamage && !isOnFire)
        {
            isOnFire = true;
            spawnedFireParticles.Play();
            spawnedSound.Play();
            StartCoroutine(FireDamage());
        }
        else if (!isFireDamage)
        {
            spawnedDamageParticles.Play();
            spawnedSound.pitch = Random.Range(damageSoundPitchMin, damageSoundPitchMax);
            spawnedSound.PlayOneShot(damageSound, Random.Range(damageSoundVolumeMin, damageSoundVolumeMax));
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
        if (health <= 0 && !isDead)
        {
            isDead = true;

            ParticleSystem spawnedBreakParticles = Instantiate(breakParticles, transform.position, transform.rotation);
            spawnedBreakParticles.Play();

            // Tell GameManager this block was destroyed
            GameManager.Instance.OnBlockDestroyed();

            Destroy(gameObject);
        }
    }

    IEnumerator FireDamage()
    {
        while (!isDead)
        {
            GameManager.Instance.AddScore(fireDamage);
            subtractHealth(fireDamage);
            checkIfDead();
            yield return new WaitForSeconds(fireDamageRate);
        }

        spawnedFireParticles.Stop();
        spawnedSound.Stop();
    }
}