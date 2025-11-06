using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleBlock : MonoBehaviour, IDamageable
{
    public GameObject damageText;

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

    private void Awake()
    {
        spawnedFireParticles = Instantiate(fireParticles, transform.position, transform.rotation, transform);
        spawnedDamageParticles = Instantiate(damageParticles, transform.position, transform.rotation, transform);

        spawnedSound = Instantiate(audioSource, transform.position, transform.rotation, transform);
        spawnedSound.clip = fireSound;
        spawnedSound.loop = true;

        // Register this block with the GameManager
        GameManager.Instance.RegisterCastleBlock(this);
    }

    public void Damage(int damageValue, bool isFireDamage)
    {
        if (isDead) return;

        GameManager.Instance.CheckForCelebration(damageValue, transform.position);
        GameManager.Instance.RegisterHitForCombo(damageValue, transform.position);

        subtractHealth(damageValue);

        // Tell the GameManager the castle has been hit (triggers panic music if not already)
        GameManager.Instance.CastleUnderAttack();

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
            spawnedSound.PlayOneShot(damageSound);
        }

        checkIfDead();
    }

    void subtractHealth(int damageValue)
    {
        health = Mathf.Clamp(health - damageValue, 0, 100);

        DamagePopup popup = Instantiate(damageText, transform.position, Quaternion.identity).GetComponent<DamagePopup>();
        popup.SetDamageText(damageValue);

        Debug.Log("Health: " + health);
    }

    void checkIfDead()
    {
        if (health <= 0 && !isDead)
        {
            isDead = true;

            ParticleSystem spawnedBreakParticles = Instantiate(breakParticles, transform.position, transform.rotation);
            spawnedBreakParticles.Play();

            SoundFXManager.instance.PlaySoundFXClip(breakSound, transform, 1);

            GameManager.Instance.UnregisterCastleBlock(this);
            Destroy(gameObject);
        }
    }

    IEnumerator FireDamage()
    {
        while (!isDead)
        {
            subtractHealth(fireDamage);
            checkIfDead();
            yield return new WaitForSeconds(fireDamageRate);
        }

        // Stop fire when block is destroyed
        spawnedFireParticles.Stop();
        spawnedSound.Stop();
    }
}
