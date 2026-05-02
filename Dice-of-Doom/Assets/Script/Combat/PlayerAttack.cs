using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public ClickInput clickInput;
    public RollDicePlayer dice;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float attackDistance = 2f;
    public float returnDelay = 0.5f;
    
    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip attackSoundClip;
    
    [Header("Enemy Turn")]
    public EnemyDiceRoller enemyDiceRoller;
    
    [Header("Turn Indicator")]
    public TurnIndicator turnIndicator;
    
    private Vector3 originalPosition;
    private bool originalFlipX;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        if (clickInput == null) clickInput = FindObjectOfType<ClickInput>();
        if (dice == null) dice = FindObjectOfType<RollDicePlayer>();
        if (enemyDiceRoller == null) enemyDiceRoller = FindObjectOfType<EnemyDiceRoller>();
        if (turnIndicator == null) turnIndicator = FindObjectOfType<TurnIndicator>();
        
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalFlipX = spriteRenderer.flipX;
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void DealDiceDamage()
    {
        if (isAttacking) return;
        
        Transform enemyTransform = null;
        EnemyStats enemyStats = null;
        
        // Coba dari ClickInput jika ada
        if (clickInput != null)
        {
            TargetManager tm = clickInput.GetTargetManager();
            if (tm != null && tm.CurrentTarget != null)
            {
                enemyTransform = tm.CurrentTarget.GetTransform();
                enemyStats = enemyTransform?.GetComponent<EnemyStats>();
            }
        }
        
        // Jika tidak ada target, cari musuh terdekat
        if (enemyStats == null)
        {
            EnemyStats[] enemies = FindObjectsOfType<EnemyStats>();
            float closestDist = Mathf.Infinity;
            foreach (EnemyStats e in enemies)
            {
                if (e.currentHp > 0)
                {
                    float d = Vector3.Distance(transform.position, e.transform.position);
                    if (d < closestDist)
                    {
                        closestDist = d;
                        enemyStats = e;
                    }
                }
            }
            if (enemyStats != null) enemyTransform = enemyStats.transform;
        }
        
        if (enemyTransform == null || enemyStats == null)
        {
            Debug.LogWarning("Tidak ada musuh yang bisa diserang!");
            // Jika tidak ada musuh, langsung ke enemy turn? Tapi musuh habis, player menang.
            if (FindObjectsOfType<EnemyStats>().Length == 0)
                Debug.Log("Player menang!");
            else
                Debug.LogError("Gagal menemukan target musuh.");
            return;
        }
        
        StartCoroutine(AttackSequence(enemyTransform, enemyStats));
    }
    
    private IEnumerator AttackSequence(Transform enemy, EnemyStats stats)
    {
        isAttacking = true;
        originalPosition = transform.position;
        if (spriteRenderer != null) originalFlipX = spriteRenderer.flipX;
        
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        Vector3 attackPosition = enemy.position - (directionToEnemy * attackDistance);
        
        if (spriteRenderer != null) spriteRenderer.flipX = directionToEnemy.x < 0;
        
        // Gerak maju
        while (Vector3.Distance(transform.position, attackPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        PlayAttackSound();
        int damage = dice != null ? dice.GetLastRollDamage() : 0;
        Debug.Log($"Player menyerang {stats.enemyName} dengan damage {damage}");
        stats.TakeDamage(damage);
        
        yield return new WaitForSeconds(returnDelay);
        
        // Gerak mundur
        Vector3 directionToOriginal = (originalPosition - transform.position).normalized;
        if (spriteRenderer != null) spriteRenderer.flipX = directionToOriginal.x < 0;
        
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = originalPosition;
        if (spriteRenderer != null) spriteRenderer.flipX = originalFlipX;
        
        // Hapus target jika menggunakan TargetManager
        if (clickInput != null)
        {
            TargetManager tm = clickInput.GetTargetManager();
            if (tm != null) tm.ClearTarget();
        }
        
        isAttacking = false;
        
        // Setelah player selesai, mulai enemy turn
        if (enemyDiceRoller != null)
        {
            Debug.Log("Player selesai menyerang, sekarang giliran enemy.");
            yield return new WaitForSeconds(0.5f);
            if (turnIndicator != null) turnIndicator.ShowEnemyTurn();
            yield return new WaitForSeconds(0.5f);
            enemyDiceRoller.StartEnemyTurn();
        }
        else
        {
            Debug.LogError("EnemyDiceRoller not assigned!");
        }
    }
    
    private void PlayAttackSound()
    {
        if (audioSource != null && attackSoundClip != null)
            audioSource.PlayOneShot(attackSoundClip);
    }
}