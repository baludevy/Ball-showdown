using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [NonSerialized] public Player owner;
    [NonSerialized] public BallType type;

    [SerializeField] protected float speed = 7f;
    [SerializeField] protected int contactDamage = 10;

    [SerializeField] private TMP_Text username;
    [SerializeField] private Slider healthBar;

    public int health = 100;

    protected Rigidbody2D rb;
    public Vector2 direction;

    public bool move;
    public Transform arrow;

    protected bool IsHost => NetworkManager.Instance.isHost;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsHost)
            rb.simulated = false;
    }

    public void Initialize()
    {
        if (owner == null)
        {
            Debug.LogError("Cannot initialize a ball without an owner.");
            return;
        }

        if (username != null)
            username.text = owner.username;

        UpdateHealthBar();
    }

    public virtual void TakeDamage(int damage, Vector2 hitPoint)
    {
        if (!IsHost || health <= 0 || damage <= 0)
            return;

        health = Mathf.Max(0, health - damage);
        UpdateHealthBar();

        Vector3 markerPosition = Vector2.Lerp(hitPoint, rb.position, 0.8f);

        ServerSend.DamageMarker(markerPosition, damage);
        DamageMarkerHelper.Instance?.SpawnDamageMarker(markerPosition, damage);

        if (health <= 0)
        {
            ServerSend.DestroyBall(owner.id);
            BallManager.Instance.DestroyBall(owner.id);
            return;
        }

        ServerSend.BallHealth(owner.id, health);
    }

    public void SetHealth(int health)
    {
        this.health = Mathf.Max(0, health);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = health / 100f;
    }

    protected virtual void Start()
    {
        if (owner.id != NetworkManager.Instance.myId)
            arrow.gameObject.SetActive(false);

        if (!IsHost)
            return;

        direction = UnityEngine.Random.insideUnitCircle.normalized;
        UpdateVelocity();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsHost)
            return;

        UpdateVelocity();
    }

    protected void UpdateVelocity()
    {
        if (move)
            rb.linearVelocity = direction * speed;
    }

    public virtual int GetContactDamage()
    {
        return contactDamage;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsHost)
            return;

        bool isWall = collision.gameObject.CompareTag("Wall");
        Ball otherBall = collision.gameObject.GetComponent<Ball>();

        if (!isWall && otherBall == null)
            return;

        if (isWall)
            OnCollisionWithWall();

        if (otherBall != null)
            OnCollisionWithBall(otherBall, collision.GetContact(0).point);

        Vector2 normal = collision.GetContact(0).normal;

        direction = Vector2.Reflect(direction, normal).normalized;

        if (Vector2.Dot(direction, normal) <= 0.01f)
            direction = (normal + direction * 0.2f).normalized;

        UpdateVelocity();
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsHost)
            return;

        if (!collision.gameObject.CompareTag("Wall") &&
            collision.gameObject.GetComponent<Ball>() == null)
            return;

        Vector2 normal = collision.GetContact(0).normal;

        if (Vector2.Dot(direction, normal) <= 0.01f)
        {
            direction = normal.normalized;
            UpdateVelocity();
        }
    }
    public virtual void OnCollisionWithWall()
    {
    }

    public virtual void OnCollisionWithBall(Ball otherBall, Vector2 hitPoint)
    {
        if (health <= 0 || otherBall.health <= 0)
            return;

        otherBall.TakeDamage(GetContactDamage(), hitPoint);
    }

    public void ApplySnapshot(BallSnapshot from, BallSnapshot to, float t)
    {
    }
}
