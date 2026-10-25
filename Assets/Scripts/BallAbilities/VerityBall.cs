using UnityEngine;

public class VerityBall : Ball
{
    [SerializeField, Range(0f, 1f)]
    private float upgradeChance = 0.25f;

    [SerializeField] private int damageIncrease = 5;

    public override void TakeDamage(int damage, Vector2 hitPoint)
    {
        if (!IsHost || health <= 0 || damage <= 0)
            return;

        base.TakeDamage(damage, hitPoint);

        if (health <= 0)
            return;

        if (Random.value <= upgradeChance)
        {
            contactDamage += damageIncrease;
        }
    }
}
