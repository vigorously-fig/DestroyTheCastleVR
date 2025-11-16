using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI text;
    public Transform playerCamera; // Assign your VR camera here

    [Header("Popup Settings")]
    public float lifetime = 0.6f;
    public float minDist = 2f;
    public float maxDist = 3f;
    public bool scaleWithDamage = true;

    private Vector3 iniPos;
    private Vector3 targetPos;
    private float timer;

    private int damageAmount;

    void Start()
    {
        // Make sure we have a camera
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        // Face the camera
        if (playerCamera != null)
            transform.LookAt(2 * transform.position - playerCamera.position);

        // Initialize position
        iniPos = transform.position;

        // Random direction in 3D space
        Vector3 randomDir = Random.onUnitSphere;
        float dist = Random.Range(minDist, maxDist);
        targetPos = iniPos + randomDir * dist;

        // Optional: scale with damage
        transform.localScale = Vector3.zero;
        if (scaleWithDamage)
        {
            float scaleFactor = Mathf.Clamp(damageAmount / 50f, 0.8f, 2f);
            transform.localScale = Vector3.zero; // Start from zero, lerp to target
            targetScale = Vector3.one * scaleFactor;
        }
        else
        {
            targetScale = Vector3.one;
        }
    }

    private Vector3 targetScale;

    void Update()
    {
        timer += Time.deltaTime;

        float fraction = lifetime / 2f;

        // Fade out
        if (timer > fraction && text != null)
            text.color = Color.Lerp(text.color, Color.clear, (timer - fraction) / (lifetime - fraction));

        // Move and scale
        float t = Mathf.Sin(timer / lifetime);
        transform.position = Vector3.Lerp(iniPos, targetPos, t);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

        // Destroy after lifetime
        if (timer > lifetime)
            Destroy(gameObject);
    }

    /// <summary>
    /// Sets the damage value and optionally adjusts scale
    /// </summary>
    public void SetDamageText(int damage)
    {
        damageAmount = damage;
        if (text != null)
            text.text = damage.ToString();
    }
}
