using UnityEngine;

public class SpeedyBall : Ball
{
    [SerializeField] private float speedIncrease = 0.5f;
    [SerializeField] private int damageIncrease = 2;

    public override void OnCollisionWithBall(Ball otherBall, Vector2 hitPoint)
    {
        if (health <= 0 || otherBall == null || otherBall.health <= 0)
            return;

        base.OnCollisionWithBall(otherBall, hitPoint);

        speed += speedIncrease;
        contactDamage += damageIncrease;

        UpdateVelocity();
    }
}
