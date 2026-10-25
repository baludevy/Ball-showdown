using UnityEngine;

public class VerityBall : Ball
{
    [SerializeField, Range(0f, 1f)]
    private float upgradeChance = 0.25f;

    [SerializeField] private int damageIncrease = 30;

    private int stage;
    private int maxStage = 2;

    public Sprite first;
    public Sprite second;

    public SpriteRenderer renderer;

    public override void TakeDamage(int damage, Vector2 hitPoint)
    {
        if (!IsHost || health <= 0 || damage <= 0)
            return;

        base.TakeDamage(damage, hitPoint);

        if (health <= 0)
            return;

        if (Random.value <= upgradeChance && stage < maxStage)
        {
            contactDamage += damageIncrease;
            stage++;


            UpgradeSprite(stage);

            ServerSend.VerityUpgrade(owner.id, stage);
        }
    }

    public void UpgradeSprite(int stage)
    {
        if (stage == 1)
        {
            renderer.sprite = first;
        }
        else if (stage == 2)
        {
            renderer.sprite = second;
        }
    }
}
