using UnityEngine;

public class AttackAction
{
    public void Attack(ITarget target, int damage)
    {
        if (target == null) return;

        Health hp = target.GetTransform().GetComponent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
