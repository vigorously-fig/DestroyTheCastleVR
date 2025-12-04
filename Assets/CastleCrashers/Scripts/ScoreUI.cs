using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            UpdateUI(GameManager.Instance.currentScore, GameManager.Instance.highScore);
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            UpdateUI(GameManager.Instance.currentScore, GameManager.Instance.highScore);
        }
    }

    private void UpdateUI(int score, int highScore)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";
    }
}