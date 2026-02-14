using UnityEngine;
using System.Collections;

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
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalFlipX = spriteRenderer.flipX;
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        if (turnIndicator == null)
        {
            turnIndicator = FindObjectOfType<TurnIndicator>();
        }
        
        if (enemyDiceRoller == null)
        {
            enemyDiceRoller = FindObjectOfType<EnemyDiceRoller>();
        }
    }
    
    public void DealDiceDamage()
    {
        TargetManager tm = clickInput.GetTargetManager();
        if (tm.CurrentTarget == null)
        {
            Debug.LogWarning("No enemy selected!");
            return;
        }
        
        Transform enemyTransform = tm.CurrentTarget.GetTransform();
        EnemyStats stats = enemyTransform.GetComponent<EnemyStats>();
        
        if (stats == null)
        {
            Debug.LogError("Target missing EnemyStats component!");
            return;
        }
        
        if (!isAttacking)
        {
            StartCoroutine(AttackSequence(enemyTransform, stats));
        }
    }
    
    private IEnumerator AttackSequence(Transform enemy, EnemyStats stats)
    {
        isAttacking = true;
        originalPosition = transform.position;
        
        if (spriteRenderer != null)
        {
            originalFlipX = spriteRenderer.flipX;
        }
        
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        Vector3 attackPosition = enemy.position - (directionToEnemy * attackDistance);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = directionToEnemy.x < 0;
        }
        
        while (Vector3.Distance(transform.position, attackPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                attackPosition, 
                moveSpeed * Time.deltaTime
            );
            
            yield return null;
        }
        
        PlayAttackSound();
        
        int damage = dice.GetLastRollDamage();
        Debug.Log("Player attacks " + stats.enemyName + " for " + damage + " damage");
        stats.TakeDamage(damage);
        
        yield return new WaitForSeconds(returnDelay);
        
        Vector3 directionToOriginal = (originalPosition - transform.position).normalized;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = directionToOriginal.x < 0;
        }
        
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                originalPosition, 
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.position = originalPosition;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = originalFlipX;
        }
        
        TargetManager tm = clickInput.GetTargetManager();
        if (tm != null)
        {
            tm.ClearTarget();
        }
        
        isAttacking = false;
        
        if (enemyDiceRoller != null)
        {
            yield return new WaitForSeconds(0.5f);
            
            if (turnIndicator != null)
            {
                turnIndicator.ShowEnemyTurn();
            }
            
            yield return new WaitForSeconds(1f);
            
            enemyDiceRoller.StartEnemyTurn();
        }
        else
        {
            Debug.LogWarning("EnemyDiceRoller not assigned!");
        }
    }
    
    private void PlayAttackSound()
    {
        if (audioSource != null && attackSoundClip != null)
        {
            audioSource.PlayOneShot(attackSoundClip);
        }
    }
}