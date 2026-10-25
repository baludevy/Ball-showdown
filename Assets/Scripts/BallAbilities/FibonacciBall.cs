using UnityEngine;

public class FibonacciBall : Ball
{
    private int previousDamage = 0;
    private int currentDamage;

    protected override void Awake()
    {
        base.Awake();
        currentDamage = contactDamage;
    }

    public override int GetContactDamage()
    {
        return currentDamage;
    }

    public override void OnCollisionWithBall(Ball otherBall, Vector2 hitPoint)
    {
        if (health <= 0 || otherBall.health <= 0)
            return;

        base.OnCollisionWithBall(otherBall, hitPoint);

        int nextDamage = currentDamage + previousDamage;

        previousDamage = currentDamage;
        currentDamage = nextDamage;
    }
}
