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
        yield return new WaitForSeconds(0.50f);

        EnemyTarget closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Transform enemy in enemiesParent)
        {
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
            Debug.Log("Auto-selected enemy: " + closest.name);
        }
        else
        {
            Debug.LogWarning("Auto-select gagal — belum ada enemy!");
        }
    }
}
    