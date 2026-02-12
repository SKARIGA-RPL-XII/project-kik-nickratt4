using UnityEngine;

public class ClickInput : MonoBehaviour
{
    public TargetManager targetManager;  

    void Awake()
    {
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();

            if (targetManager == null)
            {
                Debug.LogError("TIDAK ADA TargetManager di Scene!");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                ITarget target = hit.collider.GetComponent<ITarget>();

                if (target != null && targetManager != null)
                {
                    targetManager.SetTarget(target);
                }
            }
        }
    }

    public TargetManager GetTargetManager()
    {
        return targetManager;
    }
}
