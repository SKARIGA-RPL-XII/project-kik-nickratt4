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
    
    private Vector3 originalPosition;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    public void DealDiceDamage()
    {
        TargetManager tm = clickInput.GetTargetManager();
        if (tm.CurrentTarget == null)
        {
            Debug.LogWarning("Tidak ada musuh terpilih!");
            return;
        }
        
        Transform enemyTransform = tm.CurrentTarget.GetTransform();
        EnemyStats stats = enemyTransform.GetComponent<EnemyStats>();
        
        if (stats == null)
        {
            Debug.LogError("Target tidak punya EnemyStats!");
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
        
        // Hitung posisi target (di depan enemy)
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        Vector3 attackPosition = enemy.position - (directionToEnemy * attackDistance);
        
        // Flip sprite berdasarkan arah ke enemy
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = directionToEnemy.x < 0;
        }
        
        // Gerak ke enemy
        while (Vector3.Distance(transform.position, attackPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                attackPosition, 
                moveSpeed * Time.deltaTime
            );
            
            yield return null;
        }
        
        // PUTAR SOUND EFFECT PEDANG
        PlayAttackSound();
        
        // Serang!
        int damage = dice.GetLastRollDamage();
        Debug.Log("Player menyerang " + stats.enemyName + " dengan damage: " + damage);
        stats.TakeDamage(damage);
        
        // Delay sebentar
        yield return new WaitForSeconds(returnDelay);
        
        // Hitung arah kembali ke posisi awal
        Vector3 directionToOriginal = (originalPosition - transform.position).normalized;
        
        // Flip sprite berdasarkan arah kembali
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = directionToOriginal.x < 0;
        }
        
        // Kembali ke posisi awal
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
        isAttacking = false;
    }
    
    private void PlayAttackSound()
    {
        if (audioSource != null && attackSoundClip != null)
        {
            audioSource.PlayOneShot(attackSoundClip);
        }
        else
        {
            Debug.LogWarning("Audio Source atau Attack Sound Clip belum diset!");
        }
    }
}