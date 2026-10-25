using UnityEngine;

public class SpeedyBall : Ball
{
    [SerializeField] private float speedIncrease = 0.5f;
    [SerializeField] private int damageIncrease = 2;

    public override void OnCollisionWithBall(Ball otherBall)
    {
        speed += speedIncrease;
        contactDamage += damageIncrease;

        UpdateVelocity();
    }
}
