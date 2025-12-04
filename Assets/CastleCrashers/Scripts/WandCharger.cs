using UnityEngine;

public class WandCharger : MonoBehaviour
{
    [Header("Wand Settings")]
    [SerializeField] private Wand wandPrefab;     // fully charged wand prefab
    [SerializeField] private Transform spawnPoint;

    [Header("Charging Settings")]
    [SerializeField] private float chargeTime = 2f;   // delay before new wand appears
    private bool isCharging = false;

    [Header("Optional Effects")]
    [SerializeField] private ParticleSystem startChargeFX;  // plays when wand inserted
    [SerializeField] private ParticleSystem finishChargeFX; // plays when wand is finished
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip finishSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCharging) return; // prevent double-charging

        Wand wand = other.GetComponent<Wand>();
        if (wand == null) return;

        // Only recharge empty wands
        if (wand.GetRemainingUses() > 0) return;

        StartCoroutine(ChargeSequence(wand));
    }

    private System.Collections.IEnumerator ChargeSequence(Wand oldWand)
    {
        isCharging = true;

        // Start FX
        if (startChargeFX != null) startChargeFX.Play();
        if (audioSource != null && startSound != null)
            audioSource.PlayOneShot(startSound);

        // Remove old wand
        Destroy(oldWand.gameObject);

        // Wait for charging
        yield return new WaitForSeconds(chargeTime);

        // Finish FX
        if (finishChargeFX != null) finishChargeFX.Play();
        if (audioSource != null && finishSound != null)
            audioSource.PlayOneShot(finishSound);

        // Spawn new wand
        Instantiate(wandPrefab, spawnPoint.position, spawnPoint.rotation);

        isCharging = false;
    }
}
