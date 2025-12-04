using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GameObject floatingPopupPrefab;
    public SoundManager soundManager;

    [Header("Gameplay Stats")]
    public float startTime;
    public float completionTime;
    public bool gameOver = false;
    private int totalBlocks;
    private int destroyedBlocks = 0;

    // === SCORE SYSTEM ===
    [Header("Score System")]
    public int currentScore = 0;
    public int highScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        totalBlocks = FindObjectsOfType<CastleBlock>().Length;
        destroyedBlocks = 0;
        currentScore = 0;

        startTime = Time.time;

        if (soundManager)
            soundManager.PlayParadeAmbience();

        // Load saved highscore
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    // === SCORE FUNCTION ===
    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log("Score: " + currentScore);
    }

    // === REGISTER BLOCKS ===
    private List<CastleBlock> castleBlocks = new List<CastleBlock>();

    public void RegisterCastleBlock(CastleBlock block)
    {
        if (!castleBlocks.Contains(block))
        {
            castleBlocks.Add(block);
            totalBlocks = castleBlocks.Count;
        }
    }

    public void UnregisterCastleBlock(CastleBlock block)
    {
        if (castleBlocks.Contains(block))
        {
            castleBlocks.Remove(block);
            destroyedBlocks = totalBlocks - castleBlocks.Count;

            float percent = (float)destroyedBlocks / totalBlocks;
            soundManager.UpdateBattleIntensity(percent);

            if (castleBlocks.Count == 0)
                OnVictory();
        }
    }

    // Called by CastleBlock when destroyed
    public void OnBlockDestroyed()
    {
        destroyedBlocks++;

        soundManager.UpdateBattleIntensity((float)destroyedBlocks / totalBlocks);

        if (destroyedBlocks >= totalBlocks)
            OnVictory();
    }

    // === VICTORY ===
    private void OnVictory()
    {
        gameOver = true;
        completionTime = Time.time - startTime;

        // Save new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        ShowFloatingPopup(
            $"<b><color=yellow>CASTLE DESTROYED!</color></b>\n" +
            $"Time: {completionTime:F1}s\n" +
            $"Score: {currentScore}\n" +
            $"High Score: {highScore}",
            Camera.main.transform.position + Camera.main.transform.forward * 3f + Vector3.up * 1.5f
        );

        soundManager.PlayVictoryMusic();
    }

    // === POPUP ===
    public void ShowFloatingPopup(string message, Vector3 position)
    {
        if (floatingPopupPrefab == null) return;
        GameObject popup = Instantiate(floatingPopupPrefab, position, Quaternion.identity);

        var fp = popup.GetComponent<FloatingPopupVR>();
        if (fp != null)
            fp.SetText(message);
    }
}

