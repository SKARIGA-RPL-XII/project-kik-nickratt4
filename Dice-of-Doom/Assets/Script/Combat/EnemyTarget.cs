using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyTarget : MonoBehaviour, ITarget
{
    [SerializeField] private GameObject selectIndicator;

    void Awake()
    {
        if (selectIndicator != null)
            selectIndicator.SetActive(false);
    }

    public void Select()
    {
        if (selectIndicator != null)
            selectIndicator.SetActive(true);
    }

    public void Deselect()
    {
        if (selectIndicator != null)
            selectIndicator.SetActive(false);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}