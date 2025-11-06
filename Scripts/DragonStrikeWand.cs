using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class DragonStrikeWand : MonoBehaviour
{
    [Header("References")]
    //public ParticleSystem chargeEffect;       // Wand charge VFX
    public ParticleSystem strikeEffect;       // Impact VFX
    public AudioClip strikeSound;             // Strike audio
    public Image cooldownUI;                  // Radial cooldown UI
    public LayerMask castleLayer;             // Layer for castle blocks


    //remove this and use dragon to handle strike damage and radius instead
    [Header("Settings")]
    public float strikeDamage = 150f;         // Damage per block
    public float strikeRadius = 10f;          // Area radius
    public float chargeTime = 1f;             // Seconds to charge (how long before use after cooldown)

    //[Header("XR Input")]
    //public XRNode wandHand = XRNode.RightHand; // Wand hand for input will be directly from xr grab interactable instead.

    private AudioSource audioSource;
    private InputDevice wandDevice;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (cooldownUI != null)
            cooldownUI.fillAmount = 1f; // Ready at start
    }

    //private void Start()
   // {
        // Initialize XR device
        //wandDevice = InputDevices.GetDeviceAtXRNode(wandHand);
    //}

    private void Update()
    {
        // Update cooldown UI make wand glow or something 
        if (cooldownUI != null)
        {
            cooldownUI.fillAmount = Mathf.Clamp01(
                (Time.time - GameManager.Instance.LastDragonStrikeTime) / GameManager.Instance.DragonStrikeCooldown
            );
        }

        // Check wand trigger input
        if (wandDevice.isValid)
        {
            bool triggerPressed = false;
            if (wandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
            {
                if (GameManager.Instance.CanUseDragonStrike())
                {
                    StartCoroutine(PerformDragonStrike());
                }
            }
        }
    }


    
    private IEnumerator PerformDragonStrike()
    {
        // Register strike and start cooldown
        GameManager.Instance.UseDragonStrike();

        // Play charge effect
        if (chargeEffect != null)
            chargeEffect.Play();

        yield return new WaitForSeconds(chargeTime);

        // Play strike effect
        if (strikeEffect != null)
            strikeEffect.Play();

        // Play strike sound
        if (audioSource != null && strikeSound != null)
            audioSource.PlayOneShot(strikeSound);

        // Deal area damage to all CastleBlocks in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, strikeRadius, castleLayer);
        foreach (var hit in hits)
        {
            CastleBlock block = hit.GetComponent<CastleBlock>();
            if (block != null)
                block.Damage((int)strikeDamage, true); // Fire damage
        }

        // Show Dragon Strike popup
        GameManager.Instance.ShowFloatingPopup("🔥 DRAGON STRIKE! 🔥", transform.position + Vector3.up * 2f);

        // Play hype voice line
        SoundManager.Instance.PlayRandomVoiceLine();
    }

    // Optional: visualize strike radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, strikeRadius);
    }
}
