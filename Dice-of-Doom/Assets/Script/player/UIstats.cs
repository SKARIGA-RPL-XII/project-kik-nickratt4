using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIstats : MonoBehaviour
{
    [Header("Player Stats Display")]
    public TMP_Text username;
    public TMP_Text playerHealth;
    public TMP_Text playerDamage;
    
    [Header("HP Bar (Optional)")]
    public Slider hpSlider;
    public Image hpFillImage;
    public Color hpColorHigh = Color.green;
    public Color hpColorMid = Color.yellow;
    public Color hpColorLow = Color.red;
    
    [Header("Player Stats")]
    public int maxHp = 75;
    public int currentHp;
    public int baseDamage;
    
    [Header("Animation (Optional)")]
    public bool animateHpChange = true;
    public float animationSpeed = 50f;
    
    private int targetHp;
    private string playerUsername;
    
    void OnEnable()
    {
        LoadStatsFromPlayerPrefs();
        UpdateStatsDisplay();
    }
    
    void Start()
    {
        LoadStatsFromPlayerPrefs();
        UpdateStatsDisplay();
    }
    
    void Update()
    {
        if (animateHpChange && currentHp != targetHp)
        {
            currentHp = Mathf.RoundToInt(
                Mathf.MoveTowards(currentHp, targetHp, animationSpeed * Time.deltaTime)
            );
            UpdateStatsDisplay();
        }
    }
    
    public void LoadStatsFromPlayerPrefs()
    {
        playerUsername = PlayerPrefs.GetString("username", "Player");
        maxHp = 75; // FORCE MAX HP = 75
        currentHp = PlayerPrefs.GetInt("current_hp", maxHp);
        
        // Pastikan current HP tidak lebih dari 75
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
            PlayerPrefs.SetInt("current_hp", maxHp);
            PlayerPrefs.Save();
        }
        
        baseDamage = PlayerPrefs.GetInt("base_damage", 10);
        targetHp = currentHp;
        
        Debug.Log(
            "Stats Loaded - Username: " + playerUsername +
            ", Player ID: " + PlayerPrefs.GetInt("player_id") +
            ", Max HP: " + maxHp +
            ", Current HP: " + currentHp +
            ", Base Damage: " + baseDamage
        );
    }
    
    public void UpdateStatsDisplay()
    {
        if (username != null)
            username.text = playerUsername;
            
        if (playerHealth != null)
            playerHealth.text = currentHp + " / " + maxHp;
            
        if (playerDamage != null)
            playerDamage.text = baseDamage.ToString();
        
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
        
        if (hpFillImage != null)
        {
            float hpPercent = (float)currentHp / maxHp;
            
            if (hpPercent > 0.6f)
                hpFillImage.color = hpColorHigh;
            else if (hpPercent > 0.3f)
                hpFillImage.color = hpColorMid;
            else
                hpFillImage.color = hpColorLow;
        }
    }
    
    public void TakeDamage(int damage)
    {
        targetHp -= damage;
        targetHp = Mathf.Max(0, targetHp);
        
        if (!animateHpChange)
        {
            currentHp = targetHp;
        }
        
        Debug.Log("Player menerima damage: " + damage + " | HP tersisa: " + targetHp);
        
        PlayerPrefs.SetInt("current_hp", targetHp);
        PlayerPrefs.Save();
        
        UpdateStatsDisplay();
        
        if (targetHp <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        targetHp += amount;
        targetHp = Mathf.Min(maxHp, targetHp);
        
        if (!animateHpChange)
        {
            currentHp = targetHp;
        }
        
        Debug.Log("Player heal: " + amount + " | HP sekarang: " + targetHp);
        
        PlayerPrefs.SetInt("current_hp", targetHp);
        PlayerPrefs.Save();
        
        UpdateStatsDisplay();
    }
    
    public void SetHP(int newHp)
    {
        targetHp = Mathf.Clamp(newHp, 0, maxHp);
        
        if (!animateHpChange)
        {
            currentHp = targetHp;
        }
        
        PlayerPrefs.SetInt("current_hp", targetHp);
        PlayerPrefs.Save();
        
        UpdateStatsDisplay();
    }
    
    public void ResetHP()
    {
        targetHp = maxHp;
        currentHp = maxHp;
        
        PlayerPrefs.SetInt("current_hp", maxHp);
        PlayerPrefs.Save();
        
        UpdateStatsDisplay();
        
        Debug.Log("HP direset ke maksimal: " + maxHp);
    }
    
    void Die()
    {
        Debug.Log("Player Mati! Game Over!");
    }
    
    public int GetCurrentHP()
    {
        return currentHp;
    }
    
    public int GetMaxHP()
    {
        return maxHp;
    }
    
    public bool IsAlive()
    {
        return currentHp > 0;
    }
}