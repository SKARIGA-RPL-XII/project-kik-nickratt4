using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDiceRoller : MonoBehaviour
{
    public UIstats playerStats;
    public Transform playerTransform;
    public Transform enemiesParent;
    public AudioSource audioSource;
    public AudioClip enemyAttackSound;
    public TurnIndicator turnIndicator;
    public float enemyspeed = 25f;

    private bool isEnemyTurn = false;
    private RollDicePlayer playerDiceScript;

    void Start()
    {
        playerDiceScript = FindObjectOfType<RollDicePlayer>();
        if (turnIndicator == null) turnIndicator = FindObjectOfType<TurnIndicator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public bool IsEnemyTurn() => isEnemyTurn;

    public void StartEnemyTurn()
    {
        if (isEnemyTurn) return;
        StartCoroutine(EnemyAttackSequence());
    }

    IEnumerator EnemyAttackSequence()
    {
        isEnemyTurn = true;
        Debug.Log("Enemy turn dimulai");

        EnemyStats[] aliveEnemies = GetAliveEnemies();
        if (aliveEnemies.Length == 0)
        {
            Debug.Log("Player menang!");
            isEnemyTurn = false;
            yield break;
        }

        int totalDamage = 0;
        foreach (EnemyStats e in aliveEnemies) totalDamage += e.damage;
        Debug.Log($"Total damage enemy: {totalDamage}");

        EnemyStats closest = GetClosestEnemy(aliveEnemies);
        if (closest != null) yield return StartCoroutine(MoveEnemyToPlayer(closest));

        if (audioSource && enemyAttackSound) audioSource.PlayOneShot(enemyAttackSound);
        if (playerStats) playerStats.TakeDamage(totalDamage);

        yield return new WaitForSeconds(0.5f);
        if (closest != null) yield return StartCoroutine(MoveEnemyBack(closest));

        isEnemyTurn = false;
        yield return new WaitForSeconds(0.5f);
        if (playerDiceScript) playerDiceScript.OnEnemyTurnFinished();
    }

    EnemyStats[] GetAliveEnemies()
    {
        List<EnemyStats> list = new List<EnemyStats>();
        foreach (Transform child in enemiesParent)
        {
            EnemyStats s = child.GetComponent<EnemyStats>();
            if (s != null && s.currentHp > 0) list.Add(s);
        }
        return list.ToArray();
    }

    EnemyStats GetClosestEnemy(EnemyStats[] enemies)
    {
        if (enemies.Length == 0 || playerTransform == null) return null;
        EnemyStats closest = enemies[0];
        float minDist = Vector3.Distance(playerTransform.position, closest.transform.position);
        foreach (EnemyStats e in enemies)
        {
            float d = Vector3.Distance(playerTransform.position, e.transform.position);
            if (d < minDist) { minDist = d; closest = e; }
        }
        return closest;
    }

    IEnumerator MoveEnemyToPlayer(EnemyStats enemy)
    {
        Vector3 originalPos = enemy.transform.position;
        Vector3 dir = (playerTransform.position - enemy.transform.position).normalized;
        Vector3 attackPos = playerTransform.position - dir * 2f;
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        bool originalFlip = sr ? sr.flipX : false;
        if (sr) sr.flipX = dir.x > 0;
        EnemyOriginalPosition eop = enemy.GetComponent<EnemyOriginalPosition>();
        if (eop) { eop.SetOriginalPosition(originalPos); eop.SetOriginalFlip(originalFlip); }
        while (Vector3.Distance(enemy.transform.position, attackPos) > 0.1f)
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackPos, enemyspeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveEnemyBack(EnemyStats enemy)
    {
        EnemyOriginalPosition eop = enemy.GetComponent<EnemyOriginalPosition>();
        if (eop == null) yield break;
        Vector3 originalPos = eop.GetOriginalPosition();
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        Vector3 dirBack = (originalPos - enemy.transform.position).normalized;
        if (sr) sr.flipX = dirBack.x > 0;
        float timeout = 5f, elapsed = 0f;
        while (Vector3.Distance(enemy.transform.position, originalPos) > 0.1f && elapsed < timeout)
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, originalPos, enemyspeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        enemy.transform.position = originalPos;
        if (sr && eop) sr.flipX = eop.GetOriginalFlip();
    }
}