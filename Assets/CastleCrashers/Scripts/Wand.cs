using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject fireballPrefab;

    // Sound + VFX
    private AudioSource audioSource;

    [SerializeField] private AudioClip wandSoundEffect;
    [SerializeField] private ParticleSystem particles;

    // Wand settings
    [SerializeField] public float wandForce = 500f;
    [SerializeField] public float cooldownDelay = 1.5f;
    private Coroutine cooldownTimer;
    [SerializeField] private Transform shootPoint;

    private float wandSoundPitchMin = 0.8f;
    private float wandSoundPitchMax = 1.5f;

    private bool shouldCast = true;

    // === USE LIMIT SETTINGS ===
    [Header("Use Limit")]
    [SerializeField] private int maxUses = 10;     // how many shots allowed
    private int currentUses;

    [SerializeField] private bool autoRecharge = false;
    [SerializeField] private float rechargeDelay = 3f;
    [SerializeField] private int rechargeAmount = 1;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentUses = maxUses;
    }

    public void Trigger()
    {
        if (!shouldCast) return;
        if (currentUses <= 0) return;  // 🔥 BLOCK SHOOTING

        currentUses--;                 // 🔥 CONSUME USE

        // Spawn at shoot point
        GameObject spawnedMagic = Instantiate(
            fireballPrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        // Push forward
        Rigidbody rb = spawnedMagic.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * wandForce, ForceMode.Impulse);

        PlayFX();

        shouldCast = false;
        cooldownTimer = StartCoroutine(Cooldown());

        // auto recharge if enabled
        if (autoRecharge)
            StartCoroutine(Recharge());
    }

    private void PlayFX()
    {
        audioSource.pitch = Random.Range(wandSoundPitchMin, wandSoundPitchMax);
        audioSource.PlayOneShot(wandSoundEffect);

        if (particles != null)
            particles.Play();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);
        shouldCast = true;
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeDelay);

        currentUses = Mathf.Clamp(currentUses + rechargeAmount, 0, maxUses);
    }

    // Optional: expose this for UI
    public int GetRemainingUses() => currentUses;
    public int GetMaxUses() => maxUses;
}
