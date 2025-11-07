using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingPopupVR : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float lifetime = 1f;
    public float minDist = 2f;
    public float maxDist = 3f;
    public bool scaleWithImportance = true; // optional scaling for big hits

    private Vector3 iniPos;
    private Vector3 targetPos;
    private float timer;
    private Transform playerCamera;
    private Vector3 targetScale = Vector3.one;

    void Start()
    {
        if (Camera.main != null)
            playerCamera = Camera.main.transform;

        // Face the camera
        if (playerCamera != null)
            transform.LookAt(2 * transform.position - playerCamera.position);

        iniPos = transform.position;

        // Random 3D direction
        Vector3 randomDir = Random.onUnitSphere;
        float dist = Random.Range(minDist, maxDist);
        targetPos = iniPos + randomDir * dist;

        transform.localScale = Vector3.zero;
        if (scaleWithImportance)
            targetScale = Vector3.one * Random.Range(1f, 2f); // bigger popups feel hype
        else
            targetScale = Vector3.one;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Sin(timer / lifetime);

        // Move & scale
        transform.position = Vector3.Lerp(iniPos, targetPos, t);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

        // Fade text
        if (timer > lifetime / 2f && textMesh != null)
            textMesh.color = Color.Lerp(textMesh.color, Color.clear, (timer - lifetime / 2f) / (lifetime / 2f));

        // Destroy after lifetime
        if (timer > lifetime)
            Destroy(gameObject);
    }

    public void SetText(string message)
    {
        if (textMesh != null)
            textMesh.text = message;
    }
}
