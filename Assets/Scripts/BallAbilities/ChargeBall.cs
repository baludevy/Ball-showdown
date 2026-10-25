using UnityEngine;

public class ChargeBall : Ball
{
    [SerializeField] private float chargeInterval = 3f;
    [SerializeField] private float sizeIncrease = 0.2f;
    [SerializeField] private int damageIncrease = 5;
    [SerializeField] private int maxCharges = 10;

    private float chargeTimer;
    private Vector3 originalScale;
    private int originalDamage;
    private int charges;

    protected override void Awake()
    {
        base.Awake();

        originalScale = transform.localScale;
        originalDamage = contactDamage;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsHost || health <= 0)
            return;

        if(move)
            chargeTimer += Time.fixedDeltaTime;

        if (chargeTimer >= chargeInterval)
        {
            chargeTimer -= chargeInterval;
            Charge();
        }
    }

    private void Charge()
    {
        if (charges >= maxCharges)
            return;

        charges++;

        transform.localScale =
            originalScale * (1f + charges * sizeIncrease);

        contactDamage =
            originalDamage + charges * damageIncrease;
    }

    public override void OnCollisionWithBall(Ball otherBall, Vector2 hitPoint)
    {
        if (health <= 0 || otherBall.health <= 0)
            return;

        base.OnCollisionWithBall(otherBall, hitPoint);

        ResetCharge();
    }

    private void ResetCharge()
    {
        charges = 0;
        chargeTimer = 0f;

        transform.localScale = originalScale;
        contactDamage = originalDamage;
    }
}
