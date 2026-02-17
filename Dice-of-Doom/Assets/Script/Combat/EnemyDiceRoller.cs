using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyDiceRoller : MonoBehaviour
{
    [Header("Dice UI - Shared with Player")]
    public RollDicePlayer playerDiceScript;
    
    [Header("Roll Settings")]
    public float rollSpeed = 0.05f;
    public float rollDuration = 2f;
    public bool autoRoll = false; 
    
    [Header("References")]
    public UIstats playerStats;
    public Transform playerTransform;
    public Transform enemiesParent;
    
    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip enemyAttackSound;
    
    [Header("Turn Indicator")]
    public TurnIndicator turnIndicator;

    [Header("Enemy Movement Settings")]
    public float enemyspeed = 25f;
    
    private int[] currentDiceValues;
    private int[] maxDiceValues;
    private int activeDiceCount;
    private bool isRolling = false;
    private bool isEnemyTurn = false;
    private bool diceRolled = false;
    private EnemyStats closestEnemy;
    
    public bool IsEnemyTurn()
    {
        return isEnemyTurn;
    }

    void Start()
    {
        currentDiceValues = new int[5];
        maxDiceValues = new int[5];
        
        if (playerDiceScript == null)
        {
            playerDiceScript = FindObjectOfType<RollDicePlayer>();
        }
        
        if (turnIndicator == null)
        {
            turnIndicator = FindObjectOfType<TurnIndicator>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    public void StartEnemyTurn()
    {
        if (!autoRoll)
        {
            StartCoroutine(EnemyTurnManualRoll());
        }
        else
        {
            StartCoroutine(EnemyAttackSequence());
        }
    }
    
    IEnumerator EnemyTurnManualRoll()
    {
        EnemyStats[] aliveEnemies = GetAliveEnemies();
        
        if (aliveEnemies.Length == 0)
        {
            Debug.Log("Player wins!");
            yield break;
        }
        
        Debug.Log("Enemy turn started (manual roll)");
        
        isEnemyTurn = true;
        diceRolled = false;
        
        HidePlayerDice();
        SetupDiceForEnemies(aliveEnemies);
        ShowDiceContainer(true);
        
        closestEnemy = GetClosestEnemy(aliveEnemies);
        
        Debug.Log("Waiting for player to click button to roll enemy dice");
    }
    
    public void OnEnemyRollButtonClicked()
    {
        if (!isEnemyTurn)
        {
            Debug.LogWarning("Not enemy turn!");
            return;
        }
        
        if (isRolling)
        {
            StopRollingAndAttack();
        }
        else if (!diceRolled)
        {
            StartCoroutine(StartEnemyRolling());
        }
    }
    
    IEnumerator StartEnemyRolling()
    {
        Debug.Log("Starting enemy dice roll");
        yield return StartCoroutine(RollEnemyDice());
    }
    
    void StopRollingAndAttack()
    {
        isRolling = false;
        diceRolled = true;
        
        Debug.Log("Enemy dice roll stopped");
        
        StartCoroutine(ExecuteEnemyAttack());
    }
    
    IEnumerator ExecuteEnemyAttack()
    {
        int totalDamage = CalculateTotalDamage();
        Debug.Log("Enemy total damage: " + totalDamage);
        
        if (closestEnemy != null)
        {
            yield return StartCoroutine(MoveEnemyToPlayer(closestEnemy));
        }
        
        if (audioSource != null && enemyAttackSound != null)
        {
            audioSource.PlayOneShot(enemyAttackSound);
        }
        
        if (playerStats != null)
        {
            playerStats.TakeDamage(totalDamage);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (closestEnemy != null)
        {
            yield return StartCoroutine(MoveEnemyBack(closestEnemy));
        }
        
        ShowDiceContainer(false);
        ShowPlayerDice();
        
        isEnemyTurn = false;
        diceRolled = false;
        closestEnemy = null;
        
        Debug.Log("Enemy turn finished");
        
        yield return new WaitForSeconds(0.5f);
        
        if (turnIndicator != null)
        {
            turnIndicator.ShowPlayerTurn();
        }
    }
    
    IEnumerator EnemyAttackSequence()
    {
        EnemyStats[] aliveEnemies = GetAliveEnemies();
        
        if (aliveEnemies.Length == 0)
        {
            Debug.Log("Player wins!");
            yield break;
        }
        
        Debug.Log("Enemy turn started");
        
        HidePlayerDice();
        SetupDiceForEnemies(aliveEnemies);
        ShowDiceContainer(true);
        
        yield return StartCoroutine(RollEnemyDice());
        
        int totalDamage = CalculateTotalDamage();
        Debug.Log("Enemy total damage: " + totalDamage);
        
        EnemyStats closestEnemy = GetClosestEnemy(aliveEnemies);
        
        if (closestEnemy != null)
        {
            yield return StartCoroutine(MoveEnemyToPlayer(closestEnemy));
        }
        
        if (audioSource != null && enemyAttackSound != null)
        {
            audioSource.PlayOneShot(enemyAttackSound);
        }
        
        if (playerStats != null)
        {
            playerStats.TakeDamage(totalDamage);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (closestEnemy != null)
        {
            yield return StartCoroutine(MoveEnemyBack(closestEnemy));
        }
        
        ShowDiceContainer(false);
        ShowPlayerDice();
        
        Debug.Log("Enemy turn finished");
        
        yield return new WaitForSeconds(0.5f);
        
        if (turnIndicator != null)
        {
            turnIndicator.ShowPlayerTurn();
        }
    }
    
    void HidePlayerDice()
    {
        if (playerDiceScript == null || playerDiceScript.diceObjects == null) return;
        
        foreach (GameObject dice in playerDiceScript.diceObjects)
        {
            if (dice != null)
                dice.SetActive(false);
        }
        
        Debug.Log("Player dice hidden");
    }
    
    void ShowPlayerDice()
    {
        if (playerDiceScript == null || playerDiceScript.diceObjects == null) return;
        
        int playerDiceCount = GetPlayerActiveDiceCount();
        
        for (int i = 0; i < playerDiceScript.diceObjects.Length; i++)
        {
            if (playerDiceScript.diceObjects[i] != null)
            {
                playerDiceScript.diceObjects[i].SetActive(i < playerDiceCount);
            }
        }
        
        Debug.Log("Player dice shown: " + playerDiceCount + " dice");
    }
    
    int GetPlayerActiveDiceCount()
    {
        int baseDamage = PlayerPrefs.GetInt("base_damage", 10);
        
        if (baseDamage <= 8) return 1;
        if (baseDamage <= 14) return 2;
        if (baseDamage <= 20) return 3;
        return 5;
    }
    
    void ShowDiceContainer(bool show)
    {
        if (playerDiceScript == null || playerDiceScript.diceObjects == null) return;
        
        if (show)
        {
            for (int i = 0; i < playerDiceScript.diceObjects.Length; i++)
            {
                if (playerDiceScript.diceObjects[i] != null)
                {
                    playerDiceScript.diceObjects[i].SetActive(i < activeDiceCount);
                }
            }
            Debug.Log("Enemy dice shown: " + activeDiceCount + " dice");
        }
        else
        {
            for (int i = 0; i < playerDiceScript.diceObjects.Length; i++)
            {
                if (playerDiceScript.diceObjects[i] != null)
                {
                    playerDiceScript.diceObjects[i].SetActive(false);
                }
            }
        }
    }
    
    void SetupDiceForEnemies(EnemyStats[] enemies)
    {
        if (playerDiceScript == null) return;
        
        activeDiceCount = Mathf.Min(5, enemies.Length);
        
        for (int i = 0; i < 5; i++)
        {
            if (i < activeDiceCount)
            {
                maxDiceValues[i] = enemies[i].damage;
                
                if (playerDiceScript.diceTexts != null && 
                    i < playerDiceScript.diceTexts.Length && 
                    playerDiceScript.diceTexts[i] != null)
                {
                    playerDiceScript.diceTexts[i].text = "0";
                }
            }
            else
            {
                if (playerDiceScript.diceObjects != null &&
                    i < playerDiceScript.diceObjects.Length &&
                    playerDiceScript.diceObjects[i] != null)
                {
                    playerDiceScript.diceObjects[i].SetActive(false);
                }
            }
        }
        
        Debug.Log("Enemy dice setup: " + activeDiceCount + " dice");
    }
    
    IEnumerator RollEnemyDice()
    {
        if (playerDiceScript == null || playerDiceScript.diceTexts == null) yield break;
        
        Debug.Log("Rolling enemy dice");
        isRolling = true;
        float elapsed = 0f;
        
        while (elapsed < rollDuration && isRolling)
        {
            for (int i = 0; i < activeDiceCount; i++)
            {
                currentDiceValues[i] = Random.Range(0, maxDiceValues[i] + 1);
                
                if (i < playerDiceScript.diceTexts.Length && 
                    playerDiceScript.diceTexts[i] != null)
                {
                    playerDiceScript.diceTexts[i].text = currentDiceValues[i].ToString();
                }
            }
            
            yield return new WaitForSeconds(rollSpeed);
            elapsed += rollSpeed;
        }
        
        isRolling = false;
        diceRolled = true;
        
        for (int i = 0; i < activeDiceCount; i++)
        {
            currentDiceValues[i] = Random.Range(0, maxDiceValues[i] + 1);
            
            if (i < playerDiceScript.diceTexts.Length && 
                playerDiceScript.diceTexts[i] != null)
            {
                playerDiceScript.diceTexts[i].text = currentDiceValues[i].ToString();
            }
        }
        
        Debug.Log("Dice roll finished");
    }
    
    int CalculateTotalDamage()
    {
        int total = 0;
        for (int i = 0; i < activeDiceCount; i++)
        {
            total += currentDiceValues[i];
        }
        return total;
    }
    
    EnemyStats[] GetAliveEnemies()
    {
        System.Collections.Generic.List<EnemyStats> aliveList = 
            new System.Collections.Generic.List<EnemyStats>();
        
        foreach (Transform child in enemiesParent)
        {
            EnemyStats stats = child.GetComponent<EnemyStats>();
            if (stats != null && stats.currentHp > 0)
            {
                aliveList.Add(stats);
            }
        }
        
        return aliveList.ToArray();
    }
    
    EnemyStats GetClosestEnemy(EnemyStats[] enemies)
    {
        if (enemies.Length == 0 || playerTransform == null) return null;
        
        EnemyStats closest = enemies[0];
        float minDistance = Vector3.Distance(
            playerTransform.position, 
            closest.transform.position
        );
        
        foreach (EnemyStats enemy in enemies)
        {
            float distance = Vector3.Distance(
                playerTransform.position, 
                enemy.transform.position
            );
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy;
            }
        }
        
        Debug.Log("Closest enemy: " + closest.enemyName);
        return closest;
    }
    
    IEnumerator MoveEnemyToPlayer(EnemyStats enemy)
    {
        Debug.Log(enemy.enemyName + " moving to player");
        
        Vector3 originalPos = enemy.transform.position;
        Vector3 directionToPlayer = 
            (playerTransform.position - enemy.transform.position).normalized;
        Vector3 attackPos = 
            playerTransform.position - (directionToPlayer * 2f);
        
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        bool originalFlip = false;
        if (sr != null)
        {
            originalFlip = sr.flipX;
            sr.flipX = directionToPlayer.x > 0;
        }
        
        EnemyOriginalPosition eop = enemy.GetComponent<EnemyOriginalPosition>();
        if (eop != null)
        {
            eop.SetOriginalPosition(originalPos);
            eop.SetOriginalFlip(originalFlip);
        }
        
        float moveSpeed = enemyspeed    ;
        while (Vector3.Distance(enemy.transform.position, attackPos) > 0.1f)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                attackPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
    
    IEnumerator MoveEnemyBack(EnemyStats enemy)
    {
        Debug.Log(enemy.enemyName + " returning");
        
        EnemyOriginalPosition eop = enemy.GetComponent<EnemyOriginalPosition>();
        
        if (eop == null)
        {
            Debug.LogError(enemy.enemyName + " missing EnemyOriginalPosition component!");
            yield break;
        }
        
        Vector3 originalPos = eop.GetOriginalPosition();
        
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        Vector3 directionBack = (originalPos - enemy.transform.position).normalized;
        if (sr != null)
        {
            sr.flipX = directionBack.x > 0;
        }
        
        float moveSpeed = enemyspeed    ;
        float timeout = 5f;
        float elapsed = 0f;
        
        while (Vector3.Distance(enemy.transform.position, originalPos) > 0.1f && elapsed < timeout)
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                originalPos,
                moveSpeed * Time.deltaTime
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        enemy.transform.position = originalPos;
        
        if (sr != null && eop != null)
        {
            sr.flipX = eop.GetOriginalFlip();
        }
        
        Debug.Log(enemy.enemyName + " returned to original position");
    }
}