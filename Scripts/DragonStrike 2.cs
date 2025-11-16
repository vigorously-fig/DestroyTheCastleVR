using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonStrike : MonoBehaviour
{
    [Header("Dragon Strike Settings")]
    public float cooldown = 10f;
    public float strikeRadius = 5f;
    public int strikeDamage = 50;

    [Header("VFX & Audio")]
    public ParticleSystem chargeEffect;
    public ParticleSystem strikeEffect;
    public AudioClip strikeSound;

    private float lastStrikeTime = -Mathf.Infinity;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AttemptStrike()
    {
        if (!GameManager.Instance.CanUseDragonStrike()) return;

        GameManager.Instance.UseDragonStrike();

        StartCoroutine(PerformStrike());
    }

    private IEnumerator PerformStrike()
    {
        lastStrikeTime = Time.time;

        // Charge-up effect
        if (chargeEffect != null)
            chargeEffect.Play();

        yield return new WaitForSeconds(1f); // 1-second charge

        // Strike effect
        if (strikeEffect != null)
            strikeEffect.Play();

        if (audioSource != null && strikeSound != null)
            audioSource.PlayOneShot(strikeSound);

        // Deal area damage to all CastleBlocks in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, strikeRadius);
        foreach (var hit in hits)
        {
            CastleBlock block = hit.GetComponent<CastleBlock>();
            if (block != null)
            {
                block.Damage(strikeDamage, false);
            }
        }

        // Optional: Mega celebration popup
        GameManager.Instance.ShowFloatingPopup("<b><color=#FF0000>MEGA STRIKE!</color></b>", transform.position + Vector3.up * 3);
    }

    // Helper to show cooldown remaining (optional)
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, cooldown - (Time.time - lastStrikeTime));
    }

}
