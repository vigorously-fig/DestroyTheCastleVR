using TMPro;
using UnityEngine;

public class FloatingPopup : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float floatSpeed = 1f;
    public float fadeDuration = 1.5f;

    private Color startColor;

    private void Start()
    {
        startColor = textMesh.color;
        Destroy(gameObject, fadeDuration);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        textMesh.color = Color.Lerp(startColor, Color.clear, Time.deltaTime * 2f);
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); // face the camera
    }
}

