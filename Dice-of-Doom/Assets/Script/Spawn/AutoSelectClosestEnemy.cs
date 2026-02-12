using UnityEngine;
using System.Collections;

public class AutoSelectClosestEnemy : MonoBehaviour
{
    public Transform player;
    public Transform enemiesParent;
    public ClickInput clickInput;  
    
    public void SelectClosestEnemy()
    {
        StartCoroutine(SelectWithDelay());
    }
    
    IEnumerator SelectWithDelay()
    {
        // Tunggu sebentar untuk memastikan enemy sudah spawn
        yield return new WaitForSeconds(0.1f);
        
        EnemyTarget closest = null;
        float closestDist = Mathf.Infinity;
        
        foreach (Transform enemy in enemiesParent)
        {
            // Pastikan enemy masih aktif
            if (!enemy.gameObject.activeInHierarchy) continue;
            
            float dist = Vector2.Distance(player.position, enemy.position);
            EnemyTarget target = enemy.GetComponent<EnemyTarget>();
            
            if (target != null && dist < closestDist)
            {
                closest = target;
                closestDist = dist;
            }
        }
        
        if (closest != null)
        {
            clickInput.GetTargetManager().SetTarget(closest);
            Debug.Log("Auto-selected enemy: " + closest.name + " (distance: " + closestDist + ")");
        }
        else
        {
            Debug.LogWarning("Tidak ada enemy untuk di-select!");
        }
    }
}