using System.Collections;
using UnityEngine;

public class BurstBall : Ball
{
    [SerializeField] private float burstCooldown = 5f;
    [SerializeField] private float burstDuration = 1.5f;
    [SerializeField] private float speedMultiplier = 3f;
    [SerializeField] private int damageMultiplier = 4;

    private bool isBursting;
    private float normalSpeed;

    protected override void Start()
    {
        base.Start();

        if (!IsHost)
            return;

        normalSpeed = speed;
        StartCoroutine(BurstRoutine());
    }

    private IEnumerator BurstRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(burstCooldown);

            StartBurst();

            yield return new WaitForSeconds(burstDuration);

            EndBurst();
        }
    }

    private void StartBurst()
    {
        isBursting = true;

        speed = normalSpeed * speedMultiplier;
        UpdateVelocity();   
    }

    private void EndBurst()
    {
        isBursting = false;

        speed = normalSpeed;
        UpdateVelocity();
    }

    public override int GetContactDamage()
    {
        if (isBursting)
            return contactDamage * damageMultiplier;

        return base.GetContactDamage();
    }

    public override void TakeDamage(int damage)
    {
        if (isBursting)
            return;

        base.TakeDamage(damage);
    }   
}
    