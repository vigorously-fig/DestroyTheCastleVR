using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GameObject floatingPopupPrefab;
    public SoundManager soundManager;

    [Header("Dragon Strike Settings")]
    [SerializeField] private float dragonStrikeCooldown = 10f;  // seconds
    [SerializeField] private float lastDragonStrikeTime = -Mathf.Infinity;

    public float LastDragonStrikeTime => lastDragonStrikeTime;
    public float DragonStrikeCooldown => dragonStrikeCooldown;

    [Header("Gameplay Stats")]
    public float startTime;
    public float completionTime;
    public bool gameOver = false;
    private int totalBlocks;
    private int destroyedBlocks;

    [Header("Combo System")]
    [SerializeField] private float comboTimeout = 3f;
    [SerializeField] private float comboHitThreshold = 50f;
    [SerializeField]
    private string[] comboPhrases = {
        "Combo x{0}!",
        "x{0} DESTRUCTION!",
        "CASTLE CARNAGE x{0}!",
        "WIZARD RAMPAGE x{0}!"
    };
    private int currentCombo = 0;
    private float lastHitTime = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //totalBlocks = FindObjectsOfType<CastleBlock>().Length;
        destroyedBlocks = 0;
        startTime = Time.time;

        if (soundManager)
            soundManager.PlayParadeAmbience();
    }

    // Called when any castle block is destroyed
    public void OnBlockDestroyed()
    {
        destroyedBlocks++;
        float percent = (float)destroyedBlocks / Mathf.Max(totalBlocks, 1);
        soundManager.UpdateBattleIntensity(percent);

        if (destroyedBlocks >= totalBlocks)
        {
            OnVictory();
        }
    }

    private void OnVictory()
    {
        gameOver = true;
        completionTime = Time.time - startTime;

        ShowFloatingPopup($"<b><color=yellow>🏰 CASTLE DESTROYED!</color></b>\nTime: {completionTime:F1}s",
                          Camera.main.transform.position + Camera.main.transform.forward * 3f + Vector3.up * 1.5f);

        soundManager.PlayVictoryMusic();
    }
    
    // === Celebration Popups for Big Hits ===
    [Header("Celebration Settings")]
    [SerializeField] private float celebrationThreshold = 75f; // damage needed for celebration
    [SerializeField]
    private string[] celebrationMessages = {
    "💥 MASSIVE HIT!",
    "🔥 CASTLE CRUMBLE!",
    "💣 DESTRUCTIVE STRIKE!",
    "⚡ POWER SURGE!"
};

    public void CheckForCelebration(float damageAmount, Vector3 position)
    {
        if (damageAmount < celebrationThreshold || gameOver) return;

        // Pick a random phrase
        string message = celebrationMessages[Random.Range(0, celebrationMessages.Length)];

        // Show popup near hit position
        ShowFloatingPopup($"<b><color=#FFD700>{message}</color></b>", position + Vector3.up * 2f);

        // Optional: play a hype sound or voice line
        if (soundManager != null)
            soundManager.PlayRandomVoiceLine();
    }

    // === Combo System ===
    public void RegisterHitForCombo(float damageAmount, Vector3 position)
    {
        if (damageAmount < comboHitThreshold) return;

        float timeSinceLast = Time.time - lastHitTime;
        lastHitTime = Time.time;

        if (timeSinceLast <= comboTimeout)
            currentCombo++;
        else
            currentCombo = 1;

        ShowComboPopup(currentCombo, position + Vector3.up * 3);

        if (currentCombo % 3 == 0)
            soundManager.PlayRandomVoiceLine();

        StopCoroutine(nameof(ComboResetTimer));
        StartCoroutine(nameof(ComboResetTimer));
    }

    IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboTimeout);
        currentCombo = 0;
    }

    private void ShowComboPopup(int comboCount, Vector3 position)
    {
        if (floatingPopupPrefab == null) return;
        string phrase = comboPhrases[Random.Range(0, comboPhrases.Length)];
        string formatted = string.Format(phrase, comboCount);

        GameObject popup = Instantiate(floatingPopupPrefab, position, Quaternion.identity);
        var fp = popup.GetComponent<FloatingPopup>();
        if (fp != null)
        {
            string color = comboCount switch
            {
                <= 2 => "#FFD700",
                <= 4 => "#FF6F00",
                _ => "#FF0000"
            };
            fp.SetText($"<b><color={color}>{formatted}</color></b>");
        }
    }

    public void ShowFloatingPopup(string message, Vector3 position)
    {
        if (floatingPopupPrefab == null) return;
        GameObject popup = Instantiate(floatingPopupPrefab, position, Quaternion.identity);
        var fp = popup.GetComponent<FloatingPopupVR>();
        if (fp != null)
            fp.SetText(message);
    }
    
    // === Block Registration ===
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

    // === Castle Attack Intensity ===
    private float lastAttackTime = 0f;
    private float attackCooldown = 2f;

    public void CastleUnderAttack()
    {
        // Prevent spam updates
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        // Trigger a short panic cue or crowd reaction
        if (soundManager != null)
        {
            soundManager.UpdateBattleIntensity((float)destroyedBlocks / Mathf.Max(totalBlocks, 1));
            soundManager.PlayRandomVoiceLine();
        }
    }

    public bool CanUseDragonStrike()
    {
        return Time.time - lastDragonStrikeTime >= dragonStrikeCooldown;
    }
    public void UseDragonStrike()
    {
        if (!CanUseDragonStrike()) return;

        lastDragonStrikeTime = Time.time;

        // Show a celebration popup
        ShowFloatingPopup("<b><color=#FF0000>MEGA STRIKE!</color></b>", Camera.main.transform.position + Vector3.up * 3);

        // Optional: voice line
        if (soundManager != null)
            soundManager.PlayRandomVoiceLine();
    }

}
