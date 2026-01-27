using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;
    public int CurrentHP { get; private set; }

    public bool IsDead => CurrentHP <= 0;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;

        Debug.Log($"{gameObject.name} HP: {CurrentHP}");
    }
}
