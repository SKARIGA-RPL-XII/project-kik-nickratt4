using UnityEngine;

public class ClickTargetInput2D : MonoBehaviour
{
    private TargetManager targetManager = new TargetManager();

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
                targetManager.SetTarget(target);
            }
        }
    }

    public TargetManager GetTargetManager()
    {
        return targetManager;
    }
}
