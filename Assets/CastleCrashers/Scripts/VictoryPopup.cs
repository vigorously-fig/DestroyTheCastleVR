using TMPro;
using UnityEngine;

public class VictoryPopup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro textMesh;

    [Header("Behaviour")]
    public float floatSpeed = 0.5f;        // gentle float upward
    public float fadeInSpeed = 2f;
    public float holdTime = 5f;            // how long the panel stays fully visible
    public float fadeOutSpeed = 1f;
    public bool destroyAfterFade = true;

    private Color originalColor;
    private float timer = 0f;
    private enum PopupState { FadeIn, Hold, FadeOut }
    private PopupState state = PopupState.FadeIn;

    void Start()
    {
        originalColor = textMesh.color;
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    public void SetText(string content)
    {
        textMesh.text = content;
    }

    void Update()
    {
        // Always face player
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);

        // Gentle floating
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;

        switch (state)
        {
            case PopupState.FadeIn:
                textMesh.color = Color.Lerp(textMesh.color, originalColor, Time.deltaTime * fadeInSpeed);
                if (timer > 1f)
                {
                    state = PopupState.Hold;
                    timer = 0f;
                }
                break;

            case PopupState.Hold:
                if (timer > holdTime)
                {
                    state = PopupState.FadeOut;
                    timer = 0f;
                }
                break;

            case PopupState.FadeOut:
                textMesh.color = Color.Lerp(textMesh.color, new Color(originalColor.r, originalColor.g, originalColor.b, 0f),
                                            Time.deltaTime * fadeOutSpeed);
                if (destroyAfterFade && textMesh.color.a < 0.05f)
                    Destroy(gameObject);
                break;
        }
    }
}
