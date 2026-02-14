using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Data")]
    public int enemyId;
    public string enemyName;
    public int maxHp;
    public int currentHp;
    public int damage;
    public int levelGame;
    public bool isBoss;
    
    [Header("UI")]
    public Slider hpBar;
    public TMP_Text hpText;
    public TMP_Text nameText;
    
    void Start()
    {
        UpdateUI();
    }
    
    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;
        currentHp = Mathf.Max(0, currentHp);
        
        Debug.Log(enemyName + " menerima damage: " + damageAmount + " | HP tersisa: " + currentHp);
        
        UpdateUI();
        
        if (currentHp <= 0)
        {
            Die();
        }
    }
    
    void UpdateUI()
    {
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = currentHp;
        }
        
        if (hpText != null)
        {
            hpText.text = currentHp + " / " + maxHp;
        }
        
        if (nameText != null)
        {
            nameText.text = enemyName;
        }
    }
    
    void Die()
    {
        Debug.Log(enemyName + " mati!");
        
        Destroy(gameObject);
        
        CheckAllEnemiesDead();
    }
    
    void CheckAllEnemiesDead()
    {
        StageManager stageManager = FindObjectOfType<StageManager>();
        
        if (stageManager != null)
        {
            stageManager.Invoke("CheckVictory", 0.5f);
        }
    }
}